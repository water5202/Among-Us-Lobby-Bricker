using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using InnerNet;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FuckPDFs
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        internal static new ManualLogSource Log;
        private readonly Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

        public override void Load()
        {
            [DllImport("kernel32.dll")]
            static extern IntPtr CreateMutex(IntPtr lpMutexAttributes, bool bInitialOwner, string lpName);
            CreateMutex(IntPtr.Zero, false, FuckPDFs.MainClass.randomize(10, false));

            Log = base.Log;
            harmony.PatchAll();
            ClassInjector.RegisterTypeInIl2Cpp<MainClass>(); // register class
            AddComponent<MainClass>();
        }
    }
    public class MainClass : MonoBehaviour
    {
        public static int GameID = 0;
        public static bool LobbyHack = false;
        public static bool ToggleUI = false;
        public static bool RNDName = false;
        private Rect windowRect = new Rect(20, 20, 260, 240);

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1)) ToggleUI = !ToggleUI;
            if (Input.GetKeyDown(KeyCode.F2)) LobbyHack = !LobbyHack;
        }

        private void OnGUI()
        {
            if (!ToggleUI) return;
            windowRect = GUILayout.Window(9001, windowRect, (GUI.WindowFunction)Window, "WNC");
        }

        public static string randomize(int length, bool filter)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string result;
            do
            {
                var arr = new char[length];
                for (int i = 0; i < length; i++)
                    arr[i] = chars[UnityEngine.Random.Range(0, chars.Length)];
                result = new string(arr);
            } while (filter && BlockedWords.ContainsWord(result));
            return result;
        }

        private void Window(int winId)
        {
            GUILayout.Space(4);

            GUILayout.Label("Current Lobby Code: ");
            GUILayout.Label(GameCode.IntToGameName(GameID));

            GUILayout.Space(6);

            if (GUILayout.Button("Force Login"))
                AmongUs.Data.DataManager.Player.Account.LoginStatus = EOSManager.AccountLoginStatus.LoggedIn;

            GUILayout.Space(6);

            if (GUILayout.Button("Capture Code"))
                GameID = AmongUsClient.Instance.GameId;

            GUILayout.Space(6);

            bool lhb = (GUILayout.Toggle(LobbyHack, "LobbyHack [F2]"));
            if (lhb != LobbyHack)
            {
                LobbyHack = lhb;
                if (!LobbyHack) return;
                if (GameID == 0) return;
                AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);
                AmongUsClient.Instance.StartCoroutine(
                    AmongUsClient.Instance.CoJoinOnlineGameFromCode(GameID) // join again
                );
            }
            GUILayout.Space(6);

            bool rn = (GUILayout.Toggle(RNDName, "Randomize Name on rejoin"));
            if (rn != RNDName)
            {
                RNDName = rn;
            }

            if (GUILayout.Button("Force Disconnect"))
                AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);

            GUI.DragWindow(new Rect(0, 0, windowRect.width, 40)); // dragwindow duh
        }
    }
}
