using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using HarmonyLib;
using System.Reflection.Emit;

namespace FFAnimals
{
    [HarmonyPatch(typeof(WildAnimalSpawner), "SpawnRandomWildAnimalAt")]
    public static class Patch_WildAnimalSpawner_SpawnRandomWildAnimalAt
    {
        [HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
		{
			List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
			Label label = ilg.DefineLabel();
			int i = 0;
			foreach (CodeInstruction item in codes)
			{
				if (item.opcode == OpCodes.Stloc_0)
				{
					codes[i + 1].labels.Add(label);
					yield return new CodeInstruction(OpCodes.Stloc_0, (object)null);
					yield return new CodeInstruction(OpCodes.Ldloc_0, (object)null);
					yield return new CodeInstruction(OpCodes.Call, (object)typeof(Patch_WildAnimalSpawner_SpawnRandomWildAnimalAt).GetMethod("DetectFFAnimals"));
					yield return new CodeInstruction(OpCodes.Brfalse, (object)label);
					yield return new CodeInstruction(OpCodes.Ret, (object)null);
				}
				else
				{
					yield return item;
				}
				i++;
			}
		}

		public static bool DetectFFAnimals(PawnKindDef animal)
        {
			if(animal != null)
            {
                if (FFAnimalsMod.settings.animalToggles.ContainsKey(animal.defName))
                {
					return FFAnimalsMod.settings.animalToggles[animal.defName];
				}
            }
			return false;
        }
	}
}
