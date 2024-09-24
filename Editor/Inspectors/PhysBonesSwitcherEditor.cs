using UnityEditor;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    [DisallowMultipleComponent, CustomEditor(typeof(PhysBonesSwitcher))]
    public class PhysBonesSwitcherEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox("本コンポーネントには設定項目はありません", MessageType.Info);

            if (EditorApplication.isPlaying)
            {
                return;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
