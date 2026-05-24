using BepInEx;
using DebugMod.SaveStates;
using HutongGames.PlayMaker.Actions;

namespace GlitchDebug;

[BepInAutoPlugin(id: "io.github.hk-speedrunning.glitchdebug")]
[BepInDependency("io.github.hk-speedrunning.debugmod", "1.0.2")]
[BepInDependency("org.silksong-modding.modlist", "0.2.0")]
public partial class GlitchDebugPlugin : BaseUnityPlugin
{
    private static GlitchDebugPlugin? _instance;
    internal static GlitchDebugPlugin Instance => _instance!;

    internal bool SaveDupedStates = false;
    internal bool undupeThisState = false;
    
    private void Awake()
    {
        if (_instance == null) _instance = this;
        
        DebugMod.DebugMod.AddToKeyBindList(typeof(Keybinds));
        SaveState.BeforeLoad += Savestates.BeforeLoad;
        SaveState.AfterLoad += Savestates.AfterLoad;
        SaveState.OnSave += Savestates.OnSave;
    }
    
    public static void Log(string message)
    {
        Instance.Logger.LogInfo(message);
    }
}