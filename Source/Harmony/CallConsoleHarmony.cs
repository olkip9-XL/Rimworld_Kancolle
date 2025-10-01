using FixedPawnGenerate;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace FPG_Kancolle
{
    [HarmonyPatch(typeof(Building_CommsConsole), "GetFloatMenuOptions")]
    internal static class Building_CommsConsole_Patch
    {
        private static IEnumerable<FloatMenuOption> Postfix(IEnumerable<FloatMenuOption> __result, Building_CommsConsole __instance, Pawn myPawn)
        {
            List<FloatMenuOption> list = Enumerable.ToList<FloatMenuOption>(__result);
            foreach (FloatMenuOption floatMenuOption in __result)
            {
                yield return floatMenuOption;
            }
            if (Enumerable.Count<FloatMenuOption>(list) == 1 && list[0].action == null)
            {
                yield break;
            }

            //gacha ui
            FloatMenuOption option = new FloatMenuOption("KC_CallKancolle".Translate(), delegate ()
            {
                Job job = JobMaker.MakeJob(KC_JobDefOf.KC_CallKancolle, __instance);
                myPawn.jobs.TryTakeOrderedJob(job, new JobTag?(JobTag.Misc), false);
            }, MenuOptionPriority.InitiateSocial, null, null, 0f, null, null, true, 0);
            yield return FloatMenuUtility.DecoratePrioritizedTask(option, myPawn, __instance, "ReservedBy", null);

            yield break;
        }
    }
}
