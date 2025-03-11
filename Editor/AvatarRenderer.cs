using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace MitarashiDango.AvatarUtils
{
    public class AvatarRenderer : System.IDisposable
    {
        private PreviewRenderUtility _previewRenderUtility;

        public Vector3 CameraScale;
        public Vector3 CameraPosition;
        public Quaternion CameraRotation;
        public Color BackgroundColor;

        public RenderTexture RenderTexture { get; private set; }

        public AvatarRenderer()
        {
            ResetCameraSettings();
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

        public Texture2D Render(GameObject avatarRootObject, int width, int height, Dictionary<string, float> defaultBlendShapes, Dictionary<string, float> animationClipBlendShapes, bool allowHDR)
        {
            var format = allowHDR ? DefaultFormat.HDR : DefaultFormat.LDR;
            var renderTexture = new RenderTexture(width, height, 32, format);

            try
            {
                renderTexture.hideFlags = HideFlags.HideAndDontSave;
                Render(avatarRootObject, renderTexture, defaultBlendShapes, animationClipBlendShapes, allowHDR);
                return RenderToTexture2D(renderTexture);
            }
            finally
            {
                Object.DestroyImmediate(renderTexture);
            }
        }

        public void Render(GameObject avatarRootObject, RenderTexture renderTexture, Dictionary<string, float> defaultBlendShapes, Dictionary<string, float> animationClipBlendShapes, bool allowHDR)
        {
            _previewRenderUtility.BeginPreview(new Rect(0, 0, renderTexture.width, renderTexture.height), GUIStyle.none);

            SetupLights();
            SetupCamera(renderTexture, allowHDR);

            if (avatarRootObject != null)
            {
                var go = Object.Instantiate(avatarRootObject);
                try
                {
                    go.SetActive(true);

                    var bodyGameObject = go.transform.Find("Body");
                    if (bodyGameObject != null)
                    {
                        var headGameObject = bodyGameObject.gameObject;
                        if (headGameObject != null)
                        {
                            var skinnedMeshRenderer = headGameObject.GetComponent<SkinnedMeshRenderer>();
                            if (skinnedMeshRenderer != null)
                            {
                                var skinnedMesh = skinnedMeshRenderer.sharedMesh;
                                for (var i = 0; i < skinnedMesh.blendShapeCount; i++)
                                {
                                    var blendShapeName = skinnedMesh.GetBlendShapeName(i);
                                    if (animationClipBlendShapes.ContainsKey(blendShapeName))
                                    {
                                        skinnedMeshRenderer.SetBlendShapeWeight(i, animationClipBlendShapes[blendShapeName]);
                                    }
                                    else if (defaultBlendShapes.ContainsKey(blendShapeName))
                                    {
                                        skinnedMeshRenderer.SetBlendShapeWeight(i, defaultBlendShapes[blendShapeName]);
                                    }
                                }
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

        public void ResetCameraSettings()
        {
            CameraScale = new Vector3(1, 1, 1);
            CameraPosition = new Vector3(0, 0, 0);
            CameraRotation = Quaternion.Euler(0, 0, 0);
            BackgroundColor = Color.white;
        }

        private void SetupCamera(RenderTexture renderTexture, bool allowHDR)
        {
            _previewRenderUtility.camera.clearFlags = CameraClearFlags.SolidColor;
            _previewRenderUtility.camera.backgroundColor = BackgroundColor;
            _previewRenderUtility.camera.depth = -1;
            _previewRenderUtility.camera.useOcclusionCulling = true;
            _previewRenderUtility.camera.allowMSAA = true;
            _previewRenderUtility.camera.allowHDR = allowHDR;
            _previewRenderUtility.camera.aspect = 1;
            _previewRenderUtility.camera.stereoSeparation = 0.022f;
            _previewRenderUtility.camera.stereoConvergence = 10;
            _previewRenderUtility.camera.farClipPlane = 100;
            _previewRenderUtility.camera.nearClipPlane = 0.001f;
            _previewRenderUtility.camera.transform.localScale = CameraScale;
            _previewRenderUtility.camera.transform.position = CameraPosition;
            _previewRenderUtility.camera.transform.rotation = CameraRotation;
            _previewRenderUtility.camera.targetTexture = renderTexture;
            _previewRenderUtility.camera.pixelRect = new Rect(0, 0, renderTexture.width, renderTexture.height);
        }

        public void InitializePreviewRenderUtility()
        {
            if (_previewRenderUtility != null)
            {
                _previewRenderUtility.Cleanup();
            }

            _previewRenderUtility = new PreviewRenderUtility();
        }

        private void SetupLights()
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
    }
}
