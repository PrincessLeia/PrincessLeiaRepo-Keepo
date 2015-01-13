using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace Prince_Urgot
{
    internal class DrawingClass
    {
        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }
        private static Menu DrawMenu;

        public DrawingClass(Menu drawMenu)
        {
            DrawMenu = drawMenu;
            Menu();
            Drawing.OnDraw += OnDraw;
        }

        private static void Menu()
        {
            DrawMenu.AddSubMenu(new Menu("Drawing", "drawing"));
            DrawMenu.SubMenu("drawing").AddItem(new MenuItem("drawQ", "Draw Q").SetValue(new Circle(true, Color.AntiqueWhite)));
            DrawMenu.SubMenu("drawing").AddItem(new MenuItem("drawE", "Draw E").SetValue(new Circle(true, Color.AntiqueWhite)));
            DrawMenu.SubMenu("drawing").AddItem(new MenuItem("drawR", "Draw R").SetValue(new Circle(true, Color.AntiqueWhite)));
            DrawMenu.SubMenu("drawing").AddItem(new MenuItem("drawEHit", "Draw extended Q range if hit by E").SetValue(new Circle(true, Color.AntiqueWhite)));
            DrawMenu.SubMenu("drawing").AddItem(new MenuItem("hitbye", "Draw Circle on Enemy if hit by E").SetValue(true));
        }
        private static void OnDraw(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            var drawQ = DrawMenu.Item("drawQ").GetValue<Circle>();
            var drawE = DrawMenu.Item("drawE").GetValue<Circle>();
            var drawR = DrawMenu.Item("drawR").GetValue<Circle>();
            var drawEHit = DrawMenu.Item("drawEHit").GetValue<Circle>();

            if (drawQ.Active)
            {
                Render.Circle.DrawCircle(Player.Position, SpellClass.Q.Range, drawQ.Color);
            }
            if (drawE.Active)
            {
                Render.Circle.DrawCircle(Player.Position, SpellClass.E.Range, drawE.Color);
            }
            if (drawR.Active)
            {
                Render.Circle.DrawCircle(Player.Position, SpellClass.R.Range, drawR.Color);
            }

            if (drawEHit.Active)
            {
                foreach (var obj in ObjectManager.Get<Obj_AI_Hero>().Where(obj => obj.IsValidTarget(1600) && obj.HasBuff("urgotcorrosivedebuff", true)))
                {
                    Render.Circle.DrawCircle(Player.Position, SpellClass.Q2.Range, drawEHit.Color);
                }
            }
        }
    }
}