using System.Collections;
using System.Collections.Generic;
using Code;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.UIElements;
using Image = UnityEngine.UI.Image;

public class Cells
{
    public string normalImg;
    public string itemImg;
    public CellEvents cEvent;

    private CellEvent 事件;
}


/// <summary>
/// 事件类型（）
/// </summary>
public enum CellEvent
{
    GetItem = 0,
    Run,
}

/// <summary>
/// 具体事件数据结构
/// </summary>
public class CellEvents
{
    public int EventType;
    public string eventjson; //参数
}

//[MapEvent("CellEditor")]
public class Cell : MonoBehaviour
{
    [Header("背景路径")]
    public string normalImgPath;
    [Header("物品路径")]
    public string itemImgPath;
    public   CellEvents cEvent=new CellEvents(){EventType=0,eventjson = "-1"};

   
     
    private  CellEvent 事件;

    private string[] dropDownValues1 = null;

    private Image bgSprite=null;
    private Image ItemSprite=null;

    /// <summary>
    /// 当前角色id
    /// </summary>
    public int Id;
    
    //事件类型
    [Header("事件类型")]
    [OnInspectorGUI("GetEventType",append:true)]
    public CellEvent eventType;

    /// <summary>
    /// 获取事件类型
    /// </summary>
    public void GetEventType()
    {
        cEvent.EventType = (int)eventType;
    }
    
    
   //事件参数
    [Header("事件参数")]
    [OnInspectorGUI("GetEventParam", append: true)]
    public string  eventParams;

    /// <summary>
    /// 获取事件参数
    /// </summary>
    public void GetEventParam()
    {
        cEvent.eventjson = eventParams;
    }

   
   

    //图片
    [Header("背景图片")]
    [OnInspectorGUI("GetBgPath", append: false)]
    [PreviewField(150,ObjectFieldAlignment.Right)]
    public Sprite bgImg;

    public void GetBgPath()
    {
        if (bgImg != null)
        {
          
            //图片路径
            normalImgPath = AssetDatabase.GetAssetPath(bgImg)==""?normalImgPath:AssetDatabase.GetAssetPath(bgImg);
            //显示选择的图片
            // GUILayout.Label(bgImg.texture, GUILayout.Width(150), GUILayout.Height(150));
            
            if (bgSprite == null)
            {
                bgSprite =  transform.Find("cellbg").GetComponent<Image>();
            }
        
            var bgimgObj = AssetDatabase.LoadAssetAtPath(normalImgPath, typeof(Sprite)) as Sprite;
            if(bgimgObj!=null)
                bgSprite.sprite = Instantiate(bgimgObj);
           
        }
        else
        {
            normalImgPath = "";
        }
        
    }
   
    [Header("物品图片")]
    [OnInspectorGUI("GetItemPath", append: false )]
    [PreviewField(150,ObjectFieldAlignment.Right)]
    public Sprite ItemImg;

    /// <summary>
    /// 获取物品图片路径
    /// </summary>
    public void GetItemPath()
    {
        if (ItemImg != null)
        {
            itemImgPath = AssetDatabase.GetAssetPath(ItemImg)==""?itemImgPath:AssetDatabase.GetAssetPath(ItemImg);
           // GUILayout.Label(ItemImg.texture, GUILayout.Width(150), GUILayout.Height(150));
            
            if (ItemSprite == null)
            {
                ItemSprite = transform.Find("item").GetComponent<Image>();
            }
        
            var bgimgObj = AssetDatabase.LoadAssetAtPath(itemImgPath, typeof(Sprite)) as Sprite;
            if (bgimgObj != null)
            {
                ItemSprite.sprite = Instantiate<Sprite>(bgimgObj);
            }
        }
        else
        {
            itemImgPath = "";
        }
            
    }
    
    

    /// <summary>
    /// 寻找子物体组件，并记录组件资源的位置（不能使用默认图片）。
    /// </summary>
    /// <returns></returns>
    public Cells GetCells()
    {
        //获取自身的组件
        return new Cells(){normalImg=normalImgPath, itemImg = itemImgPath,cEvent =cEvent };
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
