using UnityEditor.Animations;

namespace MitarashiDango.AvatarUtils
{
    public class AnimatorTransitionUtil
    {
        public static AnimatorTransitionBuilder AddEntryTransition(AnimatorStateMachine stateMachine, AnimatorStateMachine destinationStateMachine)
        {
            return new AnimatorTransitionBuilder(stateMachine.AddEntryTransition(destinationStateMachine));
        }

        public static AnimatorTransitionBuilder AddEntryTransition(AnimatorStateMachine stateMachine, AnimatorState destinationState)
        {
            return new AnimatorTransitionBuilder(stateMachine.AddEntryTransition(destinationState));
        }

        public static AnimatorTransitionBuilder AddExitTransition(AnimatorStateMachine sourceStateMachine, AnimatorStateMachine destinationStateMachine)
        {
            return new AnimatorTransitionBuilder(destinationStateMachine.AddStateMachineExitTransition(sourceStateMachine));
        }

        public static AnimatorStateTransitionBuilder AddExitTransition(AnimatorState destinationState)
        {
            return new AnimatorStateTransitionBuilder(destinationState.AddExitTransition());
        }

        public static AnimatorTransitionBuilder AddTransition(AnimatorStateMachine sourceStateMachine, AnimatorStateMachine destinationStateMachine)
        {
            return new AnimatorTransitionBuilder(destinationStateMachine.AddStateMachineTransition(sourceStateMachine));
        }

        public static AnimatorStateTransitionBuilder AddTransition(AnimatorState sourceState, AnimatorStateMachine destinationStateMachine)
        {
            return new AnimatorStateTransitionBuilder(sourceState.AddTransition(destinationStateMachine));
        }

        public static AnimatorStateTransitionBuilder AddTransition(AnimatorState sourceState, AnimatorState destinationState)
        {
            return new AnimatorStateTransitionBuilder(sourceState.AddTransition(destinationState));
        }
    }
}