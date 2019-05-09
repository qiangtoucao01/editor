using UnityEditor;
using UnityEngine;

namespace Code
{
    public class NewTest
    {
        [MenuItem("新Tools/宏设置")]
        public static void Settings()
        {
            MapData win = (MapData)EditorWindow.GetWindow(typeof(MapData));
            win.titleContent = new GUIContent("全局设置");
            win.Show();
        }
    }
}