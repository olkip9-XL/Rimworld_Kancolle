using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace FPG_Kancolle
{
    internal class CompShipGirlTurret : CompTurretGun
    {
        public new CompProperties_ShipGirlTurret Props => (CompProperties_ShipGirlTurret)props;

        private bool fireAtWill = true;


        private Texture2D uiIconInt = null;
        public Texture2D UIIcon
        {
            get
            {
                if (uiIconInt == null && Props.turretIcon != null)
                {
                    uiIconInt = ContentFinder<Texture2D>.Get(Props.turretIcon, true);
                }
                return uiIconInt;
            }
        }


        private bool CanShoot
        {
            get
            {
                if (parent is Pawn pawn)
                {
                    if (!pawn.Spawned || pawn.Downed || pawn.Dead || !pawn.Awake())
                    {
                        return false;
                    }

                    if (pawn.stances.stunner.Stunned)
                    {
                        return false;
                    }

                    if (TurretDestroyed)
                    {
                        return false;
                    }

                    if (pawn.IsPlayerControlled && !fireAtWill)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public override void CompTick()
        {
            if (!CanShoot)
            {
                return;
            }

            base.CompTick();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo item in base.CompGetGizmosExtra())
            {
                yield return item;
            }

            if (parent is Pawn pawn && pawn.Faction == Faction.OfPlayer)
            {
                Command_Toggle command_Toggle = new Command_Toggle();
                command_Toggle.defaultLabel = "KC_ToggleTurret".Translate();
                command_Toggle.defaultDesc = "KC_ToggleTurretDesc".Translate();
                command_Toggle.isActive = () => fireAtWill;
                command_Toggle.icon = UIIcon;
                command_Toggle.toggleAction = delegate
                {
                    fireAtWill = !fireAtWill;
                };
                yield return command_Toggle;
            }
        }

    }
}
