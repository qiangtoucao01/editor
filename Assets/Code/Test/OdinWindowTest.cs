using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Demos.RPGEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ObjectFieldAlignment = Sirenix.OdinInspector.ObjectFieldAlignment;

public class OdinWindowTest : OdinEditorWindow
{
    private MapGrid mapGrid;
    
    public  static void Open()
    {
        var window = GetWindow<OdinWindowTest>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
    }
    
    [EnumToggleButtons]
    [InfoBox("Inherit from OdinEditorWindow instead of EditorWindow in order to create editor windows like you would inspectors - by exposing members and using attributes.")]
    public ViewTool SomeField;

    [Title("打开窗口")]//标题
    [VerticalGroup(PaddingTop = 10)]//和上面的间距
    [Sirenix.OdinInspector.Button(ButtonSizes.Large)]//控件格式
    public void FindPicByPath()
    {
      Debug.Log("你看到了吗");
    }
    
    //图片
    [Header("背景图片")]
    [OnInspectorGUI("GetPath", append: false)]
    [PreviewField(150,ObjectFieldAlignment.Right)]
    public Sprite bgImg;
    public void GetPath()
    {
      
        
    }
    

    public void GetPath11()
    {
        if (mapGrid == null)
        {
            mapGrid = GameObject.Find("map").GetComponent<MapGrid>();
        }
       // mapGrid.bgImg =Icon;
    }
 
    public void Path22()
    {
       
      
    }
    [Header("元素图片1")]
    [OnInspectorGUI("GetPath11", append: false)]
    [HideLabel, PreviewField(150, ObjectFieldAlignment.Left)]
    public Sprite Icon;
    [Header("元素图片2")]
    [OnInspectorGUI("Path22", append: false)]
    [HideLabel, PreviewField(150, ObjectFieldAlignment.Left)]
    public Sprite Icon1;
}
