using System.Collections.Generic;
using System.Linq;
using nadena.dev.ndmf;
using UnityEditor.Animations;
using UnityEngine;
using static VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

namespace MitarashiDango.AvatarUtils
{
    public class AnimatorControllerModifierProcessor
    {
        private AnimationClip blankAnimationClip = new AnimationClip
        {
            name = "blank"
        };

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
                var customAnimLayer = customAnimLayers[i];

                if (!modifierOptions.ContainsKey(customAnimLayer.type))
                {
                    continue;
                }

                if (customAnimLayer.animatorController is AnimatorController ac)
                {
                    ac.layers = ac.layers.ToList().Where(layer =>
                    {
                        if (!modifierOptions[customAnimLayer.type].ContainsKey(layer.name))
                        {
                            return true;
                        }

                        var modifierOption = modifierOptions[customAnimLayer.type][layer.name];

                        return !modifierOption.removeLayer || (modifierOption.removeLayer && modifierOption.replaceToDummyLayer);
                    })
                    .Select((layer, index) =>
                    {
                        if (!modifierOptions[customAnimLayer.type].ContainsKey(layer.name))
                        {
                            return layer;
                        }

                        var modifierOption = modifierOptions[customAnimLayer.type][layer.name];
                        if (modifierOption.removeLayer && modifierOption.replaceToDummyLayer)
                        {
                            return CreateDummyLayer($"{layer.name} (Dummy)", layer);
                        }

                        return new AnimatorControllerLayer()
                        {
                            name = layer.name,
                            defaultWeight = modifierOption.overwriteDefaultWeight ? modifierOption.defaultWeight : layer.defaultWeight,
                            avatarMask = modifierOption.overwriteAvatarMask ? modifierOption.avatarMask : layer.avatarMask,
                            blendingMode = modifierOption.overwriteBlendingMode ? modifierOption.blendingMode : layer.blendingMode,
                            iKPass = modifierOption.overwriteIkPass ? modifierOption.ikPass : layer.iKPass,
                            stateMachine = layer.stateMachine,
                            syncedLayerAffectsTiming = layer.syncedLayerAffectsTiming,
                            syncedLayerIndex = layer.syncedLayerIndex,
                        };
                    }).ToArray();
                }
            }
        }

        private AnimatorControllerLayer CreateDummyLayer(string name, AnimatorControllerLayer oldLayer)
        {
            var stateMachine = new AnimatorStateMachine();

            var state = stateMachine.AddState("Dummy State");
            state.motion = blankAnimationClip;
            state.writeDefaultValues = GetAnimatorStates(oldLayer.stateMachine).Exists(s => s.writeDefaultValues == true);

            return new AnimatorControllerLayer()
            {
                name = name,
                stateMachine = stateMachine,
                defaultWeight = 0
            };
        }

        private List<AnimatorState> GetAnimatorStates(AnimatorStateMachine stateMachine)
        {
            var states = new List<AnimatorState>();

            foreach (var childState in stateMachine.states)
            {
                states.Add(childState.state);
            }

            foreach (var childStateMachine in stateMachine.stateMachines)
            {
                states.AddRange(GetAnimatorStates(childStateMachine.stateMachine));
            }

            return states;
        }
    }
}