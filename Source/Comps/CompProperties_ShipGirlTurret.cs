using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FPG_Kancolle
{
    internal class CompProperties_ShipGirlTurret : CompProperties_TurretGun
    {
        public CompProperties_ShipGirlTurret()
        {
            this.compClass = typeof(CompShipGirlTurret);
        }

        public string turretIcon = null;
    }
}
