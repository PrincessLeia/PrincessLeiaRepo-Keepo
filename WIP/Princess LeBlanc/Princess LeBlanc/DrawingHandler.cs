using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_LeBlanc
{
    internal class DrawingHandler
    {
        public static void Init()
        {
            Drawing.OnDraw += OnDraw;
        }
        private static void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
                return;

            var drawQ = MenuHandler.LeBlancConfig.Item("drawQ").GetValue<Circle>();
            var drawW = MenuHandler.LeBlancConfig.Item("drawW").GetValue<Circle>();
            var drawE = MenuHandler.LeBlancConfig.Item("drawE").GetValue<Circle>();

            if (drawQ.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, SkillHandler.Q.Range, drawQ.Color);
            }
            if (drawW.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, SkillHandler.W.Range, drawW.Color);
            }
            if (drawE.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, SkillHandler.E.Range, drawE.Color);
            }
        }
    }
}
