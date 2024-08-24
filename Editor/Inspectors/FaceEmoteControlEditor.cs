
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MitarashiDango.FaceEmoteControl
{
#if UNITY_EDITOR
    [DisallowMultipleComponent, CustomEditor(typeof(FaceEmoteControl))]
    public class FaceEmoteControlEditor : Editor
    {
        private SerializedProperty defaultFaceAnimationClip;
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
        private bool isLeftHandFieldsOpen;
        private bool isRightHandFieldsOpen;

        private void OnEnable()
        {
            defaultFaceAnimationClip = serializedObject.FindProperty("defaultFaceAnimationClip");

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
                drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Additional Face Emotes"),
                elementHeightCallback = index => EditorGUI.GetPropertyHeight(additionalFaceEmotes.GetArrayElementAtIndex(index)) + EditorGUIUtility.standardVerticalSpacing * 2
            };

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("表情設定", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(defaultFaceAnimationClip, new GUIContent("Default Face"), false);

            isLeftHandFieldsOpen = EditorGUILayout.BeginFoldoutHeaderGroup(isLeftHandFieldsOpen, "Left hand");
            if (isLeftHandFieldsOpen)
            {
                RenderGestureFaceEmoteItem("Fist", leftFist);
                RenderGestureFaceEmoteItem("Hand Open", leftHandOpen);
                RenderGestureFaceEmoteItem("Finger Point", leftFingerPoint);
                RenderGestureFaceEmoteItem("Victory", leftVictory);
                RenderGestureFaceEmoteItem("Rock N Roll", leftRockNRoll);
                RenderGestureFaceEmoteItem("Hand Gun", leftHandGun);
                RenderGestureFaceEmoteItem("Thumbs Up", leftThumbsUp);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            isRightHandFieldsOpen = EditorGUILayout.BeginFoldoutHeaderGroup(isRightHandFieldsOpen, "Right hand");
            if (isRightHandFieldsOpen)
            {
                RenderGestureFaceEmoteItem("Fist", rightFist);
                RenderGestureFaceEmoteItem("Hand Open", rightHandOpen);
                RenderGestureFaceEmoteItem("Finger Point", rightFingerPoint);
                RenderGestureFaceEmoteItem("Victory", rightVictory);
                RenderGestureFaceEmoteItem("Rock N Roll", rightRockNRoll);
                RenderGestureFaceEmoteItem("Hand Gun", rightHandGun);
                RenderGestureFaceEmoteItem("Thumbs Up", rightThumbsUp);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

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