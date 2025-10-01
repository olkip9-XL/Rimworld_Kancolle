using FixedPawnGenerate;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace FPG_Kancolle
{
    internal class JobDriver_CallKancolle : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed, false);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.InteractionCell).FailOn((Toil to) => !((Building_CommsConsole)to.actor.jobs.curJob.GetTarget(TargetIndex.A).Thing).CanUseCommsNow);
            Toil openComms = new Toil();
            openComms.initAction = delegate ()
            {
                if (((Building_CommsConsole)openComms.actor.jobs.curJob.GetTarget(TargetIndex.A).Thing).CanUseCommsNow)
                {
                    Find.WindowStack.Add(new Dialog_GachaInterface(this.pawn));
                }
            };
            yield return openComms;
            yield break;
        }
    }
}
