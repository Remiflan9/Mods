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
    [DefOf]
    public static class FFAnimalsDefOf
    {
        static FFAnimalsDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(FFAnimalsDefOf));
        }

        public static PawnKindDef ERN_BigCarbuncle;
    }
}
