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
        private SerializedProperty leftFaceEmoteGestureGroupNumber;
        private SerializedProperty rightFaceEmoteGestureGroupNumber;
        private SerializedProperty faceEmoteGroups;

        private void OnEnable()
        {
            defaultFaceMotion = serializedObject.FindProperty("defaultFaceMotion");
            time = serializedObject.FindProperty("time");
            faceEmoteGestureGroups = serializedObject.FindProperty("faceEmoteGestureGroups");
            leftFaceEmoteGestureGroupNumber = serializedObject.FindProperty("leftFaceEmoteGestureGroupNumber");
            rightFaceEmoteGestureGroupNumber = serializedObject.FindProperty("rightFaceEmoteGestureGroupNumber");
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

            var faceEmoteGestureGroupNames = new GUIContent[faceEmoteGestureGroups.arraySize + 1];
            faceEmoteGestureGroupNames[0] = new GUIContent("未割り当て");
            for (var i = 0; i < faceEmoteGestureGroups.arraySize; i++)
            {
                var elem = faceEmoteGestureGroups.GetArrayElementAtIndex(i);
                var groupName = "";
                if (elem?.objectReferenceValue != null)
                {
                    var faceEmoteGestureGroup = new SerializedObject(elem.objectReferenceValue);
                    groupName = faceEmoteGestureGroup.FindProperty("groupName")?.stringValue ?? "";
                }

                if (groupName == "")
                {
                    groupName = $"表情ジェスチャーグループ{i + 1}";
                }

                faceEmoteGestureGroupNames[i + 1] = new GUIContent($"{i + 1}:{groupName}");
            }

            EditorGUILayout.LabelField("デフォルトの表情ジェスチャーグループ割り当て設定");
            if (leftFaceEmoteGestureGroupNumber.intValue > faceEmoteGestureGroups.arraySize)
            {
                leftFaceEmoteGestureGroupNumber.intValue = 0;
            }

            if (rightFaceEmoteGestureGroupNumber.intValue > faceEmoteGestureGroups.arraySize)
            {
                rightFaceEmoteGestureGroupNumber.intValue = 0;
            }

            EditorGUI.indentLevel++;
            leftFaceEmoteGestureGroupNumber.intValue = EditorGUILayout.Popup(new GUIContent("左手"), leftFaceEmoteGestureGroupNumber.intValue, faceEmoteGestureGroupNames);
            rightFaceEmoteGestureGroupNumber.intValue = EditorGUILayout.Popup(new GUIContent("右手"), rightFaceEmoteGestureGroupNumber.intValue, faceEmoteGestureGroupNames);
            EditorGUI.indentLevel--;

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