using UnityEditor;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    public class ExportBlendShapesEditorWindow : EditorWindow
    {
        private GameObject _gameObject;
        private int _pathTypeIndex;
        private bool _excludeZeroWeightBlendShapes;
        private string _excludeBlendShapeNameStartWith = "";

        private string _excludeBlendShapeNameEndWith = "";

        [MenuItem("GameObject/Export BlendShapes", false, 0)]
        internal static void OpenWindow()
        {
            var window = GetWindow<ExportBlendShapesEditorWindow>("Export BlendShapes");
            if (Selection.activeGameObject != null)
            {
                window._gameObject = Selection.activeGameObject;
            }

            var pos = window.position;
            pos.width = 350;
            pos.height = 160;
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

            _gameObject = (GameObject)EditorGUILayout.ObjectField(_gameObject, typeof(GameObject), true);
            _pathTypeIndex = EditorGUILayout.Popup(new GUIContent("パス出力モード"), _pathTypeIndex, pathTypeOptions);

            EditorGUILayout.LabelField("出力対象外とするシェイプキー");

            EditorGUI.indentLevel++;
            _excludeZeroWeightBlendShapes = EditorGUILayout.Toggle("値が0のシェイプキー", _excludeZeroWeightBlendShapes);
            _excludeBlendShapeNameStartWith = EditorGUILayout.TextField("名前のプレフィックス", _excludeBlendShapeNameStartWith);
            _excludeBlendShapeNameEndWith = EditorGUILayout.TextField("名前のサフィックス", _excludeBlendShapeNameEndWith);
            EditorGUI.indentLevel--;

            var generateButtonRect = EditorGUILayout.GetControlRect(GUILayout.Height(EditorGUIUtility.singleLineHeight * 2));
            if (GUI.Button(generateButtonRect, new GUIContent("アニメーションクリップ生成")))
            {
                ExportBlendShapes();
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

            var skinnedMesh = skinnedMeshRenderer.sharedMesh;

            var animationClip = new AnimationClip()
            {
                frameRate = 60
            };

            var rootObject = _pathTypeIndex == 0 ? MiscUtil.GetAvatarRoot(_gameObject.transform) : _gameObject;
            var objectPath = MiscUtil.GetPathInHierarchy(_gameObject, rootObject);

            for (var i = 0; i < skinnedMesh.blendShapeCount; i++)
            {
                var blendShapeName = skinnedMesh.GetBlendShapeName(i);
                if ((_excludeBlendShapeNameStartWith != "" && blendShapeName.StartsWith(_excludeBlendShapeNameStartWith))
                    || (_excludeBlendShapeNameEndWith != "" && blendShapeName.EndsWith(_excludeBlendShapeNameEndWith)))
                {
                    continue;
                }

                var blendShapeWeight = skinnedMeshRenderer.GetBlendShapeWeight(i);

                var animationCurve = new AnimationCurve();
                animationCurve.AddKey(0, blendShapeWeight);

                animationClip.SetCurve(objectPath, typeof(SkinnedMeshRenderer), $"blendShape.{blendShapeName}", animationCurve);
            }

            var filePath = EditorUtility.SaveFilePanelInProject("名前を付けて保存", $"AnimationClip_{_gameObject.name}", "anim", "アニメーションクリップの保存先を選択してください", "Assets");
            if (filePath == "")
            {
                EditorUtility.DisplayDialog("情報", "キャンセルされました", "OK");
                return;
            }

            AssetDatabase.CreateAsset(animationClip, filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}