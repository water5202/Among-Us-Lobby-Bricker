using AmongUs.Data;
using AmongUs.Data.Player;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FuckPDFs;
using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace OnlinePatches
{
    public class Patches
    {
        [HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsFreechatAllowed))]
        public static class OP1
        {
            public static void Postfix(ref bool __result)
            {
                __result = true; // non freechat accounts can freechat
            }
        }

        [HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsMinorOrWaiting))]
        public static class OP2
        {
            public static void Postfix(ref bool __result)
            {
                __result = false; // remove minor status
            }
        }

        [HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsFriendsListAllowed))]
        public static class OP3
        {
            public static void Postfix(ref bool __result)
            {
                __result = true; // allows the useless friends list system
            }
        }

        [HarmonyPatch(typeof(FullAccount), nameof(FullAccount.CanSetCustomName))]
        public static class OP4
        {
            public static void Prefix(ref bool canSetName)
            {
                canSetName = true;
            }
        }

        [HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsAllowedOnline))]
        public static class OP6
        {
            public static void Prefix(ref bool canOnline)
            {
                canOnline = true; // allows online mode
            }
        }

        [HarmonyPatch(typeof(AccountManager), nameof(AccountManager.CanPlayOnline))]
        public static class OP7
        {
            public static void Postfix(ref bool __result)
            {
                __result = true;
            }
        }

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.Instance.OnGameJoined))]
        public static class MatchSploit
        {
            public static void Postfix()
            {
                if (!MainClass.LobbyHack) return;
                AmongUsClient.Instance.StartCoroutine(Rejoin(AmongUsClient.Instance).WrapToIl2Cpp());
            }
            private static IEnumerator Rejoin(AmongUsClient __instance)
            {
                yield return new WaitForSeconds(0.8f);
                __instance.ExitGame(DisconnectReasons.ExitGame);
                if (MainClass.RNDName)
                    DataManager.player.customization.Name = MainClass.randomize(10, true);
                __instance.StartCoroutine(
                    __instance.CoJoinOnlineGameFromCode(MainClass.GameID, true) // the game will handle most of this  
                );
            }
        }
    }
}