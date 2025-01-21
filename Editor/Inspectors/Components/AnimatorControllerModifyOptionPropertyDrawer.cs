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
                var replaceToDummyLayerProperty = property.FindPropertyRelative("replaceToDummyLayer");
                var overwriteDefaultWeightProperty = property.FindPropertyRelative("overwriteDefaultWeight");
                var defaultWeightProperty = property.FindPropertyRelative("defaultWeight");
                var overwriteAvatarMaskProperty = property.FindPropertyRelative("overwriteAvatarMask");
                var avatarMaskProperty = property.FindPropertyRelative("avatarMask");
                var overwriteBlendingMode = property.FindPropertyRelative("overwriteBlendingMode");
                var blendingMode = property.FindPropertyRelative("blendingMode");
                var overwriteIkPass = property.FindPropertyRelative("overwriteIkPass");
                var ikPass = property.FindPropertyRelative("ikPass");

                var rect = PutLayerTypePropertyField(position, layerTypeProperty);

                var layerNames = new List<GUIContent>
                {
                    new GUIContent("")
                };
                layerNames.AddRange(GetLayerNames(gameObject, (PlayableLayerType)layerTypeProperty.enumValueIndex));

                rect = PutLayerNamePropertyFields(rect, layerNameProperty, layerNames);

                rect = PutPropertyField(rect, removeLayerProperty, new GUIContent("レイヤーを削除"));

                if (removeLayerProperty.boolValue)
                {
                    PutPropertyField(rect, replaceToDummyLayerProperty, new GUIContent("削除時にダミーレイヤーで置き替え"));
                }
                else
                {
                    rect = PutPropertyField(rect, overwriteDefaultWeightProperty, new GUIContent("デフォルトウェイトの設定値を上書き"));
                    if (overwriteDefaultWeightProperty.boolValue)
                    {
                        rect = PutPropertyField(rect, defaultWeightProperty, new GUIContent("デフォルトウェイト"));
                    }

                    rect = PutPropertyField(rect, overwriteAvatarMaskProperty, new GUIContent("アバターマスクの設定値を上書き"));
                    if (overwriteAvatarMaskProperty.boolValue)
                    {
                        rect = PutPropertyField(rect, avatarMaskProperty, new GUIContent("アバターマスク"));
                    }

                    rect = PutPropertyField(rect, overwriteBlendingMode, new GUIContent("ブレンドモードの設定値を上書き"));
                    if (overwriteBlendingMode.boolValue)
                    {
                        rect = PutPropertyField(rect, blendingMode, new GUIContent("ブレンドモード"));
                    }

                    rect = PutPropertyField(rect, overwriteIkPass, new GUIContent("IK Passの設定値を上書き"));
                    if (overwriteIkPass.boolValue)
                    {
                        PutPropertyField(rect, ikPass, new GUIContent("IK Pass"));
                    }
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 3;

            var removeLayerProperty = property.FindPropertyRelative("removeLayer");
            if (removeLayerProperty.boolValue)
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                var overwriteDefaultWeightProperty = property.FindPropertyRelative("overwriteDefaultWeight");
                height += overwriteDefaultWeightProperty.boolValue ?
                     (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2 : EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                var overwriteAvatarMaskProperty = property.FindPropertyRelative("overwriteAvatarMask");
                height += overwriteAvatarMaskProperty.boolValue ?
                     (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2 : EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                var overwriteBlendingMode = property.FindPropertyRelative("overwriteBlendingMode");
                height += overwriteBlendingMode.boolValue ?
                     (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2 : EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                var overwriteIkPass = property.FindPropertyRelative("overwriteIkPass");
                height += overwriteIkPass.boolValue ?
                     (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2 : EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }

        private Rect PutLayerTypePropertyField(Rect position, SerializedProperty property)
        {
            var rect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight,
                width = position.width,
            };
            EditorGUI.PropertyField(rect, property, new GUIContent("レイヤー種別"));
            return rect;
        }

        private Rect PutLayerNamePropertyFields(Rect position, SerializedProperty property, List<GUIContent> layerNames)
        {
            var layerNamePopupRect = new Rect(position)
            {
                y = position.y + position.height + EditorGUIUtility.standardVerticalSpacing,
                height = EditorGUIUtility.singleLineHeight
            };

            var previousValue = 0;
            var layerNameIndex = layerNames.FindIndex(value => value.text == property.stringValue);
            if (layerNameIndex != -1)
            {
                previousValue = layerNameIndex;
            }

            var changedValue = EditorGUI.Popup(layerNamePopupRect, new GUIContent("レイヤー名"), previousValue, layerNames.ToArray());
            if (changedValue != previousValue && changedValue != 0)
            {
                property.stringValue = layerNames[changedValue].text;
            }

            return PutPropertyField(layerNamePopupRect, property, new GUIContent(" "));
        }

        private Rect PutPropertyField(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect = new Rect(position)
            {
                y = position.y + position.height + EditorGUIUtility.standardVerticalSpacing,
                height = EditorGUIUtility.singleLineHeight
            };
            EditorGUI.PropertyField(rect, property, label);

            return rect;
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