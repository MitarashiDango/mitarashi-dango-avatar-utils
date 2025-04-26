using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace MitarashiDango.AvatarUtils
{
    public class AvatarRenderer : System.IDisposable
    {
        private PreviewRenderUtility _previewRenderUtility;
        public RenderTexture renderTexture { get; private set; }

        public AvatarRenderer()
        {
            InitializePreviewRenderUtility();
        }

        ~AvatarRenderer()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_previewRenderUtility != null)
            {
                _previewRenderUtility.Cleanup();
            }
        }

        public Texture2D Render(GameObject avatarRootObject, CameraSetting cameraSetting, int width, int height, Dictionary<string, float> defaultBlendShapes, Dictionary<string, float> animationClipBlendShapes, bool allowHDR)
        {
            var format = allowHDR ? DefaultFormat.HDR : DefaultFormat.LDR;
            var renderTexture = new RenderTexture(width, height, 32, format);

            try
            {
                renderTexture.hideFlags = HideFlags.HideAndDontSave;
                Render(avatarRootObject, cameraSetting, renderTexture, defaultBlendShapes, animationClipBlendShapes, allowHDR);
                return RenderToTexture2D(renderTexture);
            }
            finally
            {
                Object.DestroyImmediate(renderTexture);
            }
        }

        public bool Render(GameObject avatarRootObject, CameraSetting cameraSetting, RenderTexture renderTexture, Dictionary<string, float> defaultBlendShapes, Dictionary<string, float> animationClipBlendShapes, bool allowHDR)
        {
            _previewRenderUtility.BeginPreview(new Rect(0, 0, renderTexture.width, renderTexture.height), GUIStyle.none);

            SetupDefaultLights();
            SetupCamera(renderTexture, avatarRootObject, cameraSetting, allowHDR);

            if (avatarRootObject != null)
            {
                var go = Object.Instantiate(avatarRootObject);
                try
                {
                    go.SetActive(true);

                    var bodyObject = go.transform.Find("Body");
                    if (bodyObject == null)
                    {
                        return false;
                    }

                    var headObject = bodyObject.gameObject;
                    if (headObject == null)
                    {
                        return false;
                    }

                    var skinnedMeshRenderer = headObject.GetComponent<SkinnedMeshRenderer>();
                    if (skinnedMeshRenderer != null)
                    {
                        var skinnedMesh = skinnedMeshRenderer.sharedMesh;

                        for (var i = 0; i < skinnedMesh.blendShapeCount; i++)
                        {
                            var blendShapeName = skinnedMesh.GetBlendShapeName(i);
                            if (animationClipBlendShapes != null && animationClipBlendShapes.ContainsKey(blendShapeName))
                            {
                                skinnedMeshRenderer.SetBlendShapeWeight(i, animationClipBlendShapes[blendShapeName]);
                            }
                            else if (defaultBlendShapes != null && defaultBlendShapes.ContainsKey(blendShapeName))
                            {
                                skinnedMeshRenderer.SetBlendShapeWeight(i, defaultBlendShapes[blendShapeName]);
                            }
                        }
                    }

                    _previewRenderUtility.AddSingleGO(go);
                    _previewRenderUtility.camera.Render();
                }
                finally
                {
                    Object.DestroyImmediate(go);
                    _previewRenderUtility.EndPreview();
                }
            }
            else
            {
                try
                {
                    _previewRenderUtility.camera.Render();
                }
                finally
                {
                    _previewRenderUtility.EndPreview();
                }
            }

            return true;
        }

        private Texture2D RenderToTexture2D(RenderTexture renderTexture)
        {
            var currentRT = RenderTexture.active;
            try
            {
                RenderTexture.active = renderTexture;

                var texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
                texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0, false);
                texture.Apply();

                return texture;
            }
            finally
            {
                RenderTexture.active = currentRT;
            }
        }

        private void SetupCamera(RenderTexture renderTexture, GameObject avatarRootObject, CameraSetting cameraSetting, bool allowHDR)
        {
            _previewRenderUtility.camera.clearFlags = CameraClearFlags.SolidColor;
            _previewRenderUtility.camera.backgroundColor = cameraSetting.BackgroundColor;
            _previewRenderUtility.camera.depth = -1;
            _previewRenderUtility.camera.useOcclusionCulling = true;
            _previewRenderUtility.camera.allowMSAA = true;
            _previewRenderUtility.camera.allowHDR = allowHDR;
            _previewRenderUtility.camera.aspect = 1;
            _previewRenderUtility.camera.stereoSeparation = 0.022f;
            _previewRenderUtility.camera.stereoConvergence = 10;
            _previewRenderUtility.camera.farClipPlane = 100;
            _previewRenderUtility.camera.nearClipPlane = 0.001f;
            _previewRenderUtility.camera.transform.localScale = cameraSetting.Scale;
            _previewRenderUtility.camera.transform.rotation = cameraSetting.Rotation;
            _previewRenderUtility.camera.targetTexture = renderTexture;
            _previewRenderUtility.camera.pixelRect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            _previewRenderUtility.camera.transform.position = GetCameraPosition(avatarRootObject, cameraSetting.PositionOffset);
        }

        private Bounds GetObjectBounds(GameObject obj)
        {
            var headGameObject = obj.transform.Find("Body")?.gameObject;
            Renderer[] renderers = headGameObject.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0)
            {
                return new Bounds(obj.transform.position, Vector3.zero);
            }

            Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            return bounds;
        }

        private Vector3 GetCameraPosition(GameObject avatarRootObject, Vector3 positionOffset)
        {
            var bounds = GetObjectBounds(avatarRootObject);
            var position = bounds.center;
            var maxBoundsSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            var cameraDistance = maxBoundsSize / (2.0f * Mathf.Tan(_previewRenderUtility.camera.fieldOfView * 0.5f * Mathf.Deg2Rad));
            position -= _previewRenderUtility.camera.transform.forward * cameraDistance;
            position += positionOffset;
            return position;
        }

        public void InitializePreviewRenderUtility()
        {
            if (_previewRenderUtility != null)
            {
                _previewRenderUtility.Cleanup();
            }

            _previewRenderUtility = new PreviewRenderUtility();
        }

        private void SetupDefaultLights()
        {
            _previewRenderUtility.lights[0].color = new Color(1, 1, 1, 1f);
            _previewRenderUtility.lights[0].renderMode = LightRenderMode.Auto;
            _previewRenderUtility.lights[0].type = LightType.Directional;
            _previewRenderUtility.lights[0].intensity = 1;
            _previewRenderUtility.lights[0].shadowStrength = 1f;
            _previewRenderUtility.lights[0].shadowBias = 0.05f;
            _previewRenderUtility.lights[0].shadowNormalBias = 0.4f;
            _previewRenderUtility.lights[0].shadowNearPlane = 0.2f;

            _previewRenderUtility.lights[1].color = new Color(1, 1, 1, 1f);
            _previewRenderUtility.lights[1].renderMode = LightRenderMode.Auto;
            _previewRenderUtility.lights[1].intensity = 1;
            _previewRenderUtility.lights[1].shadowStrength = 1f;
            _previewRenderUtility.lights[1].shadowBias = 0.05f;
            _previewRenderUtility.lights[1].shadowNormalBias = 0.4f;
            _previewRenderUtility.lights[1].shadowNearPlane = 0.2f;
        }

        public class CameraSetting
        {
            public Vector3 Scale { get; set; }
            public Vector3 PositionOffset { get; set; }
            public Quaternion Rotation { get; set; }
            public Color BackgroundColor { get; set; }

            public CameraSetting()
            {
                Scale = new Vector3(1, 1, 1);
                PositionOffset = new Vector3(0, 0, 0);
                Rotation = Quaternion.Euler(0, 0, 0);
                BackgroundColor = Color.white;
            }

            public CameraSetting(Vector3 scale, Vector3 positionOffset, Quaternion rotation, Color backgroundColor)
            {
                Scale = scale;
                PositionOffset = positionOffset;
                Rotation = rotation;
                BackgroundColor = backgroundColor;
            }
        }
    }
}
