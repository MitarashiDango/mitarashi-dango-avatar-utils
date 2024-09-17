using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

namespace MitarashiDango.AvatarUtils
{
    public class FaceEmoteControlAnimatorControllerGenerator
    {
        private AnimationClip blankAnimationClip = new AnimationClip
        {
            name = "blank"
        };

        public AnimatorController GenerateAnimatorController(GameObject avatarRoot, FaceEmoteControl faceEmoteControl, GameObject faceEmoteLockIndicator)
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
            animatorController.AddLayer(GenerateHandGestureLayer("FEC_LEFT_HAND_GESTURE", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT, VRCParameters.GESTURE_LEFT));
            animatorController.AddLayer(GenerateHandGestureLayer("FEC_RIGHT_HAND_GESTURE", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT, VRCParameters.GESTURE_RIGHT));
            animatorController.AddLayer(GenerateSetFaceEmoteTypeLayer(faceEmoteControl));
            animatorController.AddLayer(GenerateFaceEmoteLockIndicatorControlLayer(avatarRoot, faceEmoteLockIndicator));
            animatorController.AddLayer(GenerateDefaultFaceEmoteLayer(faceEmoteControl));
            animatorController.AddLayer(GenerateFaceEmoteSettingsLayer(faceEmoteControl));

            return animatorController;
        }

        private AnimatorControllerParameter[] GenerateParameters()
        {
            return new AnimatorControllerParameter[]{
                new AnimatorControllerParameter{
                    name = VRCParameters.AFK,
                    type = AnimatorControllerParameterType.Bool,
                    defaultBool = false,
                },
                new AnimatorControllerParameter{
                    name = VRCParameters.IN_STATION,
                    type = AnimatorControllerParameterType.Bool,
                    defaultBool = false,
                },
                new AnimatorControllerParameter{
                    name = VRCParameters.IS_LOCAL,
                    type = AnimatorControllerParameterType.Bool,
                    defaultBool = false,
                },
                new AnimatorControllerParameter{
                    name = VRCParameters.GESTURE_LEFT,
                    type = AnimatorControllerParameterType.Int,
                    defaultInt = 0,
                },
                new AnimatorControllerParameter{
                    name = VRCParameters.GESTURE_RIGHT,
                    type = AnimatorControllerParameterType.Int,
                    defaultInt = 0,
                },
                new AnimatorControllerParameter{
                    name = VRCParameters.GESTURE_LEFT_WEIGHT,
                    type = AnimatorControllerParameterType.Float,
                    defaultFloat = 0,
                },
                new AnimatorControllerParameter{
                    name = VRCParameters.GESTURE_RIGHT_WEIGHT,
                    type = AnimatorControllerParameterType.Float,
                    defaultFloat = 0,
                },
                new AnimatorControllerParameter{
                    name = FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE,
                    type = AnimatorControllerParameterType.Int,
                    defaultInt = 0,
                },
                new AnimatorControllerParameter{
                    name = FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKED,
                    type = AnimatorControllerParameterType.Bool,
                    defaultBool = false,
                },
                new AnimatorControllerParameter{
                    name = FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_ENABLED,
                    type = AnimatorControllerParameterType.Bool,
                    defaultBool = true,
                },
                new AnimatorControllerParameter{
                    name = FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_AUTO_DISABLE_ON_SIT,
                    type = AnimatorControllerParameterType.Bool,
                    defaultBool = false,
                },
                new AnimatorControllerParameter{
                    name = FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_CONTACT,
                    type = AnimatorControllerParameterType.Bool,
                    defaultBool = false,
                },
                new AnimatorControllerParameter{
                    name = FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT,
                    type = AnimatorControllerParameterType.Int,
                    defaultInt = 0,
                },
                new AnimatorControllerParameter{
                    name = FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT,
                    type = AnimatorControllerParameterType.Int,
                    defaultInt = 0,
                },
                new AnimatorControllerParameter{
                    name = FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE,
                    type = AnimatorControllerParameterType.Int,
                    defaultInt = 0,
                },
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
                GenerateVRCAvatarParameterLocalSetDriver(FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKED, 0)
            };

            var setEnableState = layer.stateMachine.AddState("Set Enable", new Vector3(600, 160, 0));
            setEnableState.writeDefaultValues = false;
            setEnableState.motion = blankAnimationClip;
            setEnableState.behaviours = new StateMachineBehaviour[]{
                GenerateVRCAvatarParameterLocalSetDriver(FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKED, 1)
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
            initialToGestureLockDisabledTransition1.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IS_LOCAL);
            initialToGestureLockDisabledTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKED);

            // [Initial State] -> [Gesture Lock Enabled]
            var initialToGestureLockEnabledTransition1 = initialState.AddTransition(gestureLockEnabledState);
            SetImmediateTransitionSetting(initialToGestureLockEnabledTransition1);
            initialToGestureLockEnabledTransition1.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IS_LOCAL);
            initialToGestureLockEnabledTransition1.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKED);

            // [Gesture Lock Disabled] -> [Gesture Lock Enabled]
            var gestureLockDisabledToGestureLockEnabledTransition1 = gestureLockDisabledState.AddTransition(gestureLockEnabledState);
            SetImmediateTransitionSetting(gestureLockDisabledToGestureLockEnabledTransition1);
            gestureLockDisabledToGestureLockEnabledTransition1.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKED);

            // [Gesture Lock Enabled] -> [Gesture Lock Disabled]
            var gestureLockEnabledToGestureLockDisabledTransition1 = gestureLockEnabledState.AddTransition(gestureLockDisabledState);
            SetImmediateTransitionSetting(gestureLockEnabledToGestureLockDisabledTransition1);
            gestureLockEnabledToGestureLockDisabledTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKED);

            // [Gesture Lock Disabled] -> [Set Enable] (Transition 1)
            var gestureLockDisabledToSetEnableTransition1 = gestureLockDisabledState.AddTransition(setEnableState);
            SetImmediateTransitionSetting(gestureLockDisabledToSetEnableTransition1);
            gestureLockDisabledToSetEnableTransition1.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_CONTACT);
            gestureLockDisabledToSetEnableTransition1.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_ENABLED);
            gestureLockDisabledToSetEnableTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_AUTO_DISABLE_ON_SIT);
            gestureLockDisabledToSetEnableTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);

            // [Gesture Lock Disabled] -> [Set Enable] (Transition 2)
            var gestureLockDisabledToSetEnableTransition2 = gestureLockDisabledState.AddTransition(setEnableState);
            SetImmediateTransitionSetting(gestureLockDisabledToSetEnableTransition2);
            gestureLockDisabledToSetEnableTransition2.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_CONTACT);
            gestureLockDisabledToSetEnableTransition2.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_ENABLED);
            gestureLockDisabledToSetEnableTransition2.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_AUTO_DISABLE_ON_SIT);
            gestureLockDisabledToSetEnableTransition2.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
            gestureLockDisabledToSetEnableTransition2.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.IN_STATION);

            // [Gesture Lock Enabled] -> [Set Disable] (Transition 1)
            var gestureLockEnabledToSetDisableTransition1 = gestureLockEnabledState.AddTransition(setDisableState);
            SetImmediateTransitionSetting(gestureLockEnabledToSetDisableTransition1);
            gestureLockEnabledToSetDisableTransition1.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_CONTACT);
            gestureLockEnabledToSetDisableTransition1.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_ENABLED);
            gestureLockEnabledToSetDisableTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_AUTO_DISABLE_ON_SIT);
            gestureLockEnabledToSetDisableTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);

            // [Gesture Lock Enabled] -> [Set Disable] (Transition 2)
            var gestureLockEnabledToSetDisableTransition2 = gestureLockEnabledState.AddTransition(setDisableState);
            SetImmediateTransitionSetting(gestureLockEnabledToSetDisableTransition2);
            gestureLockEnabledToSetDisableTransition2.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_CONTACT);
            gestureLockEnabledToSetDisableTransition2.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_ENABLED);
            gestureLockEnabledToSetDisableTransition2.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_AUTO_DISABLE_ON_SIT);
            gestureLockEnabledToSetDisableTransition2.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
            gestureLockEnabledToSetDisableTransition2.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.IN_STATION);

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
            lockToUnlockSleepToGestureLockDisabledTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_CONTACT);

            // [Sleep (Unlock to Lock)] -> [Gesture Lock Enabled]
            var unlockToLockSleepToGestureLockEnabledTransition1 = unlockToLockSleepState.AddTransition(gestureLockEnabledState);
            SetImmediateTransitionSetting(unlockToLockSleepToGestureLockEnabledTransition1);
            unlockToLockSleepToGestureLockEnabledTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_CONTACT);

            return layer;
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
                transition1.AddCondition(AnimatorConditionMode.IfNot, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKED);
                transition1.AddCondition(AnimatorConditionMode.Equals, gestureNumber, gestureParameterName);
                transition1.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IS_LOCAL);
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
                transition1.AddCondition(AnimatorConditionMode.IfNot, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKED);
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
                GenerateVRCAvatarParameterLocalSetDriver(FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE, 0)
            };

            var initialToIdleTransition1 = initialState.AddTransition(idleState);
            SetImmediateTransitionSetting(initialToIdleTransition1);
            initialToIdleTransition1.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IS_LOCAL);
            initialToIdleTransition1.AddCondition(AnimatorConditionMode.Equals, 0, FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE);
            initialToIdleTransition1.AddCondition(AnimatorConditionMode.Equals, 0, FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT);
            initialToIdleTransition1.AddCondition(AnimatorConditionMode.Equals, 0, FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT);

            var idleToExitTransition1 = idleState.AddExitTransition();
            SetImmediateTransitionSetting(idleToExitTransition1);
            idleToExitTransition1.AddCondition(AnimatorConditionMode.NotEqual, 0, FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE);

            var idleToExitTransition2 = idleState.AddExitTransition();
            SetImmediateTransitionSetting(idleToExitTransition2);
            idleToExitTransition2.AddCondition(AnimatorConditionMode.NotEqual, 0, FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT);

            var idleToExitTransition3 = idleState.AddExitTransition();
            SetImmediateTransitionSetting(idleToExitTransition3);
            idleToExitTransition3.AddCondition(AnimatorConditionMode.NotEqual, 0, FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT);

            Action<AnimatorStateMachine, string, string, int, int, Vector3> addStateAndTransition = (AnimatorStateMachine stateMachine, string stateName, string selectedGestureParameterName, int gestureNumber, int faceEmoteNumber, Vector3 position) =>
            {
                var state = stateMachine.AddState(stateName, position);
                state.writeDefaultValues = false;
                state.motion = blankAnimationClip;
                state.behaviours = new StateMachineBehaviour[]{
                    GenerateVRCAvatarParameterLocalSetDriver(FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE, faceEmoteNumber)
                };

                var fromEntryTransition1 = stateMachine.AddEntryTransition(state);
                fromEntryTransition1.AddCondition(AnimatorConditionMode.Equals, gestureNumber, selectedGestureParameterName);

                var toExitTransition1 = state.AddExitTransition();
                SetImmediateTransitionSetting(toExitTransition1);
                toExitTransition1.AddCondition(AnimatorConditionMode.NotEqual, 0, FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE);

                var toExitTransition2 = state.AddExitTransition();
                SetImmediateTransitionSetting(toExitTransition2);
                toExitTransition2.AddCondition(AnimatorConditionMode.NotEqual, gestureNumber, selectedGestureParameterName);
            };

            var leftHandStateMachine = layer.stateMachine.AddStateMachine("Left Hand", new Vector3(500, 60, 0));

            var initialToLeftHandTransition1 = initialState.AddTransition(leftHandStateMachine);
            SetImmediateTransitionSetting(initialToLeftHandTransition1);
            initialToLeftHandTransition1.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IS_LOCAL);
            initialToLeftHandTransition1.AddCondition(AnimatorConditionMode.Equals, 0, FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE);
            initialToLeftHandTransition1.AddCondition(AnimatorConditionMode.NotEqual, 0, FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT);

            layer.stateMachine.AddStateMachineExitTransition(leftHandStateMachine);

            addStateAndTransition(leftHandStateMachine, "Fist", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT, 1, 1, new Vector3(400, 00, 0));
            addStateAndTransition(leftHandStateMachine, "HandOpen", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT, 2, 2, new Vector3(400, 60, 0));
            addStateAndTransition(leftHandStateMachine, "FingerPoint", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT, 3, 3, new Vector3(400, 120, 0));
            addStateAndTransition(leftHandStateMachine, "Victory", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT, 4, 4, new Vector3(400, 180, 0));
            addStateAndTransition(leftHandStateMachine, "RockNRoll", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT, 5, 5, new Vector3(400, 240, 0));
            addStateAndTransition(leftHandStateMachine, "HandGun", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT, 6, 6, new Vector3(400, 300, 0));
            addStateAndTransition(leftHandStateMachine, "ThumbsUp", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT, 7, 7, new Vector3(400, 360, 0));

            var rightHandStateMachine = layer.stateMachine.AddStateMachine("Right Hand", new Vector3(500, 120, 0));

            var initialToRightHandTransition1 = initialState.AddTransition(rightHandStateMachine);
            SetImmediateTransitionSetting(initialToRightHandTransition1);
            initialToRightHandTransition1.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IS_LOCAL);
            initialToRightHandTransition1.AddCondition(AnimatorConditionMode.Equals, 0, FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE);
            initialToRightHandTransition1.AddCondition(AnimatorConditionMode.NotEqual, 0, FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT);

            layer.stateMachine.AddStateMachineExitTransition(rightHandStateMachine);

            addStateAndTransition(rightHandStateMachine, "Fist", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT, 1, 8, new Vector3(400, 00, 0));
            addStateAndTransition(rightHandStateMachine, "HandOpen", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT, 2, 9, new Vector3(400, 60, 0));
            addStateAndTransition(rightHandStateMachine, "FingerPoint", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT, 3, 10, new Vector3(400, 120, 0));
            addStateAndTransition(rightHandStateMachine, "Victory", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT, 4, 11, new Vector3(400, 180, 0));
            addStateAndTransition(rightHandStateMachine, "RockNRoll", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT, 5, 12, new Vector3(400, 240, 0));
            addStateAndTransition(rightHandStateMachine, "HandGun", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT, 6, 13, new Vector3(400, 300, 0));
            addStateAndTransition(rightHandStateMachine, "ThumbsUp", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT, 7, 14, new Vector3(400, 360, 0));

            var fixedFaceEmotesStateMachine = layer.stateMachine.AddStateMachine("Fixed Face Emotes", new Vector3(500, 180, 0));
            fixedFaceEmotesStateMachine.entryPosition = new Vector3(0, 0, 0);
            fixedFaceEmotesStateMachine.exitPosition = new Vector3(800, 0, 0);
            fixedFaceEmotesStateMachine.anyStatePosition = new Vector3(0, -40, 0);
            fixedFaceEmotesStateMachine.parentStateMachinePosition = new Vector3(0, -100, 0);

            var initialStateToFixedFaceEmotesStateTransition1 = initialState.AddTransition(fixedFaceEmotesStateMachine);
            SetImmediateTransitionSetting(initialStateToFixedFaceEmotesStateTransition1);
            initialStateToFixedFaceEmotesStateTransition1.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IS_LOCAL);
            initialStateToFixedFaceEmotesStateTransition1.AddCondition(AnimatorConditionMode.NotEqual, 0, FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE);

            layer.stateMachine.AddStateMachineExitTransition(fixedFaceEmotesStateMachine);

            Action<AnimatorStateMachine, int, Vector3> addFixedFaceEmoteState = (AnimatorStateMachine stateMachine, int faceEmoteNumber, Vector3 position) =>
            {
                var state = stateMachine.AddState($"Fixed Face Emote {faceEmoteNumber}", position);

                state.writeDefaultValues = false;
                state.motion = blankAnimationClip;
                state.behaviours = new StateMachineBehaviour[]{
                    GenerateVRCAvatarParameterLocalSetDriver(FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE, faceEmoteNumber)
                };

                var entryTransition1 = stateMachine.AddEntryTransition(state);
                entryTransition1.AddCondition(AnimatorConditionMode.Equals, faceEmoteNumber, FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE);

                var exitTransition1 = state.AddExitTransition();
                SetImmediateTransitionSetting(exitTransition1);
                exitTransition1.AddCondition(AnimatorConditionMode.NotEqual, faceEmoteNumber, FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE);
            };

            // ハンドジェスチャー割当済みの表情用ステートを追加する
            for (var i = 0; i < Constants.ADDITIONAL_FACE_EMOTE_MIN_NUMBER - 1; i++)
            {
                addFixedFaceEmoteState(fixedFaceEmotesStateMachine, i + 1, new Vector3(400, 60 * i, 0));
            }

            // 追加の表情用ステートを追加する
            if (faceEmoteControl.faceEmoteGroups.Count > 0)
            {
                List<FaceEmote> faceEmotes = new List<FaceEmote>();
                foreach (var faceEmoteGroup in faceEmoteControl.faceEmoteGroups)
                {
                    if (faceEmoteGroup == null)
                    {
                        continue;
                    }

                    faceEmotes.AddRange(faceEmoteGroup.faceEmotes);
                }

                for (var i = 0; i < faceEmotes.Count; i++)
                {
                    addFixedFaceEmoteState(fixedFaceEmotesStateMachine, i + Constants.ADDITIONAL_FACE_EMOTE_MIN_NUMBER, new Vector3(400, 60 * (i + Constants.ADDITIONAL_FACE_EMOTE_MIN_NUMBER), 0));
                }
            }
            else
            {
                for (var i = 0; i < faceEmoteControl.additionalFaceEmotes.Count; i++)
                {
                    addFixedFaceEmoteState(fixedFaceEmotesStateMachine, i + Constants.ADDITIONAL_FACE_EMOTE_MIN_NUMBER, new Vector3(400, 60 * (i + Constants.ADDITIONAL_FACE_EMOTE_MIN_NUMBER), 0));
                }
            }

            return layer;
        }

        private AnimatorControllerLayer GenerateFaceEmoteLockIndicatorControlLayer(GameObject avatarRoot, GameObject faceEmoteLockIndicator)
        {
            var objectHierarchyPath = MiscUtil.GetPathInHierarchy(faceEmoteLockIndicator, avatarRoot);
            var hideLockIndicatorAnimationClip = GenerateHideLockIndicatorAnimationClip(objectHierarchyPath);
            var showLockIndicatorAnimationClip = GenerateShowLockIndicatorAnimationClip(objectHierarchyPath);
            var flashLockIndicatorAnimationClip = GenerateFlashLockIndicatorAnimationClip(objectHierarchyPath);

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
            initialStateToUnlockStateTransition1.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IS_LOCAL);
            initialStateToUnlockStateTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKED);

            var initialStateToLockStateTransition1 = initialState.AddTransition(lockState);
            SetImmediateTransitionSetting(initialStateToLockStateTransition1);
            initialStateToLockStateTransition1.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IS_LOCAL);
            initialStateToLockStateTransition1.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKED);

            var lockStateToUnlockStateTransition1 = lockState.AddTransition(unlockState);
            SetImmediateTransitionSetting(lockStateToUnlockStateTransition1);
            lockStateToUnlockStateTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKED);

            var unlockStateToUnlockToLockFlashStateTransition1 = unlockState.AddTransition(unlockToLockFlashState);
            SetImmediateTransitionSetting(unlockStateToUnlockToLockFlashStateTransition1);
            unlockStateToUnlockToLockFlashStateTransition1.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKED);

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
            if (faceEmoteControl.defaultFaceMotion != null)
            {
                setDefaultFaceEmoteState.motion = faceEmoteControl.defaultFaceMotion;
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

            if (faceEmoteControl.faceEmoteGroups.Count > 0)
            {
                AddFaceEmoteGroups(layer.stateMachine, faceEmoteControl, Constants.ADDITIONAL_FACE_EMOTE_MIN_NUMBER, new Vector3(500, 180, 0));
            }
            else
            {
                AddAdditionalEmoteStates(layer.stateMachine, new Vector3(500, 180, 0), faceEmoteControl);
            }

            return layer;
        }

        private void AddAFKState(AnimatorStateMachine stateMachine, Vector3 position)
        {
            var state = stateMachine.AddState("AFK", position);
            state.writeDefaultValues = false;
            state.motion = blankAnimationClip;

            var fromEntryTransition1 = stateMachine.AddEntryTransition(state);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.AFK);

            var toExitTransition1 = state.AddExitTransition();
            SetImmediateTransitionSetting(toExitTransition1);
            toExitTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
        }

        private void AddNeutralFaceEmoteState(AnimatorStateMachine stateMachine, Vector3 position)
        {
            var state = stateMachine.AddState("Face Emote (Neutral)", position);
            state.writeDefaultValues = false;
            state.motion = blankAnimationClip;
            state.behaviours = new StateMachineBehaviour[]{
                new VRCAnimatorTrackingControl(){
                    trackingHead = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingLeftHand = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingRightHand = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingHip = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingLeftFoot = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingRightFoot = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingLeftFingers = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingRightFingers = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingEyes = VRC_AnimatorTrackingControl.TrackingType.Tracking,
                    trackingMouth = VRC_AnimatorTrackingControl.TrackingType.Tracking,
                }
            };

            var fromEntryTransition1 = stateMachine.AddEntryTransition(state);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.Equals, 0, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);

            var toExitTransition1 = state.AddExitTransition();
            toExitTransition1.hasExitTime = false;
            toExitTransition1.exitTime = 0;
            toExitTransition1.hasFixedDuration = true;
            toExitTransition1.duration = 0.1f;
            toExitTransition1.offset = 0;
            toExitTransition1.interruptionSource = TransitionInterruptionSource.None;
            toExitTransition1.orderedInterruption = true;
            toExitTransition1.AddCondition(AnimatorConditionMode.NotEqual, 0, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);

            var toExitTransition2 = state.AddExitTransition();
            SetImmediateTransitionSetting(toExitTransition2);
            toExitTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.AFK);
        }

        private void AddLeftGestureEmoteStates(AnimatorStateMachine stateMachine, Vector3 position, FaceEmoteControl faceEmoteControl)
        {
            var leftGestureEmoteStateMachine = stateMachine.AddStateMachine("Left Gesture Face Emotes", position);

            leftGestureEmoteStateMachine.entryPosition = new Vector3(0, 0, 0);
            leftGestureEmoteStateMachine.exitPosition = new Vector3(1000, 0, 0);
            leftGestureEmoteStateMachine.anyStatePosition = new Vector3(0, -40, 0);
            leftGestureEmoteStateMachine.parentStateMachinePosition = new Vector3(0, -100, 0);

            var fromEntryTransition1 = stateMachine.AddEntryTransition(leftGestureEmoteStateMachine);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.Greater, 0, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.Less, 8, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);

            stateMachine.AddStateMachineExitTransition(leftGestureEmoteStateMachine);

            if (faceEmoteControl.leftFaceEmoteGestureGroup != null)
            {
                AddGestureFaceEmoteState(leftGestureEmoteStateMachine, "Fist (Left Gesture)", 1, faceEmoteControl.leftFaceEmoteGestureGroup.fist, VRCParameters.GESTURE_LEFT_WEIGHT, new Vector3(500, 0, 0));
                AddGestureFaceEmoteState(leftGestureEmoteStateMachine, "HandOpen (Left Gesture)", 2, faceEmoteControl.leftFaceEmoteGestureGroup.handOpen, new Vector3(500, 60, 0));
                AddGestureFaceEmoteState(leftGestureEmoteStateMachine, "FingerPoint (Left Gesture)", 3, faceEmoteControl.leftFaceEmoteGestureGroup.fingerPoint, new Vector3(500, 120, 0));
                AddGestureFaceEmoteState(leftGestureEmoteStateMachine, "Victory (Left Gesture)", 4, faceEmoteControl.leftFaceEmoteGestureGroup.victory, new Vector3(500, 180, 0));
                AddGestureFaceEmoteState(leftGestureEmoteStateMachine, "RockNRoll (Left Gesture)", 5, faceEmoteControl.leftFaceEmoteGestureGroup.rockNRoll, new Vector3(500, 240, 0));
                AddGestureFaceEmoteState(leftGestureEmoteStateMachine, "HandGun (Left Gesture)", 6, faceEmoteControl.leftFaceEmoteGestureGroup.handGun, new Vector3(500, 300, 0));
                AddGestureFaceEmoteState(leftGestureEmoteStateMachine, "ThumbsUp (Left Gesture)", 7, faceEmoteControl.leftFaceEmoteGestureGroup.thumbsUp, new Vector3(500, 360, 0));
            }
            else
            {
                AddGestureFaceEmoteState(leftGestureEmoteStateMachine, "Fist (Left Gesture)", 1, faceEmoteControl.leftFist, VRCParameters.GESTURE_LEFT_WEIGHT, new Vector3(500, 0, 0));
                AddGestureFaceEmoteState(leftGestureEmoteStateMachine, "HandOpen (Left Gesture)", 2, faceEmoteControl.leftHandOpen, new Vector3(500, 60, 0));
                AddGestureFaceEmoteState(leftGestureEmoteStateMachine, "FingerPoint (Left Gesture)", 3, faceEmoteControl.leftFingerPoint, new Vector3(500, 120, 0));
                AddGestureFaceEmoteState(leftGestureEmoteStateMachine, "Victory (Left Gesture)", 4, faceEmoteControl.leftVictory, new Vector3(500, 180, 0));
                AddGestureFaceEmoteState(leftGestureEmoteStateMachine, "RockNRoll (Left Gesture)", 5, faceEmoteControl.leftRockNRoll, new Vector3(500, 240, 0));
                AddGestureFaceEmoteState(leftGestureEmoteStateMachine, "HandGun (Left Gesture)", 6, faceEmoteControl.leftHandGun, new Vector3(500, 300, 0));
                AddGestureFaceEmoteState(leftGestureEmoteStateMachine, "ThumbsUp (Left Gesture)", 7, faceEmoteControl.leftThumbsUp, new Vector3(500, 360, 0));
            }
        }

        private void AddRightGestureEmoteStates(AnimatorStateMachine stateMachine, Vector3 position, FaceEmoteControl faceEmoteControl)
        {
            var rightGestureEmoteStateMachine = stateMachine.AddStateMachine("Right Gesture Face Emotes", position);

            rightGestureEmoteStateMachine.entryPosition = new Vector3(0, 0, 0);
            rightGestureEmoteStateMachine.exitPosition = new Vector3(1000, 0, 0);
            rightGestureEmoteStateMachine.anyStatePosition = new Vector3(0, -40, 0);
            rightGestureEmoteStateMachine.parentStateMachinePosition = new Vector3(0, -100, 0);

            var fromEntryTransition1 = stateMachine.AddEntryTransition(rightGestureEmoteStateMachine);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.Greater, 7, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.Less, 15, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);

            stateMachine.AddStateMachineExitTransition(rightGestureEmoteStateMachine);

            if (faceEmoteControl.rightFaceEmoteGestureGroup != null)
            {
                AddGestureFaceEmoteState(rightGestureEmoteStateMachine, "Fist (Right Gesture)", 8, faceEmoteControl.rightFaceEmoteGestureGroup.fist, VRCParameters.GESTURE_RIGHT_WEIGHT, new Vector3(500, 0, 0));
                AddGestureFaceEmoteState(rightGestureEmoteStateMachine, "HandOpen (Right Gesture)", 9, faceEmoteControl.rightFaceEmoteGestureGroup.handOpen, new Vector3(500, 60, 0));
                AddGestureFaceEmoteState(rightGestureEmoteStateMachine, "FingerPoint (Right Gesture)", 10, faceEmoteControl.rightFaceEmoteGestureGroup.fingerPoint, new Vector3(500, 120, 0));
                AddGestureFaceEmoteState(rightGestureEmoteStateMachine, "Victory (Right Gesture)", 11, faceEmoteControl.rightFaceEmoteGestureGroup.victory, new Vector3(500, 180, 0));
                AddGestureFaceEmoteState(rightGestureEmoteStateMachine, "RockNRoll (Right Gesture)", 12, faceEmoteControl.rightFaceEmoteGestureGroup.rockNRoll, new Vector3(500, 240, 0));
                AddGestureFaceEmoteState(rightGestureEmoteStateMachine, "HandGun (Right Gesture)", 13, faceEmoteControl.rightFaceEmoteGestureGroup.handGun, new Vector3(500, 300, 0));
                AddGestureFaceEmoteState(rightGestureEmoteStateMachine, "ThumbsUp (Right Gesture)", 14, faceEmoteControl.rightFaceEmoteGestureGroup.thumbsUp, new Vector3(500, 360, 0));
            }
            else
            {
                AddGestureFaceEmoteState(rightGestureEmoteStateMachine, "Fist (Right Gesture)", 8, faceEmoteControl.rightFist, VRCParameters.GESTURE_RIGHT_WEIGHT, new Vector3(500, 0, 0));
                AddGestureFaceEmoteState(rightGestureEmoteStateMachine, "HandOpen (Right Gesture)", 9, faceEmoteControl.rightHandOpen, new Vector3(500, 60, 0));
                AddGestureFaceEmoteState(rightGestureEmoteStateMachine, "FingerPoint (Right Gesture)", 10, faceEmoteControl.rightFingerPoint, new Vector3(500, 120, 0));
                AddGestureFaceEmoteState(rightGestureEmoteStateMachine, "Victory (Right Gesture)", 11, faceEmoteControl.rightVictory, new Vector3(500, 180, 0));
                AddGestureFaceEmoteState(rightGestureEmoteStateMachine, "RockNRoll (Right Gesture)", 12, faceEmoteControl.rightRockNRoll, new Vector3(500, 240, 0));
                AddGestureFaceEmoteState(rightGestureEmoteStateMachine, "HandGun (Right Gesture)", 13, faceEmoteControl.rightHandGun, new Vector3(500, 300, 0));
                AddGestureFaceEmoteState(rightGestureEmoteStateMachine, "ThumbsUp (Right Gesture)", 14, faceEmoteControl.rightThumbsUp, new Vector3(500, 360, 0));
            }
        }

        private void AddAdditionalEmoteStates(AnimatorStateMachine stateMachine, Vector3 startPosition, FaceEmoteControl faceEmoteControl)
        {
            var stateMachinePosition = startPosition;
            var statePosition = new Vector3(500, 0, 0);
            AnimatorStateMachine currentStateMachine = null;

            for (var i = 0; i < faceEmoteControl.additionalFaceEmotes.Count; i++)
            {
                if (i % 10 == 0)
                {
                    var start = i + 1;
                    var end = Math.Min(start + 9, faceEmoteControl.additionalFaceEmotes.Count);
                    currentStateMachine = stateMachine.AddStateMachine($"Additional Face Emotes ({start} ~ {end})", stateMachinePosition);

                    var fromEntryTransition1 = stateMachine.AddEntryTransition(currentStateMachine);
                    fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
                    fromEntryTransition1.AddCondition(AnimatorConditionMode.Greater, i + Constants.ADDITIONAL_FACE_EMOTE_MIN_NUMBER - 1, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);
                    fromEntryTransition1.AddCondition(AnimatorConditionMode.Less, i + Constants.ADDITIONAL_FACE_EMOTE_MIN_NUMBER + 10, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);

                    stateMachine.AddStateMachineExitTransition(currentStateMachine);

                    stateMachinePosition = new Vector3(stateMachinePosition.x, stateMachinePosition.y + 60, stateMachinePosition.z);
                    statePosition = new Vector3(500, 0, 0);
                }

                var faceEmote = faceEmoteControl.additionalFaceEmotes[i];
                var faceEmoteNumber = i + Constants.ADDITIONAL_FACE_EMOTE_MIN_NUMBER;
                AddFaceEmoteState(currentStateMachine, $"Additional Face Emote {i + 1} ({faceEmoteNumber})", faceEmoteNumber, faceEmoteControl.additionalFaceEmotes[i].motion, faceEmote.eyeControlType, faceEmote.mouthControlType, statePosition);
                statePosition = new Vector3(statePosition.x, statePosition.y + 60, statePosition.z);
            }
        }

        private void AddFaceEmoteGroups(AnimatorStateMachine stateMachine, FaceEmoteControl faceEmoteControl, int startFaceEmoteNumber, Vector3 startPosition)
        {
            var stateMachinePosition = startPosition;
            var statePosition = new Vector3(500, 0, 0);
            AnimatorStateMachine currentStateMachine = null;

            List<FaceEmote> faceEmotes = new List<FaceEmote>();
            foreach (var faceEmoteGroup in faceEmoteControl.faceEmoteGroups)
            {
                if (faceEmoteGroup == null)
                {
                    continue;
                }

                faceEmotes.AddRange(faceEmoteGroup.faceEmotes);
            }

            for (var i = 0; i < faceEmotes.Count; i++)
            {
                var faceEmoteNumber = i + startFaceEmoteNumber;

                if (i % 10 == 0)
                {
                    var start = i + 1;
                    var end = Math.Min(start + 9, faceEmotes.Count);
                    currentStateMachine = stateMachine.AddStateMachine($"Additional Face Emotes ({start} ~ {end})", stateMachinePosition);

                    var fromEntryTransition1 = stateMachine.AddEntryTransition(currentStateMachine);
                    fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
                    fromEntryTransition1.AddCondition(AnimatorConditionMode.Greater, faceEmoteNumber - 1, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);
                    fromEntryTransition1.AddCondition(AnimatorConditionMode.Less, faceEmoteNumber + 10, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);

                    stateMachine.AddStateMachineExitTransition(currentStateMachine);

                    stateMachinePosition = new Vector3(stateMachinePosition.x, stateMachinePosition.y + 60, stateMachinePosition.z);
                    statePosition = new Vector3(500, 0, 0);
                }

                var faceEmote = faceEmotes[i];
                AddFaceEmoteState(currentStateMachine, $"Additional Face Emote {i + 1} ({faceEmoteNumber})", faceEmoteNumber, faceEmote.motion, faceEmote.eyeControlType, faceEmote.mouthControlType, statePosition);
                statePosition = new Vector3(statePosition.x, statePosition.y + 60, statePosition.z);
            }
        }

        private void AddGestureFaceEmoteState(AnimatorStateMachine stateMachine, string name, int faceEmoteNumber, FaceEmote faceEmote, Vector3 position)
        {
            if (faceEmote == null || faceEmote.motion == null)
            {
                return;
            }

            AddFaceEmoteState(stateMachine, name, faceEmoteNumber, faceEmote.motion, "", faceEmote.eyeControlType, faceEmote.mouthControlType, position);
        }

        private void AddGestureFaceEmoteState(AnimatorStateMachine stateMachine, string name, int faceEmoteNumber, FaceEmote faceEmote, string motionTimeParameter, Vector3 position)
        {
            if (faceEmote == null || faceEmote.motion == null)
            {
                return;
            }

            AddFaceEmoteState(stateMachine, name, faceEmoteNumber, faceEmote.motion, motionTimeParameter, faceEmote.eyeControlType, faceEmote.mouthControlType, position);
        }

        private void AddFaceEmoteState(AnimatorStateMachine stateMachine, string name, int faceEmoteNumber, Motion motion, TrackingControlType eyeTrackingType, TrackingControlType mouthTrackingType, Vector3 position)
        {
            AddFaceEmoteState(stateMachine, name, faceEmoteNumber, motion, "", eyeTrackingType, mouthTrackingType, position);
        }

        private void AddFaceEmoteState(AnimatorStateMachine stateMachine, string name, int faceEmoteNumber, Motion motion, string motionTimeParameter, TrackingControlType eyeTrackingType, TrackingControlType mouthTrackingType, Vector3 position)
        {
            var state = stateMachine.AddState(name, position);
            state.writeDefaultValues = false;
            state.motion = motion != null ? motion : blankAnimationClip;
            if (motionTimeParameter != "")
            {
                state.timeParameterActive = true;
                state.timeParameter = motionTimeParameter;
            }

            state.behaviours = new StateMachineBehaviour[]{
                new VRCAnimatorTrackingControl(){
                    trackingHead = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingLeftHand = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingRightHand = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingHip = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingLeftFoot = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingRightFoot = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingLeftFingers = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingRightFingers = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingEyes = GetTrackingType(eyeTrackingType),
                    trackingMouth = GetTrackingType(mouthTrackingType),
                }
            };

            var fromEntryTransition1 = stateMachine.AddEntryTransition(state);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.Equals, faceEmoteNumber, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);

            var toExitTransition1 = state.AddExitTransition();
            toExitTransition1.hasExitTime = false;
            toExitTransition1.exitTime = 0;
            toExitTransition1.hasFixedDuration = true;
            toExitTransition1.duration = 0.1f;
            toExitTransition1.offset = 0;
            toExitTransition1.interruptionSource = TransitionInterruptionSource.None;
            toExitTransition1.orderedInterruption = true;
            toExitTransition1.AddCondition(AnimatorConditionMode.NotEqual, faceEmoteNumber, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);

            var toExitTransition2 = state.AddExitTransition();
            SetImmediateTransitionSetting(toExitTransition2);
            toExitTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.AFK);
        }

        private VRC_AnimatorTrackingControl.TrackingType GetTrackingType(TrackingControlType trackingControlType)
        {
            switch (trackingControlType)
            {
                case TrackingControlType.Animation:
                    return VRC_AnimatorTrackingControl.TrackingType.Animation;
                case TrackingControlType.Tracking:
                    return VRC_AnimatorTrackingControl.TrackingType.Tracking;
                default:
                    return VRC_AnimatorTrackingControl.TrackingType.NoChange;
            }
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

        private AnimationClip GenerateHideLockIndicatorAnimationClip(string objectHierarchyPath)
        {
            var animationCurve = new AnimationCurve();
            animationCurve.AddKey(0, 0);
            var animationClip = new AnimationClip
            {
                name = "LockIndicator_OFF",
                frameRate = 60
            };
            animationClip.SetCurve(objectHierarchyPath, typeof(GameObject), "m_IsActive", animationCurve);

            return animationClip;
        }

        private AnimationClip GenerateShowLockIndicatorAnimationClip(string objectHierarchyPath)
        {
            var animationCurve = new AnimationCurve();
            animationCurve.AddKey(0, 1);
            var animationClip = new AnimationClip
            {
                name = "LockIndicator_ON",
                frameRate = 60
            };
            animationClip.SetCurve(objectHierarchyPath, typeof(GameObject), "m_IsActive", animationCurve);

            return animationClip;
        }

        private AnimationClip GenerateFlashLockIndicatorAnimationClip(string objectHierarchyPath)
        {
            var animationCurve = new AnimationCurve();
            animationCurve.AddKey(new Keyframe(0, 0, float.PositiveInfinity, float.PositiveInfinity));
            animationCurve.AddKey(new Keyframe(0, 0, float.PositiveInfinity, float.PositiveInfinity));
            animationCurve.AddKey(new Keyframe(0.16666667f, 1, float.PositiveInfinity, float.PositiveInfinity));
            animationCurve.AddKey(new Keyframe(0.33333334f, 0, float.PositiveInfinity, float.PositiveInfinity));
            animationCurve.AddKey(new Keyframe(0.5f, 1, float.PositiveInfinity, float.PositiveInfinity));
            animationCurve.AddKey(new Keyframe(0.6666667f, 0, float.PositiveInfinity, float.PositiveInfinity));
            animationCurve.AddKey(new Keyframe(0.8333333f, 1, float.PositiveInfinity, float.PositiveInfinity));
            animationCurve.AddKey(new Keyframe(1, 0, float.PositiveInfinity, float.PositiveInfinity));
            var animationClip = new AnimationClip
            {
                name = "LockIndicator_FLASH",
                frameRate = 60
            };
            animationClip.SetCurve(objectHierarchyPath, typeof(GameObject), "m_IsActive", animationCurve);

            return animationClip;
        }
    }
}