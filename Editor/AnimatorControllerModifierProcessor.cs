using System.Collections.Generic;
using nadena.dev.ndmf;
using UnityEditor.Animations;
using UnityEngine;
using static VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

namespace MitarashiDango.AvatarUtils
{
    public class AnimatorControllerModifierProcessor
    {
        public void Run(BuildContext ctx)
        {
            var animatorControllerLayerModifiers = ctx.AvatarRootObject.GetComponentsInChildren<AnimatorControllerModifier>();
            if (animatorControllerLayerModifiers.Length == 0)
            {
                return;
            }

            var layerModifyOptions = BuildLayerModifyOptions(animatorControllerLayerModifiers);
            var avatarDescriptor = ctx.AvatarDescriptor;
            ModifyAnimationLayers(avatarDescriptor.baseAnimationLayers, layerModifyOptions);
            ModifyAnimationLayers(avatarDescriptor.specialAnimationLayers, layerModifyOptions);

            foreach (var animatorControllerLayerModifier in animatorControllerLayerModifiers)
            {
                Object.DestroyImmediate(animatorControllerLayerModifier);
            }
        }

        private Dictionary<AnimLayerType, Dictionary<string, AnimatorControllerLayerModifyOption>> BuildLayerModifyOptions(AnimatorControllerModifier[] animatorControllerLayerModifiers)
        {
            var layerModifyOptions = new Dictionary<AnimLayerType, Dictionary<string, AnimatorControllerLayerModifyOption>>();

            foreach (var animatorControllerModifier in animatorControllerLayerModifiers)
            {
                foreach (var layerModifyOption in animatorControllerModifier.layerModifyOptions)
                {
                    AnimLayerType playableLayerType;
                    switch (layerModifyOption.layerType)
                    {
                        case PlayableLayerType.Action:
                            playableLayerType = AnimLayerType.Action;
                            break;
                        case PlayableLayerType.Additive:
                            playableLayerType = AnimLayerType.Additive;
                            break;
                        case PlayableLayerType.Base:
                            playableLayerType = AnimLayerType.Base;
                            break;
                        case PlayableLayerType.FX:
                            playableLayerType = AnimLayerType.FX;
                            break;
                        case PlayableLayerType.Gesture:
                            playableLayerType = AnimLayerType.Gesture;
                            break;
                        case PlayableLayerType.IKPose:
                            playableLayerType = AnimLayerType.IKPose;
                            break;
                        case PlayableLayerType.Sitting:
                            playableLayerType = AnimLayerType.Sitting;
                            break;
                        case PlayableLayerType.TPose:
                            playableLayerType = AnimLayerType.TPose;
                            break;
                        default:
                            continue;
                    }

                    if (!layerModifyOptions.ContainsKey(playableLayerType))
                    {
                        layerModifyOptions.Add(playableLayerType, new Dictionary<string, AnimatorControllerLayerModifyOption>());
                    }

                    layerModifyOptions[playableLayerType].Add(layerModifyOption.layerName, layerModifyOption);
                }
            }

            return layerModifyOptions;
        }

        private void ModifyAnimationLayers(CustomAnimLayer[] customAnimLayers, Dictionary<AnimLayerType, Dictionary<string, AnimatorControllerLayerModifyOption>> modifierOptions)
        {
            for (var i = 0; i < customAnimLayers.Length; i++)
            {
                var layer = customAnimLayers[i];

                if (!modifierOptions.ContainsKey(layer.type))
                {
                    continue;
                }

                if (layer.animatorController is AnimatorController c)
                {
                    for (var j = 0; j < c.layers.Length;)
                    {
                        var targetLayer = c.layers[j];
                        if (!modifierOptions[layer.type].ContainsKey(targetLayer.name))
                        {
                            j++;
                            continue;
                        }

                        var modifierOption = modifierOptions[layer.type][targetLayer.name];

                        if (modifierOption.removeLayer)
                        {
                            c.RemoveLayer(j);
                            continue;
                        }

                        j++;
                    }
                }
            }
        }
    }
}