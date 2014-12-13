using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace Prince_Warwick
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

            var DrawQ = MenuHandler.WarwickConfig.Item("drawQ").GetValue<Circle>();
            if (DrawQ.Active)
            {
                Utility.DrawCircle(Player.Position, SkillHandler.Q.Range, DrawQ.Color);
            }

            var DrawE = MenuHandler.WarwickConfig.Item("drawE").GetValue<Circle>();
            if (DrawE.Active)
            {

                foreach (
                    var obj in ObjectManager.Get<Obj_AI_Hero>().Where(obj => obj.IsValidTarget(5000) && obj.HasBuff("warwickbloodscent", true)))
                    if (obj.IsValid)
                    {
                        Utility.DrawCircle(Player.Position, SkillHandler.E.Range, DrawE.Color);
                    }
            }

            var DrawR = MenuHandler.WarwickConfig.Item("drawR").GetValue<Circle>();
            if (DrawR.Active)
            {
                Utility.DrawCircle(Player.Position, SkillHandler.R.Range, DrawR.Color);
            }

            var DrawQss = MenuHandler.WarwickConfig.Item("drawQss").GetValue<Circle>();
            if (DrawQss.Active)
            {
                var target = SimpleTs.GetTarget(2000, SimpleTs.DamageType.Physical);

                foreach (
                    var obj in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(obj => obj.IsValidTarget(2000) && target.InventoryItems.Equals(ItemHandler.Qss) || target.InventoryItems.Equals(ItemHandler.Mercurial)))

                    Utility.DrawCircle(target.Position, 100, DrawQss.Color);
            }


            if (MenuHandler.WarwickConfig.Item("HUD").GetValue<bool>())
            {
                if (MenuHandler.WarwickConfig.Item("KSi").GetValue<KeyBind>().Active || MenuHandler.WarwickConfig.Item("KSq").GetValue<KeyBind>().Active)
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.68f, System.Drawing.Color.Yellow,
                        "KillSteal : On");
                else
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.68f, System.Drawing.Color.DarkRed,
                        "KillSteal : Off");

                if (MenuHandler.WarwickConfig.Item("autoULT").GetValue<KeyBind>().Active)
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.66f, System.Drawing.Color.Yellow, "ULT on Click : On");
                else
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.66f, System.Drawing.Color.DarkRed,
                        "ULT on Click : Off");

                if (MenuHandler.WarwickConfig.Item("InterR").GetValue<bool>() == true)
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.64f, System.Drawing.Color.Yellow, "Interrupt R : On");
                else
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.64f, System.Drawing.Color.DarkRed,
                        "Interrupt R : Off");

                if (MenuHandler.WarwickConfig.Item("notULT").GetValue<bool>() == true)
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.62f, System.Drawing.Color.Yellow, "Not ULT on QSS : On");
                else
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.62f, System.Drawing.Color.DarkRed,
                        "Not ULT on QSS : Off");
            }

        }
    }
}
