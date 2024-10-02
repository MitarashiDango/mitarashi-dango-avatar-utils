using System;
using System.Collections.Generic;
using System.Linq;
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
                parameters = GenerateParameters(faceEmoteControl)
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

        private AnimatorControllerParameter[] GenerateParameters(FaceEmoteControl faceEmoteControl)
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
                    name = VRCParameters.SEATED,
                    type = AnimatorControllerParameterType.Bool,
                    defaultBool = false,
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
                    name = FaceEmoteControlParameters.FEC_SELECTED_LEFT_GESTURE_GROUP,
                    type = AnimatorControllerParameterType.Int,
                    defaultInt = faceEmoteControl.leftFaceEmoteGestureGroupNumber,
                },
                new AnimatorControllerParameter{
                    name = FaceEmoteControlParameters.FEC_SELECTED_RIGHT_GESTURE_GROUP,
                    type = AnimatorControllerParameterType.Int,
                    defaultInt = faceEmoteControl.rightFaceEmoteGestureGroupNumber,
                },
                new AnimatorControllerParameter{
                    name = FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE,
                    type = AnimatorControllerParameterType.Int,
                    defaultInt = 0,
                },
                new AnimatorControllerParameter{
                    name = FaceEmoteControlParameters.FEC_FIXED_GESTURE_WEIGHT,
                    type = AnimatorControllerParameterType.Float,
                    defaultInt = 1,
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

            // [Gesture Lock Disabled] -> [Set Enable]
            // - AFKでもSit状態でもない場合、ロック有効化を行う
            // - Sit判定時のロック機能自動無効化がONの場合、AFKでもSit状態でもない場合のみロック有効化を行う
            //   - InStation = falseな状態が保証されているため、Sit判定時のロック機能自動無効化の状態についてはチェックを行わない
            var gestureLockDisabledToSetEnableTransition1 = gestureLockDisabledState.AddTransition(setEnableState);
            SetImmediateTransitionSetting(gestureLockDisabledToSetEnableTransition1);
            gestureLockDisabledToSetEnableTransition1.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_CONTACT);
            gestureLockDisabledToSetEnableTransition1.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_ENABLED);
            gestureLockDisabledToSetEnableTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
            gestureLockDisabledToSetEnableTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.IN_STATION);

            // [Gesture Lock Disabled] -> [Set Enable]
            // - Sit判定時にロック機能自動無効化がOFFの場合、Sit判定かつSeatedな状態でもロック有効化を行う
            //   - MMDワールドではロック機能自動無効化がOFFの場合でもロック状態の切り替えを行わないようにする(InStation = true かつ Seated = falseの場合にMMDワールド判定)
            var gestureLockDisabledToSetEnableTransition2 = gestureLockDisabledState.AddTransition(setEnableState);
            SetImmediateTransitionSetting(gestureLockDisabledToSetEnableTransition2);
            gestureLockDisabledToSetEnableTransition2.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_CONTACT);
            gestureLockDisabledToSetEnableTransition2.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_ENABLED);
            gestureLockDisabledToSetEnableTransition2.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
            gestureLockDisabledToSetEnableTransition2.AddCondition(AnimatorConditionMode.IfNot, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_AUTO_DISABLE_ON_SIT);
            gestureLockDisabledToSetEnableTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IN_STATION);
            gestureLockDisabledToSetEnableTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.SEATED);

            // [Gesture Lock Enabled] -> [Set Disable]
            // - AFKでもSit状態でもない場合、ロック無効化を行う
            // - Sit判定時のロック機能自動無効化がONの場合、AFKでもSit状態でもない場合のみロック無効化を行う
            //   - InStation = falseな状態が保証されているため、Sit判定時のロック機能自動無効化の状態についてはチェックを行わない
            var gestureLockEnabledToSetDisableTransition1 = gestureLockEnabledState.AddTransition(setDisableState);
            SetImmediateTransitionSetting(gestureLockEnabledToSetDisableTransition1);
            gestureLockEnabledToSetDisableTransition1.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_CONTACT);
            gestureLockEnabledToSetDisableTransition1.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_ENABLED);
            gestureLockEnabledToSetDisableTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
            gestureLockEnabledToSetDisableTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.IN_STATION);

            // [Gesture Lock Enabled] -> [Set Disable]
            // - Sit判定時にロック機能自動無効化がOFFの場合、Sit判定かつSeatedな状態でもロック無効化を行う
            //   - MMDワールドではロック機能自動無効化がOFFの場合でもロック状態の切り替えを行わないようにする(InStation = true かつ Seated = falseの場合にMMDワールド判定)
            var gestureLockEnabledToSetDisableTransition2 = gestureLockEnabledState.AddTransition(setDisableState);
            SetImmediateTransitionSetting(gestureLockEnabledToSetDisableTransition2);
            gestureLockEnabledToSetDisableTransition2.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_CONTACT);
            gestureLockEnabledToSetDisableTransition2.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_ENABLED);
            gestureLockEnabledToSetDisableTransition2.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
            gestureLockEnabledToSetDisableTransition2.AddCondition(AnimatorConditionMode.IfNot, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_AUTO_DISABLE_ON_SIT);
            gestureLockEnabledToSetDisableTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IN_STATION);
            gestureLockEnabledToSetDisableTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.SEATED);

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

            // 左手の表情ジェスチャーグループが未割り当て、かつ右手のジェスチャーがNeutralの場合、Idleステートへ進入させる
            var initialToIdleTransition2 = initialState.AddTransition(idleState);
            SetImmediateTransitionSetting(initialToIdleTransition2);
            initialToIdleTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IS_LOCAL);
            initialToIdleTransition2.AddCondition(AnimatorConditionMode.Equals, 0, FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE);
            initialToIdleTransition2.AddCondition(AnimatorConditionMode.Equals, 0, FaceEmoteControlParameters.FEC_SELECTED_LEFT_GESTURE_GROUP);
            initialToIdleTransition2.AddCondition(AnimatorConditionMode.Equals, 0, FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT);

            // 右手の表情ジェスチャーグループが未割り当て、かつ左手のジェスチャーがNeutralの場合、Idleステートへ進入させる
            var initialToIdleTransition3 = initialState.AddTransition(idleState);
            SetImmediateTransitionSetting(initialToIdleTransition3);
            initialToIdleTransition3.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IS_LOCAL);
            initialToIdleTransition3.AddCondition(AnimatorConditionMode.Equals, 0, FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE);
            initialToIdleTransition3.AddCondition(AnimatorConditionMode.Equals, 0, FaceEmoteControlParameters.FEC_SELECTED_RIGHT_GESTURE_GROUP);
            initialToIdleTransition3.AddCondition(AnimatorConditionMode.Equals, 0, FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT);

            // 右手・左手の両方に表情ジェスチャーグループが未割り当ての場合、Idleステートへ進入させる
            var initialToIdleTransition4 = initialState.AddTransition(idleState);
            SetImmediateTransitionSetting(initialToIdleTransition4);
            initialToIdleTransition4.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IS_LOCAL);
            initialToIdleTransition4.AddCondition(AnimatorConditionMode.Equals, 0, FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE);
            initialToIdleTransition4.AddCondition(AnimatorConditionMode.Equals, 0, FaceEmoteControlParameters.FEC_SELECTED_LEFT_GESTURE_GROUP);
            initialToIdleTransition4.AddCondition(AnimatorConditionMode.Equals, 0, FaceEmoteControlParameters.FEC_SELECTED_RIGHT_GESTURE_GROUP);

            // 表情固定設定が有効になった場合、Idleステートから脱出させる
            var idleToExitTransition1 = idleState.AddExitTransition();
            SetImmediateTransitionSetting(idleToExitTransition1);
            idleToExitTransition1.AddCondition(AnimatorConditionMode.NotEqual, 0, FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE);

            // 左手のジェスチャーが有効になった場合、Idleステートから脱出させる
            var idleToExitTransition2 = idleState.AddExitTransition();
            SetImmediateTransitionSetting(idleToExitTransition2);
            idleToExitTransition2.AddCondition(AnimatorConditionMode.NotEqual, 0, FaceEmoteControlParameters.FEC_SELECTED_LEFT_GESTURE_GROUP);
            idleToExitTransition2.AddCondition(AnimatorConditionMode.NotEqual, 0, FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT);

            // 右手のジェスチャーが有効になった場合、Idleステートから脱出させる
            var idleToExitTransition3 = idleState.AddExitTransition();
            SetImmediateTransitionSetting(idleToExitTransition3);
            idleToExitTransition3.AddCondition(AnimatorConditionMode.NotEqual, 0, FaceEmoteControlParameters.FEC_SELECTED_RIGHT_GESTURE_GROUP);
            idleToExitTransition3.AddCondition(AnimatorConditionMode.NotEqual, 0, FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT);

            Action<AnimatorStateMachine, string, string, string, int, int, int, Vector3> addStateAndTransition = (AnimatorStateMachine stateMachine, string stateName, string selectedGestureParameterName, string selectedGestureGroupParameterName, int gestureGroupNumber, int gestureNumber, int faceEmoteNumber, Vector3 position) =>
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

                var toExitTransition3 = state.AddExitTransition();
                SetImmediateTransitionSetting(toExitTransition3);
                toExitTransition3.AddCondition(AnimatorConditionMode.NotEqual, gestureGroupNumber, selectedGestureGroupParameterName);
            };

            // 左手ジェスチャー用表情選択フローの生成
            foreach ((var faceEmoteGestureGroup, var index) in faceEmoteControl.faceEmoteGestureGroups.Select((v, i) => (v, i)))
            {
                var faceEmoteNumberOffset = index * 7;
                var leftHandStateMachine = layer.stateMachine.AddStateMachine($"Left Hand ({index + 1})", new Vector3(500, 60 + 60 * index, 0));

                var initialToLeftHandTransition1 = initialState.AddTransition(leftHandStateMachine);
                SetImmediateTransitionSetting(initialToLeftHandTransition1);
                initialToLeftHandTransition1.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IS_LOCAL);
                initialToLeftHandTransition1.AddCondition(AnimatorConditionMode.Equals, 0, FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE);
                initialToLeftHandTransition1.AddCondition(AnimatorConditionMode.Equals, index + 1, FaceEmoteControlParameters.FEC_SELECTED_LEFT_GESTURE_GROUP);
                initialToLeftHandTransition1.AddCondition(AnimatorConditionMode.NotEqual, 0, FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT);

                layer.stateMachine.AddStateMachineExitTransition(leftHandStateMachine);

                addStateAndTransition(leftHandStateMachine, "Fist", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT, FaceEmoteControlParameters.FEC_SELECTED_LEFT_GESTURE_GROUP, index + 1, 1, 1 + faceEmoteNumberOffset, new Vector3(400, 00, 0));
                addStateAndTransition(leftHandStateMachine, "HandOpen", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT, FaceEmoteControlParameters.FEC_SELECTED_LEFT_GESTURE_GROUP, index + 1, 2, 2 + faceEmoteNumberOffset, new Vector3(400, 60, 0));
                addStateAndTransition(leftHandStateMachine, "FingerPoint", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT, FaceEmoteControlParameters.FEC_SELECTED_LEFT_GESTURE_GROUP, index + 1, 3, 3 + faceEmoteNumberOffset, new Vector3(400, 120, 0));
                addStateAndTransition(leftHandStateMachine, "Victory", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT, FaceEmoteControlParameters.FEC_SELECTED_LEFT_GESTURE_GROUP, index + 1, 4, 4 + faceEmoteNumberOffset, new Vector3(400, 180, 0));
                addStateAndTransition(leftHandStateMachine, "RockNRoll", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT, FaceEmoteControlParameters.FEC_SELECTED_LEFT_GESTURE_GROUP, index + 1, 5, 5 + faceEmoteNumberOffset, new Vector3(400, 240, 0));
                addStateAndTransition(leftHandStateMachine, "HandGun", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT, FaceEmoteControlParameters.FEC_SELECTED_LEFT_GESTURE_GROUP, index + 1, 6, 6 + faceEmoteNumberOffset, new Vector3(400, 300, 0));
                addStateAndTransition(leftHandStateMachine, "ThumbsUp", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_LEFT, FaceEmoteControlParameters.FEC_SELECTED_LEFT_GESTURE_GROUP, index + 1, 7, 7 + faceEmoteNumberOffset, new Vector3(400, 360, 0));
            }

            // 右手ジェスチャー用表情選択フローの生成
            foreach ((var faceEmoteGestureGroup, var index) in faceEmoteControl.faceEmoteGestureGroups.Select((v, i) => (v, i)))
            {
                var faceEmoteNumberOffset = index * 7 + faceEmoteControl.faceEmoteGestureGroups.Count * 7;
                var rightHandStateMachine = layer.stateMachine.AddStateMachine($"Right Hand ({index + 1})", new Vector3(500, 60 + 60 * index + 60 * faceEmoteControl.faceEmoteGestureGroups.Count, 0));

                var initialToRightHandTransition1 = initialState.AddTransition(rightHandStateMachine);
                SetImmediateTransitionSetting(initialToRightHandTransition1);
                initialToRightHandTransition1.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IS_LOCAL);
                initialToRightHandTransition1.AddCondition(AnimatorConditionMode.Equals, 0, FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE);
                initialToRightHandTransition1.AddCondition(AnimatorConditionMode.Equals, index + 1, FaceEmoteControlParameters.FEC_SELECTED_RIGHT_GESTURE_GROUP);
                initialToRightHandTransition1.AddCondition(AnimatorConditionMode.NotEqual, 0, FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT);

                layer.stateMachine.AddStateMachineExitTransition(rightHandStateMachine);

                addStateAndTransition(rightHandStateMachine, "Fist", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT, FaceEmoteControlParameters.FEC_SELECTED_RIGHT_GESTURE_GROUP, index + 1, 1, 1 + faceEmoteNumberOffset, new Vector3(400, 00, 0));
                addStateAndTransition(rightHandStateMachine, "HandOpen", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT, FaceEmoteControlParameters.FEC_SELECTED_RIGHT_GESTURE_GROUP, index + 1, 2, 2 + faceEmoteNumberOffset, new Vector3(400, 60, 0));
                addStateAndTransition(rightHandStateMachine, "FingerPoint", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT, FaceEmoteControlParameters.FEC_SELECTED_RIGHT_GESTURE_GROUP, index + 1, 3, 3 + faceEmoteNumberOffset, new Vector3(400, 120, 0));
                addStateAndTransition(rightHandStateMachine, "Victory", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT, FaceEmoteControlParameters.FEC_SELECTED_RIGHT_GESTURE_GROUP, index + 1, 4, 4 + faceEmoteNumberOffset, new Vector3(400, 180, 0));
                addStateAndTransition(rightHandStateMachine, "RockNRoll", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT, FaceEmoteControlParameters.FEC_SELECTED_RIGHT_GESTURE_GROUP, index + 1, 5, 5 + faceEmoteNumberOffset, new Vector3(400, 240, 0));
                addStateAndTransition(rightHandStateMachine, "HandGun", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT, FaceEmoteControlParameters.FEC_SELECTED_RIGHT_GESTURE_GROUP, index + 1, 6, 6 + faceEmoteNumberOffset, new Vector3(400, 300, 0));
                addStateAndTransition(rightHandStateMachine, "ThumbsUp", FaceEmoteControlParameters.FEC_SELECTED_GESTURE_RIGHT, FaceEmoteControlParameters.FEC_SELECTED_RIGHT_GESTURE_GROUP, index + 1, 7, 7 + faceEmoteNumberOffset, new Vector3(400, 360, 0));
            }

            var fixedFaceEmotesStateMachine = layer.stateMachine.AddStateMachine("Fixed Face Emotes", new Vector3(500, 60 + 60 * faceEmoteControl.faceEmoteGestureGroups.Count * 2, 0));
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

            // 両手のジェスチャーグループ分番号を確保する (グループ数 * 7(ジェスチャー数) * 2(両手))
            var gestureFaceEmotesCount = faceEmoteControl.faceEmoteGestureGroups.Count * 14;

            // 表情ジェスチャーグループ用ステートを追加する
            for (var i = 0; i < gestureFaceEmotesCount; i++)
            {
                addFixedFaceEmoteState(fixedFaceEmotesStateMachine, i + 1, new Vector3(400, 60 * i, 0));
            }

            // 追加の表情用ステートを追加する
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
                var faceEmoteNumber = i + gestureFaceEmotesCount + 1;
                addFixedFaceEmoteState(fixedFaceEmotesStateMachine, faceEmoteNumber, new Vector3(400, 60 * faceEmoteNumber, 0));
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

            AddNeutralFaceEmoteState(layer.stateMachine, faceEmoteControl, new Vector3(500, 0, 0));
            AddAFKState(layer.stateMachine, new Vector3(500, -60, 0));
            AddMMDState(layer.stateMachine, new Vector3(500, -120, 0));
            AddLeftGestureEmoteStates(layer.stateMachine, new Vector3(500, 60, 0), faceEmoteControl);
            AddRightGestureEmoteStates(layer.stateMachine, new Vector3(500, 120, 0), faceEmoteControl);

            AddFaceEmoteGroups(layer.stateMachine, faceEmoteControl, faceEmoteControl.faceEmoteGestureGroups.Count * 14 + 1, new Vector3(500, 180, 0));

            return layer;
        }

        private void AddMMDState(AnimatorStateMachine stateMachine, Vector3 position)
        {
            var state = stateMachine.AddState("MMD", position);
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
                    trackingEyes = VRC_AnimatorTrackingControl.TrackingType.Animation,
                    trackingMouth = VRC_AnimatorTrackingControl.TrackingType.Animation,
                }
            };

            // MMDダンスワールドなどでダンスモードになった場合（Sit状態でワールド側のアニメーションが適用されている状態）にステートへ進入させる
            var fromEntryTransition1 = stateMachine.AddEntryTransition(state);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IN_STATION);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.SEATED);

            // Sit状態かつSeatedな状態になった場合、ステートから退出する
            var toExitTransition1 = state.AddExitTransition();
            SetImmediateTransitionSetting(toExitTransition1);
            toExitTransition1.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IN_STATION);
            toExitTransition1.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.SEATED);

            // Sit状態ではなくなった場合、ステートから退出する
            var toExitTransition2 = state.AddExitTransition();
            SetImmediateTransitionSetting(toExitTransition2);
            toExitTransition2.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.IN_STATION);
            toExitTransition2.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.SEATED);
        }

        private void AddAFKState(AnimatorStateMachine stateMachine, Vector3 position)
        {
            var state = stateMachine.AddState("AFK", position);
            state.writeDefaultValues = false;
            state.motion = blankAnimationClip;

            // Sit状態ではない場合
            var fromEntryTransition1 = stateMachine.AddEntryTransition(state);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.AFK);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.IN_STATION);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.SEATED);

            // Sit状態（MMDワールドでのダンス時除く）
            var fromEntryTransition2 = stateMachine.AddEntryTransition(state);
            fromEntryTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.AFK);
            fromEntryTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IN_STATION);
            fromEntryTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.SEATED);

            var toExitTransition1 = state.AddExitTransition();
            SetImmediateTransitionSetting(toExitTransition1);
            toExitTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);

            var toExitTransition2 = state.AddExitTransition();
            SetImmediateTransitionSetting(toExitTransition2);
            toExitTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IN_STATION);
            toExitTransition2.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.SEATED);
        }

        private void AddNeutralFaceEmoteState(AnimatorStateMachine stateMachine, FaceEmoteControl faceEmoteControl, Vector3 position)
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

            // Sit状態ではない場合
            var fromEntryTransition1 = stateMachine.AddEntryTransition(state);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.Equals, 0, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.IN_STATION);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.SEATED);

            // Sit状態（MMDワールドでのダンス時除く）
            var fromEntryTransition2 = stateMachine.AddEntryTransition(state);
            fromEntryTransition2.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
            fromEntryTransition2.AddCondition(AnimatorConditionMode.Equals, 0, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);
            fromEntryTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IN_STATION);
            fromEntryTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.SEATED);

            var toExitTransition1 = state.AddExitTransition();
            toExitTransition1.hasExitTime = false;
            toExitTransition1.exitTime = 0;
            toExitTransition1.hasFixedDuration = true;
            toExitTransition1.duration = faceEmoteControl.time;
            toExitTransition1.offset = 0;
            toExitTransition1.interruptionSource = TransitionInterruptionSource.None;
            toExitTransition1.orderedInterruption = true;
            toExitTransition1.AddCondition(AnimatorConditionMode.NotEqual, 0, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);

            var toExitTransition2 = state.AddExitTransition();
            SetImmediateTransitionSetting(toExitTransition2);
            toExitTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.AFK);

            var toExitTransition3 = state.AddExitTransition();
            SetImmediateTransitionSetting(toExitTransition3);
            toExitTransition3.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IN_STATION);
            toExitTransition3.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.SEATED);
        }

        private void AddLeftGestureEmoteStates(AnimatorStateMachine stateMachine, Vector3 position, FaceEmoteControl faceEmoteControl)
        {
            var leftGestureEmoteStateMachine = stateMachine.AddStateMachine("Left Gesture Face Emotes", position);

            leftGestureEmoteStateMachine.entryPosition = new Vector3(0, 0, 0);
            leftGestureEmoteStateMachine.exitPosition = new Vector3(1000, 0, 0);
            leftGestureEmoteStateMachine.anyStatePosition = new Vector3(0, -40, 0);
            leftGestureEmoteStateMachine.parentStateMachinePosition = new Vector3(0, -100, 0);

            // Sit状態ではない場合
            var fromEntryTransition1 = stateMachine.AddEntryTransition(leftGestureEmoteStateMachine);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.Greater, 0, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.Less, faceEmoteControl.faceEmoteGestureGroups.Count * 7 + 1, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.IN_STATION);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.SEATED);

            // Sit状態（MMDワールドでのダンス時除く）
            var fromEntryTransition2 = stateMachine.AddEntryTransition(leftGestureEmoteStateMachine);
            fromEntryTransition2.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
            fromEntryTransition2.AddCondition(AnimatorConditionMode.Greater, 0, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);
            fromEntryTransition2.AddCondition(AnimatorConditionMode.Less, faceEmoteControl.faceEmoteGestureGroups.Count * 7 + 1, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);
            fromEntryTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IN_STATION);
            fromEntryTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.SEATED);

            stateMachine.AddStateMachineExitTransition(leftGestureEmoteStateMachine);

            foreach ((var faceEmoteGestureGroup, var index) in faceEmoteControl.faceEmoteGestureGroups.Select((v, i) => (v, i)))
            {
                var faceEmoteNumberOffset = index * 7;
                AddGestureFaceEmoteStates(leftGestureEmoteStateMachine, $"Fist {index + 1}", faceEmoteControl, faceEmoteGestureGroup?.fist, 1 + faceEmoteNumberOffset, VRCParameters.GESTURE_LEFT_WEIGHT, new Vector3(500, 0 + 120 * index * 7, 0));
                AddGestureFaceEmoteStates(leftGestureEmoteStateMachine, $"HandOpen {index + 1}", faceEmoteControl, faceEmoteGestureGroup?.handOpen, 2 + faceEmoteNumberOffset, "", new Vector3(500, 120 + 120 * index * 7, 0));
                AddGestureFaceEmoteStates(leftGestureEmoteStateMachine, $"FingerPoint {index + 1}", faceEmoteControl, faceEmoteGestureGroup?.fingerPoint, 3 + faceEmoteNumberOffset, "", new Vector3(500, 240 + 120 * index * 7, 0));
                AddGestureFaceEmoteStates(leftGestureEmoteStateMachine, $"Victory {index + 1}", faceEmoteControl, faceEmoteGestureGroup?.victory, 4 + faceEmoteNumberOffset, "", new Vector3(500, 360 + 120 * index * 7, 0));
                AddGestureFaceEmoteStates(leftGestureEmoteStateMachine, $"RockNRoll {index + 1}", faceEmoteControl, faceEmoteGestureGroup?.rockNRoll, 5 + faceEmoteNumberOffset, "", new Vector3(500, 480 + 120 * index * 7, 0));
                AddGestureFaceEmoteStates(leftGestureEmoteStateMachine, $"HandGun {index + 1}", faceEmoteControl, faceEmoteGestureGroup?.handGun, 6 + faceEmoteNumberOffset, "", new Vector3(500, 600 + 120 * index * 7, 0));
                AddGestureFaceEmoteStates(leftGestureEmoteStateMachine, $"ThumbsUp {index + 1}", faceEmoteControl, faceEmoteGestureGroup?.thumbsUp, 7 + faceEmoteNumberOffset, "", new Vector3(500, 720 + 120 * index * 7, 0));
            }
        }

        private void AddRightGestureEmoteStates(AnimatorStateMachine stateMachine, Vector3 position, FaceEmoteControl faceEmoteControl)
        {
            var rightGestureEmoteStateMachine = stateMachine.AddStateMachine("Right Gesture Face Emotes", position);

            rightGestureEmoteStateMachine.entryPosition = new Vector3(0, 0, 0);
            rightGestureEmoteStateMachine.exitPosition = new Vector3(1000, 0, 0);
            rightGestureEmoteStateMachine.anyStatePosition = new Vector3(0, -40, 0);
            rightGestureEmoteStateMachine.parentStateMachinePosition = new Vector3(0, -100, 0);

            // Sit状態ではない場合
            var fromEntryTransition1 = stateMachine.AddEntryTransition(rightGestureEmoteStateMachine);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.Greater, faceEmoteControl.faceEmoteGestureGroups.Count * 7, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.Less, faceEmoteControl.faceEmoteGestureGroups.Count * 14 + 1, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.IN_STATION);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.SEATED);

            // Sit状態（MMDワールドでのダンス時除く）
            var fromEntryTransition2 = stateMachine.AddEntryTransition(rightGestureEmoteStateMachine);
            fromEntryTransition2.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
            fromEntryTransition2.AddCondition(AnimatorConditionMode.Greater, faceEmoteControl.faceEmoteGestureGroups.Count * 7, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);
            fromEntryTransition2.AddCondition(AnimatorConditionMode.Less, faceEmoteControl.faceEmoteGestureGroups.Count * 14 + 1, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);
            fromEntryTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IN_STATION);
            fromEntryTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.SEATED);

            stateMachine.AddStateMachineExitTransition(rightGestureEmoteStateMachine);

            foreach ((var faceEmoteGestureGroup, var index) in faceEmoteControl.faceEmoteGestureGroups.Select((v, i) => (v, i)))
            {
                var faceEmoteNumberOffset = index * 7 + faceEmoteControl.faceEmoteGestureGroups.Count * 7;
                AddGestureFaceEmoteStates(rightGestureEmoteStateMachine, $"Fist {index + 1}", faceEmoteControl, faceEmoteGestureGroup?.fist, 1 + faceEmoteNumberOffset, VRCParameters.GESTURE_RIGHT_WEIGHT, new Vector3(500, 0 + 120 * index * 7, 0));
                AddGestureFaceEmoteStates(rightGestureEmoteStateMachine, $"HandOpen {index + 1}", faceEmoteControl, faceEmoteGestureGroup?.handOpen, 2 + faceEmoteNumberOffset, "", new Vector3(500, 120 + 120 * index * 7, 0));
                AddGestureFaceEmoteStates(rightGestureEmoteStateMachine, $"FingerPoint {index + 1}", faceEmoteControl, faceEmoteGestureGroup?.fingerPoint, 3 + faceEmoteNumberOffset, "", new Vector3(500, 240 + 120 * index * 7, 0));
                AddGestureFaceEmoteStates(rightGestureEmoteStateMachine, $"Victory {index + 1}", faceEmoteControl, faceEmoteGestureGroup?.victory, 4 + faceEmoteNumberOffset, "", new Vector3(500, 360 + 120 * index * 7, 0));
                AddGestureFaceEmoteStates(rightGestureEmoteStateMachine, $"RockNRoll {index + 1}", faceEmoteControl, faceEmoteGestureGroup?.rockNRoll, 5 + faceEmoteNumberOffset, "", new Vector3(500, 480 + 120 * index * 7, 0));
                AddGestureFaceEmoteStates(rightGestureEmoteStateMachine, $"HandGun {index + 1}", faceEmoteControl, faceEmoteGestureGroup?.handGun, 6 + faceEmoteNumberOffset, "", new Vector3(500, 600 + 120 * index * 7, 0));
                AddGestureFaceEmoteStates(rightGestureEmoteStateMachine, $"ThumbsUp {index + 1}", faceEmoteControl, faceEmoteGestureGroup?.thumbsUp, 7 + faceEmoteNumberOffset, "", new Vector3(500, 720 + 120 * index * 7, 0));
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

                    // Sit状態ではない場合
                    var fromEntryTransition1 = stateMachine.AddEntryTransition(currentStateMachine);
                    fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
                    fromEntryTransition1.AddCondition(AnimatorConditionMode.Greater, faceEmoteNumber - 1, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);
                    fromEntryTransition1.AddCondition(AnimatorConditionMode.Less, faceEmoteNumber + 10, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);
                    fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.IN_STATION);
                    fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.SEATED);

                    // Sit状態（MMDワールドでのダンス時除く）
                    var fromEntryTransition2 = stateMachine.AddEntryTransition(currentStateMachine);
                    fromEntryTransition2.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
                    fromEntryTransition2.AddCondition(AnimatorConditionMode.Greater, faceEmoteNumber - 1, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);
                    fromEntryTransition2.AddCondition(AnimatorConditionMode.Less, faceEmoteNumber + 10, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);
                    fromEntryTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IN_STATION);
                    fromEntryTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.SEATED);

                    stateMachine.AddStateMachineExitTransition(currentStateMachine);

                    stateMachinePosition = new Vector3(stateMachinePosition.x, stateMachinePosition.y + 60, stateMachinePosition.z);
                    statePosition = new Vector3(500, 0, 0);
                }

                var faceEmote = faceEmotes[i];
                AddFixedFaceEmoteState(currentStateMachine, $"Additional Face Emote {i + 1} ({faceEmoteNumber})", faceEmoteControl, faceEmote, faceEmoteNumber, statePosition);
                statePosition = new Vector3(statePosition.x, statePosition.y + 60, statePosition.z);
            }
        }

        private void AddGestureFaceEmoteStates(AnimatorStateMachine stateMachine, string name, FaceEmoteControl faceEmoteControl, FaceEmote faceEmote, int faceEmoteNumber, string motionTimeParameter, Vector3 position)
        {
            var unlockState = AddUnlockedGestureFaceEmoteState(stateMachine, name, faceEmoteControl, faceEmote, faceEmoteNumber, motionTimeParameter, position);
            var lockedState = AddLockedGestureFaceEmoteState(stateMachine, name, faceEmoteControl, faceEmote, faceEmoteNumber, motionTimeParameter != "" ? FaceEmoteControlParameters.FEC_FIXED_GESTURE_WEIGHT : "", new Vector3(position.x, position.y + 60, position.z));

            Action<AnimatorStateTransition> AddCommonTransitionOptions = (AnimatorStateTransition transition) =>
            {
                transition.hasExitTime = false;
                transition.exitTime = 0;
                transition.hasFixedDuration = true;
                transition.duration = faceEmoteControl.time;
                transition.offset = 0;
                transition.interruptionSource = TransitionInterruptionSource.None;
                transition.orderedInterruption = true;
            };

            var unlockStateToLockStateTransition1 = unlockState.AddTransition(lockedState);
            AddCommonTransitionOptions(unlockStateToLockStateTransition1);
            unlockStateToLockStateTransition1.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKED);

            var unlockStateToLockStateTransition2 = unlockState.AddTransition(lockedState);
            AddCommonTransitionOptions(unlockStateToLockStateTransition2);
            unlockStateToLockStateTransition2.AddCondition(AnimatorConditionMode.NotEqual, 0, FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE);

            var lockStateToUnlockStateTransition1 = lockedState.AddTransition(unlockState);
            AddCommonTransitionOptions(lockStateToUnlockStateTransition1);
            lockStateToUnlockStateTransition1.AddCondition(AnimatorConditionMode.Equals, faceEmoteNumber, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);
            lockStateToUnlockStateTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKED);
            lockStateToUnlockStateTransition1.AddCondition(AnimatorConditionMode.Equals, 0, FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE);
            if (motionTimeParameter != "")
            {
                lockStateToUnlockStateTransition1.AddCondition(AnimatorConditionMode.Greater, 0, motionTimeParameter);
            }
        }

        private AnimatorState AddUnlockedGestureFaceEmoteState(AnimatorStateMachine stateMachine, string name, FaceEmoteControl faceEmoteControl, FaceEmote faceEmote, int faceEmoteNumber, string motionTimeParameter, Vector3 position)
        {
            var state = stateMachine.AddState($"{name} (Unlocked)", position);
            state.speed = 1 / faceEmoteControl.time;
            state.writeDefaultValues = false;
            state.motion = faceEmote?.motion != null ? faceEmote?.motion : blankAnimationClip;
            if (motionTimeParameter != "")
            {
                state.timeParameterActive = true;
                state.timeParameter = motionTimeParameter;
            }

            state.behaviours = new StateMachineBehaviour[]
            {
                new VRCAnimatorTrackingControl()
                {
                    trackingHead = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingLeftHand = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingRightHand = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingHip = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingLeftFoot = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingRightFoot = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingLeftFingers = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingRightFingers = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingEyes = GetTrackingType(faceEmote?.eyeControlType ?? TrackingControlType.Tracking),
                    trackingMouth = GetTrackingType(faceEmote?.mouthControlType ?? TrackingControlType.Tracking),
                }
            };

            var fromEntryTransition1 = stateMachine.AddEntryTransition(state);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKED);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.Equals, faceEmoteNumber, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.Equals, 0, FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE);

            var toExitTransition1 = state.AddExitTransition();
            toExitTransition1.hasExitTime = false;
            toExitTransition1.exitTime = 0;
            toExitTransition1.hasFixedDuration = true;
            toExitTransition1.duration = faceEmoteControl.time;
            toExitTransition1.offset = 0;
            toExitTransition1.interruptionSource = TransitionInterruptionSource.None;
            toExitTransition1.orderedInterruption = true;
            toExitTransition1.AddCondition(AnimatorConditionMode.NotEqual, faceEmoteNumber, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);

            var toExitTransition2 = state.AddExitTransition();
            SetImmediateTransitionSetting(toExitTransition2);
            toExitTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.AFK);

            var toExitTransition3 = state.AddExitTransition();
            SetImmediateTransitionSetting(toExitTransition3);
            toExitTransition3.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IN_STATION);
            toExitTransition3.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.SEATED);

            return state;
        }

        private AnimatorState AddLockedGestureFaceEmoteState(AnimatorStateMachine stateMachine, string name, FaceEmoteControl faceEmoteControl, FaceEmote faceEmote, int faceEmoteNumber, string motionTimeParameter, Vector3 position)
        {
            var state = stateMachine.AddState($"{name} (Locked)", position);
            state.speed = 1 / faceEmoteControl.time;
            state.writeDefaultValues = false;
            state.motion = faceEmote?.motion != null ? faceEmote?.motion : blankAnimationClip;
            if (motionTimeParameter != "")
            {
                state.timeParameterActive = true;
                state.timeParameter = motionTimeParameter;
            }

            state.behaviours = new StateMachineBehaviour[]
            {
                new VRCAnimatorTrackingControl()
                {
                    trackingHead = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingLeftHand = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingRightHand = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingHip = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingLeftFoot = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingRightFoot = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingLeftFingers = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingRightFingers = VRC_AnimatorTrackingControl.TrackingType.NoChange,
                    trackingEyes = GetTrackingType(faceEmote?.eyeControlType ?? TrackingControlType.Tracking),
                    trackingMouth = GetTrackingType(faceEmote?.mouthControlType ?? TrackingControlType.Tracking),
                }
            };

            // 表情ロック時
            var fromEntryTransition1 = stateMachine.AddEntryTransition(state);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.If, 0, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKED);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.Equals, faceEmoteNumber, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);

            // 表情固定時
            var fromEntryTransition2 = stateMachine.AddEntryTransition(state);
            fromEntryTransition2.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
            fromEntryTransition2.AddCondition(AnimatorConditionMode.NotEqual, 0, FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE);
            fromEntryTransition2.AddCondition(AnimatorConditionMode.Equals, faceEmoteNumber, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);

            var toExitTransition1 = state.AddExitTransition();
            toExitTransition1.hasExitTime = false;
            toExitTransition1.exitTime = 0;
            toExitTransition1.hasFixedDuration = true;
            toExitTransition1.duration = faceEmoteControl.time;
            toExitTransition1.offset = 0;
            toExitTransition1.interruptionSource = TransitionInterruptionSource.None;
            toExitTransition1.orderedInterruption = true;
            toExitTransition1.AddCondition(AnimatorConditionMode.NotEqual, faceEmoteNumber, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);

            var toExitTransition2 = state.AddExitTransition();
            SetImmediateTransitionSetting(toExitTransition2);
            toExitTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.AFK);

            var toExitTransition3 = state.AddExitTransition();
            SetImmediateTransitionSetting(toExitTransition3);
            toExitTransition3.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IN_STATION);
            toExitTransition3.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.SEATED);

            return state;
        }

        private AnimatorState AddFixedFaceEmoteState(AnimatorStateMachine stateMachine, string name, FaceEmoteControl faceEmoteControl, FaceEmote faceEmote, int faceEmoteNumber, Vector3 position)
        {
            var state = stateMachine.AddState(name, position);
            state.speed = 1 / faceEmoteControl.time;
            state.writeDefaultValues = false;
            state.motion = faceEmote?.motion != null ? faceEmote?.motion : blankAnimationClip;
            state.timeParameterActive = true;
            state.timeParameter = FaceEmoteControlParameters.FEC_FIXED_GESTURE_WEIGHT;

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
                    trackingEyes = GetTrackingType(faceEmote?.eyeControlType ?? TrackingControlType.Tracking),
                    trackingMouth = GetTrackingType(faceEmote?.mouthControlType ?? TrackingControlType.Tracking),
                }
            };

            var fromEntryTransition1 = stateMachine.AddEntryTransition(state);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.AFK);
            fromEntryTransition1.AddCondition(AnimatorConditionMode.Equals, faceEmoteNumber, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);

            var toExitTransition1 = state.AddExitTransition();
            toExitTransition1.hasExitTime = false;
            toExitTransition1.exitTime = 0;
            toExitTransition1.hasFixedDuration = true;
            toExitTransition1.duration = faceEmoteControl.time;
            toExitTransition1.offset = 0;
            toExitTransition1.interruptionSource = TransitionInterruptionSource.None;
            toExitTransition1.orderedInterruption = true;
            toExitTransition1.AddCondition(AnimatorConditionMode.NotEqual, faceEmoteNumber, FaceEmoteControlParameters.FEC_SELECTED_FACE_EMOTE);

            var toExitTransition2 = state.AddExitTransition();
            SetImmediateTransitionSetting(toExitTransition2);
            toExitTransition2.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.AFK);

            var toExitTransition3 = state.AddExitTransition();
            SetImmediateTransitionSetting(toExitTransition3);
            toExitTransition3.AddCondition(AnimatorConditionMode.If, 0, VRCParameters.IN_STATION);
            toExitTransition3.AddCondition(AnimatorConditionMode.IfNot, 0, VRCParameters.SEATED);

            return state;
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