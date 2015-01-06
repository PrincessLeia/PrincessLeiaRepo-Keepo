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
        public static void OnDraw(EventArgs args)
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
        }
    }
}
