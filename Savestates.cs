using DebugMod.SaveStates;
using UnityEngine;

namespace GlitchDebug;

internal static class Savestates
{
    private static bool StateFlagEnabled(SaveState state, string flagName)
    {
        state.data.customData.TryGetValue(flagName, out var flag);
        return flag == "true";
    }

    internal static void OnSave(SaveState state)
    {
        if (GlitchDebugPlugin.Instance.SaveDupedStates && !GlitchDebugPlugin.Instance.undupeThisState)
        {
            state.data.customData["GlitchDebug.Duped"] = "true";
        }

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

        GlitchDebugPlugin.Instance.undupeThisState = false; // reset Undupe Active Room override
    }

    internal static void BeforeLoad(SaveState state)
    {
        if (StateFlagEnabled(state, "GlitchDebug.Duped"))
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