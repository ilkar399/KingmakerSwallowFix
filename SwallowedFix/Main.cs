using System;
using System.Reflection;

using UnityModManagerNet;
using HarmonyLib;
using UnityEngine;
using Kingmaker.EntitySystem.Entities;
using Kingmaker;
using Kingmaker.UnitLogic.Parts;


namespace SwallowedFix
{
#if (DEBUG)
    [EnableReloading]
#endif

    static class Main
    {
        public static UnityModManager.ModEntry ModEntry;
        public static UnityModManager.ModEntry.ModLogger logger;
        [System.Diagnostics.Conditional("DEBUG")]
        public static void Log(string msg)
        {
            if (logger != null) logger.Log(msg);
        }
        public static void Error(Exception ex)
        {
            if (logger != null) logger.Log(ex.ToString());
        }
        public static void Error(string msg)
        {
            if (logger != null) logger.Log(msg);
        }

        public static bool enabled;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            ModEntry = modEntry;
            logger = modEntry.Logger;
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
#if (DEBUG)
            modEntry.OnUnload = Unload;
            return true;
        }
        static bool Unload(UnityModManager.ModEntry modEntry)
        {
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.UnpatchAll(modEntry.Info.Id);
            return true;
        }
#else
            return true;
        }
#endif

        // Called when the mod is turned to on/off.
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value /* active or inactive */)
        {
            enabled = value;
            return true; // Permit or not.
        }
        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            try
            {
                if (!enabled) return;
                if (Game.Instance.Player.ControllableCharacters == null) return;

                if (GUILayout.Button("Unswallow"))
                {
                    GUILayout.BeginHorizontal();
                    foreach (UnitEntityData unitEntityData in Game.Instance.Player.PartyCharacters)
                    {
#if (DEBUG)
                        Log("Party character: " + unitEntityData.CharacterName);
#endif
                        UnitPartSwallowed unitPartSwallowed = (unitEntityData != null) ? unitEntityData.Get<UnitPartSwallowed>() : null;
                        if (unitPartSwallowed)
                        {
#if (DEBUG)
                            Log("Swallowed character: " + unitEntityData.CharacterName);
#endif
                            unitEntityData.Remove<Kingmaker.UnitLogic.Parts.UnitPartSwallowed>();
                        }
                    }
                    GUILayout.EndHorizontal();
                }

            }
            catch (Exception e)
            {
                Log(e.ToString() + " " + e.StackTrace);
            }

        }
    }

}
