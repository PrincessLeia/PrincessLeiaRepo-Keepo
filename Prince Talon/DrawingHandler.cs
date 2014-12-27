using System;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace PrinceTalon
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
            var DrawW = MenuHandler.TalonConfig.SubMenu("Drawing").Item("drawW").GetValue<Circle>();
            var DrawE = MenuHandler.TalonConfig.SubMenu("Drawing").Item("drawE").GetValue<Circle>();
            var DrawR = MenuHandler.TalonConfig.SubMenu("Drawing").Item("drawR").GetValue<Circle>();

            if (DrawW.Active)
            {
                Utility.DrawCircle(Player.Position, SkillHandler.W.Range, DrawW.Color);
            }

            if (DrawE.Active)
            {
                Utility.DrawCircle(Player.Position, SkillHandler.E.Range, DrawE.Color);
            }

            if (DrawR.Active)
            {
                Utility.DrawCircle(Player.Position, SkillHandler.R.Range, DrawR.Color);
            }

            if (MenuHandler.TalonConfig.Item("HUD").GetValue<bool>())
            {
                if (MenuHandler.TalonConfig.Item("KSi").GetValue<KeyBind>().Active ||
                    MenuHandler.TalonConfig.Item("KSq").GetValue<KeyBind>().Active ||
                    MenuHandler.TalonConfig.Item("KSw").GetValue<KeyBind>().Active)
                {
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.68f, Color.Yellow, "KillSteal : On");
                }
                else
                {
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.68f, Color.DarkRed, "KillSteal : Off");
                }

                if (MenuHandler.TalonConfig.SubMenu("Harass").Item("HarassToggle").GetValue<KeyBind>().Active)
                {
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.66f, Color.Yellow, "AutoHarass : On");
                }
                else
                {
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.66f, Color.DarkRed, "AutoHarass : Off");
                }
            }
        }
    }
}
