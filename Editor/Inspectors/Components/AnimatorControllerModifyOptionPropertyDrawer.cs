using UnityEditor;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    [CustomPropertyDrawer(typeof(AnimatorControllerLayerModifyOption))]
    public class AnimatorControllerModifyOptionPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (var scope = new EditorGUI.PropertyScope(position, label, property))
            {
                var layerTypeProperty = property.FindPropertyRelative("layerType");
                var layerNameProperty = property.FindPropertyRelative("layerName");
                var removeLayerProperty = property.FindPropertyRelative("removeLayer");

                var layerTypeRect = new Rect(position)
                {
                    height = EditorGUIUtility.singleLineHeight,
                    width = position.width,
                };
                EditorGUI.PropertyField(layerTypeRect, layerTypeProperty, new GUIContent("レイヤー種別"));

                var layerNameRect = new Rect(layerTypeRect)
                {
                    y = layerTypeRect.y + layerTypeRect.height + EditorGUIUtility.standardVerticalSpacing,
                    height = EditorGUIUtility.singleLineHeight
                };
                EditorGUI.PropertyField(layerNameRect, layerNameProperty, new GUIContent("レイヤー名"));

                var removeLayerRect = new Rect(layerNameRect)
                {
                    y = layerNameRect.y + layerNameRect.height + EditorGUIUtility.standardVerticalSpacing,
                    height = EditorGUIUtility.singleLineHeight
                };
                EditorGUI.PropertyField(removeLayerRect, removeLayerProperty, new GUIContent("レイヤーを削除"));
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2;
        }
    }
}