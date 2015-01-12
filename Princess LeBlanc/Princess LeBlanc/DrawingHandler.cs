using System;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

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
            var wts = Drawing.WorldToScreen(ObjectManager.Player.Position);

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

            if (FightHandler.StatusW() == "return")
            {
                Drawing.DrawText(wts[0] - 35, wts[1] + 10, Color.White, "Second W: " + FightHandler.ee.ToString("0.0"));
            }
            if (MenuHandler.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                switch (
                    MenuHandler.LeBlancConfig.SubMenu("LaneMode")
                        .Item("LaneModeSwitch")
                        .GetValue<StringList>()
                        .SelectedIndex)
                {
                    case 0:
                    {
                        Drawing.DrawText(wts[0] - 55, wts[1] + 30, Color.LawnGreen, "LaneClear");
                        break;
                    }
                    case 1:
                    {
                        Drawing.DrawText(wts[0] - 55, wts[1] + 30, Color.LawnGreen, "Harass");
                        break;
                    }
                }
            }
        }
    }
}
