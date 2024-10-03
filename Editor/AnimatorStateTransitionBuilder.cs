using System;
using UnityEditor.Animations;

namespace MitarashiDango.AvatarUtils
{
    public class AnimatorStateTransitionBuilder
    {
        public AnimatorStateTransition Transition { get; private set; }

        public AnimatorStateTransitionBuilder(AnimatorStateTransition transition)
        {
            Transition = transition;
        }

        public AnimatorStateTransitionBuilder If(string parameter)
        {
            Transition.AddCondition(AnimatorConditionMode.If, 0, parameter);
            return this;
        }

        public AnimatorStateTransitionBuilder IfNot(string parameter)
        {
            Transition.AddCondition(AnimatorConditionMode.IfNot, 0, parameter);
            return this;
        }

        public AnimatorStateTransitionBuilder Greater(string parameter, float value)
        {
            Transition.AddCondition(AnimatorConditionMode.Greater, value, parameter);
            return this;
        }

        public AnimatorStateTransitionBuilder Less(string parameter, float value)
        {
            Transition.AddCondition(AnimatorConditionMode.Less, value, parameter);
            return this;
        }

        public AnimatorStateTransitionBuilder Equals(string parameter, float value)
        {
            Transition.AddCondition(AnimatorConditionMode.Equals, value, parameter);
            return this;
        }

        public AnimatorStateTransitionBuilder NotEqual(string parameter, float value)
        {
            Transition.AddCondition(AnimatorConditionMode.NotEqual, value, parameter);
            return this;
        }

        public AnimatorStateTransitionBuilder Exec(Action<AnimatorStateTransitionBuilder> func)
        {
            func(this);
            return this;
        }

        public void SetImmediateTransitionSettings()
        {
            Transition.hasExitTime = false;
            Transition.exitTime = 0;
            Transition.hasFixedDuration = true;
            Transition.duration = 0;
            Transition.offset = 0;
            Transition.interruptionSource = TransitionInterruptionSource.None;
            Transition.orderedInterruption = true;
        }
    }
}