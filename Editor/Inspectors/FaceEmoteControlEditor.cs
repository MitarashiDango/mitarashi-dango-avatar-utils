
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
#if UNITY_EDITOR
    [DisallowMultipleComponent, CustomEditor(typeof(FaceEmoteControl))]
    public class FaceEmoteControlEditor : Editor
    {
        private SerializedProperty defaultFaceMotion;
        private SerializedProperty leftFist;
        private SerializedProperty leftHandOpen;
        private SerializedProperty leftFingerPoint;
        private SerializedProperty leftVictory;
        private SerializedProperty leftRockNRoll;
        private SerializedProperty leftHandGun;
        private SerializedProperty leftThumbsUp;
        private SerializedProperty rightFist;
        private SerializedProperty rightHandOpen;
        private SerializedProperty rightFingerPoint;
        private SerializedProperty rightVictory;
        private SerializedProperty rightRockNRoll;
        private SerializedProperty rightHandGun;
        private SerializedProperty rightThumbsUp;
        private SerializedProperty additionalFaceEmotes;

        private ReorderableList reorderableList;

        private void OnEnable()
        {
            defaultFaceMotion = serializedObject.FindProperty("defaultFaceMotion");

            leftFist = serializedObject.FindProperty("leftFist");
            leftHandOpen = serializedObject.FindProperty("leftHandOpen");
            leftFingerPoint = serializedObject.FindProperty("leftFingerPoint");
            leftVictory = serializedObject.FindProperty("leftVictory");
            leftRockNRoll = serializedObject.FindProperty("leftRockNRoll");
            leftHandGun = serializedObject.FindProperty("leftHandGun");
            leftThumbsUp = serializedObject.FindProperty("leftThumbsUp");

            rightFist = serializedObject.FindProperty("rightFist");
            rightHandOpen = serializedObject.FindProperty("rightHandOpen");
            rightFingerPoint = serializedObject.FindProperty("rightFingerPoint");
            rightVictory = serializedObject.FindProperty("rightVictory");
            rightRockNRoll = serializedObject.FindProperty("rightRockNRoll");
            rightHandGun = serializedObject.FindProperty("rightHandGun");
            rightThumbsUp = serializedObject.FindProperty("rightThumbsUp");

            additionalFaceEmotes = serializedObject.FindProperty("additionalFaceEmotes");

            reorderableList = new ReorderableList(serializedObject, additionalFaceEmotes)
            {
                drawElementCallback = (rect, index, active, focused) =>
                {
                    var additionalFaceEmote = additionalFaceEmotes.GetArrayElementAtIndex(index);
                    var position = new Rect(rect)
                    {
                        y = rect.y + EditorGUIUtility.standardVerticalSpacing
                    };
                    EditorGUI.PropertyField(position, additionalFaceEmote);
                },
                drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Additional Face Emotes", EditorStyles.boldLabel),
                elementHeightCallback = index => EditorGUI.GetPropertyHeight(additionalFaceEmotes.GetArrayElementAtIndex(index)) + EditorGUIUtility.standardVerticalSpacing * 2
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("表情設定", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(defaultFaceMotion, new GUIContent("Default Face"), false);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Left hand", EditorStyles.boldLabel);
            RenderGestureFaceEmoteItem("Left Fist", leftFist);
            RenderGestureFaceEmoteItem("Left Hand Open", leftHandOpen);
            RenderGestureFaceEmoteItem("Left Finger Point", leftFingerPoint);
            RenderGestureFaceEmoteItem("Left Victory", leftVictory);
            RenderGestureFaceEmoteItem("Left Rock N Roll", leftRockNRoll);
            RenderGestureFaceEmoteItem("Left Hand Gun", leftHandGun);
            RenderGestureFaceEmoteItem("Left Thumbs Up", leftThumbsUp);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Right hand", EditorStyles.boldLabel);
            RenderGestureFaceEmoteItem("Right Fist", rightFist);
            RenderGestureFaceEmoteItem("Right Hand Open", rightHandOpen);
            RenderGestureFaceEmoteItem("Right Finger Point", rightFingerPoint);
            RenderGestureFaceEmoteItem("Right Victory", rightVictory);
            RenderGestureFaceEmoteItem("Right Rock N Roll", rightRockNRoll);
            RenderGestureFaceEmoteItem("Right Hand Gun", rightHandGun);
            RenderGestureFaceEmoteItem("Right Thumbs Up", rightThumbsUp);

            EditorGUILayout.Space();

            reorderableList.DoLayoutList();

            if (EditorApplication.isPlaying)
            {
                return;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderGestureFaceEmoteItem(string gestureName, SerializedProperty faceEmote)
        {
            using (new EditorGUILayout.VerticalScope("Helpbox"))
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField(new GUIContent(gestureName));
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(faceEmote, false);
                    }
                }
            }
        }
    }
#endif
}