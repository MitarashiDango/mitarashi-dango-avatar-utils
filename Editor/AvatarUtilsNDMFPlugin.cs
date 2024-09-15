
using System.Collections.Generic;
using MitarashiDango.AvatarUtils;
using nadena.dev.modular_avatar.core;
using nadena.dev.ndmf;
using UnityEngine;
using UnityEngine.Animations;
using VRC.SDK3.Dynamics.Contact.Components;

[assembly: ExportsPlugin(typeof(AvatarUtilsNDMFPlugin))]

namespace MitarashiDango.AvatarUtils
{
#if UNITY_EDITOR
    public class AvatarUtilsNDMFPlugin : Plugin<AvatarUtilsNDMFPlugin>
    {
        protected override void Configure()
        {
            InPhase(BuildPhase.Generating)
                .BeforePlugin("nadena.dev.modular-avatar")
                .Run("Generate face emote controller assets", ctx => Processing(ctx));
        }

        private void Processing(BuildContext ctx)
        {
            FaceEmoteControlProcess(ctx);
        }

        private void FaceEmoteControlProcess(BuildContext ctx)
        {
            var faceEmoteControl = ctx.AvatarRootObject.GetComponentInChildren<FaceEmoteControl>();
            if (faceEmoteControl != null)
            {
                var fecRootGameObject = GenerateHeadBoneChildObject();
                fecRootGameObject.transform.SetParent(faceEmoteControl.gameObject.transform);

                var faceEmoteLocker = GenerateFaceEmoteLocker(fecRootGameObject);
                faceEmoteLocker.transform.SetParent(faceEmoteControl.gameObject.transform);

                var faceEmoteLockIndicator = GenerateFaceEmoteLockIndicator(fecRootGameObject);
                faceEmoteLockIndicator.transform.SetParent(faceEmoteControl.gameObject.transform);

                // SetupFaceEmoteLockIndicator(faceEmoteControl, ctx.AvatarRootObject);
                AddParameters(faceEmoteControl.gameObject);
                AddMenuItems(faceEmoteControl.gameObject, faceEmoteControl);
                AddAnimatorController(faceEmoteControl.gameObject, faceEmoteControl);
                Object.DestroyImmediate(faceEmoteControl);
            }
        }

        private void AddParameters(GameObject obj)
        {
            var parameters = new FaceEmoteControlParameters();
            var modularAvatarParameters = obj.AddComponent<ModularAvatarParameters>();
            modularAvatarParameters.parameters = parameters.GetParameterConfigs();
        }

        private void AddMenuItems(GameObject obj, FaceEmoteControl faceEmoteControl)
        {
            var menuBuilder = new FaceEmoteControlMenuGenerator();
            menuBuilder.GenerateMenus(faceEmoteControl).transform.parent = obj.transform;
        }

        private void AddAnimatorController(GameObject obj, FaceEmoteControl faceEmoteControl)
        {
            var faceEmoteSettingsAnimatorControllerBuilder = new FaceEmoteControlAnimatorControllerGenerator();
            var mergeAnimator = obj.AddComponent<ModularAvatarMergeAnimator>();
            mergeAnimator.animator = faceEmoteSettingsAnimatorControllerBuilder.GenerateAnimatorController(faceEmoteControl);
            mergeAnimator.layerType = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType.FX;
            mergeAnimator.pathMode = MergeAnimatorPathMode.Absolute;
            mergeAnimator.matchAvatarWriteDefaults = true;
        }

        private GameObject GenerateHeadBoneChildObject()
        {
            var go = new GameObject("FaceEmoteControl_Root");

            var modularAvatarBoneProxy = go.AddComponent<ModularAvatarBoneProxy>();
            modularAvatarBoneProxy.attachmentMode = BoneProxyAttachmentMode.AsChildAtRoot;
            modularAvatarBoneProxy.boneReference = HumanBodyBones.Head;

            return go;
        }

        private GameObject GenerateFaceEmoteLocker(GameObject rootGameObject)
        {
            var go = new GameObject("FaceEmoteLocker");

            var vrcContactReceiver = go.AddComponent<VRCContactReceiver>();
            vrcContactReceiver.rootTransform = rootGameObject.transform;
            vrcContactReceiver.shapeType = VRC.Dynamics.ContactBase.ShapeType.Sphere;
            vrcContactReceiver.radius = 0.14f;
            vrcContactReceiver.position = new Vector3(0, 0.09f, 0);
            vrcContactReceiver.allowSelf = true;
            vrcContactReceiver.allowOthers = false;
            vrcContactReceiver.localOnly = true;
            vrcContactReceiver.collisionTags = new List<string>
            {
                "Hand",
                "Finger"
            };
            vrcContactReceiver.receiverType = VRC.Dynamics.ContactReceiver.ReceiverType.Constant;
            vrcContactReceiver.parameter = FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_CONTACT;

            return go;
        }

        private GameObject GenerateFaceEmoteLockIndicator(GameObject sourceGameObject)
        {
            var go = new GameObject("FaceEmoteLockIndicator");
            go.SetActive(false);

            var parentConstraint = go.AddComponent<ParentConstraint>();
            parentConstraint.constraintActive = true;
            parentConstraint.weight = 1;
            parentConstraint.locked = true;
            parentConstraint.AddSource(new ConstraintSource()
            {
                sourceTransform = sourceGameObject.transform,
                weight = 1
            });

            var iconGameObject = new GameObject("FaceEmoteLockingIcon");
            iconGameObject.transform.SetParent(go.transform);

            iconGameObject.transform.position = new Vector3(0.06f, -0.05f, 0.3f);
            iconGameObject.transform.rotation = Quaternion.Euler(90, 180, 0);
            iconGameObject.transform.localScale = new Vector3(0.0015f, 0.0015f, 0.0015f);

            var meshFilter = iconGameObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = CreatePrimitiveMesh(PrimitiveType.Plane);

            var iconMaterial = AssetUtil.LoadAssetAtGUID<Material>(Constants.ASSET_GUID_FACE_EMOTE_LOCKING_ICON);
            var renderer = iconGameObject.AddComponent<MeshRenderer>();
            renderer.material = iconMaterial;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

            return go;
        }

        private Mesh CreatePrimitiveMesh(PrimitiveType type)
        {
            var go = GameObject.CreatePrimitive(type);
            var mesh = go.GetComponent<MeshFilter>().sharedMesh;
            GameObject.DestroyImmediate(go);
            return mesh;
        }
    }
#endif
}