using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    [CustomEditor(typeof(AnimatorControllerModifier))]
    public class AnimatorControllerModifierEditor : Editor
    {
        private SerializedProperty layerModifyOptions;

        private ReorderableList reorderableList;

        private void OnEnable()
        {
            layerModifyOptions = serializedObject.FindProperty("layerModifyOptions");

            reorderableList = new ReorderableList(serializedObject, layerModifyOptions)
            {
                drawElementCallback = (rect, index, active, focused) =>
                {
                    var additionalFaceEmote = layerModifyOptions.GetArrayElementAtIndex(index);
                    var position = new Rect(rect)
                    {
                        y = rect.y + EditorGUIUtility.standardVerticalSpacing
                    };
                    EditorGUI.PropertyField(position, additionalFaceEmote);
                },
                drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Animator Controller Layer Modify Options", EditorStyles.boldLabel),
                elementHeightCallback = index => EditorGUI.GetPropertyHeight(layerModifyOptions.GetArrayElementAtIndex(index)) + EditorGUIUtility.standardVerticalSpacing * 2
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            reorderableList.DoLayoutList();

            if (EditorApplication.isPlaying)
            {
                return;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
