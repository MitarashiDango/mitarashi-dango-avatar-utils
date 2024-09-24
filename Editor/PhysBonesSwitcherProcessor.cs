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
        public void Run(BuildContext ctx, PhysBonesSwitcher physBonesSwitcher)
        {
            AddParameters(physBonesSwitcher);
            AddMenuItems(physBonesSwitcher);

            var animatorController = GeneratePhysBonesSwitcherAnimatorController();
            animatorController.AddLayer(GeneratePhysBonesSwitcherLayer(ctx));

            var mergeAnimator = physBonesSwitcher.gameObject.AddComponent<ModularAvatarMergeAnimator>();
            mergeAnimator.animator = animatorController;
            mergeAnimator.layerType = AnimLayerType.FX;
            mergeAnimator.pathMode = MergeAnimatorPathMode.Absolute;
            mergeAnimator.matchAvatarWriteDefaults = true;
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
            layer.stateMachine.anyStatePosition = new Vector3(0, -40, 0);
            layer.stateMachine.exitPosition = new Vector3(800, 0, 0);

            var (blankAnimationClip, toEnableAnimationClip, toDisableAnimationClip) = GenerateAnimationClips(ctx);

            var initialState = layer.stateMachine.AddState("Initial State", new Vector3(200, 0, 0));
            initialState.writeDefaultValues = false;
            initialState.motion = blankAnimationClip;

            var physBonesOnState = layer.stateMachine.AddState("PhysBones_ON", new Vector3(400, 0, 0));
            physBonesOnState.motion = toEnableAnimationClip;

            var physBonesOnTransition = initialState.AddTransition(physBonesOnState);
            SetImmediateTransitionSetting(physBonesOnTransition);
            physBonesOnTransition.AddCondition(AnimatorConditionMode.IfNot, 0, PhysBonesSwitcherParameters.PBS_PHYS_BONES_OFF);

            var physBonesOffState = layer.stateMachine.AddState("PhysBones_OFF", new Vector3(200, 60, 0));
            physBonesOffState.motion = toDisableAnimationClip;

            var physBonesOffTransition = initialState.AddTransition(physBonesOffState);
            SetImmediateTransitionSetting(physBonesOffTransition);
            physBonesOffTransition.AddCondition(AnimatorConditionMode.If, 0, PhysBonesSwitcherParameters.PBS_PHYS_BONES_OFF);

            var physBonesOnToOffTransition = physBonesOnState.AddTransition(physBonesOffState);
            SetImmediateTransitionSetting(physBonesOnToOffTransition);
            physBonesOnToOffTransition.AddCondition(AnimatorConditionMode.If, 0, PhysBonesSwitcherParameters.PBS_PHYS_BONES_OFF);

            var physBonesOffToOnTransition = physBonesOffState.AddTransition(physBonesOnState);
            SetImmediateTransitionSetting(physBonesOffToOnTransition);
            physBonesOffToOnTransition.AddCondition(AnimatorConditionMode.IfNot, 0, PhysBonesSwitcherParameters.PBS_PHYS_BONES_OFF);

            return layer;
        }

        private void SetImmediateTransitionSetting(AnimatorStateTransition ast)
        {
            ast.hasExitTime = false;
            ast.exitTime = 0;
            ast.hasFixedDuration = true;
            ast.duration = 0;
            ast.offset = 0;
            ast.interruptionSource = TransitionInterruptionSource.None;
            ast.orderedInterruption = true;
        }
    }
}