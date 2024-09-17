using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    [DisallowMultipleComponent, CustomEditor(typeof(FaceEmoteGroup))]
    public class FaceEmoteGroupEditor : Editor
    {
        private SerializedProperty groupName;
        private SerializedProperty faceEmotes;

        private ReorderableList reorderableList;

        private void OnEnable()
        {
            groupName = serializedObject.FindProperty("groupName");
            faceEmotes = serializedObject.FindProperty("faceEmotes");

            reorderableList = new ReorderableList(serializedObject, faceEmotes)
            {
                drawElementCallback = (rect, index, active, focused) =>
                {
                    var additionalFaceEmote = faceEmotes.GetArrayElementAtIndex(index);
                    var position = new Rect(rect)
                    {
                        y = rect.y + EditorGUIUtility.standardVerticalSpacing
                    };
                    EditorGUI.PropertyField(position, additionalFaceEmote);
                },
                drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Face Emotes", EditorStyles.boldLabel),
                elementHeightCallback = index => EditorGUI.GetPropertyHeight(faceEmotes.GetArrayElementAtIndex(index)) + EditorGUIUtility.standardVerticalSpacing * 2
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("表情グループ設定", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(groupName, new GUIContent("グループ名"), false);
            EditorGUILayout.Space();

            reorderableList.DoLayoutList();

            if (EditorApplication.isPlaying)
            {
                return;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
