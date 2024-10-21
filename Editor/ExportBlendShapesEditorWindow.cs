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

        [SerializeField]
        private string[] _excludeBlendShapeNames = new string[] { };

        [SerializeField]
        private string[] _excludeBlendShapeNamesStartWith = new string[] { };

        [SerializeField]
        private string[] _excludeBlendShapeNamesEndWith = new string[] { };

        private Vector2 _scrollPosition = Vector2.zero;

        [MenuItem("GameObject/Export BlendShapes", false, 0)]
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
            var pathTypeOptions = new GUIContent[]
            {
                new GUIContent("アバタールートからのパス"),
                new GUIContent("このオブジェクトからのパス"),
            };

            var excludeOptions = new GUIContent[]
            {
                new GUIContent("出力対象に含める"),
                new GUIContent("出力対象外とする"),
            };

            _gameObject = (GameObject)EditorGUILayout.ObjectField(_gameObject, typeof(GameObject), true);

            using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(_scrollPosition))
            {
                _scrollPosition = scrollViewScope.scrollPosition;

                var so = new SerializedObject(this);
                so.Update();

                _pathTypeIndex = EditorGUILayout.Popup(new GUIContent("パス出力モード"), _pathTypeIndex, pathTypeOptions);
                _zeroWeightBlendShapesIncludeOptionIndex = EditorGUILayout.Popup(new GUIContent("値が0のシェイプキー"), _zeroWeightBlendShapesIncludeOptionIndex, excludeOptions);
                _vrcVisemeBlendShapesIncludeOptionIndex = EditorGUILayout.Popup(new GUIContent("vrc.で始まるシェイプキー"), _vrcVisemeBlendShapesIncludeOptionIndex, excludeOptions);
                _mmdBlendShapesIncludeOptionIndex = EditorGUILayout.Popup(new GUIContent("MMD用シェイプキー"), _mmdBlendShapesIncludeOptionIndex, excludeOptions);

                EditorGUILayout.PropertyField(so.FindProperty("_excludeBlendShapeNames"), new GUIContent("出力対象外とするシェイプキー"), true);
                EditorGUILayout.PropertyField(so.FindProperty("_excludeBlendShapeNamesStartWith"), new GUIContent("出力対象外とするシェイプキーのプレフィックス"), true);
                EditorGUILayout.PropertyField(so.FindProperty("_excludeBlendShapeNamesEndWith"), new GUIContent("出力対象外とするシェイプキーのサフィックス"), true);

                so.ApplyModifiedProperties();

                var exportBlendShapeButtonRect = EditorGUILayout.GetControlRect(GUILayout.Height(EditorGUIUtility.singleLineHeight * 2));
                if (GUI.Button(exportBlendShapeButtonRect, new GUIContent("エクスポート")))
                {
                    ExportBlendShapes();
                }

                var generateAnimationClipButtonRect = EditorGUILayout.GetControlRect(GUILayout.Height(EditorGUIUtility.singleLineHeight * 2));
                if (GUI.Button(generateAnimationClipButtonRect, new GUIContent("アニメーションクリップ生成")))
                {
                    ExportToAnimationClip();
                }
            }
        }

        private void ExportBlendShapes()
        {
            if (_gameObject == null)
            {
                EditorUtility.DisplayDialog("エラー", "オブジェクトが指定されていません", "OK");
                return;
            }

            var skinnedMeshRenderer = _gameObject.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer == null)
            {
                EditorUtility.DisplayDialog("エラー", "指定されたオブジェクトにSkinnedMeshRendererが存在しません", "OK");
                return;
            }

            var filePath = EditorUtility.SaveFilePanelInProject("名前を付けて保存", $"AnimationClip_{_gameObject.name}", "asset", "ブレンドシェイプのエクスポート先を選択してください", "Assets");
            if (filePath == "")
            {
                EditorUtility.DisplayDialog("情報", "キャンセルされました", "OK");
                return;
            }

            var filename = Path.GetFileNameWithoutExtension(filePath);

            EditorUtility.DisplayProgressBar("処理中", "", 0);

            var skinnedMesh = skinnedMeshRenderer.sharedMesh;

            var blendShapeSet = new BlendShapeSet
            {
                name = filename,
            };

            var rootObject = _pathTypeIndex == 0 ? MiscUtil.GetAvatarRoot(_gameObject.transform) : _gameObject;
            var objectPath = MiscUtil.GetPathInHierarchy(_gameObject, rootObject);

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

                blendShapeSet.Set(blendShapeName, blendShapeWeight);
            }

            EditorUtility.ClearProgressBar();

            // ファイル保存
            var asset = AssetDatabase.LoadAssetAtPath(filePath, typeof(BlendShapeSet));
            if (asset == null)
            {
                AssetDatabase.CreateAsset(blendShapeSet, filePath);
            }
            else
            {
                EditorUtility.CopySerialized(blendShapeSet, asset);
                AssetDatabase.SaveAssets();
            }

            AssetDatabase.Refresh();
            return;
        }

        private void ExportToAnimationClip()
        {
            if (_gameObject == null)
            {
                EditorUtility.DisplayDialog("エラー", "オブジェクトが指定されていません", "OK");
                return;
            }

            var skinnedMeshRenderer = _gameObject.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer == null)
            {
                EditorUtility.DisplayDialog("エラー", "指定されたオブジェクトにSkinnedMeshRendererが存在しません", "OK");
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
    }
}