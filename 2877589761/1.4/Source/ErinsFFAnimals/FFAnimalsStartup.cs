using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace FFAnimals
{
    [StaticConstructorOnStartup]
    public static class FFAnimalsStartup
    {
        static FFAnimalsStartup()
        {
            CheckSettingsSetup();

            ApplySettings();
        }

        public static void CheckSettingsSetup()
        {
            List<ThingDef> allModAnimals = FFAnimalsStartup.GetAllModAnimals();

            foreach (ThingDef def in allModAnimals)
            {
                if (FFAnimalsMod.settings.animalToggles.NullOrEmpty())
                {
                    FFAnimalsMod.settings.animalToggles = new Dictionary<string, bool>();
                }
                if (!FFAnimalsMod.settings.animalToggles.ContainsKey(def.defName))
                {
                    FFAnimalsMod.settings.animalToggles.Add(def.defName, true);
                }
                if (FFAnimalsMod.settings.animalTraining.NullOrEmpty())
                {
                    FFAnimalsMod.settings.animalTraining = new Dictionary<string, TrainabilityEnum>();
                }
                if (FFAnimalsMod.settings.animalTraining.NullOrEmpty() || !FFAnimalsMod.settings.animalTraining.ContainsKey(def.defName))
                {
                    TrainabilityEnum trainability = TrainabilityEnum.None;
                    switch (def.race.trainability.defName)
                    {
                        case "Intermediate":
                            trainability = TrainabilityEnum.Intermediate;
                            break;
                        case "Advanced":
                            trainability = TrainabilityEnum.Advanced;
                            break;
                        default:
                            break;

                    }
                    FFAnimalsMod.settings.animalTraining.Add(def.defName, trainability);
                }
            }
        }

        public static List<ThingDef> GetAllModAnimals()
        {
            List<PawnKindDef> pawnKinds = DefDatabase<PawnKindDef>.AllDefs.Where(t => t.modContentPack == FFAnimalsMod.mod.Content).ToList();
            List<ThingDef> results = new List<ThingDef>();

            foreach (PawnKindDef pkd in pawnKinds)
            {
                if (!results.Contains(pkd.race))
                {
                    results.Add(pkd.race);
                }
            }

            return results;
        }

        public static void ApplySettings()
        {
            List<ThingDef> allModAnimals = GetAllModAnimals();

            foreach (ThingDef def in allModAnimals)
            {
                if (FFAnimalsMod.settings.animalTraining.ContainsKey(def.defName))
                {
                    TrainabilityEnum trainability = FFAnimalsMod.settings.animalTraining[def.defName];
                    switch (trainability)
                    {
                        case TrainabilityEnum.Intermediate:
                            def.race.trainability = TrainabilityDefOf.Intermediate;
                            LogUtil.LogWarning($"Setting {def.defName} trainability to {def.race.trainability}");
                            break;
                        case TrainabilityEnum.Advanced:
                            def.race.trainability = TrainabilityDefOf.Advanced;
                            LogUtil.LogWarning($"Setting {def.defName} trainability to {def.race.trainability}");
                            break;
                        default:
                            def.race.trainability = TrainabilityDefOf.None;
                            LogUtil.LogWarning($"Setting {def.defName} trainability to {def.race.trainability}");
                            break;
                    }
                }
            }
        }
    }
}
