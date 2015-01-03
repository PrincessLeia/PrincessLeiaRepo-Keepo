using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace Princess_LeBlanc
{
    internal class DrawingHandler
    {
        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        public static void Init()
        {
            Drawing.OnDraw += OnDraw;
        }
        private static void OnDraw(EventArgs args)
        {

            if (ObjectManager.Player.IsDead)
                return;

            var DrawQ = MenuHandler.LeBlancConfig.Item("drawQ").GetValue<Circle>();
            if (DrawQ.Active)
            {
                Utility.DrawCircle(Player.Position, SkillHandler.Q.Range, DrawQ.Color);
            }

            var DrawW = MenuHandler.LeBlancConfig.Item("drawW").GetValue<Circle>();
            if (DrawW.Active)
            {
                Utility.DrawCircle(Player.Position, SkillHandler.W.Range, DrawW.Color);
            }

            var DrawE = MenuHandler.LeBlancConfig.Item("drawE").GetValue<Circle>();
            if (DrawE.Active)
            {
                Utility.DrawCircle(Player.Position, SkillHandler.E.Range, DrawE.Color);
            }

            var DrawR = MenuHandler.LeBlancConfig.Item("drawR").GetValue<Circle>();
            if (DrawR.Active)
            {
                Utility.DrawCircle(Player.Position, SkillHandler.R.Range, DrawR.Color);
            }
            var vec = new Vector3(FightHandler.WPosition[0], FightHandler.WPosition[1], FightHandler.WPosition[2]);

            Utility.DrawCircle(vec, 100, Color.Red);


            if (MenuHandler.LeBlancConfig.Item("HUD").GetValue<bool>())
            {
                if (MenuHandler.LeBlancConfig.Item("KSi").GetValue<KeyBind>().Active || MenuHandler.LeBlancConfig.Item("KSq").GetValue<KeyBind>().Active || MenuHandler.LeBlancConfig.Item("KSw").GetValue<KeyBind>().Active)
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.68f, Color.Yellow,
                        "KillSteal : On");
                else
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.68f, Color.DarkRed,
                        "KillSteal : Off");

                /*if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "leblancslidereturn" || Player.Spellbook.GetSpell(SpellSlot.W).Name == "leblancslidereturnm")
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.66f, Color.Yellow,
                        "W Timer : " + timer);
                else
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.66f, Color.DarkRed,
                        "W Timer : Off");*/
            }
        }
    }
}
