using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    public class ImportBlendShapesEditorWindow : EditorWindow
    {
        private GameObject _gameObject;
        private BlendShapeSet _blendShapeSet;
        private AnimationClip _animationClip;
        private int _importSourceTypeIndex;
        private int _pathTypeIndex;
        private int _zeroWeightBlendShapesIncludeOptionIndex;
        private int _vrcVisemeBlendShapesIncludeOptionIndex;
        private int _mmdBlendShapesIncludeOptionIndex;

        [SerializeField]
        private string[] _excludeBlendShapeNames = new string[] { };

        [SerializeField]
        private string[] _excludeBlendShapeNamesStartWith = new string[] { };

        [SerializeField]
        private string[] _excludeBlendShapeNamesEndWith = new string[] { };

        private Vector2 _scrollPosition = Vector2.zero;

        [MenuItem("GameObject/MitarashiDango Avatar Utils/Import BlendShapes", false, 0)]
        internal static void OpenWindow()
        {
            var window = GetWindow<ImportBlendShapesEditorWindow>("Import BlendShapes");
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
            var importSourceTypeOptions = new GUIContent[]
            {
                new GUIContent("アニメーションクリップ"),
                new GUIContent("ブレンドシェイプセット"),
            };

            var pathTypeOptions = new GUIContent[]
            {
                new GUIContent("アバタールートからのパス"),
                new GUIContent("このオブジェクトからのパス"),
            };

            var excludeOptions = new GUIContent[]
            {
                new GUIContent("インポート対象に含める"),
                new GUIContent("インポート対象外とする"),
            };

            _gameObject = (GameObject)EditorGUILayout.ObjectField(new GUIContent("インポート先オブジェクト"), _gameObject, typeof(GameObject), true);

            using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(_scrollPosition))
            {
                _scrollPosition = scrollViewScope.scrollPosition;

                var so = new SerializedObject(this);
                so.Update();

                _importSourceTypeIndex = EditorGUILayout.Popup(new GUIContent("インポート元種別"), _importSourceTypeIndex, importSourceTypeOptions);

                switch (_importSourceTypeIndex)
                {
                    case 0:
                        _animationClip = (AnimationClip)EditorGUILayout.ObjectField(new GUIContent("インポート元オブジェクト"), _animationClip, typeof(AnimationClip), true);
                        EditorGUILayout.HelpBox("複数フレームを持つアニメーションクリップの場合、最終フレーム時点の値をインポートします", MessageType.Info);
                        break;
                    case 1:
                        _blendShapeSet = (BlendShapeSet)EditorGUILayout.ObjectField(new GUIContent("インポート元オブジェクト"), _blendShapeSet, typeof(BlendShapeSet), true);
                        EditorGUILayout.HelpBox("BlendShape Setのサポートは近日中に削除されます", MessageType.Warning);
                        break;
                }

                _pathTypeIndex = EditorGUILayout.Popup(new GUIContent("パス種別"), _pathTypeIndex, pathTypeOptions);

                _zeroWeightBlendShapesIncludeOptionIndex = EditorGUILayout.Popup(new GUIContent("値が0のシェイプキー"), _zeroWeightBlendShapesIncludeOptionIndex, excludeOptions);
                _vrcVisemeBlendShapesIncludeOptionIndex = EditorGUILayout.Popup(new GUIContent("vrc.で始まるシェイプキー"), _vrcVisemeBlendShapesIncludeOptionIndex, excludeOptions);
                _mmdBlendShapesIncludeOptionIndex = EditorGUILayout.Popup(new GUIContent("MMD関連のシェイプキー"), _mmdBlendShapesIncludeOptionIndex, excludeOptions);

                EditorGUILayout.PropertyField(so.FindProperty("_excludeBlendShapeNames"), new GUIContent("インポート対象外とするシェイプキー"), true);
                EditorGUILayout.PropertyField(so.FindProperty("_excludeBlendShapeNamesStartWith"), new GUIContent("インポート対象外とするシェイプキーのプレフィックス"), true);
                EditorGUILayout.PropertyField(so.FindProperty("_excludeBlendShapeNamesEndWith"), new GUIContent("インポート対象外とするシェイプキーのサフィックス"), true);

                so.ApplyModifiedProperties();

                var generateButtonRect = EditorGUILayout.GetControlRect(GUILayout.Height(EditorGUIUtility.singleLineHeight * 2));
                if (GUI.Button(generateButtonRect, new GUIContent("インポート")))
                {
                    ImportBlendShapes();
                }
            }
        }

        private void ImportBlendShapes()
        {
            if (_gameObject == null)
            {
                EditorUtility.DisplayDialog("エラー", "インポート先オブジェクトが指定されていません", "OK");
                return;
            }

            if ((_importSourceTypeIndex == 0 && _animationClip == null) || (_importSourceTypeIndex == 1 && _blendShapeSet == null))
            {
                EditorUtility.DisplayDialog("エラー", "インポート元オブジェクトが指定されていません", "OK");
                return;
            }

            var skinnedMeshRenderer = _gameObject.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer == null)
            {
                EditorUtility.DisplayDialog("エラー", "インポート先オブジェクトにSkinnedMeshRendererが存在しません", "OK");
                return;
            }

            EditorUtility.DisplayProgressBar("処理中", "", 0);

            var skinnedMesh = skinnedMeshRenderer.sharedMesh;

            var rootObject = _pathTypeIndex == 0 ? MiscUtil.GetAvatarRoot(_gameObject.transform) : _gameObject;
            var objectPath = MiscUtil.GetPathInHierarchy(_gameObject, rootObject);

            var blendShapes = GetBlendShapes();
            if (blendShapes == null)
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            var isDirty = false;
            for (var i = 0; i < skinnedMesh.blendShapeCount; i++)
            {
                float progress = i / (float)skinnedMesh.blendShapeCount;
                EditorUtility.DisplayProgressBar("処理中", $"{i} {skinnedMesh.blendShapeCount} ({(int)(progress * 100)}%)", progress);

                var blendShapeName = skinnedMesh.GetBlendShapeName(i);
                if (_excludeBlendShapeNames.ToList().Exists(name => blendShapeName == name)
                    || _excludeBlendShapeNamesStartWith.ToList().Exists(name => name != "" && blendShapeName.StartsWith(name))
                    || _excludeBlendShapeNamesEndWith.ToList().Exists(name => name != "" && blendShapeName.EndsWith(name))
                    || (_vrcVisemeBlendShapesIncludeOptionIndex == 1 && blendShapeName.StartsWith("vrc."))
                    || (_mmdBlendShapesIncludeOptionIndex == 1 && Constants.MMD_BLEND_SHAPE_NAMES.ToList().Exists(name => blendShapeName == name))
                    || !blendShapes.ContainsKey(blendShapeName))
                {
                    continue;
                }

                var currentBlendShapeWeight = skinnedMeshRenderer.GetBlendShapeWeight(i);
                var blendShapeWeight = blendShapes.GetValueOrDefault(blendShapeName, currentBlendShapeWeight);
                if (blendShapeWeight == currentBlendShapeWeight || _zeroWeightBlendShapesIncludeOptionIndex == 1 && blendShapeWeight == 0)
                {
                    continue;
                }

                if (!isDirty)
                {
                    if (PrefabUtility.GetPrefabAssetType(skinnedMeshRenderer) == PrefabAssetType.NotAPrefab)
                    {
                        Undo.RegisterCompleteObjectUndo(skinnedMeshRenderer, "Import BlendShapes");
                    }
                    else
                    {
                        Undo.RecordObject(skinnedMeshRenderer, "Import BlendShapes");
                        PrefabUtility.RecordPrefabInstancePropertyModifications(skinnedMeshRenderer);
                    }

                    isDirty = true;
                }

                skinnedMeshRenderer.SetBlendShapeWeight(i, blendShapeWeight);
            }

            EditorUtility.ClearProgressBar();
        }

        private Dictionary<string, float> GetBlendShapes()
        {
            if (_importSourceTypeIndex == 0)
            {
                if (_pathTypeIndex == 0)
                {
                    return GetBlendShapesFromAnimationClip(_animationClip, MiscUtil.GetPathInHierarchy(_gameObject, MiscUtil.GetAvatarRoot(_gameObject.transform)), _animationClip.length);
                }

                return GetBlendShapesFromAnimationClip(_animationClip, "", _animationClip.length);
            }

            if (_importSourceTypeIndex == 1)
            {
                return _blendShapeSet.ToDictionary();
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