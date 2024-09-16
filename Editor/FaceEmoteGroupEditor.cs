using UnityEditor;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    [DisallowMultipleComponent, CustomEditor(typeof(FaceEmoteGroup))]
    public class FaceEmoteGroupEditor : Editor
    {
        private SerializedProperty fist;
        private SerializedProperty handOpen;
        private SerializedProperty fingerPoint;
        private SerializedProperty victory;
        private SerializedProperty rockNRoll;
        private SerializedProperty handGun;
        private SerializedProperty thumbsUp;

        private void OnEnable()
        {
            fist = serializedObject.FindProperty("fist");
            handOpen = serializedObject.FindProperty("handOpen");
            fingerPoint = serializedObject.FindProperty("fingerPoint");
            victory = serializedObject.FindProperty("victory");
            rockNRoll = serializedObject.FindProperty("rockNRoll");
            handGun = serializedObject.FindProperty("handGun");
            thumbsUp = serializedObject.FindProperty("thumbsUp");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("表情設定", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            RenderGestureFaceEmoteItem("Fist", fist);
            RenderGestureFaceEmoteItem("Hand Open", handOpen);
            RenderGestureFaceEmoteItem("Finger Point", fingerPoint);
            RenderGestureFaceEmoteItem("Victory", victory);
            RenderGestureFaceEmoteItem("Rock N Roll", rockNRoll);
            RenderGestureFaceEmoteItem("Hand Gun", handGun);
            RenderGestureFaceEmoteItem("Thumbs Up", thumbsUp);

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
}
