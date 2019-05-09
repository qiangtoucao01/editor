using Sirenix.OdinInspector;
using UnityEngine;

namespace Code
{
    public class OdinDemon:MonoBehaviour
    {
        /// <summary>
        /// 通过texture1获取图片，在方法中调用。
        /// </summary>
        [OnInspectorGUI("DrawPreview1", append: true)]
        public Sprite Texture1;

        private void DrawPreview1()
        {
            if (this.Texture1 == null) return;

            GUILayout.BeginVertical(GUI.skin.box);
            //GUILayout.Label(this.Texture1);
            GUILayout.EndVertical();
        }
    }
}