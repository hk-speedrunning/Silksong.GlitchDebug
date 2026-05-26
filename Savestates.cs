using DebugMod.SaveStates;
using UnityEngine;

namespace GlitchDebug;

internal static class Savestates
{
    internal static bool UndupeThisState = false;
    
    private static bool StateFlagEnabled(SaveState state, string flagName)
    {
        state.data.customData.TryGetValue(flagName, out var flag);
        return flag == "true";
    }

    internal static void OnSave(SaveState state)
    {
        if (GlitchDebugPlugin.Instance.SaveDupedStates.Value || !UndupeThisState)
        {
            state.data.customData["GlitchDebug.Duped"] = "true";
        }
        UndupeThisState = false; // reset Undupe Active Room override
        
        if (HeroController.instance.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Kinematic)
        {
            state.data.customData["GlitchDebug.Noclip"] = "true";
        }

        if (PlayerData.instance.atBench)
        {
            state.data.customData["GlitchDebug.BenchStorage"] = "true";
        }

        if (HeroController.instance.currentDownspike &&
            !HeroController.instance.currentDownspike.EnemyDamager.endedDamage)
        {
            state.data.customData["GlitchDebug.PogoStorage"] = "true";
        }
        
    }

    private static bool StateIsDupedHeuristic(SaveState state)
    {
        return state.data.loadedScenes.Length switch
        {
            1 => false,
            2 => state.data.loadedScenes[1] == $"{state.data.loadedScenes[0]}_boss",
            _ => true
        };
    }

    internal static void BeforeLoad(SaveState state)
    {
        if (StateFlagEnabled(state, "GlitchDebug.Duped") || StateIsDupedHeuristic(state))
        {
            SaveState.LoadDuped = true;
        }
    }
    
    internal static void AfterLoad(SaveState state)
    {
        // Cleanup
        SaveState.LoadDuped = false;
        
        // Restore glitched states
        if (StateFlagEnabled(state, "GlitchDebug.Noclip"))
        {
            var rb2d = HeroController.instance.GetComponent<Rigidbody2D>();
            rb2d.bodyType = RigidbodyType2D.Kinematic;
        }
        if (StateFlagEnabled(state, "GlitchDebug.BenchStorage"))
        {
            GameManager.instance.SetPlayerDataBool(nameof(PlayerData.atBench), true);
        }
        if (StateFlagEnabled(state, "GlitchDebug.Invulnerable"))
        {
            HeroController.instance.cState.invulnerable = true;
        }

        if (StateFlagEnabled(state, "GlitchDebug.PogoStorage"))
        {
            // TODO: other crests?
            HeroController.instance.currentDownspike = HeroController.instance.downSpike;
            HeroController.instance.currentDownspike.StartSlash();
        }
    }
}