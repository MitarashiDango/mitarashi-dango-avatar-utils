
using UnityEditor;
using UnityEngine;

namespace MitarashiDango.FaceEmoteControl
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(FaceEmote))]
    public class FaceEmotePropertyDrawer : PropertyDrawer
    {
        private readonly float PADDING_HEIGHT = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                var faceEmoteNameProperty = property.FindPropertyRelative("faceEmoteName");
                var faceEmoteProperty = property.FindPropertyRelative("faceEmote");
                var iconProperty = property.FindPropertyRelative("icon");

                var faceEmoteNameRect = new Rect(position)
                {
                    y = position.y + EditorGUIUtility.singleLineHeight + PADDING_HEIGHT,
                    height = EditorGUIUtility.singleLineHeight,
                    width = iconProperty.objectReferenceValue != null ? position.width - 68f : position.width
                };
                EditorGUI.PropertyField(faceEmoteNameRect, faceEmoteNameProperty, new GUIContent("表情名"));

                var faceEmoteRect = new Rect(faceEmoteNameRect)
                {
                    y = faceEmoteNameRect.y + EditorGUIUtility.singleLineHeight + PADDING_HEIGHT,
                    height = EditorGUIUtility.singleLineHeight
                };
                EditorGUI.PropertyField(faceEmoteRect, faceEmoteProperty, new GUIContent("表情アニメーション"));

                var iconRect = new Rect(faceEmoteRect)
                {
                    y = faceEmoteRect.y + EditorGUIUtility.singleLineHeight + PADDING_HEIGHT,
                    height = EditorGUIUtility.singleLineHeight
                };
                EditorGUI.PropertyField(iconRect, iconProperty, new GUIContent("アイコン"));

                if (iconProperty.objectReferenceValue != null)
                {
                    var iconViewRect = new Rect(position)
                    {
                        height = 64f,
                        width = 64f,
                        x = position.x + position.width - 64f,
                        y = position.y + EditorGUIUtility.singleLineHeight + PADDING_HEIGHT,
                    };
                    GUI.Box(iconViewRect, new GUIContent((Texture2D)iconProperty.objectReferenceValue));
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Mathf.Max((EditorGUIUtility.singleLineHeight + 2f) * 4, EditorGUIUtility.singleLineHeight + 68f);
        }
    }
#endif
}