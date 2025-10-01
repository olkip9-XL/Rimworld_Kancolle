using FixedPawnGenerate;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace FPG_Kancolle
{
    internal class Dialog_GachaInterface : Window
    {


        private List<int> gachaMatCounts = new List<int>() { 30, 30, 30, 30 };

        private List<Texture2D> numberTex;

        private Pawn caller;

        private GameComponent_Kancolle GameComp => Current.Game.GetComponent<GameComponent_Kancolle>();

        public Dialog_GachaInterface(Pawn caller)
        {
            numberTex = new List<Texture2D>()
            {
                ContentFinder<Texture2D>.Get("UI/KC_Numbers/0"),
                ContentFinder<Texture2D>.Get("UI/KC_Numbers/1"),
                ContentFinder<Texture2D>.Get("UI/KC_Numbers/2"),
                ContentFinder<Texture2D>.Get("UI/KC_Numbers/3"),
                ContentFinder<Texture2D>.Get("UI/KC_Numbers/4"),
                ContentFinder<Texture2D>.Get("UI/KC_Numbers/5"),
                ContentFinder<Texture2D>.Get("UI/KC_Numbers/6"),
                ContentFinder<Texture2D>.Get("UI/KC_Numbers/7"),
                ContentFinder<Texture2D>.Get("UI/KC_Numbers/8"),
                ContentFinder<Texture2D>.Get("UI/KC_Numbers/9"),
            };
            this.caller = caller;
        }

        public override void PreOpen()
        {
            this.windowRect = new Rect((UI.screenWidth - this.InitialSize.x) / 2f, (UI.screenHeight - this.InitialSize.y) / 2f, this.InitialSize.x, this.InitialSize.y).Rounded();
            this.draggable = true;
            this.forcePause = true;
            this.doCloseX = true;
        }

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(768, 432);
            }
        }

        public override void OnAcceptKeyPressed()
        {
            base.OnAcceptKeyPressed();
        }

        public override void DoWindowContents(Rect inRect)
        {

            Rect matRect = new Rect(inRect.x, inRect.y, inRect.width, Text.LineHeight);
            Rect buttonRect = new Rect(inRect.x, inRect.y + inRect.height - 30f, inRect.width, 30f);
            Rect contentRect = new Rect(inRect.x, inRect.y + Text.LineHeight + 10f, inRect.width, inRect.height - matRect.height - buttonRect.height - 10f);

            DrawMaterialInfo(matRect);
            DrawGachaContent(contentRect);
            DrawButton(buttonRect);
        }

        void DrawMaterialInfo(Rect rect)
        {
            float subRectWidth = rect.width / 5f;

            List<ThingDef> resourcesDefs = new List<ThingDef>()
            {
                ThingDef.Named("Chemfuel"),
                ThingDef.Named("Silver"),
                ThingDef.Named("Steel"),
                ThingDef.Named("Plasteel"),
                ThingDef.Named("KC_DevMat"),
            };

            float curX = rect.x;
            for (int i = 0; i < 5; i++)
            {
                Rect subRect = new Rect(curX, rect.y, subRectWidth, rect.height);
                curX += subRectWidth;

                int curAmount = TradeUtility.AllLaunchableThingsForTrade(Find.CurrentMap).Where(x => x.def == resourcesDefs[i]).Sum(x => x.stackCount);

                Rect iconRect = new Rect(subRect.x, subRect.y, subRect.height, subRect.height);
                Widgets.DrawTextureFitted(iconRect, resourcesDefs[i].uiIcon, 1f);

                Rect labelRect = new Rect(subRect.x + subRect.height + 5f, subRect.y, subRect.width - subRect.height - 5f, subRect.height);
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(labelRect, curAmount.ToString());
                Text.Anchor = TextAnchor.UpperLeft;
            }
        }

        void DrawGachaContent(Rect rect)
        {
            rect = rect.ContractedBy(10f);

            List<ThingDef> resourcesDefs = new List<ThingDef>()
            {
                ThingDef.Named("Chemfuel"),
                ThingDef.Named("Silver"),
                ThingDef.Named("Steel"),
                ThingDef.Named("Plasteel")
            };

            for (int i = 0; i < 4; i++)
            {
                //calculate subRect
                Rect subRect = new Rect(0, 0, rect.width / 2f, rect.height / 2f);
                switch (i)
                {
                    case 0:
                        subRect.x = rect.x;
                        subRect.y = rect.y;
                        break;
                    case 1:
                        subRect.x = rect.x;
                        subRect.y = rect.y + rect.height / 2f;
                        break;
                    case 2:
                        subRect.x = rect.x + rect.width / 2f;
                        subRect.y = rect.y;
                        break;
                    case 3:
                        subRect.x = rect.x + rect.width / 2f;
                        subRect.y = rect.y + rect.height / 2f;
                        break;
                    default:
                        break;
                }

                subRect = subRect.ContractedBy(10f);
                Widgets.DrawBoxSolidWithOutline(subRect, Color.clear, new Color(0.2f, 0.2f, 0.2f), 2);
                subRect = subRect.ContractedBy(10f);

                //draw subRect
                Text.Font = GameFont.Medium;
                Rect leftPart = subRect.LeftPart(0.6f);
                DrawLeft(leftPart, i);

                Rect rightPart = subRect.RightPart(0.4f);
                DrawRight(rightPart, i);
                Text.Font = GameFont.Small;
            }

            void DrawLeft(Rect leftRect, int resIndex)
            {
                string label = resourcesDefs[resIndex].label;
                float TextHeight = Text.CalcHeight(label, leftRect.width);

                Rect iconRect = new Rect(leftRect.x, leftRect.y, TextHeight, TextHeight);
                Widgets.DrawTextureFitted(iconRect, resourcesDefs[resIndex].uiIcon, 1f);

                Rect labelRect = new Rect(leftRect.x + TextHeight + 5f, leftRect.y, leftRect.width - TextHeight - 5f, TextHeight);
                Widgets.Label(labelRect, label);

                //draw number
                int count = gachaMatCounts[resIndex];
                string countStr = count.ToString().PadLeft(3, '0');

                Rect numberRect = new Rect(0, 0, leftRect.width / 2f, leftRect.width / 2f);
                numberRect = numberRect.CenteredOnXIn(leftRect);
                numberRect.y = leftRect.y + leftRect.height - numberRect.height;

                float singleWidth = numberRect.width / 3f;
                Widgets.DrawTextureFitted(new Rect(numberRect.x, numberRect.y, singleWidth, numberRect.height), numberTex[int.Parse(countStr[0].ToString())], 1f);
                Widgets.DrawTextureFitted(new Rect(numberRect.x + singleWidth, numberRect.y, singleWidth, numberRect.height), numberTex[int.Parse(countStr[1].ToString())], 1f);
                Widgets.DrawTextureFitted(new Rect(numberRect.x + singleWidth * 2f, numberRect.y, singleWidth, numberRect.height), numberTex[int.Parse(countStr[2].ToString())], 1f);

                //add 1
                float buttonSize = (leftRect.height / 4f) - 10f;

                Rect buttonRect = new Rect(0, 0, buttonSize, buttonSize);
                buttonRect = buttonRect.CenteredOnXIn(numberRect);
                buttonRect.y = numberRect.y;
                if (Widgets.ButtonImage(buttonRect, TexButton.Plus))
                {
                    gachaMatCounts[resIndex] = Math.Min(999, gachaMatCounts[resIndex] + 1);
                }

                buttonRect.y += numberRect.height - buttonRect.height;
                if (Widgets.ButtonImage(buttonRect, TexButton.Minus))
                {
                    gachaMatCounts[resIndex] = Math.Max(30, gachaMatCounts[resIndex] - 1);
                }

            }

            void DrawRight(Rect rightRect, int resIndex)
            {
                Text.Anchor = TextAnchor.MiddleCenter;

                float curY = rightRect.y;
                float subHeight = rightRect.height / 4f;

                Rect subRect = new Rect(rightRect.x, curY, rightRect.width, subHeight);

                //draw plus 10 button
                Rect innerRect = subRect.ContractedBy(5f);
                if (Widgets.ButtonImage(innerRect.LeftPartPixels(innerRect.height), TexButton.Minus))
                {
                    gachaMatCounts[resIndex] = Math.Max(30, gachaMatCounts[resIndex] - 10);
                }

                if (Widgets.ButtonImage(innerRect.RightPartPixels(innerRect.height), TexButton.Plus))
                {
                    gachaMatCounts[resIndex] = Math.Min(999, gachaMatCounts[resIndex] + 10);
                }
                Rect numberRect = new Rect(innerRect.x + innerRect.height + 5f, innerRect.y, innerRect.width - innerRect.height * 2f - 10f, innerRect.height);
                Widgets.Label(numberRect, "10");

                //100
                subRect.y += subHeight;
                innerRect = subRect.ContractedBy(5f);
                if (Widgets.ButtonImage(innerRect.LeftPartPixels(innerRect.height), TexButton.Minus))
                {
                    gachaMatCounts[resIndex] = Math.Max(30, gachaMatCounts[resIndex] - 100);
                }

                if (Widgets.ButtonImage(innerRect.RightPartPixels(innerRect.height), TexButton.Plus))
                {
                    gachaMatCounts[resIndex] = Math.Min(999, gachaMatCounts[resIndex] + 100);
                }
                numberRect = new Rect(innerRect.x + innerRect.height + 5f, innerRect.y, innerRect.width - innerRect.height * 2f - 10f, innerRect.height);
                Widgets.Label(numberRect, "100");

                //max
                subRect.y += subHeight;
                innerRect = subRect.ContractedBy(5f);
                if (Widgets.ButtonText(innerRect, "Max"))
                {
                    int curAmount = TradeUtility.AllLaunchableThingsForTrade(Find.CurrentMap).Where(x => x.def == resourcesDefs[resIndex]).Sum(x => x.stackCount);

                    gachaMatCounts[resIndex] = Mathf.Clamp(curAmount, 30, 999);
                }

                //reset
                subRect.y += subHeight;
                innerRect = subRect.ContractedBy(5f);
                if (Widgets.ButtonText(innerRect, "Reset"))
                {
                    gachaMatCounts[resIndex] = 30;
                }

                Text.Anchor = TextAnchor.UpperLeft;
            }

        }

        void DrawButton(Rect rect)
        {
            Rect buttonRect = new Rect(0, 0, 100, 30);
            buttonRect = buttonRect.CenteredOnXIn(rect);
            buttonRect = buttonRect.CenteredOnYIn(rect);

            if (Widgets.ButtonText(buttonRect, "KC_StartBuilding".Translate()))
            {
                if (DoGacha())
                {
                    this.Close();
                }
            }
        }

        bool DoGacha()
        {
            //check if enough mat
            List<ThingDef> resourcesDefs = new List<ThingDef>()
                {
                    ThingDef.Named("Chemfuel"),
                    ThingDef.Named("Silver"),
                    ThingDef.Named("Steel"),
                    ThingDef.Named("Plasteel"),
                };

            bool enough = true;

            int devMatCount = TradeUtility.AllLaunchableThingsForTrade(Find.CurrentMap).Where(x => x.def == ThingDef.Named("KC_DevMat")).Sum(x => x.stackCount);
            if (devMatCount < 1)
            {
                enough = false;
            }

            for (int i = 0; i < 4; i++)
            {
                int curAmount = TradeUtility.AllLaunchableThingsForTrade(Find.CurrentMap).Where(x => x.def == resourcesDefs[i]).Sum(x => x.stackCount);
                if (curAmount < gachaMatCounts[i])
                {
                    enough = false;
                    break;
                }
            }

            if (!enough)
            {
                Messages.Message("KC_NotEnoughMat".Translate(), MessageTypeDefOf.RejectInput);
                return false;
            }

            //take mat
            for (int i = 0; i < 4; i++)
            {
                int needCount = gachaMatCounts[i];

                TradeUtility.LaunchThingsOfType(resourcesDefs[i], gachaMatCounts[i], Find.CurrentMap, null);
            }
            TradeUtility.LaunchThingsOfType(ThingDef.Named("KC_DevMat"), 1, Find.CurrentMap, null);

            //Do gacha
            FixedPawnDef resultDef = GetGachaResult();
            if (resultDef != null)
            {
                try
                {
                    GameComp.AddDropEvent(resultDef, Find.CurrentMap, UnityEngine.Random.Range(60000 * 2, 60000 * 3));

                    Messages.Message("KC_GachaSuccess".Translate(resultDef.nickName), MessageTypeDefOf.PositiveEvent);
                }
                catch (Exception e)
                {
                    Log.Error($"[Kancolle] Exception when gacha: {e}");
                }
                return true;
            }
            else
            {
                Messages.Message("KC_NoMorePawns".Translate(), MessageTypeDefOf.NegativeEvent);
                return true;
            }
        }

        FixedPawnDef GetGachaResult()
        {
            if(GameComp.RemainPawns.Count == 0)
            {
                return null;
            }

            //exact match
            string characterCode = $"{gachaMatCounts[0]:D3};{gachaMatCounts[1]:D3};{gachaMatCounts[2]:D3};{gachaMatCounts[3]:D3}";
            FixedPawnDef fixedPawnDef = GameComp.RemainPawns.Where(x => x.tags.Contains(characterCode)).FirstOrDefault();
            if (fixedPawnDef != null)
            {
                return fixedPawnDef;
            }

            //sibling
            FixedPawnDef callerDef = caller.GetFixedPawnDef();
            if (callerDef != null)
            {
                List<FixedPawnDef> siblings = GameComp.RemainPawns.Where(x => x.tags.Intersect(callerDef.tags).Count() > 2).ToList();
                if (siblings.Any() && UnityEngine.Random.Range(0f, 1f) < 0.5f)
                {
                    fixedPawnDef = siblings.RandomElementByWeight(x => (float)(x.generateWeight));
                    if (fixedPawnDef != null)
                    {
                        return fixedPawnDef;
                    }
                }
            }

            //standard gacha
            fixedPawnDef = GameComp.RemainPawns.RandomElementByWeight(x => (float)(x.generateWeight));
            if (fixedPawnDef != null)
            {
                return fixedPawnDef;
            }

            return null;
        }


    }
}
