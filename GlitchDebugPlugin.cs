using BepInEx;
using BepInEx.Configuration;
using DebugMod.SaveStates;
using HarmonyLib;
using HutongGames.PlayMaker.Actions;

namespace GlitchDebug;

[BepInAutoPlugin(id: "io.github.hk-speedrunning.glitchdebug")]
[BepInDependency("io.github.hk-speedrunning.debugmod", "1.0.3")]
[BepInDependency("org.silksong-modding.modlist", "0.2.0")]
public partial class GlitchDebugPlugin : BaseUnityPlugin
{
    private static GlitchDebugPlugin? _instance;
    internal static GlitchDebugPlugin Instance => _instance!;

    internal ConfigEntry<bool> SaveDupedStates;
    internal ConfigEntry<bool> LegacyForceDuped;
    
    private void Awake()
    {
        if (_instance == null) _instance = this;
        SaveDupedStates = Config.Bind("General",
            "SaveDupedStates",
            true,
            "Whether to save states as duped; mirrors the corresponding keybind.");
        
        LegacyForceDuped = Config.Bind("General",
            "LegacyForceDuped",
            false,
            "Forces duped loading for savestates made prior to v0.2. Will be removed in future; recreate duped states!");
        
        DebugMod.DebugMod.AddToKeyBindList(typeof(Keybinds));
        SaveState.BeforeLoad += Savestates.BeforeLoad;
        SaveState.AfterLoad += Savestates.AfterLoad;
        SaveState.OnSave += Savestates.OnSave;
        
        DebugMod.DebugMod.AddTextToInfoPanel("All Scenes", () => SceneWatcher.LoadedScenes.Join((lsi) => lsi.name));
    }
    
    public static void Log(string message)
    {
        Instance.Logger.LogInfo(message);
    }
}