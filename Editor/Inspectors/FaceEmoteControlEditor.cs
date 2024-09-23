using UnityEditor;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    [DisallowMultipleComponent, CustomEditor(typeof(FaceEmoteControl))]
    public class FaceEmoteControlEditor : Editor
    {
        private SerializedProperty defaultFaceMotion;
        private SerializedProperty time;
        private SerializedProperty faceEmoteGestureGroups;
        private SerializedProperty faceEmoteGroups;

        private void OnEnable()
        {
            defaultFaceMotion = serializedObject.FindProperty("defaultFaceMotion");

            time = serializedObject.FindProperty("time");

            faceEmoteGestureGroups = serializedObject.FindProperty("faceEmoteGestureGroups");

            faceEmoteGroups = serializedObject.FindProperty("faceEmoteGroups");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("表情設定", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(defaultFaceMotion, new GUIContent("Default Face"), false);

            EditorGUILayout.PropertyField(time, new GUIContent("Time"), false);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(faceEmoteGestureGroups, new GUIContent("表情ジェスチャーグループ"));

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(faceEmoteGroups, new GUIContent("表情グループ"));

            if (EditorApplication.isPlaying)
            {
                return;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}