﻿// Copyright (c) 2022 bradson
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#if disabled
namespace PerformanceFish.Listers;

public class Things : ClassWithFishPatches
{
	public class ThingRequest_IsUndefined_Patch : FishPatch
	{
		public override MethodBase TargetMethodInfo => AccessTools.PropertyGetter(typeof(ThingRequest), nameof(ThingRequest.IsUndefined));
		[HarmonyPriority(Priority.Last)]
		public static void Postfix(ThingRequest __instance, ref bool __result)
		{
			if (__instance.singleDef is not FishCache)
				return;

			__result = NewResult(__instance);
		}
		public static bool NewResult(ThingRequest __instance)
			=> __instance.group == ThingRequestGroup.Undefined && !((FishCache)__instance.singleDef).IsSingleDef;
	}

	public class ThingRequest_CanBeFoundInRegion_Patch : FirstPriorityFishPatch
	{
		public override MethodBase TargetMethodInfo => AccessTools.PropertyGetter(typeof(ThingRequest), nameof(ThingRequest.CanBeFoundInRegion));
		public static bool Prefix(ThingRequest __instance, ref bool __result)
		{
			if (__instance.singleDef is not FishCache)
				return true;

			__result = NewResult(__instance);
			return false;
		}
		public static bool NewResult(ThingRequest __instance)
			=> ((FishCache)__instance.singleDef).IsSingleDef
			|| (__instance.group != ThingRequestGroup.Undefined && (__instance.group == ThingRequestGroup.Nothing || __instance.group.StoreInRegion()));
	}

	public class Add_Patch : FishPatch
	{
		public override Expression<Action> TargetMethod => () => new ListerThings(default).Add(null);
		public static void Postfix(ListerThings __instance, Thing t)
		{
			if (t.TryGetComp<CompStyleable>() is not { } style || style.SourcePrecept is null)
				return;
			Cache<ListerThings, CompStyleablesInfo>.GetValue(__instance).things.Add(t);
		}
	}

	public class Remove_Patch : FishPatch
	{
		public override Expression<Action> TargetMethod => () => new ListerThings(default).Remove(null);
		public static void Postfix(ListerThings __instance, Thing t)
		{
			if (t.TryGetComp<CompStyleable>() is not { } style || style.SourcePrecept is null)
				return;
			Cache<ListerThings, CompStyleablesInfo>.GetValue(__instance).things.Remove(t);
		}
	}

	public class Clear_Patch : FishPatch
	{
		public override Expression<Action> TargetMethod => () => new ListerThings(default).Clear();
		public static void Postfix(ListerThings __instance, Thing t)
		{
			if (t.TryGetComp<CompStyleable>() is not { } style || style.SourcePrecept is null)
				return;
			Cache<ListerThings, CompStyleablesInfo>.GetValue(__instance).things.Clear();
		}
	}

	public class ThingsMatching_Patch : FirstPriorityFishPatch
	{
		public override string Description => "Part of the work scanning optimization";
		public override Expression<Action> TargetMethod => () => new ListerThings(default).ThingsMatching(default);
		public static bool Prefix(ListerThings __instance, ThingRequest req, ref List<Thing> __result)
		{
			if (req.singleDef is not FishCache)
				return true;

			__result = GetCache(__instance, req);
			return false;
		}
		public static List<Thing> GetCache(ListerThings __instance, ThingRequest req)
		{
			var fishCache = (FishCache)req.singleDef;
			var cachedThings = fishCache.FilteredThings ?? ListerThings.EmptyList;
			/*if (__instance.use == ListerThingsUse.Region && cachedThings.Count > 20)
			{
				var regionalThings = GetForRegion(__instance, req);
				return regionalThings.Count < cachedThings.Count ? regionalThings : cachedThings;
			}
			else
			{
				return cachedThings;
			}*/
			return null;
		}
		/*public static List<Thing> GetForRegion(ListerThings __instance, ThingRequest req)
			=> new(TryIssueJobPackage_Patch.Generic<Thing>.TryGetPrefilteredPotentialWorkThingsForList(
				(((FishCache)req.singleDef) is var cache && cache.IsSingleDef
				? __instance.listsByDef.TryGetValue(cache.Def, out var value) ? value : null
				: __instance.listsByGroup.TryGetItem((int)req.group, out var item) ? item : null) ?? ListerThings.EmptyList, CurrentPawn));*/
	}

	public static CompStyleablesInfo GetCompStyleablesCache(ListerThings lister) => Cache<ListerThings, CompStyleablesInfo>.GetValue(lister);

	public struct CompStyleablesInfo : ICanRefresh<ListerThings, CompStyleablesInfo>
	{
		public List<Thing> things;
		private int _nextRefreshTick;

		public bool ShouldRefreshNow
		{
			get => _nextRefreshTick < Current.Game.tickManager.TicksGame;
			set => _nextRefreshTick = value ? 0 : Current.Game.tickManager.TicksGame + 3072 + Math.Abs(FishSettings.nextRandInt % 2048);
		}
		public CompStyleablesInfo SetNewValue(ListerThings key)
		{
			if (things is null)
				things = new();
			else
				things.Clear();
			var allBuildings = key.listsByGroup[(int)ThingRequestGroup.BuildingArtificial];
			var count = allBuildings.Count;
			for (var i = 0; i < count; i++)
			{
				if (allBuildings[i].TryGetComp<CompStyleable>() is { SourcePrecept: not null })
					things.Add(allBuildings[i]);
			}
			return this;
		}
	}

	public class FishCache : ThingDef
	{
		public bool IsSingleDef => Def != null;
		public ThingDef Def { get; private set; }
		public List<Thing> FilteredThings { get; set; }
		public static FishCache ForDef(ThingDef def)
		{
			if (_dictOfDefs.TryGetValue(def, out var value))
				return value;

			_dictOfDefs[def] = value = new(def.defName) { Def = def };
			return value;
		}
		public static FishCache ForGroup(ThingRequestGroup group)
		{
			if (_dictOfGroups.TryGetValue(group, out var value))
				return value;

			_dictOfGroups[group] = value = new(group.ToString());
			return value;
		}
		public static void ClearAll()
		{
			_dictOfGroups.Clear();
			_dictOfDefs.Clear();
		}
		private FishCache(string name) => defName = name;
		private static Dictionary<ThingRequestGroup, FishCache> _dictOfGroups { get; } = new();
		private static Dictionary<ThingDef, FishCache> _dictOfDefs { get; } = new();
	}
}
#endif