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
    public class IncidentWorker_BigCarbuncleJoins : IncidentWorker
	{
		public override bool CanFireNowSub(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			if(FFAnimalsMod.settings.animalToggles["ERN_BigCarbuncle"])
			if (map.gameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout))
			{
				return false;
			}
			if (!map.mapTemperature.SeasonAndOutdoorTemperatureAcceptableFor(FFAnimalsDefOf.ERN_BigCarbuncle.race))
			{
				return false;
			}
			IntVec3 cell;
			return TryFindEntryCell(map, out cell);
		}

		public override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			if (!TryFindEntryCell(map, out var cell))
			{
				return false;
			}
			PawnKindDef carbuncle = FFAnimalsDefOf.ERN_BigCarbuncle;
			IntVec3 result = IntVec3.Invalid;
			if (!RCellFinder.TryFindRandomCellOutsideColonyNearTheCenterOfTheMap(cell, map, 10f, out result))
			{
				result = IntVec3.Invalid;
			}
			Pawn pawn = null;
			IntVec3 loc = CellFinder.RandomClosewalkCellNear(cell, map, 10);
			pawn = PawnGenerator.GeneratePawn(carbuncle);
			GenSpawn.Spawn(pawn, loc, map, Rot4.Random);
			pawn.SetFaction(Faction.OfPlayer);
			foreach (Thing item in ThingSetMakerDefOf.VisitorGift.root.Generate(new ThingSetMakerParams{totalMarketValueRange = new FloatRange(200f, 500f)}))
			{
				if (item is Pawn pawn2)
				{
					item.Destroy();
				}
				else if (!pawn.inventory.innerContainer.TryAdd(item))
				{
					item.Destroy();
				}
			}
			SendStandardLetter("ERN.LetterLabelForlornCarbuncle".Translate(carbuncle.label).CapitalizeFirst(), "ERN.LetterForlornCarbuncle".Translate(carbuncle.label), LetterDefOf.PositiveEvent, parms, pawn);
			return true;
		}

		public bool TryFindEntryCell(Map map, out IntVec3 cell)
		{
			return RCellFinder.TryFindRandomPawnEntryCell(out cell, map, CellFinder.EdgeRoadChance_Animal + 0.2f);
		}
	}
}
