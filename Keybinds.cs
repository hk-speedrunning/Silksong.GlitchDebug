using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DebugMod;
using DebugMod.Hitbox;
using DebugMod.SaveStates;
using DebugMod.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using USceneManager = UnityEngine.SceneManagement.SceneManager;

namespace GlitchDebug;

public class Keybinds
{
    //TODO: consider whether DebugMod could infer LoadDuped automatically? Or if the duped loading should come to us?
    [BindableMethod(name = "Toggle Duped States", category = "Glitches")]
    public static void ToggleDupedStates()
    {
        GlitchDebugPlugin.Instance.SaveDupedStates.Value ^= true;
        DebugMod.DebugMod.LogConsole($"Duped states {(GlitchDebugPlugin.Instance.SaveDupedStates.Value ? "enabled" : "disabled")}");
    }
    
    [BindableMethod(name = "MMS dupe to bench", category = "Glitches")]
    public static void MMS_Dupe() {
        GameManager.instance.StartCoroutine(DupeToTut_01());
    }

    private static IEnumerator DupeToTut_01()
    {
        Addressables.LoadSceneAsync($"Scenes/Tut_01");
        yield return new WaitUntil(() => USceneManager.GetActiveScene().name == "Tut_01");
        var loadop = Addressables.LoadSceneAsync($"Scenes/Tut_01", LoadSceneMode.Additive);
        yield return loadop;

        GameManager.instance.ReadyForRespawn(false);
    }
    
    [BindableMethod(name = "Toggle Noclip", category = "Glitches")]
    public static void ToggleNoclip()
    {
        var rb2d = HeroController.instance.GetComponent<Rigidbody2D>();
        rb2d.bodyType = rb2d.bodyType == RigidbodyType2D.Dynamic ? RigidbodyType2D.Kinematic :  RigidbodyType2D.Dynamic;
    }

    [BindableMethod(name = "Toggle Bench Storage", category = "Glitches")]
    public static void ToggleBenchStorage()
    {
        PlayerData.instance.atBench = !PlayerData.instance.atBench;
        DebugMod.DebugMod.LogConsole($"{(PlayerData.instance.atBench ? "Given" : "Taken away")} bench storage");
    }

    [BindableMethod(name = "Toggle Pogo Storage", category = "Glitches")]
    public static void TogglePogoStorage()
    {
        if (HeroController.instance.currentDownspike && !HeroController.instance.currentDownspike.EnemyDamager.endedDamage)
        {
            HeroController.instance.currentDownspike.CancelAttack();
        }
        else
        {
            HeroController.instance.currentDownspike = HeroController.instance.downSpike;
            HeroController.instance.currentDownspike.StartSlash();
        }
    }
    
    [BindableMethod(name = "Dupe Active Room", category = "Glitches")]
    public static void DupeActiveRoom()
    {
        GameManager.instance.StartCoroutine(LoadRoom(USceneManager.GetActiveScene().name));
    }
    private static IEnumerator LoadRoom(string sceneName)
    {
        var loadOp = USceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        loadOp!.allowSceneActivation = true;
        yield return loadOp;
        GameManager.instance.RefreshTilemapInfo(sceneName);

        var settings = DebugMod.DebugMod.settings;
        if (settings.ShowHitBoxes > 0)
        {
            int cs = settings.ShowHitBoxes;
            settings.ShowHitBoxes = 0;
            yield return new WaitUntil(() => HitboxViewer.State == 0);
            settings.ShowHitBoxes = cs;
        }
    }

    [BindableMethod(name = "Undupe Active Room", category = "Glitches")]
    public static void UndupeActiveRoom()
    {
        Savestates.UndupeThisState = true;
        var state = SaveStateManager.SaveNewState();
        GameManager.instance.StartCoroutine(state.Load());
    }

    [BindableMethod(name = "Reset All Scene Data", category = "Glitches")]
    public static void ResetAllSceneData()
    {
        SceneData.instance.Reset();
        BindableFunctions.ResetCurrentScene();
        
        DebugMod.DebugMod.LogConsole("All Scene Data reset.");
    }
    
    [BindableMethod(name = "Print load names", category = "Glitches")]
    public static void PrintLoadNames()
    {
        IEnumerable<TransitionPoint> transitionPoints = TransitionPoint.TransitionPoints;
        IEnumerable<TransitionPoint> leftLoads = transitionPoints.Where(t => t.GetGatePosition() == GlobalEnums.GatePosition.left)
            .OrderBy(load => -load.gameObject.GetComponent<Collider>().bounds.center.y);
        IEnumerable<TransitionPoint> rightLoads = transitionPoints.Where(t => t.GetGatePosition() == GlobalEnums.GatePosition.right)
            .OrderBy(load => -load.gameObject.GetComponent<Collider>().bounds.center.y);
        IEnumerable<TransitionPoint> topLoads = transitionPoints.Where(t => t.GetGatePosition() == GlobalEnums.GatePosition.top)
            .OrderBy(load => load.gameObject.GetComponent<Collider>().bounds.center.x);
        IEnumerable<TransitionPoint> bottomLoads = transitionPoints.Where(t => t.GetGatePosition() == GlobalEnums.GatePosition.bottom)
            .OrderBy(load => load.gameObject.GetComponent<Collider>().bounds.center.x);
        IEnumerable<TransitionPoint> doorLoads = transitionPoints.Where(t => t.GetGatePosition() == GlobalEnums.GatePosition.door)
            .OrderBy(load => load.gameObject.GetComponent<Collider>().bounds.center.x);
        IEnumerable<TransitionPoint> otherLoads = transitionPoints.Where(t => t.GetGatePosition() == GlobalEnums.GatePosition.unknown)
            .OrderBy(load => load.gameObject.GetComponent<Collider>().bounds.center.x);
        
        foreach (TransitionPoint tp in leftLoads.Concat(rightLoads).Concat(topLoads).Concat(bottomLoads).Concat(doorLoads).Concat(otherLoads))
        {
            DebugMod.DebugMod.LogConsole(tp.name);
        }

        // string[] names = transitionPoints.Select(t => t.name).ToArray();


        // foreach (string name in names) Console.AddLine(name);
        // TransitionPoint[] leftLoads = transitionPoints.Where(t => t.entryPoint.StartsWith("left"));
        // TransitionPoint[] rightLoads = transitionPoints.Where(t => t.entryPoint.StartsWith("left"));
        // TransitionPoint[] leftLoads = transitionPoints.Where(t => t.entryPoint.StartsWith("left"));
        // TransitionPoint[] leftLoads = transitionPoints.Where(t => t.entryPoint.StartsWith("left"));
        // var scenes = SceneWatcher.LoadedScenes;
        // string[] loadedScenes = scenes.Select(s => s.name).ToArray();
        // Console.AddLine($"{(PlayerData.instance.atBench ? "Given" : "Taken away")} bench storage");
    }
}