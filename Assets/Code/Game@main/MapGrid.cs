using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Code;
using ILRuntime.Runtime;
using LitJson;
using Mono.Cecil;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
/// <summary>
/// 地图类型
/// </summary>
class Map
{
    public int x;
    public int y;
  
    public List<Cells> cells = new List<Cells>();
    public string bgimg;
}


/// <summary>
/// 主节点
/// </summary>
public class MapGrid : MonoBehaviour
{

    private int currRows=-1;
    private int currColumns=-1;
    
    [Header("行数")]
    [OnInspectorGUI("RowsConstraint",append:false)]
    public int x;

    public void RowsConstraint()
    {
        if (cellsTrans == null)
        {
            cellsTrans = transform.Find("cells");
        }

        //只要行数和列数发生变化就对模块进行重新加载
        if (currRows != x||currColumns!=y)
        {
            currRows = x;
            currColumns = y;

            //创建相应的数量cell模块
            var cellObj = AssetDatabase.LoadAssetAtPath(cellsPath, typeof(Object)) as Object;
            if (cellObj == null)
            {
                Debug.Log(cellsPath);
            }


            //销毁原有的子节点模块，便于直接重新加载模块
            int childcount = cellsTrans.childCount;
            for (int i = 0; i < childcount; i++) {  
                DestroyImmediate(cellsTrans.GetChild(0).gameObject);  
            }  
            
            int count = x * y;
            for (int i = 0; i < count; i++)
            {
                var obj = Instantiate(cellObj) as GameObject;
                obj.transform.SetParent(cellsTrans);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
            }

            //刷新场景
            AssetDatabase.Refresh();
        }
    }

    /// <summary>
    /// 网格组件
    /// </summary>
    private GridLayoutGroup gridLayoutGroup;
    
    [Header("列数")]
    [OnInspectorGUI("ColumnsConstraint",append:false)]
    public int y;
    public void ColumnsConstraint()
    {
        if (gridLayoutGroup == null)
        {
            gridLayoutGroup = transform.Find("cells").GetComponent<GridLayoutGroup>(); 
            gridLayoutGroup.constraint=GridLayoutGroup.Constraint.FixedColumnCount;
        }
       
        //限制列数
        gridLayoutGroup.constraintCount = y;

    }
    
    private  int mapCount=-1; //地图数量
    public List<Cell> cells = new List<Cell>();
    
    /// <summary>
    /// 数据保存的文件(根据该变量位置加载地图)
    /// </summary>
    public TextAsset MapText;
    public string bgimgPath; //背景路径
    private Image bgImage;//背景图片
    
    //地图文本的位置
   // string path1 = @"C:\Users\10649\Desktop\BDFramework.Core\Assets\Data\MapData.txt";
    //cell预制体的位置
     string cellsPath ="Assets/Resource/Runtime/Map/item/cell.prefab" ;

    /// <summary>
    /// 背景图片的变换
    /// </summary>
    private Transform imgbgTrans ;
    
    /// <summary>
    /// cells根节点变换
    /// </summary>
    private Transform cellsTrans;

  

    /// <summary>
    /// 当前地图文本地址
    /// </summary>
    private string CurrMapPath = string.Empty;
    
    /// <summary>
    /// 地图目录的路径(不包含\)
    /// </summary>
   // private string CurrMapDerectoryPath = @"C:\Users\10649\Desktop\BDFramework.Core\Assets\Resource\Runtime\Map\Data";
    private  string CurrMapDerectoryPath;

    /// <summary>
    /// 当前地图名称
    /// </summary>
    private string curMapName = null;
    
    /// <summary>
    /// 地图文件名称数组
    /// </summary>
    private static IEnumerable MapDataArr=new ValueDropdownList<string >() ;

    private void Awake()
    {
       
       CurrMapDerectoryPath =Application.dataPath+@"\Resource\Runtime\Map\Data";
      
    }

    //背景图片
    [Header("背景图片")]
    [OnInspectorGUI("GetBgPath", append: false)]
    [PreviewField(150,ObjectFieldAlignment.Right)]
    public Sprite bgImg;

    public void GetBgPath()
    {
      
        if (bgImg != null)
        {
          
            //图片路径
            bgimgPath = AssetDatabase.GetAssetPath(bgImg)==""?bgimgPath:AssetDatabase.GetAssetPath(bgImg);
       
            if (bgImage == null)
            {
                bgImage =  transform.Find("bg").GetComponent<Image>();
            }

            if (!bgimgPath.Contains("Assets/Resource/Runtime/Map/bg"))
            {
                Debug.LogError("图片路径不对，请从正确的文件夹选择"+bgimgPath);
               
            }
        
            //如果不从路径加载，可能会无法及时刷新
            var bgimgObj = AssetDatabase.LoadAssetAtPath(bgimgPath, typeof(Sprite)) as Sprite;
            if(bgimgObj!=null)
                bgImage.sprite = Instantiate(bgimgObj);
           
        }
        else
        {
            bgimgPath = "";
        }
        
    }
    
    [Header("地图名称")]
    [OnInspectorGUI("CreateMapDemo",append:true)]
    [ValueDropdown("GetMap", AppendNextDrawer = false)]
    public string  mapName;

    /// <summary>
    /// 获取地图数据文件名称
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> GetMap()
    {
        if (string.IsNullOrEmpty(CurrMapDerectoryPath))
        {
            CurrMapDerectoryPath=Application.dataPath+@"\Resource\Runtime\Map\Data";
        }
       
        return GetFile();
    }


    /// <summary>
    /// 获取data路径下的地图文件名称
    /// </summary>
    /// <returns></returns>
    public string[]  GetFile()
    {
        int index = 0;
        //获取文件信息+文件内容
       // FileInfo info=new FileInfo(@"C:\Users\10649\Desktop\BDFramework.Core\Assets\Resource\Runtime\Map\Data\");

        DirectoryInfo info1=new DirectoryInfo(CurrMapDerectoryPath+"\\");
       
        //目录下所有的文件
        FileInfo[] infos1 = info1.GetFiles();
        
        //地图的数量
        mapCount = (int)(infos1.Length/2);

        string[] arr = new string[mapCount];
        
        for (int i = 0; i < infos1.Length; i++)
        {
            string name = infos1[i].Name;
            if(name.Contains(".meta"))
                continue;
            name = name.Substring(0,name.IndexOf("."));
            //往map数组添加地图名称。
           
            arr[index]=name;
            index++;
        }
        return arr;
    }
    
    /// <summary>
    /// 创建新地图的按钮
    /// </summary>
    [Title("创建新地图文件")]
    [VerticalGroup(PaddingTop = 10)]
    [Sirenix.OdinInspector.Button(ButtonSizes.Large)] 
    public void CreateNewMap()
    {
        
        if (string.IsNullOrEmpty(CurrMapDerectoryPath))
        {
            CurrMapDerectoryPath=Application.dataPath+@"\Resource\Runtime\Map\Data";
        }
        
        string newName = newMapName;
        //创建新的地图数据
        if (newName == ""&&mapCount!=-1)
        {
            newName = string.Format("0{0}",mapCount+1);
        }
        //新地图的路径
        string newPath = CurrMapDerectoryPath+"\\"+newName+".txt";

        //重置地图名称，
        NewMapName = "";
        
        //创建路径的文件,如果不存在
       // if (!Directory.Exists(CurrMapDerectoryPath))
       // {
        //    Directory.CreateDirectory(CurrMapDerectoryPath);
       // }
        
        //创建该路径的文件
        FileStream fs = new FileStream(newPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        
        fs.Close();
        //刷新一遍。
        AssetDatabase.Refresh();
    }


    /// <summary>
    /// 根据下拉框产生新的地图
    /// </summary>
    public void CreateMapDemo()
    {
        //如果没有修改地图名称，就不重新加载
        if (curMapName != mapName)
        {
           
            curMapName = mapName;
            //地图路径
            CurrMapPath =CurrMapDerectoryPath+"\\" + mapName +".txt";

            //根据路径加载地图demo
           LoadAMap(CurrMapPath);
            
           
        }
        
    }

    /// <summary>
    /// 根据地图数据文件路径，加载地图
    /// </summary>
    /// <param name="path"></param>
    public void LoadAMap(string path)
    {
        //读取路径的文件内容
        var json1= File.ReadAllText(path);

        Map map =new Map();
        //转换成map
        var json2 = JsonMapper.ToObject<Map>(json1);
        
        if(json2!=null )
         map = json2;
        
        if (map == null)
        {
            Debug.Log("这个地图啥也没有啊，老铁.只能获取一份默认套餐了");
        }

        //对行数和列数进行赋值，并同时对当前的行列赋值
        x = map.x;
        y = map.y;

        //设置当前的行列数，防止再次修改地图
        currRows = x;
        currColumns = y;

        //默认的背景地图路径
        if (map.bgimg == ""||map.bgimg ==null)
        {
            map.bgimg = "Assets/Resource/Imge/Test/timg (1).jpg";
        }

        //地图背景
        var imgbg= AssetDatabase.LoadAssetAtPath(map.bgimg,typeof( Sprite))as Sprite;
   
        var img  =Object.Instantiate<Sprite>(imgbg) ;
        //进行赋值
        transform.Find("bg").GetComponent<Image>().sprite = img;
        
        //地图模块
        int cellsCount = map.cells.Count;

        //获取cell预制体
        var cellObj=AssetDatabase.LoadAssetAtPath(cellsPath,typeof(Object))as Object;
        if (cellObj == null)
        {
            Debug.Log(cellsPath);
        }
        //找到父节点
        if(cellsTrans==null )
        cellsTrans = this.transform.Find("cells");
       
        //销毁原有的子节点模块，便于直接重新加载模块
        int childcount = cellsTrans.childCount;
        for (int i = 0; i < childcount; i++) {  
            DestroyImmediate(cellsTrans.GetChild(0).gameObject);  
        }  
        
        //循环加载cell并赋值
        for (int i = 0; i < cellsCount; i++)
        {
            CreateObj(map,cellObj,i);
        }
        //刷新场景
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 按钮是控件名称，方法名GenMap是text显示
    /// </summary>
    [Title("产生地图模型")]
    [VerticalGroup(PaddingTop = 10)]
    [Sirenix.OdinInspector.Button(ButtonSizes.Large)]
    public void GenMap()
    {
        if (MapText != null)
        {
            //获取map地图数据地址，读取txt文件
            CurrMapPath = AssetDatabase.GetAssetPath(MapText);
            LoadAMap(CurrMapPath);
        }
        else
        {
            Debug.Log("根本就没有文件，上哪造地图去");
        }
    }

    /// <summary>
    /// 创建cell并赋值
    /// </summary>
    /// <param name="map">map数据类</param>
    /// <param name="cellObj">cell预制体</param>
    /// <param name="index">当前模块所有</param>
    private void CreateObj(Map map,Object cellObj,int index)
    {
        //加载模块并放置位置
        var obj = Instantiate(cellObj) as GameObject;
        obj.transform .SetParent(cellsTrans);
        obj.transform .localPosition=Vector3.zero;
        obj.transform .localScale=Vector3.one;
            
        //进行图片信息赋值
        var cellimg = AssetDatabase.LoadAssetAtPath(map.cells[index].normalImg, typeof(Sprite)) as Sprite;

        Sprite sprBg = Instantiate<Sprite>(cellimg);
        //背景图
        obj.transform.Find("cellbg").GetComponent<Image>().sprite =sprBg;
            
        var itemimg = AssetDatabase.LoadAssetAtPath(map.cells[index].itemImg, typeof(Sprite)) as Sprite;
        
        Sprite sprImg = Instantiate<Sprite>(itemimg);
        //物品图
        obj.transform.Find("item").GetComponent<Image>().sprite = sprImg;

        //内容赋值
        Cell currCell = obj.GetComponent<Cell>();

        //给事件的参数和类型赋值
        if( map.cells[index].cEvent!=null)
        currCell.cEvent = map.cells[index].cEvent;

        currCell.eventType = (CellEvent)currCell.cEvent.EventType;
        currCell.eventParams = currCell.cEvent.eventjson;
        
        //图片
        currCell.bgImg = sprBg;
        currCell.ItemImg = sprImg;
        
        //图片路径
        currCell.normalImgPath = map.cells[index].normalImg;
        currCell.itemImgPath = map.cells[index].itemImg;

        //获取id
        currCell.Id = index + 1;
       

    }

    [Title("写入当前地图数据")]//标题
    [VerticalGroup(PaddingTop = 10)]//和上面的间距
    [Sirenix.OdinInspector.Button(ButtonSizes.Large)]//控件格式
    public void CreateMapFile()
    {
         //角色的信息
        int levelid = -1;
        int curpos = 8; 
        List<int> list = new List<int>();
        
        //地图信息
       var str = GetMapJson();
        
        //写入text。
      using(  FileStream fs=new FileStream(CurrMapPath,FileMode.Truncate,FileAccess.ReadWrite))
      {
        byte[] buffer=  Encoding.Default.GetBytes(str);
          fs.Write(buffer,0,buffer.Length);
      }
    }
    
    /// <summary>
    /// 记录地图模块数据并转换成json
    /// </summary>
    /// <returns></returns>
    public string GetMapJson()
    {
        var map = new Map();

        //地图行列数
        map.x = x;
        map.y = y;
      
        //背景地图的路径
        map.bgimg = bgimgPath;
       
        var cells =cellsTrans.GetComponentsInChildren<Cell>();

        //获取每个cell的图片+事件。
        foreach (var c in cells)
        {
            map.cells.Add((c.GetCells()));
        }
        
        //读取先前的json文件，没有修改过的地方一律采用原先的数值
        var json1= File.ReadAllText(CurrMapPath);
        //转换成map
        Map map1 = JsonMapper.ToObject<Map>(json1);

        if (map1 != null)
        {
            //比对两种数据，只修改改变的值。没有改变的值还是使用之前的值
            int count = map.cells.Count;
            for (int i = 0; i < count; i++)
            {
                Cells cell = map.cells[i];

                //可能大于原有的cell数量，就不进行比较
                if (i > map1.cells.Count - 1) break;
                Cells cell1 = map1.cells[i];

                if (string.IsNullOrEmpty(cell.itemImg))
                {
                    cell.itemImg = cell1.itemImg;
                }

                if (string.IsNullOrEmpty(cell.normalImg))
                {
                    cell.normalImg = cell1.normalImg;
                }
            }

            if (string.IsNullOrEmpty(map.bgimg))
            {
                map.bgimg = map1.bgimg;
            }
        }

        return JsonMapper.ToJson(map);
    }

   
    //事件参数
    [Header("新地图名称")]
    [OnInspectorGUI("GetMapNewName", append: true)]
    public string  NewMapName;

    private string newMapName;
    public void GetMapNewName()
    {
        newMapName = NewMapName;
    }
    
    [Title("打开窗口")]//标题
    [VerticalGroup(PaddingTop = 10)]//和上面的间距
    [Sirenix.OdinInspector.Button(ButtonSizes.Large)]//控件格式
    public void FindPicByPath()
    {
       
        //这里打开所有的图片进行展示
        OdinWindowTest.Open();
    }
    


    /// <summary>
    /// 使用当前的的地图数据作为基础写入新的地图文件
    /// </summary>
    public void CreateNewMapByOldMap()
    {
        string newName = newMapName;
        //创建新的地图数据
        if (newName == ""&&mapCount!=-1)
        {
            newName = string.Format("00{0}",mapCount);
        }
        //新地图的路径
        string newPath = CurrMapDerectoryPath+"\\"+newName+".txt";

        //重置地图名称，
        NewMapName = "";
        
        var str = GetMapJson();
        
        //写入text。
        using(  FileStream fs=new FileStream(newPath,FileMode.Truncate,FileAccess.ReadWrite))
        {
            byte[] buffer=  Encoding.Default.GetBytes(str);
            fs.Write(buffer,0,buffer.Length);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

  
}