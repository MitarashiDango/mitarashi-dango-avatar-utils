
using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

namespace MitarashiDango.FaceEmoteControl
{
#if UNITY_EDITOR
    public class AnimatorControllerGenerator
    {
        private AnimationClip blankAnimationClip = new AnimationClip
        {
            name = "blank"
        };

        public AnimatorController GenerateAnimatorController(FaceEmoteControl faceEmoteControl)
        {
            var animatorController = new AnimatorController
            {
                name = "FEC_ANIMATOR_CONTROLLER",
                parameters = GenerateParameters()
            };

            if (animatorController.layers.Length == 0)
            {
                animatorController.AddLayer("DUMMY_LAYER");
            }

            animatorController.AddLayer(GenerateFaceEmoteGestureLockLayer());
            animatorController.AddLayer(GenerateLeftHandGestureLayer());
            animatorController.AddLayer(GenerateRightHandGestureLayer());
            animatorController.AddLayer(GenerateSetFaceEmoteTypeLayer(faceEmoteControl));
            animatorController.AddLayer(GenerateFaceEmoteLockIndicatorControlLayer());
            animatorController.AddLayer(GenerateDefaultFaceEmoteLayer(faceEmoteControl));
            animatorController.AddLayer(GenerateFaceEmoteSettingsLayer(faceEmoteControl));

            return animatorController;
        }

        private AnimatorControllerParameter[] GenerateParameters()
        {
            return new AnimatorControllerParameter[]{
                new AnimatorControllerParameter{
                    name = Parameters.AFK,
                    type = AnimatorControllerParameterType.Bool,
                    defaultBool = false,
                },
                new AnimatorControllerParameter{
                    name = Parameters.IN_STATION,
                    type = AnimatorControllerParameterType.Bool,
                    defaultBool = false,
                },
                new AnimatorControllerParameter{
                    name = Parameters.IS_LOCAL,
                    type = AnimatorControllerParameterType.Bool,
                    defaultBool = false,
                },
                new AnimatorControllerParameter{
                    name = Parameters.GESTURE_LEFT,
                    type = AnimatorControllerParameterType.Int,
                    defaultInt = 0,
                },
                new AnimatorControllerParameter{
                    name = Parameters.GESTURE_RIGHT,
                    type = AnimatorControllerParameterType.Int,
                    defaultInt = 0,
                },
                new AnimatorControllerParameter{
                    name = Parameters.GESTURE_LEFT_WEIGHT,
                    type = AnimatorControllerParameterType.Float,
                    defaultFloat = 0,
                },
                new AnimatorControllerParameter{
                    name = Parameters.GESTURE_RIGHT_WEIGHT,
                    type = AnimatorControllerParameterType.Float,
                    defaultFloat = 0,
                },
                new AnimatorControllerParameter{
                    name = Parameters.FEC_SELECTED_FACE_EMOTE,
                    type = AnimatorControllerParameterType.Int,
                    defaultInt = 0,
                },
                new AnimatorControllerParameter{
                    name = Parameters.FEC_FACE_EMOTE_LOCKED,
                    type = AnimatorControllerParameterType.Bool,
                    defaultBool = false,
                },
                new AnimatorControllerParameter{
                    name = Parameters.FEC_FACE_EMOTE_LOCKER_ENABLED,
                    type = AnimatorControllerParameterType.Bool,
                    defaultBool = true,
                },
                new AnimatorControllerParameter{
                    name = Parameters.FEC_FACE_EMOTE_LOCKER_AUTO_DISABLE_ON_SIT,
                    type = AnimatorControllerParameterType.Bool,
                    defaultBool = false,
                },
                new AnimatorControllerParameter{
                    name = Parameters.FEC_FACE_EMOTE_LOCKER_CONTACT,
                    type = AnimatorControllerParameterType.Bool,
                    defaultBool = false,
                },
                new AnimatorControllerParameter{
                    name = Parameters.FEC_SELECTED_GESTURE_LEFT,
                    type = AnimatorControllerParameterType.Int,
                    defaultInt = 0,
                },
                new AnimatorControllerParameter{
                    name = Parameters.FEC_SELECTED_GESTURE_RIGHT,
                    type = AnimatorControllerParameterType.Int,
                    defaultInt = 0,
                },
                new AnimatorControllerParameter{
                    name = Parameters.FEC_SELECTED_FACE_EMOTE_BY_MENU,
                    type = AnimatorControllerParameterType.Int,
                    defaultInt = 0,
                },
            };
        }

        private VRCAvatarParameterDriver GenerateVRCAvatarParameterLocalSetDriver(string parameterName, float value)
        {
            return new VRCAvatarParameterDriver
            {
                localOnly = true,
                parameters = new List<VRC_AvatarParameterDriver.Parameter>
                {
                    new VRC_AvatarParameterDriver.Parameter
                    {
                        type = VRC_AvatarParameterDriver.ChangeType.Set,
                        name = parameterName,
                        value = value,
                    }
                }
            };
        }

        private AnimatorControllerLayer GenerateFaceEmoteGestureLockLayer()
        {
            var layer = new AnimatorControllerLayer
            {
                name = "FEC_FACE_EMOTE_GESTURE_LOCK",
                defaultWeight = 0,
                stateMachine = new AnimatorStateMachine(),
            };

            layer.stateMachine.entryPosition = new Vector3(0, 0, 0);
            layer.stateMachine.exitPosition = new Vector3(0, -40, 0);
            layer.stateMachine.anyStatePosition = new Vector3(0, -80, 0);

            var initialState = layer.stateMachine.AddState("Initial State", new Vector3(200, 0, 0));
            initialState.writeDefaultValues = false;
            initialState.motion = blankAnimationClip;

            var gestureLockDisabledState = layer.stateMachine.AddState("Gesture Lock Disabled", new Vector3(400, -80, 0));
            gestureLockDisabledState.writeDefaultValues = false;
            gestureLockDisabledState.motion = blankAnimationClip;

            var gestureLockEnabledState = layer.stateMachine.AddState("Gesture Lock Enabled", new Vector3(400, 80, 0));
            gestureLockEnabledState.writeDefaultValues = false;
            gestureLockEnabledState.motion = blankAnimationClip;

            var setDisableState = layer.stateMachine.AddState("Set Disable", new Vector3(600, -160, 0));
            setDisableState.writeDefaultValues = false;
            setDisableState.motion = blankAnimationClip;
            setDisableState.behaviours = new StateMachineBehaviour[]{
                GenerateVRCAvatarParameterLocalSetDriver(Parameters.FEC_FACE_EMOTE_LOCKED, 0)
            };

            var setEnableState = layer.stateMachine.AddState("Set Enable", new Vector3(600, 160, 0));
            setEnableState.writeDefaultValues = false;
            setEnableState.motion = blankAnimationClip;
            setEnableState.behaviours = new StateMachineBehaviour[]{
                GenerateVRCAvatarParameterLocalSetDriver(Parameters.FEC_FACE_EMOTE_LOCKED, 1)
            };

            var lockToUnlockSleepState = layer.stateMachine.AddState("Sleep (Lock to Unlock)", new Vector3(800, -80, 0));
            lockToUnlockSleepState.writeDefaultValues = false;
            lockToUnlockSleepState.motion = blankAnimationClip;

            var unlockToLockSleepState = layer.stateMachine.AddState("Sleep (Unlock to Lock)", new Vector3(800, 80, 0));
            unlockToLockSleepState.writeDefaultValues = false;
            unlockToLockSleepState.motion = blankAnimationClip;

            // [Initial State] -> [Gesture Lock Disabled]
            var initialToGestureLockDisabledTransition1 = initialState.AddTransition(gestureLockDisabledState);
            SetImmediateTransitionSetting(initialToGestureLockDisabledTransition1);
            initialToGestureLockDisabledTransition1.AddCondition(AnimatorConditionMode.If, 0, Parameters.IS_LOCAL);
            initialToGestureLockDisabledTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.FEC_FACE_EMOTE_LOCKED);

            // [Initial State] -> [Gesture Lock Enabled]
            var initialToGestureLockEnabledTransition1 = initialState.AddTransition(gestureLockEnabledState);
            SetImmediateTransitionSetting(initialToGestureLockEnabledTransition1);
            initialToGestureLockEnabledTransition1.AddCondition(AnimatorConditionMode.If, 0, Parameters.IS_LOCAL);
            initialToGestureLockEnabledTransition1.AddCondition(AnimatorConditionMode.If, 0, Parameters.FEC_FACE_EMOTE_LOCKED);

            // [Gesture Lock Disabled] -> [Gesture Lock Enabled]
            var gestureLockDisabledToGestureLockEnabledTransition1 = gestureLockDisabledState.AddTransition(gestureLockEnabledState);
            SetImmediateTransitionSetting(gestureLockDisabledToGestureLockEnabledTransition1);
            gestureLockDisabledToGestureLockEnabledTransition1.AddCondition(AnimatorConditionMode.If, 0, Parameters.FEC_FACE_EMOTE_LOCKED);

            // [Gesture Lock Enabled] -> [Gesture Lock Disabled]
            var gestureLockEnabledToGestureLockDisabledTransition1 = gestureLockEnabledState.AddTransition(gestureLockDisabledState);
            SetImmediateTransitionSetting(gestureLockEnabledToGestureLockDisabledTransition1);
            gestureLockEnabledToGestureLockDisabledTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.FEC_FACE_EMOTE_LOCKED);

            // [Gesture Lock Disabled] -> [Set Enable] (Transition 1)
            var gestureLockDisabledToSetEnableTransition1 = gestureLockDisabledState.AddTransition(setEnableState);
            SetImmediateTransitionSetting(gestureLockDisabledToSetEnableTransition1);
            gestureLockDisabledToSetEnableTransition1.AddCondition(AnimatorConditionMode.If, 0, Parameters.FEC_FACE_EMOTE_LOCKER_CONTACT);
            gestureLockDisabledToSetEnableTransition1.AddCondition(AnimatorConditionMode.If, 0, Parameters.FEC_FACE_EMOTE_LOCKER_ENABLED);
            gestureLockDisabledToSetEnableTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.FEC_FACE_EMOTE_LOCKER_AUTO_DISABLE_ON_SIT);
            gestureLockDisabledToSetEnableTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.AFK);

            // [Gesture Lock Disabled] -> [Set Enable] (Transition 2)
            var gestureLockDisabledToSetEnableTransition2 = gestureLockDisabledState.AddTransition(setEnableState);
            SetImmediateTransitionSetting(gestureLockDisabledToSetEnableTransition2);
            gestureLockDisabledToSetEnableTransition2.AddCondition(AnimatorConditionMode.If, 0, Parameters.FEC_FACE_EMOTE_LOCKER_CONTACT);
            gestureLockDisabledToSetEnableTransition2.AddCondition(AnimatorConditionMode.If, 0, Parameters.FEC_FACE_EMOTE_LOCKER_ENABLED);
            gestureLockDisabledToSetEnableTransition2.AddCondition(AnimatorConditionMode.If, 0, Parameters.FEC_FACE_EMOTE_LOCKER_AUTO_DISABLE_ON_SIT);
            gestureLockDisabledToSetEnableTransition2.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.AFK);
            gestureLockDisabledToSetEnableTransition2.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.IN_STATION);

            // [Gesture Lock Enabled] -> [Set Disable] (Transition 1)
            var gestureLockEnabledToSetDisableTransition1 = gestureLockEnabledState.AddTransition(setDisableState);
            SetImmediateTransitionSetting(gestureLockEnabledToSetDisableTransition1);
            gestureLockEnabledToSetDisableTransition1.AddCondition(AnimatorConditionMode.If, 0, Parameters.FEC_FACE_EMOTE_LOCKER_CONTACT);
            gestureLockEnabledToSetDisableTransition1.AddCondition(AnimatorConditionMode.If, 0, Parameters.FEC_FACE_EMOTE_LOCKER_ENABLED);
            gestureLockEnabledToSetDisableTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.FEC_FACE_EMOTE_LOCKER_AUTO_DISABLE_ON_SIT);
            gestureLockEnabledToSetDisableTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.AFK);

            // [Gesture Lock Enabled] -> [Set Disable] (Transition 2)
            var gestureLockEnabledToSetDisableTransition2 = gestureLockEnabledState.AddTransition(setDisableState);
            SetImmediateTransitionSetting(gestureLockEnabledToSetDisableTransition2);
            gestureLockEnabledToSetDisableTransition2.AddCondition(AnimatorConditionMode.If, 0, Parameters.FEC_FACE_EMOTE_LOCKER_CONTACT);
            gestureLockEnabledToSetDisableTransition2.AddCondition(AnimatorConditionMode.If, 0, Parameters.FEC_FACE_EMOTE_LOCKER_ENABLED);
            gestureLockEnabledToSetDisableTransition2.AddCondition(AnimatorConditionMode.If, 0, Parameters.FEC_FACE_EMOTE_LOCKER_AUTO_DISABLE_ON_SIT);
            gestureLockEnabledToSetDisableTransition2.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.AFK);
            gestureLockEnabledToSetDisableTransition2.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.IN_STATION);

            // [Set Disable] -> [Sleep (Lock to Unlock)]
            var setDisableToLockToUnlockSleepTransition1 = setDisableState.AddTransition(lockToUnlockSleepState);
            setDisableToLockToUnlockSleepTransition1.hasExitTime = true;
            setDisableToLockToUnlockSleepTransition1.exitTime = 0.5f;
            setDisableToLockToUnlockSleepTransition1.hasFixedDuration = true;
            setDisableToLockToUnlockSleepTransition1.duration = 0;
            setDisableToLockToUnlockSleepTransition1.offset = 0;
            setDisableToLockToUnlockSleepTransition1.interruptionSource = TransitionInterruptionSource.None;
            setDisableToLockToUnlockSleepTransition1.orderedInterruption = true;

            // [Set Enable] -> [Sleep (Unlock to Lock)]
            var setEnableToUnlockToLockSleepTransition1 = setEnableState.AddTransition(unlockToLockSleepState);
            setEnableToUnlockToLockSleepTransition1.hasExitTime = true;
            setEnableToUnlockToLockSleepTransition1.exitTime = 0.5f;
            setEnableToUnlockToLockSleepTransition1.hasFixedDuration = true;
            setEnableToUnlockToLockSleepTransition1.duration = 0;
            setEnableToUnlockToLockSleepTransition1.offset = 0;
            setEnableToUnlockToLockSleepTransition1.interruptionSource = TransitionInterruptionSource.None;
            setEnableToUnlockToLockSleepTransition1.orderedInterruption = true;

            // [Sleep (Lock to Unlock)] -> [Gesture Lock Disabled]
            var lockToUnlockSleepToGestureLockDisabledTransition1 = lockToUnlockSleepState.AddTransition(gestureLockDisabledState);
            SetImmediateTransitionSetting(lockToUnlockSleepToGestureLockDisabledTransition1);
            lockToUnlockSleepToGestureLockDisabledTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.FEC_FACE_EMOTE_LOCKER_CONTACT);

            // [Sleep (Unlock to Lock)] -> [Gesture Lock Enabled]
            var unlockToLockSleepToGestureLockEnabledTransition1 = unlockToLockSleepState.AddTransition(gestureLockEnabledState);
            SetImmediateTransitionSetting(unlockToLockSleepToGestureLockEnabledTransition1);
            unlockToLockSleepToGestureLockEnabledTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.FEC_FACE_EMOTE_LOCKER_CONTACT);

            return layer;
        }

        private AnimatorControllerLayer GenerateLeftHandGestureLayer()
        {
            return GenerateHandGestureLayer("FEC_LEFT_HAND_GESTURE", Parameters.FEC_SELECTED_GESTURE_LEFT, Parameters.GESTURE_LEFT);
        }

        private AnimatorControllerLayer GenerateRightHandGestureLayer()
        {
            return GenerateHandGestureLayer("FEC_RIGHT_HAND_GESTURE", Parameters.FEC_SELECTED_GESTURE_RIGHT, Parameters.GESTURE_RIGHT);
        }

        private AnimatorControllerLayer GenerateHandGestureLayer(string layerName, string selectedGestureParameterName, string gestureParameterName)
        {
            var layer = new AnimatorControllerLayer
            {
                name = layerName,
                defaultWeight = 0,
                stateMachine = new AnimatorStateMachine(),
            };

            layer.stateMachine.entryPosition = new Vector3(0, 0, 0);
            layer.stateMachine.anyStatePosition = new Vector3(0, -40, 0);
            layer.stateMachine.exitPosition = new Vector3(800, 0, 0);

            var initialState = layer.stateMachine.AddState("Initial State", new Vector3(200, 0, 0));
            initialState.writeDefaultValues = false;
            initialState.motion = blankAnimationClip;

            var neutralState = layer.stateMachine.AddState("Neutral", new Vector3(500, 0, 0));
            neutralState.writeDefaultValues = false;
            neutralState.motion = blankAnimationClip;
            neutralState.behaviours = new StateMachineBehaviour[]{
                GenerateVRCAvatarParameterLocalSetDriver(selectedGestureParameterName, 0)
            };

            var fistState = layer.stateMachine.AddState("Fist", new Vector3(500, 60, 0));
            fistState.writeDefaultValues = false;
            fistState.motion = blankAnimationClip;
            fistState.behaviours = new StateMachineBehaviour[]{
                GenerateVRCAvatarParameterLocalSetDriver(selectedGestureParameterName, 1)
            };

            var handOpenState = layer.stateMachine.AddState("HandOpen", new Vector3(500, 120, 0));
            handOpenState.writeDefaultValues = false;
            handOpenState.motion = blankAnimationClip;
            handOpenState.behaviours = new StateMachineBehaviour[]{
                GenerateVRCAvatarParameterLocalSetDriver(selectedGestureParameterName, 2)
            };

            var fingerPointState = layer.stateMachine.AddState("FingerPoint", new Vector3(500, 180, 0));
            fingerPointState.writeDefaultValues = false;
            fingerPointState.motion = blankAnimationClip;
            fingerPointState.behaviours = new StateMachineBehaviour[]{
                GenerateVRCAvatarParameterLocalSetDriver(selectedGestureParameterName, 3)
            };

            var victoryState = layer.stateMachine.AddState("Victory", new Vector3(500, 240, 0));
            victoryState.writeDefaultValues = false;
            victoryState.motion = blankAnimationClip;
            victoryState.behaviours = new StateMachineBehaviour[]{
                GenerateVRCAvatarParameterLocalSetDriver(selectedGestureParameterName, 4)
            };

            var rockNRollState = layer.stateMachine.AddState("RockNRoll", new Vector3(500, 300, 0));
            rockNRollState.writeDefaultValues = false;
            rockNRollState.motion = blankAnimationClip;
            rockNRollState.behaviours = new StateMachineBehaviour[]{
                GenerateVRCAvatarParameterLocalSetDriver(selectedGestureParameterName, 5)
            };

            var handGunState = layer.stateMachine.AddState("HandGun", new Vector3(500, 360, 0));
            handGunState.writeDefaultValues = false;
            handGunState.motion = blankAnimationClip;
            handGunState.behaviours = new StateMachineBehaviour[]{
                GenerateVRCAvatarParameterLocalSetDriver(selectedGestureParameterName, 6)
            };

            var thumbsUpState = layer.stateMachine.AddState("ThumbsUp", new Vector3(500, 420, 0));
            thumbsUpState.writeDefaultValues = false;
            thumbsUpState.motion = blankAnimationClip;
            thumbsUpState.behaviours = new StateMachineBehaviour[]{
                GenerateVRCAvatarParameterLocalSetDriver(selectedGestureParameterName, 7)
            };

            Action<AnimatorState, string, int> addFromInitialTransition = (AnimatorState state, string gestureParameterName, int gestureNumber) =>
            {
                var transition1 = initialState.AddTransition(state);
                SetImmediateTransitionSetting(transition1);
                transition1.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.FEC_FACE_EMOTE_LOCKED);
                transition1.AddCondition(AnimatorConditionMode.Equals, gestureNumber, gestureParameterName);
                transition1.AddCondition(AnimatorConditionMode.If, 0, Parameters.IS_LOCAL);
            };

            // [Initial State] -> [Neutral]
            addFromInitialTransition(neutralState, gestureParameterName, 0);

            // [Initial State] -> [Fist]
            addFromInitialTransition(fistState, gestureParameterName, 1);

            // [Initial State] -> [HandOpen]
            addFromInitialTransition(handOpenState, gestureParameterName, 2);

            // [Initial State] -> [FingerPoint]
            addFromInitialTransition(fingerPointState, gestureParameterName, 3);

            // [Initial State] -> [Victory]
            addFromInitialTransition(victoryState, gestureParameterName, 4);

            // [Initial State] -> [RockNRoll]
            addFromInitialTransition(rockNRollState, gestureParameterName, 5);

            // [Initial State] -> [HandGun]
            addFromInitialTransition(handGunState, gestureParameterName, 6);

            // [Initial State] -> [ThumbsUp]
            addFromInitialTransition(thumbsUpState, gestureParameterName, 7);

            Action<AnimatorState, string, int> addToExitTransition = (AnimatorState state, string gestureParameterName, int gestureNumber) =>
            {
                var transition1 = state.AddExitTransition();
                SetImmediateTransitionSetting(transition1);
                transition1.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.FEC_FACE_EMOTE_LOCKED);
                transition1.AddCondition(AnimatorConditionMode.NotEqual, gestureNumber, gestureParameterName);
            };

            // [Neutral] -> [Exit]
            addToExitTransition(neutralState, gestureParameterName, 0);

            // [Fist] -> [Exit]
            addToExitTransition(fistState, gestureParameterName, 1);

            // [HandOpen] -> [Exit]
            addToExitTransition(handOpenState, gestureParameterName, 2);

            // [FingerPoint] -> [Exit]
            addToExitTransition(fingerPointState, gestureParameterName, 3);

            // [Victory] -> [Exit]
            addToExitTransition(victoryState, gestureParameterName, 4);

            // [RockNRoll] -> [Exit]
            addToExitTransition(rockNRollState, gestureParameterName, 5);

            // [HandGun] -> [Exit]
            addToExitTransition(handGunState, gestureParameterName, 6);

            // [ThumbsUp] -> [Exit]
            addToExitTransition(thumbsUpState, gestureParameterName, 7);

            return layer;
        }

        private AnimatorControllerLayer GenerateSetFaceEmoteTypeLayer(FaceEmoteControl faceEmoteControl)
        {
            var layer = new AnimatorControllerLayer
            {
                name = "FEC_SET_FACE_EMOTE_TYPE",
                defaultWeight = 0,
                stateMachine = new AnimatorStateMachine(),
            };

            layer.stateMachine.entryPosition = new Vector3(0, 0, 0);
            layer.stateMachine.anyStatePosition = new Vector3(0, -40, 0);
            layer.stateMachine.exitPosition = new Vector3(800, 0, 0);

            var initialState = layer.stateMachine.AddState("Initial State", new Vector3(200, 0, 0));
            initialState.writeDefaultValues = false;
            initialState.motion = blankAnimationClip;

            var idleState = layer.stateMachine.AddState("Idle", new Vector3(500, 0, 0));
            idleState.writeDefaultValues = false;
            idleState.motion = blankAnimationClip;
            idleState.behaviours = new StateMachineBehaviour[]{
                GenerateVRCAvatarParameterLocalSetDriver(Parameters.FEC_SELECTED_FACE_EMOTE, 0)
            };

            var initialToIdleTransition1 = initialState.AddTransition(idleState);
            SetImmediateTransitionSetting(initialToIdleTransition1);
            initialToIdleTransition1.AddCondition(AnimatorConditionMode.If, 0, Parameters.IS_LOCAL);
            initialToIdleTransition1.AddCondition(AnimatorConditionMode.Equals, 0, Parameters.FEC_SELECTED_FACE_EMOTE_BY_MENU);
            initialToIdleTransition1.AddCondition(AnimatorConditionMode.Equals, 0, Parameters.FEC_SELECTED_GESTURE_LEFT);
            initialToIdleTransition1.AddCondition(AnimatorConditionMode.Equals, 0, Parameters.FEC_SELECTED_GESTURE_RIGHT);

            var idleToExitTransition1 = idleState.AddExitTransition();
            SetImmediateTransitionSetting(idleToExitTransition1);
            idleToExitTransition1.AddCondition(AnimatorConditionMode.NotEqual, 0, Parameters.FEC_SELECTED_FACE_EMOTE_BY_MENU);

            var idleToExitTransition2 = idleState.AddExitTransition();
            SetImmediateTransitionSetting(idleToExitTransition2);
            idleToExitTransition2.AddCondition(AnimatorConditionMode.NotEqual, 0, Parameters.FEC_SELECTED_GESTURE_LEFT);

            var idleToExitTransition3 = idleState.AddExitTransition();
            SetImmediateTransitionSetting(idleToExitTransition3);
            idleToExitTransition3.AddCondition(AnimatorConditionMode.NotEqual, 0, Parameters.FEC_SELECTED_GESTURE_RIGHT);

            Action<AnimatorStateMachine, string, string, int, int, Vector3> addStateAndTransition = (AnimatorStateMachine stateMachine, string stateName, string selectedGestureParameterName, int gestureNumber, int faceEmoteNumber, Vector3 position) =>
            {
                var state = stateMachine.AddState(stateName, position);
                state.writeDefaultValues = false;
                state.motion = blankAnimationClip;
                state.behaviours = new StateMachineBehaviour[]{
                    GenerateVRCAvatarParameterLocalSetDriver(Parameters.FEC_SELECTED_FACE_EMOTE, faceEmoteNumber)
                };

                var fromEntryTransition1 = stateMachine.AddEntryTransition(state);
                fromEntryTransition1.AddCondition(AnimatorConditionMode.Equals, gestureNumber, selectedGestureParameterName);

                var toExitTransition1 = state.AddExitTransition();
                SetImmediateTransitionSetting(toExitTransition1);
                toExitTransition1.AddCondition(AnimatorConditionMode.NotEqual, 0, Parameters.FEC_SELECTED_FACE_EMOTE_BY_MENU);

                var toExitTransition2 = state.AddExitTransition();
                SetImmediateTransitionSetting(toExitTransition2);
                toExitTransition2.AddCondition(AnimatorConditionMode.NotEqual, gestureNumber, selectedGestureParameterName);
            };

            var leftHandStateMachine = layer.stateMachine.AddStateMachine("Left Hand", new Vector3(500, 60, 0));

            var initialToLeftHandTransition1 = initialState.AddTransition(leftHandStateMachine);
            SetImmediateTransitionSetting(initialToLeftHandTransition1);
            initialToLeftHandTransition1.AddCondition(AnimatorConditionMode.If, 0, Parameters.IS_LOCAL);
            initialToLeftHandTransition1.AddCondition(AnimatorConditionMode.Equals, 0, Parameters.FEC_SELECTED_FACE_EMOTE_BY_MENU);
            initialToLeftHandTransition1.AddCondition(AnimatorConditionMode.NotEqual, 0, Parameters.FEC_SELECTED_GESTURE_LEFT);

            layer.stateMachine.AddStateMachineExitTransition(leftHandStateMachine);

            addStateAndTransition(leftHandStateMachine, "Fist", Parameters.FEC_SELECTED_GESTURE_LEFT, 1, 1, new Vector3(400, 00, 0));
            addStateAndTransition(leftHandStateMachine, "HandOpen", Parameters.FEC_SELECTED_GESTURE_LEFT, 2, 2, new Vector3(400, 60, 0));
            addStateAndTransition(leftHandStateMachine, "FingerPoint", Parameters.FEC_SELECTED_GESTURE_LEFT, 3, 3, new Vector3(400, 120, 0));
            addStateAndTransition(leftHandStateMachine, "Victory", Parameters.FEC_SELECTED_GESTURE_LEFT, 4, 4, new Vector3(400, 180, 0));
            addStateAndTransition(leftHandStateMachine, "RockNRoll", Parameters.FEC_SELECTED_GESTURE_LEFT, 5, 5, new Vector3(400, 240, 0));
            addStateAndTransition(leftHandStateMachine, "HandGun", Parameters.FEC_SELECTED_GESTURE_LEFT, 6, 6, new Vector3(400, 300, 0));
            addStateAndTransition(leftHandStateMachine, "ThumbsUp", Parameters.FEC_SELECTED_GESTURE_LEFT, 7, 7, new Vector3(400, 360, 0));

            var rightHandStateMachine = layer.stateMachine.AddStateMachine("Right Hand", new Vector3(500, 120, 0));

            var initialToRightHandTransition1 = initialState.AddTransition(rightHandStateMachine);
            SetImmediateTransitionSetting(initialToRightHandTransition1);
            initialToRightHandTransition1.AddCondition(AnimatorConditionMode.If, 0, Parameters.IS_LOCAL);
            initialToRightHandTransition1.AddCondition(AnimatorConditionMode.Equals, 0, Parameters.FEC_SELECTED_FACE_EMOTE_BY_MENU);
            initialToRightHandTransition1.AddCondition(AnimatorConditionMode.NotEqual, 0, Parameters.FEC_SELECTED_GESTURE_RIGHT);

            layer.stateMachine.AddStateMachineExitTransition(rightHandStateMachine);

            addStateAndTransition(rightHandStateMachine, "Fist", Parameters.FEC_SELECTED_GESTURE_RIGHT, 1, 8, new Vector3(400, 00, 0));
            addStateAndTransition(rightHandStateMachine, "HandOpen", Parameters.FEC_SELECTED_GESTURE_RIGHT, 2, 9, new Vector3(400, 60, 0));
            addStateAndTransition(rightHandStateMachine, "FingerPoint", Parameters.FEC_SELECTED_GESTURE_RIGHT, 3, 10, new Vector3(400, 120, 0));
            addStateAndTransition(rightHandStateMachine, "Victory", Parameters.FEC_SELECTED_GESTURE_RIGHT, 4, 11, new Vector3(400, 180, 0));
            addStateAndTransition(rightHandStateMachine, "RockNRoll", Parameters.FEC_SELECTED_GESTURE_RIGHT, 5, 12, new Vector3(400, 240, 0));
            addStateAndTransition(rightHandStateMachine, "HandGun", Parameters.FEC_SELECTED_GESTURE_RIGHT, 6, 13, new Vector3(400, 300, 0));
            addStateAndTransition(rightHandStateMachine, "ThumbsUp", Parameters.FEC_SELECTED_GESTURE_RIGHT, 7, 14, new Vector3(400, 360, 0));

            var fixedFaceEmotesStateMachine = layer.stateMachine.AddStateMachine("Fixed Face Emotes", new Vector3(500, 180, 0));
            fixedFaceEmotesStateMachine.entryPosition = new Vector3(0, 0, 0);
            fixedFaceEmotesStateMachine.exitPosition = new Vector3(800, 0, 0);
            fixedFaceEmotesStateMachine.anyStatePosition = new Vector3(0, -40, 0);
            fixedFaceEmotesStateMachine.parentStateMachinePosition = new Vector3(0, -100, 0);

            var initialStateToFixedFaceEmotesStateTransition1 = initialState.AddTransition(fixedFaceEmotesStateMachine);
            SetImmediateTransitionSetting(initialStateToFixedFaceEmotesStateTransition1);
            initialStateToFixedFaceEmotesStateTransition1.AddCondition(AnimatorConditionMode.If, 0, Parameters.IS_LOCAL);
            initialStateToFixedFaceEmotesStateTransition1.AddCondition(AnimatorConditionMode.NotEqual, 0, Parameters.FEC_SELECTED_FACE_EMOTE_BY_MENU);

            layer.stateMachine.AddStateMachineExitTransition(fixedFaceEmotesStateMachine);

            Action<AnimatorStateMachine, int, Vector3> addFixedFaceEmoteState = (AnimatorStateMachine stateMachine, int faceEmoteNumber, Vector3 position) =>
            {
                var state = stateMachine.AddState($"Fixed Face Emote {faceEmoteNumber}", position);

                state.writeDefaultValues = false;
                state.motion = blankAnimationClip;
                state.behaviours = new StateMachineBehaviour[]{
                    GenerateVRCAvatarParameterLocalSetDriver(Parameters.FEC_SELECTED_FACE_EMOTE, faceEmoteNumber)
                };

                var entryTransition1 = stateMachine.AddEntryTransition(state);
                entryTransition1.AddCondition(AnimatorConditionMode.Equals, faceEmoteNumber, Parameters.FEC_SELECTED_FACE_EMOTE_BY_MENU);

                var exitTransition1 = state.AddExitTransition();
                SetImmediateTransitionSetting(exitTransition1);
                exitTransition1.AddCondition(AnimatorConditionMode.NotEqual, faceEmoteNumber, Parameters.FEC_SELECTED_FACE_EMOTE_BY_MENU);
            };

            // ハンドジェスチャー割当済みの表情用ステートを追加する
            for (var i = 0; i < Constants.ADDITIONAL_FACE_EMOTE_MIN_NUMBER - 1; i++)
            {
                addFixedFaceEmoteState(fixedFaceEmotesStateMachine, i + 1, new Vector3(400, 60 * i, 0));
            }

            // 追加の表情用ステートを追加する
            for (var i = 0; i < faceEmoteControl.additionalFaceAnimationClips.Count; i++)
            {
                addFixedFaceEmoteState(fixedFaceEmotesStateMachine, i + Constants.ADDITIONAL_FACE_EMOTE_MIN_NUMBER, new Vector3(400, 60 * (i + Constants.ADDITIONAL_FACE_EMOTE_MIN_NUMBER), 0));
            }

            return layer;
        }

        private AnimatorControllerLayer GenerateFaceEmoteLockIndicatorControlLayer()
        {
            var hideLockIndicatorAnimationCurve = new AnimationCurve();
            hideLockIndicatorAnimationCurve.AddKey(0, 0);
            var hideLockIndicatorAnimationClip = new AnimationClip
            {
                name = "LockIndicator_OFF",
                frameRate = 60
            };
            hideLockIndicatorAnimationClip.SetCurve("FaceEmoteControl/FaceEmoteLockIndicator", typeof(GameObject), "m_IsActive", hideLockIndicatorAnimationCurve);

            var showLockIndicatorAnimationCurve = new AnimationCurve();
            showLockIndicatorAnimationCurve.AddKey(0, 1);
            var showLockIndicatorAnimationClip = new AnimationClip
            {
                name = "LockIndicator_ON",
                frameRate = 60
            };
            showLockIndicatorAnimationClip.SetCurve("FaceEmoteControl/FaceEmoteLockIndicator", typeof(GameObject), "m_IsActive", showLockIndicatorAnimationCurve);

            var flashLockIndicatorAnimationCurve = new AnimationCurve();
            flashLockIndicatorAnimationCurve.AddKey(new Keyframe(0, 0, float.PositiveInfinity, float.PositiveInfinity));
            flashLockIndicatorAnimationCurve.AddKey(new Keyframe(0, 0, float.PositiveInfinity, float.PositiveInfinity));
            flashLockIndicatorAnimationCurve.AddKey(new Keyframe(0.16666667f, 1, float.PositiveInfinity, float.PositiveInfinity));
            flashLockIndicatorAnimationCurve.AddKey(new Keyframe(0.33333334f, 0, float.PositiveInfinity, float.PositiveInfinity));
            flashLockIndicatorAnimationCurve.AddKey(new Keyframe(0.5f, 1, float.PositiveInfinity, float.PositiveInfinity));
            flashLockIndicatorAnimationCurve.AddKey(new Keyframe(0.6666667f, 0, float.PositiveInfinity, float.PositiveInfinity));
            flashLockIndicatorAnimationCurve.AddKey(new Keyframe(0.8333333f, 1, float.PositiveInfinity, float.PositiveInfinity));
            flashLockIndicatorAnimationCurve.AddKey(new Keyframe(1, 0, float.PositiveInfinity, float.PositiveInfinity));
            var flashLockIndicatorAnimationClip = new AnimationClip
            {
                name = "LockIndicator_FLASH",
                frameRate = 60
            };
            flashLockIndicatorAnimationClip.SetCurve("FaceEmoteControl/FaceEmoteLockIndicator", typeof(GameObject), "m_IsActive", flashLockIndicatorAnimationCurve);

            var layer = new AnimatorControllerLayer
            {
                name = "FEC_FACE_EMOTE_LOCK_INDICATOR_CONTROL",
                defaultWeight = 1,
                stateMachine = new AnimatorStateMachine(),
            };

            layer.stateMachine.entryPosition = new Vector3(0, 0, 0);
            layer.stateMachine.exitPosition = new Vector3(0, -40, 0);
            layer.stateMachine.anyStatePosition = new Vector3(0, -80, 0);

            var initialState = layer.stateMachine.AddState("Initial State", new Vector3(-20, 60, 0));
            initialState.writeDefaultValues = false;
            initialState.motion = hideLockIndicatorAnimationClip;

            var unlockState = layer.stateMachine.AddState("Unlock", new Vector3(220, 60, 0));
            unlockState.writeDefaultValues = false;
            unlockState.motion = hideLockIndicatorAnimationClip;

            var lockState = layer.stateMachine.AddState("Lock", new Vector3(-20, 140, 0));
            lockState.writeDefaultValues = false;
            lockState.motion = showLockIndicatorAnimationClip;

            var unlockToLockFlashState = layer.stateMachine.AddState("Flash (Unlock to Lock)", new Vector3(220, 140, 0));
            unlockToLockFlashState.writeDefaultValues = false;
            unlockToLockFlashState.motion = flashLockIndicatorAnimationClip;

            var initialStateToUnlockStateTransition1 = initialState.AddTransition(unlockState);
            SetImmediateTransitionSetting(initialStateToUnlockStateTransition1);
            initialStateToUnlockStateTransition1.AddCondition(AnimatorConditionMode.If, 0, Parameters.IS_LOCAL);
            initialStateToUnlockStateTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.FEC_FACE_EMOTE_LOCKED);

            var initialStateToLockStateTransition1 = initialState.AddTransition(lockState);
            SetImmediateTransitionSetting(initialStateToLockStateTransition1);
            initialStateToLockStateTransition1.AddCondition(AnimatorConditionMode.If, 0, Parameters.IS_LOCAL);
            initialStateToLockStateTransition1.AddCondition(AnimatorConditionMode.If, 0, Parameters.FEC_FACE_EMOTE_LOCKED);

            var lockStateToUnlockStateTransition1 = lockState.AddTransition(unlockState);
            SetImmediateTransitionSetting(lockStateToUnlockStateTransition1);
            lockStateToUnlockStateTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.FEC_FACE_EMOTE_LOCKED);

            var unlockStateToUnlockToLockFlashStateTransition1 = unlockState.AddTransition(unlockToLockFlashState);
            SetImmediateTransitionSetting(unlockStateToUnlockToLockFlashStateTransition1);
            unlockStateToUnlockToLockFlashStateTransition1.AddCondition(AnimatorConditionMode.If, 0, Parameters.FEC_FACE_EMOTE_LOCKED);

            var unlockToLockFlashStateToLockStateTransition1 = unlockToLockFlashState.AddTransition(lockState);
            unlockToLockFlashStateToLockStateTransition1.hasExitTime = true;
            unlockToLockFlashStateToLockStateTransition1.exitTime = 1.0f;
            unlockToLockFlashStateToLockStateTransition1.hasFixedDuration = true;
            unlockToLockFlashStateToLockStateTransition1.duration = 0;
            unlockToLockFlashStateToLockStateTransition1.offset = 0;
            unlockToLockFlashStateToLockStateTransition1.interruptionSource = TransitionInterruptionSource.None;
            unlockToLockFlashStateToLockStateTransition1.orderedInterruption = true;

            return layer;
        }

        private AnimatorControllerLayer GenerateDefaultFaceEmoteLayer(FaceEmoteControl faceEmoteControl)
        {
            var layer = new AnimatorControllerLayer
            {
                name = "FEC_DEFAULT_FACE_EMOTE",
                defaultWeight = 1,
                stateMachine = new AnimatorStateMachine(),
            };

            layer.stateMachine.entryPosition = new Vector3(0, 0, 0);
            layer.stateMachine.exitPosition = new Vector3(0, -40, 0);
            layer.stateMachine.anyStatePosition = new Vector3(0, -80, 0);

            var setDefaultFaceEmoteState = layer.stateMachine.AddState("Set Default Face Emote", new Vector3(-20, 60, 0));
            setDefaultFaceEmoteState.writeDefaultValues = false;
            if (faceEmoteControl.defaultFaceAnimationClip != null)
            {
                setDefaultFaceEmoteState.motion = faceEmoteControl.defaultFaceAnimationClip;
            }
            else
            {
                setDefaultFaceEmoteState.motion = blankAnimationClip;
            }

            return layer;
        }

        private AnimatorControllerLayer GenerateFaceEmoteSettingsLayer(FaceEmoteControl faceEmoteControl)
        {
            var layer = new AnimatorControllerLayer
            {
                name = "FEC_FACE_EMOTE_SETTINGS",
                defaultWeight = 1,
                stateMachine = new AnimatorStateMachine(),
            };

            layer.stateMachine.entryPosition = new Vector3(0, 0, 0);
            layer.stateMachine.exitPosition = new Vector3(1000, 0, 0);
            layer.stateMachine.anyStatePosition = new Vector3(0, -80, 0);

            AddNeutralFaceEmoteState(layer.stateMachine, new Vector3(500, 0, 0));
            AddAFKState(layer.stateMachine, new Vector3(500, -60, 0));
            AddLeftGestureEmoteStates(layer.stateMachine, new Vector3(500, 60, 0), faceEmoteControl);
            AddRightGestureEmoteStates(layer.stateMachine, new Vector3(500, 120, 0), faceEmoteControl);
            AddAdditionalEmoteStates(layer.stateMachine, new Vector3(500, 180, 0), faceEmoteControl);

            return layer;
        }

        private void AddAFKState(AnimatorStateMachine stateMachine, Vector3 position)
        {
            var state = stateMachine.AddState("AFK", position);
            state.writeDefaultValues = false;
            state.motion = blankAnimationClip;

            var fromEntryTransition1 = stateMachine.AddEntryTransition(state);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.If, 0, Parameters.AFK);

            var toExitTransition1 = state.AddExitTransition();
            SetImmediateTransitionSetting(toExitTransition1);
            toExitTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.AFK);
        }

        private void AddNeutralFaceEmoteState(AnimatorStateMachine stateMachine, Vector3 position)
        {
            var state = stateMachine.AddState("Face Emote (Neutral)", position);
            state.writeDefaultValues = false;
            state.motion = blankAnimationClip;

            var fromEntryTransition1 = stateMachine.AddEntryTransition(state);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.AFK);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.Equals, 0, Parameters.FEC_SELECTED_FACE_EMOTE);

            var toExitTransition1 = state.AddExitTransition();
            toExitTransition1.hasExitTime = false;
            toExitTransition1.exitTime = 0;
            toExitTransition1.hasFixedDuration = true;
            toExitTransition1.duration = 0.1f;
            toExitTransition1.offset = 0;
            toExitTransition1.interruptionSource = TransitionInterruptionSource.None;
            toExitTransition1.orderedInterruption = true;
            toExitTransition1.AddCondition(AnimatorConditionMode.NotEqual, 0, Parameters.FEC_SELECTED_FACE_EMOTE);

            var toExitTransition2 = state.AddExitTransition();
            SetImmediateTransitionSetting(toExitTransition2);
            toExitTransition2.AddCondition(AnimatorConditionMode.If, 0, Parameters.AFK);
        }

        private void AddLeftGestureEmoteStates(AnimatorStateMachine stateMachine, Vector3 position, FaceEmoteControl faceEmoteControl)
        {
            var leftGestureEmoteStateMachine = stateMachine.AddStateMachine("Left Gesture Face Emotes", position);

            leftGestureEmoteStateMachine.entryPosition = new Vector3(0, 0, 0);
            leftGestureEmoteStateMachine.exitPosition = new Vector3(1000, 0, 0);
            leftGestureEmoteStateMachine.anyStatePosition = new Vector3(0, -40, 0);
            leftGestureEmoteStateMachine.parentStateMachinePosition = new Vector3(0, -100, 0);

            var fromEntryTransition1 = stateMachine.AddEntryTransition(leftGestureEmoteStateMachine);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.AFK);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.Greater, 0, Parameters.FEC_SELECTED_FACE_EMOTE);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.Less, 8, Parameters.FEC_SELECTED_FACE_EMOTE);

            stateMachine.AddStateMachineExitTransition(leftGestureEmoteStateMachine);

            AddFaceEmoteState(leftGestureEmoteStateMachine, "Fist (Left Gesture)", 1, faceEmoteControl.leftFistAnimationClip, Parameters.GESTURE_LEFT_WEIGHT, new Vector3(500, 0, 0));
            AddFaceEmoteState(leftGestureEmoteStateMachine, "HandOpen (Left Gesture)", 2, faceEmoteControl.leftHandOpenAnimationClip, new Vector3(500, 60, 0));
            AddFaceEmoteState(leftGestureEmoteStateMachine, "FingerPoint (Left Gesture)", 3, faceEmoteControl.leftFingerPointAnimationClip, new Vector3(500, 120, 0));
            AddFaceEmoteState(leftGestureEmoteStateMachine, "Victory (Left Gesture)", 4, faceEmoteControl.leftVictoryAnimationClip, new Vector3(500, 180, 0));
            AddFaceEmoteState(leftGestureEmoteStateMachine, "RockNRoll (Left Gesture)", 5, faceEmoteControl.leftRockNRollAnimationClip, new Vector3(500, 240, 0));
            AddFaceEmoteState(leftGestureEmoteStateMachine, "HandGun (Left Gesture)", 6, faceEmoteControl.leftHandGunAnimationClip, new Vector3(500, 300, 0));
            AddFaceEmoteState(leftGestureEmoteStateMachine, "ThumbsUp (Left Gesture)", 7, faceEmoteControl.leftThumbsUpAnimationClip, new Vector3(500, 360, 0));
        }

        private void AddRightGestureEmoteStates(AnimatorStateMachine stateMachine, Vector3 position, FaceEmoteControl faceEmoteControl)
        {
            var rightGestureEmoteStateMachine = stateMachine.AddStateMachine("Right Gesture Face Emotes", position);

            rightGestureEmoteStateMachine.entryPosition = new Vector3(0, 0, 0);
            rightGestureEmoteStateMachine.exitPosition = new Vector3(1000, 0, 0);
            rightGestureEmoteStateMachine.anyStatePosition = new Vector3(0, -40, 0);
            rightGestureEmoteStateMachine.parentStateMachinePosition = new Vector3(0, -100, 0);

            var fromEntryTransition1 = stateMachine.AddEntryTransition(rightGestureEmoteStateMachine);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.AFK);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.Greater, 7, Parameters.FEC_SELECTED_FACE_EMOTE);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.Less, 15, Parameters.FEC_SELECTED_FACE_EMOTE);

            stateMachine.AddStateMachineExitTransition(rightGestureEmoteStateMachine);

            AddFaceEmoteState(rightGestureEmoteStateMachine, "Fist (Right Gesture)", 8, faceEmoteControl.rightFistAnimationClip, Parameters.GESTURE_RIGHT_WEIGHT, new Vector3(500, 0, 0));
            AddFaceEmoteState(rightGestureEmoteStateMachine, "HandOpen (Right Gesture)", 9, faceEmoteControl.rightHandOpenAnimationClip, new Vector3(500, 60, 0));
            AddFaceEmoteState(rightGestureEmoteStateMachine, "FingerPoint (Right Gesture)", 10, faceEmoteControl.rightFingerPointAnimationClip, new Vector3(500, 120, 0));
            AddFaceEmoteState(rightGestureEmoteStateMachine, "Victory (Right Gesture)", 11, faceEmoteControl.rightVictoryAnimationClip, new Vector3(500, 180, 0));
            AddFaceEmoteState(rightGestureEmoteStateMachine, "RockNRoll (Right Gesture)", 12, faceEmoteControl.rightRockNRollAnimationClip, new Vector3(500, 240, 0));
            AddFaceEmoteState(rightGestureEmoteStateMachine, "HandGun (Right Gesture)", 13, faceEmoteControl.rightHandGunAnimationClip, new Vector3(500, 300, 0));
            AddFaceEmoteState(rightGestureEmoteStateMachine, "ThumbsUp (Right Gesture)", 14, faceEmoteControl.rightThumbsUpAnimationClip, new Vector3(500, 360, 0));
        }

        private void AddAdditionalEmoteStates(AnimatorStateMachine stateMachine, Vector3 startPosition, FaceEmoteControl faceEmoteControl)
        {
            var stateMachinePosition = startPosition;
            var statePosition = new Vector3(500, 0, 0);
            AnimatorStateMachine currentStateMachine = null;

            for (var i = 0; i < faceEmoteControl.additionalFaceAnimationClips.Count; i++)
            {
                if (i % 10 == 0)
                {
                    var start = i + 1;
                    var end = Math.Min(start + 9, faceEmoteControl.additionalFaceAnimationClips.Count);
                    currentStateMachine = stateMachine.AddStateMachine($"Additional Face Emotes ({start} ~ {end})", stateMachinePosition);

                    var fromEntryTransition1 = stateMachine.AddEntryTransition(currentStateMachine);
                    fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.AFK);
                    fromEntryTransition1.AddCondition(AnimatorConditionMode.Greater, i + Constants.ADDITIONAL_FACE_EMOTE_MIN_NUMBER - 1, Parameters.FEC_SELECTED_FACE_EMOTE);
                    fromEntryTransition1.AddCondition(AnimatorConditionMode.Less, i + Constants.ADDITIONAL_FACE_EMOTE_MIN_NUMBER + 10, Parameters.FEC_SELECTED_FACE_EMOTE);

                    stateMachine.AddStateMachineExitTransition(currentStateMachine);

                    stateMachinePosition = new Vector3(stateMachinePosition.x, stateMachinePosition.y + 60, stateMachinePosition.z);
                    statePosition = new Vector3(500, 0, 0);
                }

                var faceEmoteNumber = i + Constants.ADDITIONAL_FACE_EMOTE_MIN_NUMBER;
                AddFaceEmoteState(currentStateMachine, $"Additional Face Emote {i + 1} ({faceEmoteNumber})", faceEmoteNumber, faceEmoteControl.additionalFaceAnimationClips[i], statePosition);
                statePosition = new Vector3(statePosition.x, statePosition.y + 60, statePosition.z);
            }
        }

        private void AddFaceEmoteState(AnimatorStateMachine stateMachine, string name, int faceEmoteNumber, Motion motion, Vector3 position)
        {
            AddFaceEmoteState(stateMachine, name, faceEmoteNumber, motion, "", position);
        }

        private void AddFaceEmoteState(AnimatorStateMachine stateMachine, string name, int faceEmoteNumber, Motion motion, string motionTimeParameter, Vector3 position)
        {
            var state = stateMachine.AddState(name, position);
            state.writeDefaultValues = false;
            state.motion = motion != null ? motion : blankAnimationClip;
            if (motionTimeParameter != "")
            {
                state.timeParameterActive = true;
                state.timeParameter = motionTimeParameter;
            }

            var fromEntryTransition1 = stateMachine.AddEntryTransition(state);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, Parameters.AFK);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.Equals, faceEmoteNumber, Parameters.FEC_SELECTED_FACE_EMOTE);

            var toExitTransition1 = state.AddExitTransition();
            toExitTransition1.hasExitTime = false;
            toExitTransition1.exitTime = 0;
            toExitTransition1.hasFixedDuration = true;
            toExitTransition1.duration = 0.1f;
            toExitTransition1.offset = 0;
            toExitTransition1.interruptionSource = TransitionInterruptionSource.None;
            toExitTransition1.orderedInterruption = true;
            toExitTransition1.AddCondition(AnimatorConditionMode.NotEqual, faceEmoteNumber, Parameters.FEC_SELECTED_FACE_EMOTE);

            var toExitTransition2 = state.AddExitTransition();
            SetImmediateTransitionSetting(toExitTransition2);
            toExitTransition2.AddCondition(AnimatorConditionMode.If, 0, Parameters.AFK);
        }

        private void SetImmediateTransitionSetting(AnimatorStateTransition ast)
        {
            ast.hasExitTime = false;
            ast.exitTime = 0;
            ast.hasFixedDuration = true;
            ast.duration = 0;
            ast.offset = 0;
            ast.interruptionSource = TransitionInterruptionSource.None;
            ast.orderedInterruption = true;
        }
    }
#endif
}