using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace FFAnimals
{
    public class FFAnimalsMod : Mod
    {
        public static FFAnimalsMod mod;
        public static FFAnimalsSettings settings;

        public Vector2 optionsScrollPosition;
        public float optionsViewRectHeight;

        internal static string VersionDir => Path.Combine(ModLister.GetActiveModWithIdentifier("Erin.FFAnimals").RootDir.FullName, "Version.txt");
        public static string CurrentVersion { get; private set; }

        public FFAnimalsMod(ModContentPack content) : base(content)
        {
            mod = this;
            settings = GetSettings<FFAnimalsSettings>();

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            CurrentVersion = $"{version.Major}.{version.Minor}.{version.Build}";

            LogUtil.LogMessage($"{CurrentVersion} ::");

            if (Prefs.DevMode)
            {
                File.WriteAllText(VersionDir, CurrentVersion);
            }

            Harmony OuterRimHarmony = new Harmony("Erin.FFAnimals.RimWorld");
            OuterRimHarmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string SettingsCategory() => "Erin's FF Animals";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            bool flag = optionsViewRectHeight > inRect.height;
            Rect viewRect = new Rect(inRect.x, inRect.y, inRect.width - (flag ? 26f : 0f), optionsViewRectHeight);
            Widgets.BeginScrollView(inRect, ref optionsScrollPosition, viewRect);
            Listing_Standard listing = new Listing_Standard();
            Rect rect = new Rect(viewRect.x, viewRect.y, viewRect.width, 999999f);
            listing.Begin(rect);
            // ============================ CONTENTS ================================
            DoOptionsCategoryContents(listing);
            // ======================================================================
            optionsViewRectHeight = listing.CurHeight;
            listing.End();
            Widgets.EndScrollView();
        }

        public void DoOptionsCategoryContents(Listing_Standard listing)
        {
            List<ThingDef> allAnimals = FFAnimalsStartup.GetAllModAnimals();

            listing.Label("Spawning");
            listing.Note("Enables whether or not the animal will spawn (also controls if events related to them can fire)", GameFont.Tiny);
            listing.GapLine();
            for (int i = 0; i < settings.animalToggles.Count(); i++)
            {
                string curKey = settings.animalToggles.ElementAt(i).Key;
                bool tempBool = settings.animalToggles[curKey];
                listing.CheckboxLabeled(allAnimals.Find(t => t.defName == curKey).LabelCap, ref tempBool);
                settings.animalToggles[curKey] = tempBool;

            }
            listing.GapLine();
            listing.Label("Trainability");
            listing.Note("Changes what behaviours the animal can use, whether they can haul, or be released to attack, etc.", GameFont.Tiny);
            listing.GapLine();
            for (int i = 0; i < settings.animalTraining.Count(); i++)
            {
                string curKey = settings.animalTraining.ElementAt(i).Key;
                TrainabilityEnum tempEnum = settings.animalTraining[curKey];
                listing.ValueLabeled(allAnimals.Find(t => t.defName == curKey).LabelCap, "", ref tempEnum);
                settings.animalTraining[curKey] = tempEnum;
            }
            FFAnimalsStartup.ApplySettings();
        }
    }
}
