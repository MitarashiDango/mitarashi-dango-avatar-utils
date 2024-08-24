
using MitarashiDango.FaceEmoteControl;
using nadena.dev.modular_avatar.core;
using nadena.dev.ndmf;
using UnityEngine;
using UnityEngine.Animations;

[assembly: ExportsPlugin(typeof(FaceEmoteControlPlugin))]

namespace MitarashiDango.FaceEmoteControl
{
#if UNITY_EDITOR
    public class FaceEmoteControlPlugin : Plugin<FaceEmoteControlPlugin>
    {
        public override string QualifiedName => "MitarashiDango.FaceEmoteControl";

        protected override void Configure()
        {
            InPhase(BuildPhase.Generating)
                .BeforePlugin("nadena.dev.modular-avatar")
                .Run("Generate face emote controller assets", ctx => Processing(ctx));
        }

        private void Processing(BuildContext ctx)
        {
            var faceEmoteControl = ctx.AvatarRootObject.GetComponentInChildren<FaceEmoteControl>();
            if (faceEmoteControl != null)
            {
                SetupFaceEmoteLockIndicator(faceEmoteControl, ctx.AvatarRootObject);
                AddParameters(faceEmoteControl.gameObject);
                AddRadialMenus(faceEmoteControl.gameObject, faceEmoteControl);
                AddAnimatorController(faceEmoteControl.gameObject, faceEmoteControl);
                Object.DestroyImmediate(faceEmoteControl);
            }
        }

        private void AddParameters(GameObject obj)
        {
            var parameters = new Parameters();
            var modularAvatarParameters = obj.AddComponent<ModularAvatarParameters>();
            modularAvatarParameters.parameters = parameters.GetParameterConfigs();
        }

        private void AddRadialMenus(GameObject obj, FaceEmoteControl faceEmoteControl)
        {
            var radialMenuBuilder = new RadialMenuGenerator();
            radialMenuBuilder.GenerateRadialMenu(faceEmoteControl).transform.parent = obj.transform;
        }

        private void AddAnimatorController(GameObject obj, FaceEmoteControl faceEmoteControl)
        {
            var faceEmoteSettingsAnimatorControllerBuilder = new AnimatorControllerGenerator();
            var mergeAnimator = obj.AddComponent<ModularAvatarMergeAnimator>();
            mergeAnimator.animator = faceEmoteSettingsAnimatorControllerBuilder.GenerateAnimatorController(faceEmoteControl);
            mergeAnimator.layerType = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType.FX;
            mergeAnimator.pathMode = MergeAnimatorPathMode.Absolute;
            mergeAnimator.matchAvatarWriteDefaults = true;
        }

        private void SetupFaceEmoteLockIndicator(FaceEmoteControl faceEmoteControl, GameObject avatarRootObject)
        {
            var faceEmoteLockIndicator = faceEmoteControl.transform.Find("FaceEmoteLockIndicator");
            var faceEmoteLockIndicatorParentConstraint = faceEmoteLockIndicator.GetComponent<ParentConstraint>();
            var headGameObject = avatarRootObject.transform.Find("Armature/Hips/Spine/Chest/Neck/Head");
            var constraintSource = new ConstraintSource
            {
                sourceTransform = headGameObject,
                weight = 1
            };
            faceEmoteLockIndicatorParentConstraint.AddSource(constraintSource);
        }
    }
#endif
}