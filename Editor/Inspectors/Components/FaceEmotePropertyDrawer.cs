
using UnityEditor;
using UnityEngine;

namespace MitarashiDango.FaceEmoteControl
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(FaceEmote))]
    public class FaceEmotePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (var scope = new EditorGUI.PropertyScope(position, label, property))
            {
                var nameProperty = property.FindPropertyRelative("name");
                var animationClipProperty = property.FindPropertyRelative("animationClip");
                var iconProperty = property.FindPropertyRelative("icon");

                var nameRect = new Rect(position)
                {
                    height = EditorGUIUtility.singleLineHeight,
                    width = iconProperty.objectReferenceValue != null ? position.width - 66f : position.width
                };
                EditorGUI.PropertyField(nameRect, nameProperty, new GUIContent("表情名"));

                var animationClipRect = new Rect(nameRect)
                {
                    y = nameRect.y + nameRect.height + EditorGUIUtility.standardVerticalSpacing,
                    height = EditorGUIUtility.singleLineHeight
                };
                EditorGUI.PropertyField(animationClipRect, animationClipProperty, new GUIContent("表情アニメーション"));

                var iconRect = new Rect(animationClipRect)
                {
                    y = animationClipRect.y + animationClipRect.height + EditorGUIUtility.standardVerticalSpacing,
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
                        y = position.y,
                    };

                    GUI.Box(iconViewRect, new GUIContent((Texture2D)iconProperty.objectReferenceValue));
                    return;
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var iconProperty = property.FindPropertyRelative("icon");
            if (iconProperty != null && iconProperty.objectReferenceValue != null)
            {
                return Mathf.Max(EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2, 64f);
            }

            return EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2;
        }
    }
#endif
}