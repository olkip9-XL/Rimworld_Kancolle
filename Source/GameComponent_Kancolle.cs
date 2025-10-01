using FixedPawnGenerate;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace FPG_Kancolle
{
    internal class GameComponent_Kancolle : GameComponent
    {
        public Game game;

        private List<PawnDropEvent> events = new List<PawnDropEvent>();
        public List<FixedPawnDef> RemainPawns
        {

            get
            {
                List<FixedPawnDef> list = DefDatabase<FixedPawnDef>.AllDefs.Where(x => x.tags.Contains("Kancolle")).Except(SpawnedPawns).ToList();

                list.RemoveAll(x => events.Any(y => y.def == x));

                return list;
            }

        }


        List<FixedPawnDef> SpawnedPawns => FixedPawnUtility.SpawnedPawnWithTag("Kancolle").ToList();

        private int lastHour = -1;


        public GameComponent_Kancolle(Game game) : base()
        {
            this.game = game;
        }

        internal bool IsSpawned(FixedPawnDef def)
        {
            return SpawnedPawns.Contains(def);
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();

            //drop check
            if (Find.TickManager.TicksGame % 250 == 0)
            {
                foreach (var e in events)
                {
                    e.TryTrigger();
                }
                events.RemoveAll(x => x.triggerTick < Find.TickManager.TicksGame);
            }

            //报时
            if (Find.TickManager.TicksGame % 120 == 0)
            {
                TriggerHourlie();
            }
        }

        private void TriggerHourlie()
        {
            int hour = System.DateTime.Now.Hour;
            int minute = System.DateTime.Now.Minute;

            //有无敌人
            if (Find.CurrentMap != null && Find.CurrentMap.mapPawns.AllHumanlikeSpawned.Any(x => x.Faction.HostileTo(Faction.OfPlayer)))
            {
                return;
            }

            if (hour == lastHour)
            {
                return;
            }

            if (minute > 3 && minute < 57) // 误差
            {
                return;
            }

            lastHour = hour;
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

        public void DropAllNow()
        {
            foreach (var e in events)
            {
                e.TryTrigger(true);
            }
            events.Clear();
        }


        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref events, "events", LookMode.Deep);
        }

        public void AddDropEvent(FixedPawnDef def, Map map, int afterTicks = 2500)
        {
            if (def == null || map == null)
            {
                return;
            }

            PawnDropEvent e = new PawnDropEvent
            {
                def = def,
                dropMap = map,
                triggerTick = Find.TickManager.TicksGame + afterTicks
            };
            events.Add(e);
        }


        class PawnDropEvent : IExposable
        {
            public long triggerTick = -1;
            public FixedPawnDef def = null;
            public Map dropMap = null;

            public void ExposeData()
            {
                Scribe_Values.Look(ref triggerTick, "triggerTick", -1);
                Scribe_Defs.Look(ref def, "def");
                Scribe_References.Look(ref dropMap, "dropMap");
            }

            public void TryTrigger(bool dropNow = false)
            {
                if (Find.TickManager.TicksGame < triggerTick && !dropNow)
                {
                    return;
                }

                if (def != null && dropMap != null)
                {
                    //generate pawn
                    //def.faction = Find.FactionManager.OfPlayer.def;
                    Pawn pawn = FixedPawnUtility.GenerateFixedPawnWithDef(def);
                    if (pawn.Faction != Find.FactionManager.OfPlayer)
                    {
                        Log.Error("[Kancolle] Faction is not player, Set to player");
                        pawn.SetFaction(Find.FactionManager.OfPlayer);
                    }

                    //drop
                    PawnsArrivalModeDefOf.CenterDrop.Worker.Arrive(new List<Pawn>
                    {
                        pawn
                    }, new IncidentParms
                    {
                        target = dropMap,
                        spawnCenter = DropCellFinder.TradeDropSpot(dropMap)
                    });

                    //message
                    //通知
                    Messages.Message("KC_PawnArrived".Translate(pawn.Name.ToStringShort), new LookTargets(pawn.Position, dropMap), MessageTypeDefOf.PositiveEvent);

                    ChoiceLetter let = LetterMaker.MakeLetter("KC_PawnArriveLable".Translate(), "KC_PawnArrived".Translate(pawn.Name.ToStringShort), LetterDefOf.PositiveEvent, pawn, null, null, null);
                    Find.LetterStack.ReceiveLetter(let, null, 0, true);
                }
            }
        }

    }


}
