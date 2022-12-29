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
    public class FFAnimalsSettings : ModSettings
    {
        public Dictionary<string, bool> animalToggles = new Dictionary<string, bool>();

        public Dictionary<string, TrainabilityEnum> animalTraining = new Dictionary<string, TrainabilityEnum>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref animalToggles, "animalToggles");
            Scribe_Collections.Look(ref animalTraining, "animalTraining");
        }
    }

    public enum TrainabilityEnum
    {
        None,
        Intermediate,
        Advanced
    }
}
