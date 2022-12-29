using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace ERN_Chocobos
{
    public class ChocoboSettings : ModSettings
    {
        public bool enableAdvancedTrainability;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref enableAdvancedTrainability, "enableAdvancedTrainability");
            base.ExposeData();
        }
    }

    public class ErinsChocobos : Mod
    {
        public static ChocoboSettings settings;

        public ErinsChocobos(ModContentPack content) : base(content)
        {
            settings = GetSettings<ChocoboSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.CheckboxLabeled("Enable Advanced Trainability", ref settings.enableAdvancedTrainability, "When enabled, sets chocobo trainability to Advanced so they can haul.");
            listing.End();
            base.DoSettingsWindowContents(inRect);
        }

        // Allows real-time updating of chocobo trainability when changing the option in the settings menu.
        public override void WriteSettings()
        {
            base.WriteSettings();
            OnDefsLoaded.ApplySettings();
        }

        public override string SettingsCategory()
        {
            return "Erin's Chocobos";
        }
    }

    [StaticConstructorOnStartup]
    public class OnDefsLoaded
    {
        static OnDefsLoaded()
        {
            ApplySettings();
        }

        // Applies the saved trainability settings on startup, after the defs have been loaded.
        public static void ApplySettings()
        {
            DefDatabase<ThingDef>.GetNamed("ERN_Chocobo").race.trainability = ErinsChocobos.settings.enableAdvancedTrainability ? TrainabilityDefOf.Advanced : TrainabilityDefOf.Intermediate;
        }
    }
}
