
using System.Collections.Generic;
using nadena.dev.modular_avatar.core;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace MitarashiDango.AvatarUtils
{
#if UNITY_EDITOR
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

            var additionalFaceEmoteMenu = GenerateAdditionalFaceEmoteMenu(faceEmoteControl);
            if (additionalFaceEmoteMenu != null)
            {
                subMenuItems.Add(additionalFaceEmoteMenu);
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

            if (faceEmoteControl.leftFaceEmoteGroup != null)
            {
                var faceEmoteGroup = faceEmoteControl?.leftFaceEmoteGroup;
                if (faceEmoteGroup?.fist?.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGroup.fist, "Fist", 1));
                }

                if (faceEmoteGroup?.handOpen?.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGroup.handOpen, "HandOpen", 2));
                }

                if (faceEmoteGroup?.fingerPoint?.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGroup.fingerPoint, "FingerPoint", 3));
                }

                if (faceEmoteGroup?.victory?.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGroup.victory, "Victory", 4));
                }

                if (faceEmoteGroup?.rockNRoll?.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGroup.rockNRoll, "RockNRoll", 5));
                }

                if (faceEmoteGroup?.handGun?.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGroup.handGun, "HandGun", 6));
                }

                if (faceEmoteGroup?.thumbsUp?.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGroup.thumbsUp, "ThumbsUp", 7));
                }
            }
            else
            {
                if (faceEmoteControl.leftFist != null && faceEmoteControl.leftFist.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteControl.leftFist, "Fist", 1));
                }

                if (faceEmoteControl.leftHandOpen != null && faceEmoteControl.leftHandOpen.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteControl.leftHandOpen, "HandOpen", 2));
                }

                if (faceEmoteControl.leftFingerPoint != null && faceEmoteControl.leftFingerPoint.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteControl.leftFingerPoint, "FingerPoint", 3));
                }

                if (faceEmoteControl.leftVictory != null && faceEmoteControl.leftVictory.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteControl.leftVictory, "Victory", 4));
                }

                if (faceEmoteControl.leftRockNRoll != null && faceEmoteControl.leftRockNRoll.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteControl.leftRockNRoll, "RockNRoll", 5));
                }

                if (faceEmoteControl.leftHandGun != null && faceEmoteControl.leftHandGun.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteControl.leftHandGun, "HandGun", 6));
                }

                if (faceEmoteControl.leftThumbsUp != null && faceEmoteControl.leftThumbsUp.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteControl.leftThumbsUp, "ThumbsUp", 7));
                }
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

            if (faceEmoteControl.rightFaceEmoteGroup != null)
            {
                var faceEmoteGroup = faceEmoteControl?.rightFaceEmoteGroup;
                if (faceEmoteGroup?.fist?.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGroup.fist, "Fist", 8));
                }

                if (faceEmoteGroup?.handOpen?.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGroup.handOpen, "HandOpen", 9));
                }

                if (faceEmoteGroup?.fingerPoint?.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGroup.fingerPoint, "FingerPoint", 10));
                }

                if (faceEmoteGroup?.victory?.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGroup.victory, "Victory", 11));
                }

                if (faceEmoteGroup?.rockNRoll?.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGroup.rockNRoll, "RockNRoll", 12));
                }

                if (faceEmoteGroup?.handGun?.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGroup.handGun, "HandGun", 13));
                }

                if (faceEmoteGroup?.thumbsUp?.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteGroup.thumbsUp, "ThumbsUp", 14));
                }
            }
            else
            {
                if (faceEmoteControl.rightFist != null && faceEmoteControl.rightFist.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteControl.rightFist, "Fist", 8));
                }

                if (faceEmoteControl.rightHandOpen != null && faceEmoteControl.rightHandOpen.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteControl.rightHandOpen, "HandOpen", 9));
                }

                if (faceEmoteControl.rightFingerPoint != null && faceEmoteControl.rightFingerPoint.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteControl.rightFingerPoint, "FingerPoint", 10));
                }

                if (faceEmoteControl.rightVictory != null && faceEmoteControl.rightVictory.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteControl.rightVictory, "Victory", 11));
                }

                if (faceEmoteControl.rightRockNRoll != null && faceEmoteControl.rightRockNRoll.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteControl.rightRockNRoll, "RockNRoll", 12));
                }

                if (faceEmoteControl.rightHandGun != null && faceEmoteControl.rightHandGun.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteControl.rightHandGun, "HandGun", 13));
                }

                if (faceEmoteControl.rightThumbsUp != null && faceEmoteControl.rightThumbsUp.motion != null)
                {
                    subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteControl.rightThumbsUp, "ThumbsUp", 14));
                }
            }

            foreach (var subMenuItem in subMenuItems)
            {
                subMenuItem.transform.parent = subMenu.transform;
            }

            return subMenu;
        }

        private GameObject GenerateAdditionalFaceEmoteMenu(FaceEmoteControl faceEmoteControl)
        {
            if (faceEmoteControl.additionalFaceEmotes.Count == 0)
            {
                return null;
            }

            var subMenu = GenerateSubMenu("追加の表情", null);
            var subMenuItems = new List<GameObject>();

            for (var i = 0; i < faceEmoteControl.additionalFaceEmotes.Count; i++)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem(faceEmoteControl.additionalFaceEmotes[i], $"表情{i + 1}", i + Constants.ADDITIONAL_FACE_EMOTE_MIN_NUMBER));
            }

            foreach (var subMenuItem in subMenuItems)
            {
                subMenuItem.transform.parent = subMenu.transform;
            }

            return subMenu;
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
#endif
}
