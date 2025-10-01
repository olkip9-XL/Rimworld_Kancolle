using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace FPG_Kancolle
{
    [StaticConstructorOnStartup]
    internal class HarmonyStartUp
    {
        static HarmonyStartUp()
        {
            new Harmony("LotusLand.FPGKancolle").PatchAll();
        }
    }
}
