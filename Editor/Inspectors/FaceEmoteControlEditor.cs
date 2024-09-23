using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    [DisallowMultipleComponent, CustomEditor(typeof(FaceEmoteControl))]
    public class FaceEmoteControlEditor : Editor
    {
        private SerializedProperty defaultFaceMotion;
        private SerializedProperty time;
        private SerializedProperty leftFaceEmoteGestureGroup;
        private SerializedProperty rightFaceEmoteGestureGroup;
        private SerializedProperty faceEmoteGroups;

        private void OnEnable()
        {
            defaultFaceMotion = serializedObject.FindProperty("defaultFaceMotion");

            time = serializedObject.FindProperty("time");

            leftFaceEmoteGestureGroup = serializedObject.FindProperty("leftFaceEmoteGestureGroup");

            rightFaceEmoteGestureGroup = serializedObject.FindProperty("rightFaceEmoteGestureGroup");

            faceEmoteGroups = serializedObject.FindProperty("faceEmoteGroups");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("表情設定", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(defaultFaceMotion, new GUIContent("Default Face"), false);

            EditorGUILayout.PropertyField(time, new GUIContent("Time"), false);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Left hand", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(leftFaceEmoteGestureGroup, new GUIContent("表情ジェスチャーグループ"), false);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Right hand", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(rightFaceEmoteGestureGroup, new GUIContent("表情ジェスチャーグループ"), false);
            }

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