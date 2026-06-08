using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using InnerNet;
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
            Log = base.Log;
            harmony.PatchAll();
            ClassInjector.RegisterTypeInIl2Cpp<MainClass>();
            AddComponent<MainClass>();
        }
    }
    public class MainClass : MonoBehaviour
    {
        public static int GameID = 0;
        public static bool LobbyHack = false;
        public static bool ToggleUI = false;
        private Rect windowRect = new Rect(20, 20, 260, 240);

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1)) ToggleUI = !ToggleUI;
            if (Input.GetKeyDown(KeyCode.F2)) LobbyHack = !LobbyHack;
        }

        private void OnGUI()
        {
            if (!ToggleUI) return;
            windowRect = GUILayout.Window(9001, windowRect, (GUI.WindowFunction)Window, "FuckPDFs");
        }
        private void Window(int winId)
        {
            GUILayout.Space(4);

            GUILayout.Label("Current Lobby Code: ");
            GUILayout.Label(GameCode.IntToGameName(GameID));

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

            if (GUILayout.Button("Force Disconnect"))
                AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);

            GUI.DragWindow(new Rect(0, 0, windowRect.width, 40));
        }
    }
}
