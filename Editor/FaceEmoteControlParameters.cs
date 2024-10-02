using System.Collections.Generic;
using nadena.dev.modular_avatar.core;

namespace MitarashiDango.AvatarUtils
{
    public class FaceEmoteControlParameters
    {
        public static readonly string FEC_FACE_EMOTE_LOCKER_ENABLED = "FEC_FaceEmoteLockerEnabled";
        public static readonly string FEC_FACE_EMOTE_LOCKER_CONTACT = "FEC_FaceEmoteLockerContact";
        public static readonly string FEC_FACE_EMOTE_LOCKED = "FEC_FaceEmoteLocked";
        public static readonly string FEC_SELECTED_GESTURE_LEFT = "FEC_SelectedGestureLeft";
        public static readonly string FEC_SELECTED_GESTURE_RIGHT = "FEC_SelectedGestureRight";
        public static readonly string FEC_SELECTED_LEFT_GESTURE_GROUP = "FEC_SelectedLeftGestureGroup";
        public static readonly string FEC_SELECTED_RIGHT_GESTURE_GROUP = "FEC_SelectedRightGestureGroup";
        public static readonly string FEC_FIXED_FACE_EMOTE = "FEC_FixedFaceEmote";
        public static readonly string FEC_FACE_EMOTE_LOCKER_AUTO_DISABLE_ON_SIT = "FEC_FaceEmoteLockerAutoDisableOnSit";
        public static readonly string FEC_SELECTED_FACE_EMOTE = "FEC_SelectedFaceEmote";
        public static readonly string FEC_FIXED_GESTURE_WEIGHT = "FEC_FixedGestureWeight";

        public List<ParameterConfig> GetParameterConfigs(FaceEmoteControl faceEmoteControl)
        {
            var parameterConfigs = new List<ParameterConfig>
            {
                new ParameterConfig
                {
                    nameOrPrefix = FEC_FACE_EMOTE_LOCKER_ENABLED,
                    defaultValue = 1,
                    syncType = ParameterSyncType.Bool,
                    saved = true,
                    localOnly = true,
                },
                new ParameterConfig
                {
                    nameOrPrefix = FEC_FACE_EMOTE_LOCKER_CONTACT,
                    defaultValue = 0,
                    syncType = ParameterSyncType.Bool,
                    saved = false,
                    localOnly = true,
                },
                new ParameterConfig
                {
                    nameOrPrefix = FEC_FACE_EMOTE_LOCKED,
                    defaultValue = 0,
                    syncType = ParameterSyncType.Bool,
                    saved = true,
                    localOnly = false,
                },
                new ParameterConfig
                {
                    nameOrPrefix = FEC_SELECTED_GESTURE_LEFT,
                    defaultValue = 0,
                    syncType = ParameterSyncType.Int,
                    saved = true,
                    localOnly = true,
                },
                new ParameterConfig
                {
                    nameOrPrefix = FEC_SELECTED_GESTURE_RIGHT,
                    defaultValue = 0,
                    syncType = ParameterSyncType.Int,
                    saved = true,
                    localOnly = true,
                },
                new ParameterConfig
                {
                    nameOrPrefix = FEC_SELECTED_LEFT_GESTURE_GROUP,
                    defaultValue = faceEmoteControl.leftFaceEmoteGestureGroupNumber,
                    syncType = ParameterSyncType.Int,
                    saved = true,
                    localOnly = true,
                },
                new ParameterConfig
                {
                    nameOrPrefix = FEC_SELECTED_RIGHT_GESTURE_GROUP,
                    defaultValue = faceEmoteControl.rightFaceEmoteGestureGroupNumber,
                    syncType = ParameterSyncType.Int,
                    saved = true,
                    localOnly = true,
                },
                new ParameterConfig
                {
                    nameOrPrefix = FEC_FIXED_FACE_EMOTE,
                    defaultValue = 0,
                    syncType = ParameterSyncType.Int,
                    saved = true,
                    localOnly = false,
                },
                new ParameterConfig
                {
                    nameOrPrefix = FEC_FACE_EMOTE_LOCKER_AUTO_DISABLE_ON_SIT,
                    defaultValue = 0,
                    syncType = ParameterSyncType.Bool,
                    saved = true,
                    localOnly = true,
                },
                new ParameterConfig
                {
                    nameOrPrefix = FEC_SELECTED_FACE_EMOTE,
                    defaultValue = 0,
                    syncType = ParameterSyncType.Int,
                    saved = true,
                    localOnly = false,
                },
                new ParameterConfig
                {
                    nameOrPrefix = FEC_FIXED_GESTURE_WEIGHT,
                    defaultValue = 1,
                    syncType = ParameterSyncType.Float,
                    saved = true,
                    localOnly = false,
                },
            };

            return parameterConfigs;
        }
    }
}