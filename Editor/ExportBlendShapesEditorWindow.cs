using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    public class ExportBlendShapesEditorWindow : EditorWindow
    {
        private GameObject _gameObject;
        private int _pathTypeIndex;
        private int _zeroWeightBlendShapesIncludeOptionIndex;
        private int _vrcVisemeBlendShapesIncludeOptionIndex;
        private int _mmdBlendShapesIncludeOptionIndex;
        private int _diffOptionIndex;
        private int _diffSorucePathTypeIndex;
        private AnimationClip _diffAnimationClip;

        [SerializeField]
        private string[] _excludeBlendShapeNames = new string[] { };

        [SerializeField]
        private string[] _excludeBlendShapeNamesStartWith = new string[] { };

        [SerializeField]
        private string[] _excludeBlendShapeNamesEndWith = new string[] { };

        private Vector2 _scrollPosition = Vector2.zero;

        private GUIContent[] pathTypeOptions = new GUIContent[]
        {
                new GUIContent("アバタールートからのパス"),
                new GUIContent("エクスポート対象オブジェクトからのパス"),
        };

        private GUIContent[] excludeOptions = new GUIContent[]
        {
                new GUIContent("エクスポート対象に含める"),
                new GUIContent("エクスポート対象外とする"),
        };

        private GUIContent[] diffOptions = new GUIContent[]
        {
                new GUIContent("差分エクスポートしない（条件に合致するシェイプキーを全てエクスポート）"),
                new GUIContent("指定したアニメーションクリップとの差分を検知したシェイプキーのみエクスポート"),
        };

        [MenuItem("GameObject/MitarashiDango Avatar Utils/Export BlendShapes", false, 0)]
        internal static void OpenWindow()
        {
            var window = GetWindow<ExportBlendShapesEditorWindow>("Export BlendShapes");
            if (Selection.activeGameObject != null)
            {
                window._gameObject = Selection.activeGameObject;
            }

            var pos = window.position;
            pos.width = 400;
            pos.height = 280;
            window.position = pos;

            window.Show();
        }

        private void OnGUI()
        {
            _gameObject = (GameObject)EditorGUILayout.ObjectField(new GUIContent("エクスポート対象オブジェクト"), _gameObject, typeof(GameObject), true);

            using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(_scrollPosition))
            {
                _scrollPosition = scrollViewScope.scrollPosition;

                var so = new SerializedObject(this);
                so.Update();

                _pathTypeIndex = EditorGUILayout.Popup(new GUIContent("パス種別"), _pathTypeIndex, pathTypeOptions);
                _zeroWeightBlendShapesIncludeOptionIndex = EditorGUILayout.Popup(new GUIContent("値が0のシェイプキー"), _zeroWeightBlendShapesIncludeOptionIndex, excludeOptions);
                _vrcVisemeBlendShapesIncludeOptionIndex = EditorGUILayout.Popup(new GUIContent("vrc.で始まるシェイプキー"), _vrcVisemeBlendShapesIncludeOptionIndex, excludeOptions);
                _mmdBlendShapesIncludeOptionIndex = EditorGUILayout.Popup(new GUIContent("MMD関連のシェイプキー"), _mmdBlendShapesIncludeOptionIndex, excludeOptions);

                EditorGUILayout.PropertyField(so.FindProperty("_excludeBlendShapeNames"), new GUIContent("エクスポート対象外とするシェイプキー"), true);
                EditorGUILayout.PropertyField(so.FindProperty("_excludeBlendShapeNamesStartWith"), new GUIContent("エクスポート対象外とするシェイプキーのプレフィックス"), true);
                EditorGUILayout.PropertyField(so.FindProperty("_excludeBlendShapeNamesEndWith"), new GUIContent("エクスポート対象外とするシェイプキーのサフィックス"), true);

                _diffOptionIndex = EditorGUILayout.Popup(new GUIContent("差分エクスポート設定"), _diffOptionIndex, diffOptions);

                if (_diffOptionIndex == 1)
                {
                    _diffAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(new GUIContent("差分取得元"), _diffAnimationClip, typeof(AnimationClip), true);
                    EditorGUILayout.HelpBox("複数フレームを持つアニメーションクリップの場合、最終フレーム時点の値で差分比較を行います", MessageType.Info);
                    _diffSorucePathTypeIndex = EditorGUILayout.Popup(new GUIContent("差分取得元のパス種別"), _diffSorucePathTypeIndex, pathTypeOptions);
                }

                so.ApplyModifiedProperties();

                var exportButtonRect = EditorGUILayout.GetControlRect(GUILayout.Height(EditorGUIUtility.singleLineHeight * 2));
                if (GUI.Button(exportButtonRect, new GUIContent("エクスポート")))
                {
                    ExportBlendShapes();
                }
            }
        }

        private void ExportBlendShapes()
        {
            if (_gameObject == null)
            {
                EditorUtility.DisplayDialog("エラー", "エクスポート対象オブジェクトが指定されていません", "OK");
                return;
            }

            var skinnedMeshRenderer = _gameObject.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer == null)
            {
                EditorUtility.DisplayDialog("エラー", "エクスポート対象オブジェクトにSkinnedMeshRendererが存在しません", "OK");
                return;
            }

            if (_diffOptionIndex == 1 && _diffAnimationClip == null)
            {
                EditorUtility.DisplayDialog("エラー", "差分取得元アニメーションクリップが指定されていません", "OK");
                return;
            }

            var filePath = EditorUtility.SaveFilePanelInProject("名前を付けて保存", $"AnimationClip_{_gameObject.name}", "anim", "アニメーションクリップの保存先を選択してください", "Assets");
            if (filePath == "")
            {
                EditorUtility.DisplayDialog("情報", "キャンセルされました", "OK");
                return;
            }

            var filename = Path.GetFileNameWithoutExtension(filePath);

            EditorUtility.DisplayProgressBar("処理中", "", 0);

            var skinnedMesh = skinnedMeshRenderer.sharedMesh;

            var animationClip = new AnimationClip()
            {
                frameRate = 60,
                name = filename,
            };

            var rootObject = _pathTypeIndex == 0 ? MiscUtil.GetAvatarRoot(_gameObject.transform) : _gameObject;
            var objectPath = MiscUtil.GetPathInHierarchy(_gameObject, rootObject);
            var diffBlendShapes = GetBlendShapes();

            for (var i = 0; i < skinnedMesh.blendShapeCount; i++)
            {
                float progress = i / (float)skinnedMesh.blendShapeCount;
                EditorUtility.DisplayProgressBar("処理中", $"{i} {skinnedMesh.blendShapeCount} ({(int)(progress * 100)}%)", progress);

                var blendShapeName = skinnedMesh.GetBlendShapeName(i);
                if (_excludeBlendShapeNames.ToList().Exists(name => blendShapeName == name)
                    || _excludeBlendShapeNamesStartWith.ToList().Exists(name => name != "" && blendShapeName.StartsWith(name))
                    || _excludeBlendShapeNamesEndWith.ToList().Exists(name => name != "" && blendShapeName.EndsWith(name))
                    || (_vrcVisemeBlendShapesIncludeOptionIndex == 1 && blendShapeName.StartsWith("vrc."))
                    || (_mmdBlendShapesIncludeOptionIndex == 1 && Constants.MMD_BLEND_SHAPE_NAMES.ToList().Exists(name => blendShapeName == name)))
                {
                    continue;
                }

                var blendShapeWeight = skinnedMeshRenderer.GetBlendShapeWeight(i);
                if (_zeroWeightBlendShapesIncludeOptionIndex == 1 && blendShapeWeight == 0)
                {
                    continue;
                }

                if (diffBlendShapes != null && diffBlendShapes.ContainsKey(blendShapeName) && diffBlendShapes[blendShapeName] == blendShapeWeight)
                {
                    continue;
                }

                var animationCurve = new AnimationCurve();
                animationCurve.AddKey(0, blendShapeWeight);

                animationClip.SetCurve(objectPath, typeof(SkinnedMeshRenderer), $"blendShape.{blendShapeName}", animationCurve);
            }

            EditorUtility.ClearProgressBar();

            // ファイル保存
            var asset = AssetDatabase.LoadAssetAtPath(filePath, typeof(AnimationClip));
            if (asset == null)
            {
                AssetDatabase.CreateAsset(animationClip, filePath);
            }
            else
            {
                EditorUtility.CopySerialized(animationClip, asset);
                AssetDatabase.SaveAssets();
            }

            AssetDatabase.Refresh();
            return;
        }

        private Dictionary<string, float> GetBlendShapes()
        {
            if (_diffOptionIndex == 1)
            {
                if (_diffSorucePathTypeIndex == 0)
                {
                    return GetBlendShapesFromAnimationClip(_diffAnimationClip, MiscUtil.GetPathInHierarchy(_gameObject, MiscUtil.GetAvatarRoot(_gameObject.transform)), _diffAnimationClip.length);
                }

                if (_diffSorucePathTypeIndex == 1)
                {
                    return GetBlendShapesFromAnimationClip(_diffAnimationClip, "", _diffAnimationClip.length);
                }
            }

            return null;
        }

        private Dictionary<string, float> GetBlendShapesFromAnimationClip(AnimationClip animationClip, string objectPath, float time)
        {
            var blendShapes = new Dictionary<string, float>();
            var bindings = AnimationUtility.GetCurveBindings(animationClip);

            foreach (var b in bindings)
            {
                if (b.path != objectPath && !b.propertyName.StartsWith("blendShape."))
                {
                    continue;
                }

                var curve = AnimationUtility.GetEditorCurve(animationClip, b);
                blendShapes.Add(b.propertyName.Substring("blendShape.".Length), curve.Evaluate(time));
            }

            return blendShapes;
        }
    }
}