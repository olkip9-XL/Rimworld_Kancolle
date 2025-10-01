using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using FixedPawnGenerate;
using HarmonyLib;
using RimWorld;
using System.Reflection;
using System.Runtime;

namespace FPG_Kancolle
{

    public class ModSetting_FPG_Kancolle : ModSettings
    {
        //字段
        private string currentHourlieStr = "KC_Hourlie_Fubuki";
        public HourlieDef currentHourlie = KC_HourlieDefOf.KC_Hourlie_Fubuki;

        public void Reset()
        {
            currentHourlie = KC_HourlieDefOf.KC_Hourlie_Fubuki;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                currentHourlieStr = currentHourlie?.defName ?? "null";
            }

            Scribe_Values.Look<string>(ref currentHourlieStr, "currentHourlie", "KC_Hourlie_Fubuki");
        }

        public void ResolveDefs()
        {
            if (currentHourlie == null && currentHourlieStr != null)
            {
                currentHourlie = DefDatabase<HourlieDef>.GetNamedSilentFail(currentHourlieStr) ?? KC_HourlieDefOf.KC_Hourlie_Fubuki;
            }
        }
    }

    public class Mod_FPG_Kancolle : Mod
    {
        public static ModSetting_FPG_Kancolle settings { get; private set; }

        public Mod_FPG_Kancolle(ModContentPack content) : base(content)
        {
            settings = GetSettings<ModSetting_FPG_Kancolle>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            inRect = inRect.ContractedBy(inRect.width * 0.1f, 0f);
            listingStandard.Begin(inRect);

            if (ButtonTextLine(listingStandard, "KC_Hourlie".Translate(), settings.currentHourlie?.label ?? "KC_Null".Translate()))
            {
                List<HourlieDef> list = new List<HourlieDef>();
                list.Add(null);
                list.AddRange(DefDatabase<HourlieDef>.AllDefs.ToList());

                FloatMenuUtility.MakeMenu<HourlieDef>(list,
                    (HourlieDef x) => x?.label ?? "KC_Null".Translate(),
                    (HourlieDef x) => delegate
                    {
                        settings.currentHourlie = x;
                    });
            }

            listingStandard.End();
        }

        private bool ButtonTextLine(Listing_Standard listing, string label, string buttonText)
        {
            Rect rect = listing.GetRect(Text.LineHeight);

            Widgets.Label(rect, label);

            float buttonWidth = 100f;

            Rect buttonRect = rect.RightPartPixels(buttonWidth);

            return Widgets.ButtonText(buttonRect, buttonText);
        }

        public override string SettingsCategory()
        {
            return "Characters - Kancolle";
        }
    }
}
