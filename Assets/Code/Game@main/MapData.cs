using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Code
{
    public class MapData:EditorWindow
    {
        [Header("事件参数")]
        [OnInspectorGUI("GetEventParam", append: true)]
        public string  eventParams;
     
        private void OnGUI()
        {
            GetSprite();
            
        }

        private MapEvent_CellEditor currMapEvent_CellEditor = new MapEvent_CellEditor();
        private void GetSprite()
        {
            currMapEvent_CellEditor.Test();
            
           
        }
    }
}