using System.Collections.Generic;
using MitarashiDango.AvatarUtils;
using nadena.dev.modular_avatar.core;
using nadena.dev.ndmf;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations;
using VRC.SDK3.Dynamics.Contact.Components;
using static VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

[assembly: ExportsPlugin(typeof(AvatarUtilsNDMFPlugin))]

namespace MitarashiDango.AvatarUtils
{
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
            ModifyAnimatorControllerProcess(ctx);
            FaceEmoteControlProcess(ctx);
        }

        private void ModifyAnimatorControllerProcess(BuildContext ctx)
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

                AddParameters(faceEmoteControl.gameObject);
                AddMenuItems(faceEmoteControl.gameObject, faceEmoteControl);
                AddAnimatorController(ctx.AvatarRootObject, faceEmoteControl, faceEmoteLockIndicator);
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

        private void AddAnimatorController(GameObject avatarRoot, FaceEmoteControl faceEmoteControl, GameObject faceEmoteLockIndicator)
        {
            var faceEmoteSettingsAnimatorControllerBuilder = new FaceEmoteControlAnimatorControllerGenerator();
            var mergeAnimator = faceEmoteControl.gameObject.AddComponent<ModularAvatarMergeAnimator>();
            mergeAnimator.animator = faceEmoteSettingsAnimatorControllerBuilder.GenerateAnimatorController(avatarRoot, faceEmoteControl, faceEmoteLockIndicator);
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
}