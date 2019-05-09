using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

//[CustomEditor(typeof(Cell))]
public class CellInspector :Editor
{
   
    int eventindex = 0;
    private int currindex = 0;
    private CellEvent enevt1 = CellEvent.Run;

    //cell上的图片
    private Sprite spt1 = null;
    private Sprite spt2 = null;
  

    public override void OnInspectorGUI()
    {
        Cell  curCell = target as Cell;
     
        base.OnInspectorGUI();
      
       /* if(GUILayout.Button("test"))
        {
            // cEvent.type ->cEventeditor ->class
            //cEvent = class.OnInspectorGUI(cEvent);
        }*/

    //获取枚举的数据类型
        string[] arraytest = System.Enum.GetNames(enevt1.GetType());
        eventindex = EditorGUILayout.Popup("事件：",  curCell.cEvent.EventType, arraytest);
        
        //给下拉绑定方法
        if (eventindex != currindex)
        {
            //获取到选择的枚举指引
            currindex = eventindex;
         //将数据传入到cell
            curCell.cEvent.EventType = currindex;
          
            //target as Cell;
        }
       
        //获取参数
        curCell.cEvent.eventjson= EditorGUILayout.TextField("事件参数:",curCell.cEvent.eventjson, GUILayout.Width(300));

      //选择图片（选好图片，获取图片路径赋值给cell，） 
      spt1=EditorGUILayout.ObjectField("背景Icon", spt1, typeof(Sprite), true, GUILayout.Width(300)) as Sprite;
        string img1path = AssetDatabase.GetAssetPath(spt1);
        //如果图片被改动过，才进行赋值
        if (curCell.normalImgPath != AssetDatabase.GetAssetPath(spt1)&&!string .IsNullOrEmpty(img1path))
        {
            curCell.normalImgPath = AssetDatabase.GetAssetPath(spt1);
            var bgimg = AssetDatabase.LoadAssetAtPath(curCell.normalImgPath, typeof(Sprite)) as Sprite;
            curCell.transform.Find("cellbg").GetComponent<Image>().sprite =Instantiate<Sprite>(bgimg);
        }
        
        spt2=EditorGUILayout.ObjectField("物品Icon", spt2, typeof(Sprite), true, GUILayout.Width(300)) as Sprite;
        string img2path = AssetDatabase.GetAssetPath(spt2);
        if (curCell.itemImgPath != AssetDatabase.GetAssetPath(spt2)&&!string .IsNullOrEmpty(img2path))
        {
            curCell.itemImgPath = AssetDatabase.GetAssetPath(spt2);
            var itemimg = AssetDatabase.LoadAssetAtPath(curCell.itemImgPath, typeof(Sprite)) as Sprite;
            curCell.transform.Find("item").GetComponent<Image>().sprite = Instantiate<Sprite>(itemimg);
        }
    }
}
