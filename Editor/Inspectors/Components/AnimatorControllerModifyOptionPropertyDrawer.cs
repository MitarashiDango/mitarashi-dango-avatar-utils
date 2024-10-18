using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace MitarashiDango.AvatarUtils
{
    [CustomPropertyDrawer(typeof(AnimatorControllerLayerModifyOption))]
    public class AnimatorControllerModifyOptionPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var gameObject = GetGameObject(property);

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

                var layerNamePopupRect = new Rect(layerTypeRect)
                {
                    y = layerTypeRect.y + layerTypeRect.height + EditorGUIUtility.standardVerticalSpacing,
                    height = EditorGUIUtility.singleLineHeight
                };

                var layerNames = new List<GUIContent>
                {
                    new GUIContent("")
                };
                layerNames.AddRange(GetLayerNames(gameObject, (PlayableLayerType)layerTypeProperty.enumValueIndex));

                var previousValue = 0;
                var layerNameIndex = layerNames.FindIndex(value => value.text == layerNameProperty.stringValue);
                if (layerNameIndex != -1)
                {
                    previousValue = layerNameIndex;
                }

                var changedValue = EditorGUI.Popup(layerNamePopupRect, new GUIContent("レイヤー名"), previousValue, layerNames.ToArray());
                if (changedValue != previousValue && changedValue != 0)
                {
                    layerNameProperty.stringValue = layerNames[changedValue].text;
                }

                var layerNameRect = new Rect(layerNamePopupRect)
                {
                    y = layerNamePopupRect.y + layerNamePopupRect.height + EditorGUIUtility.standardVerticalSpacing,
                    height = EditorGUIUtility.singleLineHeight
                };
                EditorGUI.PropertyField(layerNameRect, layerNameProperty, new GUIContent(" "));

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
            return EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 3;
        }

        private GameObject GetGameObject(SerializedProperty property)
        {
            if (property.serializedObject.targetObject is Component component)
            {
                return component.gameObject;
            }

            return null;
        }

        private GUIContent[] GetLayerNames(GameObject gameObject, PlayableLayerType layerType)
        {
            var avatarRoot = MiscUtil.GetAvatarRoot(gameObject.transform);
            if (avatarRoot == null)
            {
                return new GUIContent[0];
            }

            var avatarDescriptor = avatarRoot.GetComponent<VRCAvatarDescriptor>();
            if (avatarDescriptor == null)
            {
                return new GUIContent[0];
            }

            switch (layerType)
            {
                case PlayableLayerType.Action:
                    return GetAnimatorControlLayers(avatarDescriptor.baseAnimationLayers, VRCAvatarDescriptor.AnimLayerType.Action)
                        .Select(value => new GUIContent(value))
                        .ToArray();

                case PlayableLayerType.Additive:
                    return GetAnimatorControlLayers(avatarDescriptor.baseAnimationLayers, VRCAvatarDescriptor.AnimLayerType.Additive)
                        .Select(value => new GUIContent(value))
                        .ToArray();

                case PlayableLayerType.Base:
                    return GetAnimatorControlLayers(avatarDescriptor.baseAnimationLayers, VRCAvatarDescriptor.AnimLayerType.Base)
                        .Select(value => new GUIContent(value))
                        .ToArray();

                case PlayableLayerType.FX:
                    return GetAnimatorControlLayers(avatarDescriptor.baseAnimationLayers, VRCAvatarDescriptor.AnimLayerType.FX)
                        .Select(value => new GUIContent(value))
                        .ToArray();

                case PlayableLayerType.Gesture:
                    return GetAnimatorControlLayers(avatarDescriptor.baseAnimationLayers, VRCAvatarDescriptor.AnimLayerType.Gesture)
                        .Select(value => new GUIContent(value))
                        .ToArray();

                case PlayableLayerType.IKPose:
                    return GetAnimatorControlLayers(avatarDescriptor.specialAnimationLayers, VRCAvatarDescriptor.AnimLayerType.Gesture)
                        .Select(value => new GUIContent(value))
                        .ToArray();

                case PlayableLayerType.Sitting:
                    return GetAnimatorControlLayers(avatarDescriptor.specialAnimationLayers, VRCAvatarDescriptor.AnimLayerType.Sitting)
                        .Select(value => new GUIContent(value))
                        .ToArray();

                case PlayableLayerType.TPose:
                    return GetAnimatorControlLayers(avatarDescriptor.specialAnimationLayers, VRCAvatarDescriptor.AnimLayerType.TPose)
                        .Select(value => new GUIContent(value))
                        .ToArray();

                default:
                    return new GUIContent[0];
            }
        }

        private string[] GetAnimatorControlLayers(VRCAvatarDescriptor.CustomAnimLayer[] animLayers, VRCAvatarDescriptor.AnimLayerType layerType)
        {
            var animatorController = animLayers
                .Where(value => value.type == layerType)
                .Select(value => value.animatorController)
                .First();

            if (animatorController != null && animatorController is AnimatorController c)
            {
                return c.layers.Select(value => value.name).ToArray();
            }

            return new string[0];
        }
    }
}