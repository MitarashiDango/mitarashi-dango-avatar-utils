
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
        private SerializedProperty leftFistAnimationClip;
        private SerializedProperty leftFistRadialMenuIcon;
        private SerializedProperty leftHandOpenAnimationClip;
        private SerializedProperty leftHandOpenRadialMenuIcon;
        private SerializedProperty leftFingerPointAnimationClip;
        private SerializedProperty leftFingerPointRadialMenuIcon;
        private SerializedProperty leftVictoryAnimationClip;
        private SerializedProperty leftVictoryRadialMenuIcon;
        private SerializedProperty leftRockNRollAnimationClip;
        private SerializedProperty leftRockNRollRadialMenuIcon;
        private SerializedProperty leftHandGunAnimationClip;
        private SerializedProperty leftHandGunRadialMenuIcon;
        private SerializedProperty leftThumbsUpAnimationClip;
        private SerializedProperty leftThumbsUpRadialMenuIcon;
        private SerializedProperty rightFistAnimationClip;
        private SerializedProperty rightFistRadialMenuIcon;
        private SerializedProperty rightHandOpenAnimationClip;
        private SerializedProperty rightHandOpenRadialMenuIcon;
        private SerializedProperty rightFingerPointAnimationClip;
        private SerializedProperty rightFingerPointRadialMenuIcon;
        private SerializedProperty rightVictoryAnimationClip;
        private SerializedProperty rightVictoryRadialMenuIcon;
        private SerializedProperty rightRockNRollAnimationClip;
        private SerializedProperty rightRockNRollRadialMenuIcon;
        private SerializedProperty rightHandGunAnimationClip;
        private SerializedProperty rightHandGunRadialMenuIcon;
        private SerializedProperty rightThumbsUpAnimationClip;
        private SerializedProperty rightThumbsUpRadialMenuIcon;
        private SerializedProperty additionalFaceEmotes;
        private SerializedProperty additionalFaceRadialMenuIcons;

        private ReorderableList reorderableList;
        private bool isLeftHandFieldsOpen;
        private bool isRightHandFieldsOpen;

        private void OnEnable()
        {
            defaultFaceAnimationClip = serializedObject.FindProperty("defaultFaceAnimationClip");

            leftFistAnimationClip = serializedObject.FindProperty("leftFistAnimationClip");
            leftFistRadialMenuIcon = serializedObject.FindProperty("leftFistRadialMenuIcon");

            leftHandOpenAnimationClip = serializedObject.FindProperty("leftHandOpenAnimationClip");
            leftHandOpenRadialMenuIcon = serializedObject.FindProperty("leftHandOpenRadialMenuIcon");

            leftFingerPointAnimationClip = serializedObject.FindProperty("leftFingerPointAnimationClip");
            leftFingerPointRadialMenuIcon = serializedObject.FindProperty("leftFingerPointRadialMenuIcon");

            leftVictoryAnimationClip = serializedObject.FindProperty("leftVictoryAnimationClip");
            leftVictoryRadialMenuIcon = serializedObject.FindProperty("leftVictoryRadialMenuIcon");

            leftRockNRollAnimationClip = serializedObject.FindProperty("leftRockNRollAnimationClip");
            leftRockNRollRadialMenuIcon = serializedObject.FindProperty("leftRockNRollRadialMenuIcon");

            leftHandGunAnimationClip = serializedObject.FindProperty("leftHandGunAnimationClip");
            leftHandGunRadialMenuIcon = serializedObject.FindProperty("leftHandGunRadialMenuIcon");

            leftThumbsUpAnimationClip = serializedObject.FindProperty("leftThumbsUpAnimationClip");
            leftThumbsUpRadialMenuIcon = serializedObject.FindProperty("leftThumbsUpRadialMenuIcon");

            rightFistAnimationClip = serializedObject.FindProperty("rightFistAnimationClip");
            rightFistRadialMenuIcon = serializedObject.FindProperty("rightFistRadialMenuIcon");

            rightHandOpenAnimationClip = serializedObject.FindProperty("rightHandOpenAnimationClip");
            rightHandOpenRadialMenuIcon = serializedObject.FindProperty("rightHandOpenRadialMenuIcon");

            rightFingerPointAnimationClip = serializedObject.FindProperty("rightFingerPointAnimationClip");
            rightFingerPointRadialMenuIcon = serializedObject.FindProperty("rightFingerPointRadialMenuIcon");

            rightVictoryAnimationClip = serializedObject.FindProperty("rightVictoryAnimationClip");
            rightVictoryRadialMenuIcon = serializedObject.FindProperty("rightVictoryRadialMenuIcon");

            rightRockNRollAnimationClip = serializedObject.FindProperty("rightRockNRollAnimationClip");
            rightRockNRollRadialMenuIcon = serializedObject.FindProperty("rightRockNRollRadialMenuIcon");

            rightHandGunAnimationClip = serializedObject.FindProperty("rightHandGunAnimationClip");
            rightHandGunRadialMenuIcon = serializedObject.FindProperty("rightHandGunRadialMenuIcon");

            rightThumbsUpAnimationClip = serializedObject.FindProperty("rightThumbsUpAnimationClip");
            rightThumbsUpRadialMenuIcon = serializedObject.FindProperty("rightThumbsUpRadialMenuIcon");

            additionalFaceEmotes = serializedObject.FindProperty("additionalFaceEmotes");

            reorderableList = new ReorderableList(serializedObject, additionalFaceEmotes)
            {
                drawElementCallback = (rect, index, active, focused) =>
                {
                    var additionalFaceEmote = additionalFaceEmotes.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, additionalFaceEmote);
                },
                drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Additional Face Emotes"),
                elementHeightCallback = index => EditorGUI.GetPropertyHeight(additionalFaceEmotes.GetArrayElementAtIndex(index))
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
                RenderGestureFaceEmoteItem("Fist", leftFistAnimationClip, leftFistRadialMenuIcon);
                RenderGestureFaceEmoteItem("Hand Open", leftHandOpenAnimationClip, leftHandOpenRadialMenuIcon);
                RenderGestureFaceEmoteItem("Finger Point", leftFingerPointAnimationClip, leftFingerPointRadialMenuIcon);
                RenderGestureFaceEmoteItem("Victory", leftVictoryAnimationClip, leftVictoryRadialMenuIcon);
                RenderGestureFaceEmoteItem("Rock N Roll", leftRockNRollAnimationClip, leftRockNRollRadialMenuIcon);
                RenderGestureFaceEmoteItem("Hand Gun", leftHandGunAnimationClip, leftHandGunRadialMenuIcon);
                RenderGestureFaceEmoteItem("Thumbs Up", leftThumbsUpAnimationClip, leftThumbsUpRadialMenuIcon);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            isRightHandFieldsOpen = EditorGUILayout.BeginFoldoutHeaderGroup(isRightHandFieldsOpen, "Right hand");
            if (isRightHandFieldsOpen)
            {
                RenderGestureFaceEmoteItem("Fist", rightFistAnimationClip, rightFistRadialMenuIcon);
                RenderGestureFaceEmoteItem("Hand Open", rightHandOpenAnimationClip, rightHandOpenRadialMenuIcon);
                RenderGestureFaceEmoteItem("Finger Point", rightFingerPointAnimationClip, rightFingerPointRadialMenuIcon);
                RenderGestureFaceEmoteItem("Victory", rightVictoryAnimationClip, rightVictoryRadialMenuIcon);
                RenderGestureFaceEmoteItem("Rock N Roll", rightRockNRollAnimationClip, rightRockNRollRadialMenuIcon);
                RenderGestureFaceEmoteItem("Hand Gun", rightHandGunAnimationClip, rightHandGunRadialMenuIcon);
                RenderGestureFaceEmoteItem("Thumbs Up", rightThumbsUpAnimationClip, rightThumbsUpRadialMenuIcon);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            reorderableList.DoLayoutList();
            // EditorGUILayout.PropertyField(additionalFaceEmotes, new GUIContent("Additional faces"), true);

            if (EditorApplication.isPlaying)
            {
                return;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderGestureFaceEmoteItem(string gestureName, SerializedProperty animationClip, SerializedProperty icon)
        {
            using (new EditorGUILayout.VerticalScope("Helpbox"))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.LabelField(new GUIContent(gestureName));
                        using (new EditorGUI.IndentLevelScope())
                        {
                            EditorGUILayout.PropertyField(animationClip, new GUIContent("Animation Clip"));
                            EditorGUILayout.PropertyField(icon, new GUIContent("Icon"));
                        }
                    }

                    if (icon.objectReferenceValue != null)
                    {
                        GUILayout.Box(new GUIContent((Texture2D)icon.objectReferenceValue), GUILayout.Height(64), GUILayout.Width(64));
                    }
                }
            }
        }
    }
#endif
}