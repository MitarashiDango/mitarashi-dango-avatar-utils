using UnityEditor;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    [CustomPropertyDrawer(typeof(FaceEmote))]
    public class FaceEmotePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (var scope = new EditorGUI.PropertyScope(position, label, property))
            {
                var nameProperty = property.FindPropertyRelative("name");
                var motionProperty = property.FindPropertyRelative("motion");
                var iconProperty = property.FindPropertyRelative("icon");
                var eyeControlTypeProperty = property.FindPropertyRelative("eyeControlType");
                var mouthControlTypeProperty = property.FindPropertyRelative("mouthControlType");

                var nameRect = new Rect(position)
                {
                    height = EditorGUIUtility.singleLineHeight,
                    width = iconProperty.objectReferenceValue != null ? position.width - 66f : position.width
                };
                EditorGUI.PropertyField(nameRect, nameProperty, new GUIContent("表情名"));

                var motionRect = new Rect(nameRect)
                {
                    y = nameRect.y + nameRect.height + EditorGUIUtility.standardVerticalSpacing,
                    height = EditorGUIUtility.singleLineHeight
                };
                EditorGUI.PropertyField(motionRect, motionProperty, new GUIContent("表情アニメーション"));

                var iconRect = new Rect(motionRect)
                {
                    y = motionRect.y + motionRect.height + EditorGUIUtility.standardVerticalSpacing,
                    height = EditorGUIUtility.singleLineHeight
                };
                EditorGUI.PropertyField(iconRect, iconProperty, new GUIContent("アイコン"));

                var eyeControlTypeRect = new Rect(iconRect)
                {
                    y = iconRect.y + iconRect.height + EditorGUIUtility.standardVerticalSpacing,
                    height = EditorGUIUtility.singleLineHeight
                };
                EditorGUI.PropertyField(eyeControlTypeRect, eyeControlTypeProperty, new GUIContent("目の動き"));

                var mouthControlTypeRect = new Rect(eyeControlTypeRect)
                {
                    y = eyeControlTypeRect.y + eyeControlTypeRect.height + EditorGUIUtility.standardVerticalSpacing,
                    height = EditorGUIUtility.singleLineHeight
                };
                EditorGUI.PropertyField(mouthControlTypeRect, mouthControlTypeProperty, new GUIContent("口の動き"));

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
                return Mathf.Max(EditorGUIUtility.singleLineHeight * 5 + EditorGUIUtility.standardVerticalSpacing * 4, 64f);
            }

            return EditorGUIUtility.singleLineHeight * 5 + EditorGUIUtility.standardVerticalSpacing * 4;
        }
    }
}