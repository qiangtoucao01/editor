using UnityEditor;
using UnityEngine;

namespace Code
{
    
    public class MapEvent_CellEditor
    {
        private Sprite spt1 = null;
        public void Test()
        {
            EditorGUILayout.ObjectField("背景Icon", spt1, typeof(Sprite), true, GUILayout.Width(300));
         
        }
    }
}