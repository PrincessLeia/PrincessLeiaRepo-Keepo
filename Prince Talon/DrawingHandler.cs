using System;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace PrinceTalon
{
    internal class DrawingHandler
    {
        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }

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
        }
    }
}
