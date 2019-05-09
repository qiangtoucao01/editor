﻿using System.Collections.Generic;
using System.CodeDom.Compiler;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using Debug = UnityEngine.Debug;
using BDFramework.ResourceMgr;
using Tool;
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

#endif
public class ScriptBiuldTools
{
    public enum BuildStatus
    {
        Success = 0,
        Fail
    }

#if UNITY_EDITOR

    private static Dictionary<int, string> csFilesMap;
    /// <summary>
    /// 编译DLL
    /// </summary>
    static public void GenDllByMono(string dataPath, string outPath)
    {
        EditorUtility.DisplayProgressBar("编译服务", "准备编译环境...", 0.1f);
        //全局变量
        var baseDllPath = outPath + "/hotfix/base.dll";

        //清空输出环境
        var path = outPath + "/Hotfix";
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }

        Directory.CreateDirectory(path);

        //创建临时环境
#if UNITY_EDITOR_OSX
        var tempDirect = Application.persistentDataPath + "/bd_temp";
#else
        var tempDirect = "d:/bd_temp";
#endif
        if (Directory.Exists(tempDirect))
        {
            Directory.Delete(tempDirect, true);
        }

        Directory.CreateDirectory(tempDirect);

        //建立目标目录
        var outDirectory = Path.GetDirectoryName(baseDllPath);
        //准备输出环境
        try
        {
            if (Directory.Exists(outDirectory))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(outDirectory);
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("提示", "Unity拒绝创建文件夹,请重试!", "OK");
            return;
        }


        #region 创建cs搜索目录

        EditorUtility.DisplayProgressBar("编译服务", "搜索待编译Code...", 0.2f);
        //
        List<string> searchPath = new List<string>() {"3rdPlugins", "Code", "Plugins", "Resource"};
        for (int i = 0; i < searchPath.Count; i++)
        {
            searchPath[i] = IPath.Combine(dataPath, searchPath[i]);
        }

        //2018.3的package目录
        var pPath = Application.dataPath.Replace("Assets", "") + "Library/PackageCache";
        searchPath.Add(pPath);

        List<string> csFiles = new List<string>();
        foreach (var s in searchPath)
        {
            var fs = Directory.GetFiles(s, "*.cs*", SearchOption.AllDirectories).ToList();
            var _fs = fs.FindAll(f =>
            {
                if (!f.ToLower().Contains("editor")) //剔除editor
                {
                    return true;
                }

                return false;
            });

            csFiles.AddRange(_fs);
        }

        csFiles = csFiles.Distinct().ToList();
        for (int i = 0; i < csFiles.Count; i++)
        {
            csFiles[i] = csFiles[i].Replace('\\', '/').Trim();
        }

        EditorUtility.DisplayProgressBar("编译服务", "开始整理script", 0.2f);

        var baseCs = csFiles.FindAll(f => !f.Contains("@hotfix") && f.EndsWith(".cs"));
        var hotfixCs = csFiles.FindAll(f => f.Contains("@hotfix") && f.EndsWith(".cs"));

        #endregion

        //


        #region DLL引用搜集处理

        EditorUtility.DisplayProgressBar("编译服务", "收集依赖dll...", 0.3f);
        //这里是引入unity所有引用的dll

        var u3dExten = EditorApplication.applicationContentsPath + @"/UnityExtensions/Unity";
        var u3dEngine = EditorApplication.applicationContentsPath + @"/Managed/UnityEngine";
        //var monoaot = EditorApplication.applicationContentsPath + @"/MonoBleedingEdge/lib/mono/unityjit";

        var dllPathList = new List<string>() {u3dExten, u3dEngine, dataPath};
        var dllFiles = new List<string>();
        //
        foreach (var p in dllPathList)
        {
            if (Directory.Exists(p) == false)
            {
                EditorUtility.DisplayDialog("提示", "u3d根目录不存在,请修改ScriptBiuld_Service类中,u3dui 和u3dengine 的dll目录", "OK");
                return;
            }
        }


        foreach (var dp in dllPathList)
        {
            var dlls = Directory.GetFiles(dp, "*.dll", SearchOption.AllDirectories);
            foreach (var d in dlls)
            {
                var dllAbsPath = d.Replace(dp, "");
                if (dllAbsPath.Contains("Editor")
                    || dllAbsPath.Contains("iOS")
                    || dllAbsPath.Contains("Android"))
                {
                    continue;
                }

                dllFiles.Add(d);
            }
        }

        var monojit = EditorApplication.applicationContentsPath + @"/MonoBleedingEdge/lib/mono/unityjit";

        var _dlls = Directory.GetFiles(monojit, "*.dll", SearchOption.AllDirectories);

        var results = _dlls.ToList().FindAll((d) => d.Replace(monojit, "").Contains("Editor") == false
                                                    && d.Contains("System."));

        dllFiles.AddRange(results);
        //
        dllFiles = dllFiles.Distinct().ToList();
        //去除同名 重复的dll
        for (int i = 0; i < dllFiles.Count; i++)
        {
            dllFiles[i] = dllFiles[i].Replace('\\', '/').Trim();
            //
            var copyto = IPath.Combine(tempDirect, Path.GetFileName(dllFiles[i]));
            File.Copy(dllFiles[i], copyto, true);
            //File.WriteAllBytes(copyto,File.ReadAllBytes(dllFiles[i]));
            dllFiles[i] = copyto;
        }


        //添加系统依赖
        dllFiles.Add("mscorlib.dll");

        #endregion


        //

        EditorUtility.DisplayProgressBar("编译服务", "复制到临时环境...", 0.4f);

        //
        //全部拷贝到临时目录

        #region 为解决mono.exe error: 文件名太长问题

        int fileCounter = 1;
        csFilesMap = new Dictionary<int, string>();

        //拷贝 base cs
        for (int i = 0; i < baseCs.Count; i++)
        {
            var copyto = IPath.Combine(tempDirect, fileCounter + ".cs");
            //拷贝
            File.WriteAllText(copyto, File.ReadAllText(baseCs[i]));
            //保存文件索引
            csFilesMap[fileCounter] = baseCs[i];
            baseCs[i] = copyto.Replace("\\", "/");
            fileCounter++;
        }

        //拷贝 hotfix cs
        for (int i = 0; i < hotfixCs.Count; i++)
        {
            var copyto = IPath.Combine(tempDirect, fileCounter + ".cs");
            //拷贝
            File.WriteAllText(copyto, File.ReadAllText(hotfixCs[i]));
            //保存文件索引
            csFilesMap[fileCounter] = hotfixCs[i];
            //
            hotfixCs[i] = copyto.Replace("\\", "/");
            fileCounter++;
        }


        //检测dll
        for (int i = dllFiles.Count - 1; i >= 0; i--)
        {
            var r = dllFiles[i];
            if (File.Exists(r))
            {
                var fs = File.ReadAllBytes(r);
                try
                {
                    var assm = Assembly.Load(fs);
                }
                catch (Exception e)
                {
                    BDebug.Log("dll无效移除:" + r);
                    dllFiles.RemoveAt(i);
                }
            }
        }

        #endregion


        EditorUtility.DisplayProgressBar("编译服务", "[1/2]开始编译base.dll", 0.5f);

        //编译 base.dll
        try
        {
            Build(dllFiles.ToArray(), baseCs.ToArray(), baseDllPath);
        }
        catch (Exception e)
        {
            //EditorUtility.DisplayDialog("提示", "编译base.dll失败!", "OK");
            Debug.LogError(e.Message);
            EditorUtility.ClearProgressBar();
            return;
        }

        //

        EditorUtility.DisplayProgressBar("编译服务", "[2/2]开始编译hotfix.dll", 0.7f);
        var dependent = outDirectory + "/dependent";
        Directory.CreateDirectory(dependent);

        //将base.dll加入 
        dllFiles.Add(baseDllPath);
        //编译hotfix.dll
        var outHotfixDirectory = outPath + "/hotfix/hotfix.dll";
        try
        {
            Build(dllFiles.ToArray(), hotfixCs.ToArray(), outHotfixDirectory);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            EditorUtility.ClearProgressBar();
            return;
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayProgressBar("编译服务", "清理临时文件", 0.9f);
        Directory.Delete(tempDirect, true);
        EditorUtility.ClearProgressBar();
    }
#endif

    static public void BuildDLL_DotNet(string codeSource, string export)
    {
        string str1 = Application.dataPath;
        string str2 = Application.streamingAssetsPath;
        string exePath = Application.dataPath + "/" + "Code/BDFramework/Tools/ILRBuild/build.exe";
        if (File.Exists(exePath))
        {
            Debug.Log(".net编译工具存在!");
        }

        //这里是引入unity所有引用的dll
        var u3dUI = string.Format(@"""{0}\UnityExtensions\Unity\GUISystem""",
            EditorApplication.applicationContentsPath);
        var u3dEngine = string.Format(@"""{0}\Managed\UnityEngine""", EditorApplication.applicationContentsPath);

        if (Directory.Exists(u3dUI.Replace(@"""", "")) == false ||
            Directory.Exists(u3dEngine.Replace(@"""", "")) == false)
        {
            EditorUtility.DisplayDialog("提示", "u3d根目录不存在,请修改ScriptBiuld_Service类中,u3dui 和u3dengine 的dll目录", "OK");
            return;
        }

        //
        Process.Start(exePath, string.Format("{0} {1} {2} {3}", codeSource, export, u3dUI, u3dEngine));
    }


    /// <summary>
    /// 编译dll
    /// </summary>
    /// <param name="rootpaths"></param>
    /// <param name="output"></param>
    static public BuildStatus Build(string[] refAssemblies, string[] codefiles, string output)
    {
        // 设定编译参数,DLL代表需要引入的Assemblies
        CompilerParameters cp = new CompilerParameters();
        //
        cp.GenerateExecutable = false;
        //在内存中生成
        cp.GenerateInMemory = true;
        //生成调试信息
        cp.IncludeDebugInformation = true;
        cp.TempFiles = new TempFileCollection(".", true);
        cp.OutputAssembly = output;
        //warning和 error分开,不然各种warning当成error,改死你
        cp.TreatWarningsAsErrors = false;
        cp.WarningLevel = 1;
        //编译选项
        cp.CompilerOptions = "/optimize /unsafe";

        if (refAssemblies != null)
        {
            foreach (var d in refAssemblies)
            {
                cp.ReferencedAssemblies.Add(d);
            }
        }

        // 编译代理
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CompilerResults cr = provider.CompileAssemblyFromFile(cp, codefiles);


        if (true == cr.Errors.HasErrors)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
            {
                sb.Append(ce.ToString());
                sb.Append(System.Environment.NewLine);
            }

#if !UNITY_EDITOR
            Console.WriteLine(sb);

#else

            try
            {
                //log重新打出映射关系
                var lines = sb.ToString().Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    var l = lines[i];
                    var lcs = l.Split('(');
                    if (lcs.Length > 0)
                    {
                        var fn = Path.GetFileName(lcs[0]);

                        var findex = int.Parse(fn.Replace(".cs", ""));

                        Debug.LogError(l.Replace(fn, Path.GetFileName(csFilesMap[findex])));
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(sb);
                throw;
            }


#endif
        }
        else
        {
            return BuildStatus.Success;
        }


        return BuildStatus.Fail;
    }
}