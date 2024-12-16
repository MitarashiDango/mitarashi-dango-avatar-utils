using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    public class ImportBlendShapesEditorWindow : EditorWindow
    {
        private GameObject _gameObject;
        private BlendShapeSet _blendShapeSet;
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
            var excludeOptions = new GUIContent[]
            {
                new GUIContent("取り込み対象に含める"),
                new GUIContent("取り込み対象外とする"),
            };

            _gameObject = (GameObject)EditorGUILayout.ObjectField(_gameObject, typeof(GameObject), true);
            _blendShapeSet = (BlendShapeSet)EditorGUILayout.ObjectField(_blendShapeSet, typeof(BlendShapeSet), true);

            using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(_scrollPosition))
            {
                _scrollPosition = scrollViewScope.scrollPosition;

                var so = new SerializedObject(this);
                so.Update();

                _zeroWeightBlendShapesIncludeOptionIndex = EditorGUILayout.Popup(new GUIContent("値が0のシェイプキー"), _zeroWeightBlendShapesIncludeOptionIndex, excludeOptions);
                _vrcVisemeBlendShapesIncludeOptionIndex = EditorGUILayout.Popup(new GUIContent("vrc.で始まるシェイプキー"), _vrcVisemeBlendShapesIncludeOptionIndex, excludeOptions);
                _mmdBlendShapesIncludeOptionIndex = EditorGUILayout.Popup(new GUIContent("MMD用シェイプキー"), _mmdBlendShapesIncludeOptionIndex, excludeOptions);

                EditorGUILayout.PropertyField(so.FindProperty("_excludeBlendShapeNames"), new GUIContent("取り込み対象外とするシェイプキー"), true);
                EditorGUILayout.PropertyField(so.FindProperty("_excludeBlendShapeNamesStartWith"), new GUIContent("取り込み対象外とするシェイプキーのプレフィックス"), true);
                EditorGUILayout.PropertyField(so.FindProperty("_excludeBlendShapeNamesEndWith"), new GUIContent("取り込み対象外とするシェイプキーのサフィックス"), true);

                so.ApplyModifiedProperties();

                var generateButtonRect = EditorGUILayout.GetControlRect(GUILayout.Height(EditorGUIUtility.singleLineHeight * 2));
                if (GUI.Button(generateButtonRect, new GUIContent("取り込み")))
                {
                    ImportBlendShapes();
                }
            }
        }

        private void ImportBlendShapes()
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

            EditorUtility.DisplayProgressBar("処理中", "", 0);

            var skinnedMesh = skinnedMeshRenderer.sharedMesh;

            var animationClip = new AnimationClip()
            {
                frameRate = 60
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
                    || (_mmdBlendShapesIncludeOptionIndex == 1 && Constants.MMD_BLEND_SHAPE_NAMES.ToList().Exists(name => blendShapeName == name))
                    || !_blendShapeSet.Exists(blendShapeName))
                {
                    continue;
                }

                var blendShape = _blendShapeSet.Get(blendShapeName);
                if (blendShape == null || _zeroWeightBlendShapesIncludeOptionIndex == 1 && blendShape.weight == 0)
                {
                    continue;
                }

                skinnedMeshRenderer.SetBlendShapeWeight(i, blendShape.weight);
            }

            EditorUtility.ClearProgressBar();
        }
    }
}