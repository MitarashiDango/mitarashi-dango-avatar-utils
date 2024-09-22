using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MitarashiDango.AvatarUtils
{
    [CustomEditor(typeof(AnimatorControllerModifier))]
    public class AnimatorControllerModifierEditor : Editor
    {
        private SerializedProperty modifierOptions;

        private ReorderableList reorderableList;

        private void OnEnable()
        {
            modifierOptions = serializedObject.FindProperty("modifierOptions");

            reorderableList = new ReorderableList(serializedObject, modifierOptions)
            {
                drawElementCallback = (rect, index, active, focused) =>
                {
                    var additionalFaceEmote = modifierOptions.GetArrayElementAtIndex(index);
                    var position = new Rect(rect)
                    {
                        y = rect.y + EditorGUIUtility.standardVerticalSpacing
                    };
                    EditorGUI.PropertyField(position, additionalFaceEmote);
                },
                drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Animator Controller Modify Options", EditorStyles.boldLabel),
                elementHeightCallback = index => EditorGUI.GetPropertyHeight(modifierOptions.GetArrayElementAtIndex(index)) + EditorGUIUtility.standardVerticalSpacing * 2
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
