

using System.Collections.Generic;
using nadena.dev.modular_avatar.core;

namespace MitarashiDango.AvatarUtils
{
#if UNITY_EDITOR
    public class Parameters
    {
        public static readonly string AFK = "AFK";
        public static readonly string GESTURE_LEFT = "GestureLeft";
        public static readonly string GESTURE_RIGHT = "GestureRight";
        public static readonly string GESTURE_LEFT_WEIGHT = "GestureLeftWeight";
        public static readonly string GESTURE_RIGHT_WEIGHT = "GestureRightWeight";
        public static readonly string IS_LOCAL = "IsLocal";
        public static readonly string IN_STATION = "InStation";
        public static readonly string FEC_FACE_EMOTE_LOCKER_ENABLED = "FEC_FaceEmoteLockerEnabled";
        public static readonly string FEC_FACE_EMOTE_LOCKER_CONTACT = "FEC_FaceEmoteLockerContact";
        public static readonly string FEC_FACE_EMOTE_LOCKED = "FEC_FaceEmoteLocked";
        public static readonly string FEC_GESTURE_LEFT_WEIGHT = "FEC_GestureLeftWeight";
        public static readonly string FEC_GESTURE_RIGHT_WEIGHT = "FEC_GestureRightWeight";
        public static readonly string FEC_SELECTED_GESTURE_LEFT = "FEC_SelectedGestureLeft";
        public static readonly string FEC_SELECTED_GESTURE_RIGHT = "FEC_SelectedGestureRight";
        public static readonly string FEC_SELECTED_FACE_EMOTE_BY_MENU = "FEC_SelectedFaceEmoteByMenu";
        public static readonly string FEC_FACE_EMOTE_LOCKER_AUTO_DISABLE_ON_SIT = "FEC_FaceEmoteLockerAutoDisableOnSit";
        public static readonly string FEC_SELECTED_FACE_EMOTE = "FEC_SelectedFaceEmote";

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
                    localOnly = true,
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
                    nameOrPrefix = FEC_SELECTED_FACE_EMOTE_BY_MENU,
                    defaultValue = 0,
                    syncType = ParameterSyncType.Int,
                    saved = true,
                    localOnly = true,
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
                }
            };

            if (faceEmoteControl.isGenerateGestureWeightLockLogic)
            {
                parameterConfigs.Add(new ParameterConfig
                {
                    nameOrPrefix = FEC_GESTURE_LEFT_WEIGHT,
                    defaultValue = 0,
                    syncType = ParameterSyncType.Float,
                    saved = true,
                    localOnly = false,
                });

                parameterConfigs.Add(new ParameterConfig
                {
                    nameOrPrefix = FEC_GESTURE_RIGHT_WEIGHT,
                    defaultValue = 0,
                    syncType = ParameterSyncType.Float,
                    saved = true,
                    localOnly = false,
                });
            }

            return parameterConfigs;
        }
    }
#endif
}