using System.Collections.Generic;
using nadena.dev.modular_avatar.core;
using nadena.dev.ndmf;
using UnityEngine;
using VRC.SDK3.Dynamics.Contact.Components;

namespace MitarashiDango.AvatarUtils
{
    public class FaceEmoteControlProcessor
    {
        public void Run(BuildContext ctx)
        {
            var faceEmoteControl = ctx.AvatarRootObject.GetComponentInChildren<FaceEmoteControl>();
            if (faceEmoteControl == null)
            {
                return;
            }

            var headProxyGameObject = GenerateHeadBoneChildObject();
            headProxyGameObject.transform.SetParent(ctx.AvatarRootObject.transform);

            var faceEmoteLocker = GenerateFaceEmoteLocker(headProxyGameObject);
            faceEmoteLocker.transform.SetParent(ctx.AvatarRootObject.transform);

            AddParameters(faceEmoteControl.gameObject, faceEmoteControl);
            AddMenuItems(faceEmoteControl.gameObject, faceEmoteControl);
            AddAnimatorController(faceEmoteControl);
            Object.DestroyImmediate(faceEmoteControl);
        }

        private void AddParameters(GameObject obj, FaceEmoteControl faceEmoteControl)
        {
            var parameters = new FaceEmoteControlParameters();
            var modularAvatarParameters = obj.AddComponent<ModularAvatarParameters>();
            modularAvatarParameters.parameters = parameters.GetParameterConfigs(faceEmoteControl);
        }

        private void AddMenuItems(GameObject obj, FaceEmoteControl faceEmoteControl)
        {
            var menuBuilder = new FaceEmoteControlMenuGenerator();
            menuBuilder.GenerateMenus(faceEmoteControl).transform.parent = obj.transform;
        }

        private void AddAnimatorController(FaceEmoteControl faceEmoteControl)
        {
            var faceEmoteSettingsAnimatorControllerBuilder = new FaceEmoteControlAnimatorControllerGenerator();
            var mergeAnimator = faceEmoteControl.gameObject.AddComponent<ModularAvatarMergeAnimator>();
            mergeAnimator.animator = faceEmoteSettingsAnimatorControllerBuilder.GenerateAnimatorController(faceEmoteControl);
            mergeAnimator.layerType = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType.FX;
            mergeAnimator.pathMode = MergeAnimatorPathMode.Absolute;
            mergeAnimator.matchAvatarWriteDefaults = true;
        }

        private GameObject GenerateHeadBoneChildObject()
        {
            var go = new GameObject("$$FaceEmoteControl$$HeadProxy");

            var modularAvatarBoneProxy = go.AddComponent<ModularAvatarBoneProxy>();
            modularAvatarBoneProxy.attachmentMode = BoneProxyAttachmentMode.AsChildAtRoot;
            modularAvatarBoneProxy.boneReference = HumanBodyBones.Head;

            return go;
        }

        private GameObject GenerateFaceEmoteLocker(GameObject rootGameObject)
        {
            var go = new GameObject("$$FaceEmoteControl$$FaceEmoteLockContactReceiver");

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
    }
}