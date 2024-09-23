using System.Collections.Generic;
using nadena.dev.modular_avatar.core;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace MitarashiDango.AvatarUtils
{
    public class FaceEmoteControlMenuGenerator
    {
        public GameObject GenerateMenus(FaceEmoteControl faceEmoteControl)
        {
            var menuTree = GenerateFaceEmoteControlMenu(faceEmoteControl);
            menuTree.AddComponent<ModularAvatarMenuInstaller>();

            return menuTree;
        }

        private GameObject GenerateFaceEmoteControlMenu(FaceEmoteControl faceEmoteControl)
        {
            var subMenu = GenerateSubMenu("表情操作", null);

            var subMenuItems = new List<GameObject>
            {
                GenerateFaceLockMenu(),
                GenerateFaceSelectMenu(faceEmoteControl),
            };

            foreach (var subMenuItem in subMenuItems)
            {
                subMenuItem.transform.parent = subMenu.transform;
            }

            return subMenu;
        }

        private GameObject GenerateFaceLockMenu()
        {
            var subMenu = GenerateSubMenu("表情ロック設定", null);

            var subMenuItems = new List<GameObject>
            {
                GenerateToggleMenuItem("表情ロックON", null, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKED, 1),
                GenerateToggleMenuItem("表情ロック用Contact Receiver ON", null, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_ENABLED, 1),
                GenerateToggleMenuItem("Sit判定時にContact Receiverを自動OFF", null, FaceEmoteControlParameters.FEC_FACE_EMOTE_LOCKER_AUTO_DISABLE_ON_SIT, 1),
            };

            foreach (var subMenuItem in subMenuItems)
            {
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

            var leftGestureFaceEmoteMenu = GenerateLeftGestureFaceEmoteMenu(faceEmoteControl);
            if (leftGestureFaceEmoteMenu != null)
            {
                subMenuItems.Add(leftGestureFaceEmoteMenu);
            }

            var rightGestureFaceEmoteMenu = GenerateRightGestureFaceEmoteMenu(faceEmoteControl);
            if (rightGestureFaceEmoteMenu != null)
            {
                subMenuItems.Add(rightGestureFaceEmoteMenu);
            }

            var faceEmoteGroupsMenu = GenerateFaceEmoteGroupsMenu(faceEmoteControl, Constants.ADDITIONAL_FACE_EMOTE_MIN_NUMBER);
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

        private GameObject GenerateLeftGestureFaceEmoteMenu(FaceEmoteControl faceEmoteControl)
        {
            if (!faceEmoteControl.IsLeftGestureAvailable)
            {
                return null;
            }

            var subMenu = GenerateSubMenu("左手", null);
            var subMenuItems = new List<GameObject>();

            var faceEmoteGestureGroup = faceEmoteControl?.leftFaceEmoteGestureGroup;
            if (faceEmoteGestureGroup?.fist?.motion != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGestureGroup.fist, "Fist", 1));
            }

            if (faceEmoteGestureGroup?.handOpen?.motion != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGestureGroup.handOpen, "HandOpen", 2));
            }

            if (faceEmoteGestureGroup?.fingerPoint?.motion != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGestureGroup.fingerPoint, "FingerPoint", 3));
            }

            if (faceEmoteGestureGroup?.victory?.motion != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGestureGroup.victory, "Victory", 4));
            }

            if (faceEmoteGestureGroup?.rockNRoll?.motion != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGestureGroup.rockNRoll, "RockNRoll", 5));
            }

            if (faceEmoteGestureGroup?.handGun?.motion != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGestureGroup.handGun, "HandGun", 6));
            }

            if (faceEmoteGestureGroup?.thumbsUp?.motion != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGestureGroup.thumbsUp, "ThumbsUp", 7));
            }

            foreach (var subMenuItem in subMenuItems)
            {
                subMenuItem.transform.parent = subMenu.transform;
            }

            return subMenu;
        }

        private GameObject GenerateRightGestureFaceEmoteMenu(FaceEmoteControl faceEmoteControl)
        {
            if (!faceEmoteControl.IsRightGestureAvailable)
            {
                return null;
            }

            var subMenu = GenerateSubMenu("右手", null);
            var subMenuItems = new List<GameObject>();

            var faceEmoteGestureGroup = faceEmoteControl?.rightFaceEmoteGestureGroup;
            if (faceEmoteGestureGroup?.fist?.motion != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGestureGroup.fist, "Fist", 8));
            }

            if (faceEmoteGestureGroup?.handOpen?.motion != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGestureGroup.handOpen, "HandOpen", 9));
            }

            if (faceEmoteGestureGroup?.fingerPoint?.motion != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGestureGroup.fingerPoint, "FingerPoint", 10));
            }

            if (faceEmoteGestureGroup?.victory?.motion != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGestureGroup.victory, "Victory", 11));
            }

            if (faceEmoteGestureGroup?.rockNRoll?.motion != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGestureGroup.rockNRoll, "RockNRoll", 12));
            }

            if (faceEmoteGestureGroup?.handGun?.motion != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGestureGroup.handGun, "HandGun", 13));
            }

            if (faceEmoteGestureGroup?.thumbsUp?.motion != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGestureGroup.thumbsUp, "ThumbsUp", 14));
            }

            foreach (var subMenuItem in subMenuItems)
            {
                subMenuItem.transform.parent = subMenu.transform;
            }

            return subMenu;
        }

        private GameObject GenerateFaceEmoteGroupsMenu(FaceEmoteControl faceEmoteControl, int startFaceEmoteNumber)
        {
            if (faceEmoteControl.faceEmoteGroups.Count == 0)
            {
                return null;
            }

            var addedFaceEmoteCount = 0;
            var faceEmoteGroupsMenu = GenerateSubMenu("表情グループ", null);
            for (var i = 0; i < faceEmoteControl.faceEmoteGroups.Count; i++)
            {
                var faceEmoteGroup = faceEmoteControl.faceEmoteGroups[i];
                if (faceEmoteGroup == null)
                {
                    continue;
                }

                var faceEmoteGroupMenu = GenerateSubMenu(faceEmoteGroup.groupName != "" ? faceEmoteGroup.groupName : $"グループ {i + 1}", null);
                faceEmoteGroupMenu.transform.SetParent(faceEmoteGroupsMenu.transform);

                for (var j = 0; j < faceEmoteGroup.faceEmotes.Count; j++)
                {
                    var subMenuItem = GenerateFaceSelectMenuItem(faceEmoteGroup.faceEmotes[j], $"表情{j + 1}", addedFaceEmoteCount + startFaceEmoteNumber);
                    subMenuItem.transform.SetParent(faceEmoteGroupMenu.transform);

                    addedFaceEmoteCount++;
                }
            }

            return faceEmoteGroupsMenu;
        }

        private GameObject GenerateFaceSelectMenuItem(FaceEmote faceEmote, string defaultName, float parameterValue)
        {
            var name = faceEmote.name != "" ? faceEmote.name : defaultName;
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
    }
}
