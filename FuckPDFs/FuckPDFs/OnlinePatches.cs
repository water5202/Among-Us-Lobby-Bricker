using HarmonyLib;
using FuckPDFs;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using UnityEngine;

namespace OnlinePatches
{
    public class Patches
    {
        [HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsAllowedOnline))]
        public static class OnlinePatch1
        {
            public static void Prefix(ref bool canOnline)
            {
                canOnline = true; // allows online mode
            }
        }
    }
    [HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsFreechatAllowed))]
    public static class OnlinePatch2
    {
        public static void Postfix(ref bool __result)
        {
            __result = true; // non freechat accounts can freechat
        }
    }

    [HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsMinorOrWaiting))]
    public static class OnlinePatch2Point5
    {
        public static void Postfix(ref bool __result)
        {
            __result = false; // remove minor status
        }
    }

    [HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsFriendsListAllowed))]
    public static class OnlinePatch3
    {
        public static void Postfix(ref bool __result)
        {
            __result = true; // allows the useless friends list system
        }
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.Instance.OnGameJoined))]
    public static class  MatchSploit
    {
        public static void Postfix()
        {
            if (!MainClass.LobbyHack) return;
            AmongUsClient.Instance.StartCoroutine(Rejoin(AmongUsClient.Instance).WrapToIl2Cpp());
        }
        private static IEnumerator Rejoin(AmongUsClient __instance)
        {
            yield return new WaitForSeconds(0.8f);
            __instance.StartCoroutine(
                __instance.CoJoinOnlineGameFromCode(MainClass.GameID, true) // the game will handle most of this
            );
        }
    }
}