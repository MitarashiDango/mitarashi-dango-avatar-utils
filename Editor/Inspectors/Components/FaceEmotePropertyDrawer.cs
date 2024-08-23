
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
                var nameProperty = property.FindPropertyRelative("name");
                var animationClipProperty = property.FindPropertyRelative("animationClip");
                var iconProperty = property.FindPropertyRelative("icon");

                var nameRect = new Rect(position)
                {
                    y = position.y + PADDING_HEIGHT,
                    height = EditorGUIUtility.singleLineHeight,
                    width = iconProperty.objectReferenceValue != null ? position.width - 68f : position.width
                };
                EditorGUI.PropertyField(nameRect, nameProperty, new GUIContent("表情名"));

                var animationClipRect = new Rect(nameRect)
                {
                    y = nameRect.y + EditorGUIUtility.singleLineHeight + PADDING_HEIGHT,
                    height = EditorGUIUtility.singleLineHeight
                };
                EditorGUI.PropertyField(animationClipRect, animationClipProperty, new GUIContent("表情アニメーション"));

                var iconRect = new Rect(animationClipRect)
                {
                    y = animationClipRect.y + EditorGUIUtility.singleLineHeight + PADDING_HEIGHT,
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
                        y = position.y + PADDING_HEIGHT,
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