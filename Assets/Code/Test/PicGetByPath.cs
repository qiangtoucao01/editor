using System.Collections;
using System.Collections.Generic;

   using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class PicGetByPath : MonoBehaviour
{
    //根据目录找到所有的图片路径，读出每个路径的字节流。Texture2D。LoadImage（字节流）转换成Texture2D图片，Sprite.Create成Sprite。
    
    public GameObject StoreObj;

    /// <summary>
    /// 挂点
    /// </summary>
    public Transform CellParent;

    // 储存获取到的图片  
    List<Texture2D> allTex2d = new List<Texture2D>();

    // Use this for initialization  
    void Start()
    {
        load();
        for(int i = 0; i < allTex2d.Count; i++)
        {
            //加载物体
            GameObject temp= GameObject.Instantiate(StoreObj, StoreObj.transform.position, Quaternion.identity);
           
            
            //设置挂点
            temp.GetComponent<Transform>().SetParent(CellParent);
            temp.transform .localPosition=Vector3.zero;
            temp.transform .localScale=Vector3.one;
            //创建图片
            Sprite sprite = Sprite.Create(allTex2d[i], new Rect(0, 0, allTex2d[i].width, allTex2d[i].height), new Vector2(0.5f, 0.5f));
            //赋值图片
            temp.GetComponent<Image>().sprite = sprite;
            //物体取名
            temp.transform.name = "Element" + i;
            temp.SetActive(true);
        }
        StoreObj.SetActive(true);
    }

    public void GetPic()
    {
        load();
        for(int i = 0; i < allTex2d.Count; i++)
        {
            //加载物体
            GameObject temp= GameObject.Instantiate(StoreObj, StoreObj.transform.position, Quaternion.identity);
            //设置挂点
            temp.GetComponent<Transform>().SetParent(StoreObj.transform.parent);
            //创建图片
            Sprite sprite = Sprite.Create(allTex2d[i], new Rect(0, 0, allTex2d[i].width, allTex2d[i].height), new Vector2(0.5f, 0.5f));
            //赋值图片
            temp.GetComponent<Image>().sprite = sprite;
            //物体取名
            temp.transform.name = "Element" + i;
        }
        //StoreObj.SetActive(false);
    }


    //读取目录的图片字节流，并保存成texture2D
   public  void load()
    {
        List<string> filePaths = new List<string>();
        string imgtype = "*.BMP|*.JPG|*.GIF|*.PNG";
        string[] ImageType = imgtype.Split('|');
        for (int i = 0; i < ImageType.Length; i++)
        {
            //获取Application.dataPath文件夹下所有的图片路径  
            string[] dirs = Directory.GetFiles(("Assets/Resource/Runtime/Map/bg"), ImageType[i]);
            for (int j = 0; j < dirs.Length; j++)
            {
                filePaths.Add(dirs[j]);
            }
        }
        
        for (int i = 0; i < filePaths.Count; i++)
        {
           
            Texture2D tx = new Texture2D(100, 100);
            tx.LoadImage(getImageByte(filePaths[i]));
            allTex2d.Add(tx);
        }
      
    }

    /// <summary>  
    /// 根据图片路径返回图片的字节流byte[] （读取文件的字节流） 
    /// </summary>  
    /// <param name="imagePath">图片路径</param>  
    /// <returns>返回的字节流</returns>  
   public  byte[] getImageByte(string imagePath)
    {
        FileStream files = new FileStream(imagePath, FileMode.Open);
        byte[] imgByte = new byte[files.Length];
        files.Read(imgByte, 0, imgByte.Length);
        files.Close();
        return imgByte;
    }

}

