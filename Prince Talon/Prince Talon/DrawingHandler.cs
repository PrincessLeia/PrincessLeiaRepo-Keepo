using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Prince_Talon
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

            var DrawW = MenuHandler.TalonConfig.Item("drawW").GetValue<Circle>();
            if (DrawW.Active)
            {
                Utility.DrawCircle(Player.Position, SkillHandler.W.Range, DrawW.Color);
            }

            var DrawE = MenuHandler.TalonConfig.Item("drawE").GetValue<Circle>();
            if (DrawE.Active)
            {
                Utility.DrawCircle(Player.Position, SkillHandler.W.Range, DrawE.Color);
            }


            var DrawR = MenuHandler.TalonConfig.Item("drawR").GetValue<Circle>();
            if (DrawR.Active)
            {
                Utility.DrawCircle(Player.Position, SkillHandler.R.Range, DrawR.Color);
            }


            if (MenuHandler.TalonConfig.Item("HUD").GetValue<bool>())
            {
                if (MenuHandler.TalonConfig.Item("KSi").GetValue<KeyBind>().Active || MenuHandler.TalonConfig.Item("KSq").GetValue<KeyBind>().Active || MenuHandler.TalonConfig.Item("KSw").GetValue<KeyBind>().Active)
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.68f, System.Drawing.Color.Yellow,
                        "KillSteal : On");
                else
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.68f, System.Drawing.Color.DarkRed,
                        "KillSteal : Off");

                if (MenuHandler.TalonConfig.Item("haraQ").GetValue<KeyBind>().Active || MenuHandler.TalonConfig.Item("haraW").GetValue<KeyBind>().Active)
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.66f, System.Drawing.Color.Yellow, "AutoHarass : On");
                else
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.66f, System.Drawing.Color.DarkRed,
                        "AutoHarass : Off");
            }

        }
    }
}
