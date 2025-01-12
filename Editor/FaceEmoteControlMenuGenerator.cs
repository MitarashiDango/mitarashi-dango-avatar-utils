using System.Collections.Generic;
using System.Linq;
using nadena.dev.modular_avatar.core;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace MitarashiDango.AvatarUtils
{
    public class FaceEmoteControlMenuGenerator
    {
        public void GenerateMenu(FaceEmoteControl faceEmoteControl)
        {
            var modularAvatarMenuItem = faceEmoteControl.gameObject.AddComponent<ModularAvatarMenuItem>();
            modularAvatarMenuItem.Control.type = VRCExpressionsMenu.Control.ControlType.SubMenu;
            modularAvatarMenuItem.MenuSource = SubmenuSource.Children;

            GenerateFaceEmoteControlMenuItems(faceEmoteControl);
        }

        private void GenerateFaceEmoteControlMenuItems(FaceEmoteControl faceEmoteControl)
        {
            var subMenuItems = new List<GameObject>
            {
                GenerateToggleMenuItem("表情コントロールON", null, FaceEmoteControlParameters.FEC_ON, 1),
                GenerateToggleMenuItem("表情ロックON", null, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKED, 1),
                GenerateFaceLockMenu(),
                GenerateFaceGestureGroupSelectMenu(faceEmoteControl),
                GenerateFaceSelectMenu(faceEmoteControl),
            };

            foreach (var subMenuItem in subMenuItems)
            {
                subMenuItem.transform.parent = faceEmoteControl.transform;
            }
        }

        private GameObject GenerateFaceLockMenu()
        {
            var subMenu = GenerateSubMenu("動作設定", null);

            var subMenuItems = new List<GameObject>
            {
                GenerateRadialPuppetMenuItem("表情固定・ロック時のジェスチャーウェイト調整", null, FaceEmoteControlParameters.FEC_FIXED_GESTURE_WEIGHT, 1),
                GenerateToggleMenuItem("表情ロック用Contact Receiver ON", null, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_ENABLED, 1),
                GenerateToggleMenuItem("Sit判定時にContact Receiverを自動OFF", null, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_AUTO_DISABLE_ON_SIT, 1),
                GenerateToggleMenuItem("ダンスモードへの自動切り替えON", null, FaceEmoteControlParameters.FEC_AUTO_SWITCH_TO_DANCE_MODE_ON, 1),
            };

            foreach (var subMenuItem in subMenuItems)
            {
                subMenuItem.transform.parent = subMenu.transform;
            }

            return subMenu;
        }

        private GameObject GenerateFaceGestureGroupSelectMenu(FaceEmoteControl faceEmoteControl)
        {
            var subMenu = GenerateSubMenu("ジェスチャーグループ割り当て設定", null);

            var subMenuItems = new List<GameObject>
            {
                GenerateLeftFaceGestureGroupSelectMenu(faceEmoteControl),
                GenerateRightFaceGestureGroupSelectMenu(faceEmoteControl),
            };

            foreach (var subMenuItem in subMenuItems)
            {
                subMenuItem.transform.parent = subMenu.transform;
            }

            return subMenu;
        }

        private GameObject GenerateLeftFaceGestureGroupSelectMenu(FaceEmoteControl faceEmoteControl)
        {
            var subMenu = GenerateSubMenu("左手", null);

            var noSelectionMenuItem = GenerateToggleMenuItem("未割り当て", null, FaceEmoteControlParameters.FEC_SELECTED_LEFT_GESTURE_GROUP, 0);
            noSelectionMenuItem.transform.parent = subMenu.transform;

            foreach ((var faceEmoteGestureGroup, var index) in faceEmoteControl.faceEmoteGestureGroups.Select((v, i) => (v, i)))
            {
                var groupName = faceEmoteGestureGroup.groupName != "" ? faceEmoteGestureGroup.groupName : $"表情ジェスチャーグループ{index + 1}";
                var subMenuItem = GenerateToggleMenuItem(groupName, null, FaceEmoteControlParameters.FEC_SELECTED_LEFT_GESTURE_GROUP, index + 1);
                subMenuItem.transform.parent = subMenu.transform;
            }

            return subMenu;
        }

        private GameObject GenerateRightFaceGestureGroupSelectMenu(FaceEmoteControl faceEmoteControl)
        {
            var subMenu = GenerateSubMenu("右手", null);

            var noSelectionMenuItem = GenerateToggleMenuItem("未割り当て", null, FaceEmoteControlParameters.FEC_SELECTED_RIGHT_GESTURE_GROUP, 0);
            noSelectionMenuItem.transform.parent = subMenu.transform;

            foreach ((var faceEmoteGestureGroup, var index) in faceEmoteControl.faceEmoteGestureGroups.Select((v, i) => (v, i)))
            {
                var groupName = faceEmoteGestureGroup.groupName != "" ? faceEmoteGestureGroup.groupName : $"表情ジェスチャーグループ{index + 1}";
                var subMenuItem = GenerateToggleMenuItem(groupName, null, FaceEmoteControlParameters.FEC_SELECTED_RIGHT_GESTURE_GROUP, index + 1);
                subMenuItem.transform.parent = subMenu.transform;
            }

            return subMenu;
        }

        private GameObject GenerateFaceSelectMenu(FaceEmoteControl faceEmoteControl)
        {
            var subMenu = GenerateSubMenu("表情を選択", null);

            var subMenuItems = new List<GameObject>
            {
                GenerateToggleMenuItem("未選択（ジェスチャー優先）", null, FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE, 0)
            };

            if (faceEmoteControl.faceEmoteGestureGroups.Count > 0)
            {
                subMenuItems.Add(GenerateFixedGestureFaceEmoteMenu(faceEmoteControl));
            }

            var faceEmoteGroupsMenu = GenerateFixedFaceEmoteGroupsMenu(faceEmoteControl, faceEmoteControl.faceEmoteGestureGroups.Count * 7);
            if (faceEmoteGroupsMenu != null)
            {
                subMenuItems.Add(faceEmoteGroupsMenu);
            }

            foreach (var subMenuItem in subMenuItems)
            {
                subMenuItem.transform.parent = subMenu.transform;
            }

            return subMenu;
        }

        private GameObject GenerateFixedGestureFaceEmoteMenu(FaceEmoteControl faceEmoteControl)
        {
            var subMenu = GenerateSubMenu("ジェスチャー別", null);
            var subMenuItems = new List<GameObject>();

            foreach ((var faceEmoteGestureGroup, var index) in faceEmoteControl.faceEmoteGestureGroups.Select((v, i) => (v, i)))
            {
                var gestureMenuItems = new List<GameObject>();
                var groupName = faceEmoteGestureGroup.groupName != "" ? faceEmoteGestureGroup.groupName : $"表情ジェスチャーグループ{index + 1}";
                var gestureMenu = GenerateSubMenu(groupName, null);

                if (faceEmoteGestureGroup?.fist?.motion != null)
                {
                    gestureMenuItems.Add(GenerateFixedFaceEmoteMenuItem(faceEmoteGestureGroup.fist, "Fist", 1 + index * 7));
                }

                if (faceEmoteGestureGroup?.handOpen?.motion != null)
                {
                    gestureMenuItems.Add(GenerateFixedFaceEmoteMenuItem(faceEmoteGestureGroup.handOpen, "HandOpen", 2 + index * 7));
                }

                if (faceEmoteGestureGroup?.fingerPoint?.motion != null)
                {
                    gestureMenuItems.Add(GenerateFixedFaceEmoteMenuItem(faceEmoteGestureGroup.fingerPoint, "FingerPoint", 3 + index * 7));
                }

                if (faceEmoteGestureGroup?.victory?.motion != null)
                {
                    gestureMenuItems.Add(GenerateFixedFaceEmoteMenuItem(faceEmoteGestureGroup.victory, "Victory", 4 + index * 7));
                }

                if (faceEmoteGestureGroup?.rockNRoll?.motion != null)
                {
                    gestureMenuItems.Add(GenerateFixedFaceEmoteMenuItem(faceEmoteGestureGroup.rockNRoll, "RockNRoll", 5 + index * 7));
                }

                if (faceEmoteGestureGroup?.handGun?.motion != null)
                {
                    gestureMenuItems.Add(GenerateFixedFaceEmoteMenuItem(faceEmoteGestureGroup.handGun, "HandGun", 6 + index * 7));
                }

                if (faceEmoteGestureGroup?.thumbsUp?.motion != null)
                {
                    gestureMenuItems.Add(GenerateFixedFaceEmoteMenuItem(faceEmoteGestureGroup.thumbsUp, "ThumbsUp", 7 + index * 7));
                }

                foreach (var gestureMenuItem in gestureMenuItems)
                {
                    gestureMenuItem.transform.parent = gestureMenu.transform;
                }

                subMenuItems.Add(gestureMenu);
            }

            foreach (var subMenuItem in subMenuItems)
            {
                subMenuItem.transform.parent = subMenu.transform;
            }

            return subMenu;
        }

        private GameObject GenerateFixedFaceEmoteGroupsMenu(FaceEmoteControl faceEmoteControl, int faceEmoteNumberOffset)
        {
            if (faceEmoteControl.faceEmoteGroups.Count == 0)
            {
                return null;
            }

            var faceEmoteGroupsMenu = GenerateSubMenu("表情グループ別", null);

            var faceEmoteGroups = faceEmoteControl.faceEmoteGroups
                .Where(faceEmoteGroup => faceEmoteGroup != null);

            var addedFaceEmoteCount = 0;
            foreach ((var faceEmoteGroup, var groupIndex) in faceEmoteGroups.Select((v, i) => (v, i)))
            {
                var faceEmoteGroupMenu = GenerateSubMenu(faceEmoteGroup.groupName != "" ? faceEmoteGroup.groupName : $"グループ {groupIndex + 1}", null);
                faceEmoteGroupMenu.transform.SetParent(faceEmoteGroupsMenu.transform);

                foreach ((var faceEmote, var faceEmoteIndex) in faceEmoteGroup.faceEmotes.Select((v, i) => (v, i)))
                {
                    var subMenuItem = GenerateFixedFaceEmoteMenuItem(faceEmote, $"表情{faceEmoteIndex + 1}", addedFaceEmoteCount + faceEmoteNumberOffset + 1);
                    subMenuItem.transform.SetParent(faceEmoteGroupMenu.transform);
                    addedFaceEmoteCount++;
                }
            }

            return faceEmoteGroupsMenu;
        }

        private GameObject GenerateFixedFaceEmoteMenuItem(FaceEmote faceEmote, string defaultName, float parameterValue)
        {
            var name = faceEmote.FaceEmoteName != "" ? faceEmote.FaceEmoteName : defaultName;
            return GenerateToggleMenuItem(name, faceEmote.icon, FaceEmoteControlParameters.FEC_FIXED_FACE_EMOTE, parameterValue);
        }

        private GameObject GenerateSubMenu(string name, Texture2D icon)
        {
            var go = new GameObject(name);
            var modularAvatarMenuItem = go.AddComponent<ModularAvatarMenuItem>();

            modularAvatarMenuItem.Control.type = VRCExpressionsMenu.Control.ControlType.SubMenu;
            modularAvatarMenuItem.Control.icon = icon;
            modularAvatarMenuItem.MenuSource = SubmenuSource.Children;

            return go;
        }

        private GameObject GenerateToggleMenuItem(string name, Texture2D icon, string parameterName, float parameterValue)
        {
            var go = new GameObject(name);
            var modularAvatarMenuItem = go.AddComponent<ModularAvatarMenuItem>();

            modularAvatarMenuItem.Control.type = VRCExpressionsMenu.Control.ControlType.Toggle;
            modularAvatarMenuItem.Control.parameter = new VRCExpressionsMenu.Control.Parameter
            {
                name = parameterName,
            };
            modularAvatarMenuItem.Control.value = parameterValue;
            modularAvatarMenuItem.Control.icon = icon;

            return go;
        }

        private GameObject GenerateRadialPuppetMenuItem(string name, Texture2D icon, string parameterName, float parameterValue)
        {
            var go = new GameObject(name);
            var modularAvatarMenuItem = go.AddComponent<ModularAvatarMenuItem>();

            modularAvatarMenuItem.Control.type = VRCExpressionsMenu.Control.ControlType.RadialPuppet;
            modularAvatarMenuItem.Control.subParameters = new VRCExpressionsMenu.Control.Parameter[]
            {
                new VRCExpressionsMenu.Control.Parameter
                {
                    name = parameterName,
                }
            };
            modularAvatarMenuItem.Control.value = parameterValue;
            modularAvatarMenuItem.Control.icon = icon;

            return go;
        }
    }
}
