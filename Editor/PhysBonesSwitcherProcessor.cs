using nadena.dev.modular_avatar.core;
using nadena.dev.ndmf;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDK3.Dynamics.PhysBone.Components;
using static VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

namespace MitarashiDango.AvatarUtils
{
    public class PhysBonesSwitcherProcessor
    {
        public void Run(BuildContext ctx)
        {
            var physBonesSwitcher = ctx.AvatarRootObject.GetComponentInChildren<PhysBonesSwitcher>();
            if (physBonesSwitcher == null)
            {
                return;
            }

            AddParameters(physBonesSwitcher);
            AddMenuItems(physBonesSwitcher);

            var animatorController = GeneratePhysBonesSwitcherAnimatorController();
            animatorController.AddLayer(GeneratePhysBonesSwitcherLayer(ctx));

            var mergeAnimator = physBonesSwitcher.gameObject.AddComponent<ModularAvatarMergeAnimator>();
            mergeAnimator.animator = animatorController;
            mergeAnimator.layerType = AnimLayerType.FX;
            mergeAnimator.pathMode = MergeAnimatorPathMode.Absolute;
            mergeAnimator.matchAvatarWriteDefaults = true;

            Object.DestroyImmediate(physBonesSwitcher);
        }

        private void AddParameters(PhysBonesSwitcher physBonesSwitcher)
        {
            var parameters = new PhysBonesSwitcherParameters();
            var modularAvatarParameters = physBonesSwitcher.gameObject.AddComponent<ModularAvatarParameters>();
            modularAvatarParameters.parameters = parameters.GetParameterConfigs();
        }

        private void AddMenuItems(PhysBonesSwitcher physBonesSwitcher)
        {
            var menuObject = new GameObject("PhysBonesSwitcherMenu");
            menuObject.AddComponent<ModularAvatarMenuInstaller>();
            menuObject.AddComponent<ModularAvatarMenuGroup>();

            var menuItem = new GameObject("PhysBone無効化");
            var modularAvatarMenuItem = menuItem.AddComponent<ModularAvatarMenuItem>();

            modularAvatarMenuItem.Control.type = VRCExpressionsMenu.Control.ControlType.Toggle;
            modularAvatarMenuItem.Control.parameter = new VRCExpressionsMenu.Control.Parameter
            {
                name = PhysBonesSwitcherParameters.PBS_PHYS_BONES_OFF,
            };
            modularAvatarMenuItem.Control.value = 1;

            menuItem.transform.parent = menuObject.transform;

            menuObject.transform.parent = physBonesSwitcher.gameObject.transform;
        }

        private AnimatorController GeneratePhysBonesSwitcherAnimatorController()
        {
            var animatorController = new AnimatorController
            {
                name = "PHYS_BONES_SWITCHER_ANIMATOR_CONTROLLER",
                parameters = new AnimatorControllerParameter[]{
                    new AnimatorControllerParameter{
                        name = PhysBonesSwitcherParameters.PBS_PHYS_BONES_OFF,
                        type = AnimatorControllerParameterType.Bool,
                        defaultBool = false,
                    },
                },
            };

            if (animatorController.layers.Length == 0)
            {
                animatorController.AddLayer("DUMMY_LAYER");
            }

            return animatorController;
        }

        private (AnimationClip, AnimationClip, AnimationClip) GenerateAnimationClips(BuildContext ctx)
        {
            var vrcPhysBones = ctx.AvatarRootObject.GetComponentsInChildren<VRCPhysBone>();

            var blankAnimationClip = new AnimationClip
            {
                name = "blank"
            };

            var toEnableAnimationClip = new AnimationClip
            {
                name = "PhysBones_ON",
                frameRate = 60
            };

            var toDisableAnimationClip = new AnimationClip
            {
                name = "PhysBones_OFF",
                frameRate = 60
            };

            foreach (var vrcPhysBone in vrcPhysBones)
            {
                var path = MiscUtil.GetPathInHierarchy(vrcPhysBone.transform, ctx.AvatarRootObject.transform);

                var toEnableCurve = new AnimationCurve();
                toEnableCurve.AddKey(0, 1);

                var toDisableCurve = new AnimationCurve();
                toDisableCurve.AddKey(0, 0);

                toEnableAnimationClip.SetCurve(path, typeof(VRCPhysBone), "m_Enabled", toEnableCurve);
                toDisableAnimationClip.SetCurve(path, typeof(VRCPhysBone), "m_Enabled", toDisableCurve);
            }

            return (blankAnimationClip, toEnableAnimationClip, toDisableAnimationClip);
        }

        private AnimatorControllerLayer GeneratePhysBonesSwitcherLayer(BuildContext ctx)
        {
            var layer = new AnimatorControllerLayer
            {
                name = "PBS_PHYS_BONES_SWITCHER",
                defaultWeight = 1,
                stateMachine = new AnimatorStateMachine(),
            };

            layer.stateMachine.entryPosition = new Vector3(0, 0, 0);
            layer.stateMachine.exitPosition = new Vector3(0, -40, 0);
            layer.stateMachine.anyStatePosition = new Vector3(0, -80, 0);

            var (blankAnimationClip, toEnableAnimationClip, toDisableAnimationClip) = GenerateAnimationClips(ctx);

            var initialState = layer.stateMachine.AddState("Initial State", new Vector3(-20, 60, 0));
            initialState.writeDefaultValues = false;
            initialState.motion = blankAnimationClip;

            var physBonesOnState = layer.stateMachine.AddState("PhysBones_ON", new Vector3(220, 60, 0));
            physBonesOnState.motion = toEnableAnimationClip;

            AnimatorTransitionUtil.AddTransition(initialState, physBonesOnState)
                .IfNot(PhysBonesSwitcherParameters.PBS_PHYS_BONES_OFF)
                .SetImmediateTransitionSettings();

            var physBonesOffState = layer.stateMachine.AddState("PhysBones_OFF", new Vector3(-20, 140, 0));
            physBonesOffState.motion = toDisableAnimationClip;

            AnimatorTransitionUtil.AddTransition(initialState, physBonesOffState)
                .If(PhysBonesSwitcherParameters.PBS_PHYS_BONES_OFF)
                .SetImmediateTransitionSettings();

            AnimatorTransitionUtil.AddTransition(physBonesOnState, physBonesOffState)
                .If(PhysBonesSwitcherParameters.PBS_PHYS_BONES_OFF)
                .SetImmediateTransitionSettings();

            AnimatorTransitionUtil.AddTransition(physBonesOffState, physBonesOnState)
                .IfNot(PhysBonesSwitcherParameters.PBS_PHYS_BONES_OFF)
                .SetImmediateTransitionSettings();

            return layer;
        }
    }
}