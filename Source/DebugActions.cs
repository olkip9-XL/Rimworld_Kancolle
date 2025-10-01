using FixedPawnGenerate;
using LudeonTK;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace FPG_Kancolle
{
    public static class DebugActions
    {

        [DebugAction("Kancolle", "KC: Spawn All Characters", false, false, false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void SpawnAllCharacters()
        {
            List<FixedPawnDef> allDefs = DefDatabase<FixedPawnDef>.AllDefs.Where(x => x.tags.Contains("Kancolle")).ToList();

            List<Pawn> pawns = new List<Pawn>();
            foreach (FixedPawnDef def in allDefs)
            {
                Pawn pawn = FixedPawnUtility.GenerateFixedPawnWithDef(def);
                if (pawn.Faction != Faction.OfPlayer)
                {
                    pawn.SetFaction(Faction.OfPlayer);
                }
                pawns.Add(pawn);
            }

            PawnsArrivalModeDefOf.CenterDrop.Worker.Arrive(pawns, new IncidentParms
            {
                target = Find.CurrentMap,
                spawnCenter = DropCellFinder.TradeDropSpot(Find.CurrentMap)
            });
        }

        [DebugAction("Kancolle", "KC: Drop Now", false, false, false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]

        private static void DropNow()
        {
            GameComponent_Kancolle gc = Current.Game.GetComponent<GameComponent_Kancolle>();
            gc.DropAllNow();
        }

        [DebugAction("Kancolle", "KC: Trigger Hourlie", false, false, false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void TriggerHourlie()
        {
            int hour = System.DateTime.Now.Hour;
            Log.Warning($"[Kancolle] current hour is {hour}");

            //Pawn pawn = Find.CurrentMap?.PlayerPawnsForStoryteller.FirstOrDefault(x => x.HasComp<CompHourlies>());
            //if (pawn != null)
            //{
            //    CompHourlies comp = pawn.GetComp<CompHourlies>();
            //    if (comp != null)
            //    {
            //        comp.PlayHour(hour);
            //    }
            //}

            Mod_FPG_Kancolle.settings.currentHourlie?.PlayHour(hour);
        }

        //[DebugAction("Kancolle", "KC: Export Pawn Tex", false, false, false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        //private static void ExportPawnTex()
        //{
        //    List<FixedPawnDef> allDefs = DefDatabase<FixedPawnDef>.AllDefs.Where(x => x.tags.Contains("Kancolle")).ToList();

        //    FPGPawnTextureUtility.ExportPawnTexture(alldefs);
        //}

    }
}
