
using System.Collections.Generic;
using nadena.dev.modular_avatar.core;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace MitarashiDango.FaceEmoteControl
{
#if UNITY_EDITOR
    public class RadialMenuGenerator
    {
        public GameObject GenerateRadialMenu(FaceEmoteControl faceEmoteControl)
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
                GenerateToggleMenuItem("表情ロックON", null, Parameters.FEC_FACE_EMOTE_LOCKED, 1),
                GenerateToggleMenuItem("表情ロック用Contact Receiver ON", null, Parameters.FEC_FACE_EMOTE_LOCKER_ENABLED, 1),
                GenerateToggleMenuItem("Sit判定時にContact Receiverを自動OFF", null, Parameters.FEC_FACE_EMOTE_LOCKER_AUTO_DISABLE_ON_SIT, 1),
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
                GenerateFaceSelectMenuItem("未選択", null, 0)
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

        private GameObject GenerateFaceSelectMenuItem(string name, Texture2D icon, float parameterValue)
        {
            return GenerateToggleMenuItem(name, icon, Parameters.FEC_SELECTED_FACE_EMOTE_BY_MENU, parameterValue);
        }

        private GameObject GenerateLeftGestureFaceEmoteMenu(FaceEmoteControl faceEmoteControl)
        {
            if (!faceEmoteControl.IsLeftGestureAvailable)
            {
                return null;
            }

            var subMenu = GenerateSubMenu("左手", null);
            var subMenuItems = new List<GameObject>();

            if (faceEmoteControl.leftFistAnimationClip != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem("Fist", faceEmoteControl.leftFistRadialMenuIcon, 1));
            }

            if (faceEmoteControl.leftHandOpenAnimationClip != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem("HandOpen", faceEmoteControl.leftHandOpenRadialMenuIcon, 2));
            }

            if (faceEmoteControl.leftFingerPointAnimationClip != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem("FingerPoint", faceEmoteControl.leftFingerPointRadialMenuIcon, 3));
            }

            if (faceEmoteControl.leftVictoryAnimationClip != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem("Victory", faceEmoteControl.leftVictoryRadialMenuIcon, 4));
            }

            if (faceEmoteControl.leftRockNRollAnimationClip != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem("RockNRoll", faceEmoteControl.leftRockNRollRadialMenuIcon, 5));
            }

            if (faceEmoteControl.leftHandGunAnimationClip != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem("HandGun", faceEmoteControl.leftHandGunRadialMenuIcon, 6));
            }

            if (faceEmoteControl.leftThumbsUpAnimationClip != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem("ThumbsUp", faceEmoteControl.leftThumbsUpRadialMenuIcon, 7));
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

            if (faceEmoteControl.rightFistAnimationClip != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem("Fist", faceEmoteControl.rightFistRadialMenuIcon, 8));
            }

            if (faceEmoteControl.rightHandOpenAnimationClip != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem("HandOpen", faceEmoteControl.rightHandOpenRadialMenuIcon, 9));
            }

            if (faceEmoteControl.rightFingerPointAnimationClip != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem("FingerPoint", faceEmoteControl.rightFingerPointRadialMenuIcon, 10));
            }

            if (faceEmoteControl.rightVictoryAnimationClip != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem("Victory", faceEmoteControl.rightVictoryRadialMenuIcon, 11));
            }

            if (faceEmoteControl.rightRockNRollAnimationClip != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem("RockNRoll", faceEmoteControl.rightRockNRollRadialMenuIcon, 12));
            }

            if (faceEmoteControl.rightHandGunAnimationClip != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem("HandGun", faceEmoteControl.rightHandGunRadialMenuIcon, 13));
            }

            if (faceEmoteControl.rightThumbsUpAnimationClip != null)
            {
                subMenuItems.Add(GenerateFaceSelectMenuItem("ThumbsUp", faceEmoteControl.rightThumbsUpRadialMenuIcon, 14));
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
                subMenuItems.Add(GenerateToggleMenuItem($"表情{i + 1}", null, Parameters.FEC_SELECTED_FACE_EMOTE_BY_MENU, i + Constants.ADDITIONAL_FACE_EMOTE_MIN_NUMBER));
            }

            foreach (var subMenuItem in subMenuItems)
            {
                subMenuItem.transform.parent = subMenu.transform;
            }

            return subMenu;
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
