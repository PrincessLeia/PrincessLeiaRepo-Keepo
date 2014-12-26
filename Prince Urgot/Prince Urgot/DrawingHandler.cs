using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace Prince_Urgot
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

        public static
            void OnDraw
            (EventArgs
                args)
        {
            if (Player.IsDead)
            {
                return;
            }

            var DrawE = MenuHandler._uMenu.Item("drawE").GetValue<Circle>();
            if (DrawE.Active)
            {

                foreach (
                    var obj in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(obj => obj.IsValidTarget(1500) && obj.HasBuff("urgotcorrosivedebuff", true)))
                {
                    Utility.DrawCircle(Player.Position, 1110, DrawE.Color);
                }
            }

            var DrawQ = MenuHandler._uMenu.Item("drawQ").GetValue<Circle>();
            if (DrawQ.Active)
            {
                Utility.DrawCircle(Player.Position, SkillHandler.Q.Range, DrawQ.Color);
            }

            var DrawEe = MenuHandler._uMenu.Item("drawEe").GetValue<Circle>();
            if (DrawEe.Active)
            {
                Utility.DrawCircle(Player.Position, 820, DrawEe.Color);
            }

            if (MenuHandler._uMenu.Item("HUD").GetValue<bool>())
            {
                if (MenuHandler._uMenu.Item("HarassActive").GetValue<KeyBind>().Active ||
                    MenuHandler._uMenu.Item("HarassToggle").GetValue<KeyBind>().Active)
                {
                    Drawing.DrawText(Drawing.Width*0.90f, Drawing.Height*0.68f, Color.Yellow,
                        "Auto Harass : On");
                }
                else
                {
                    Drawing.DrawText(Drawing.Width*0.90f, Drawing.Height*0.68f, Color.DarkRed,
                        "Auto Harass : Off");
                }

                if (MenuHandler._uMenu.Item("KillI").GetValue<bool>() == true ||
                    MenuHandler._uMenu.Item("KillQ").GetValue<bool>() == true)
                {
                    Drawing.DrawText(Drawing.Width*0.90f, Drawing.Height*0.66f, Color.Yellow,
                        "Auto KS : On");
                }

                else
                {
                    Drawing.DrawText(Drawing.Width*0.90f, Drawing.Height*0.66f, Color.DarkRed,
                        "Auto KS : Off");
                }

                if (MenuHandler._uMenu.Item("lastHitQ").GetValue<bool>() == true)
                {
                    Drawing.DrawText(Drawing.Width*0.90f, Drawing.Height*0.60f, Color.Yellow,
                        "Q LastHit : On");
                }
                else
                {
                    Drawing.DrawText(Drawing.Width*0.90f, Drawing.Height*0.60f, Color.DarkRed,
                        "Q LastHit : Off");
                }

                if (MenuHandler._uMenu.Item("LaneClearQ").GetValue<bool>() == true ||
                    MenuHandler._uMenu.Item("LaneClearE").GetValue<bool>() == true)
                {
                    Drawing.DrawText(Drawing.Width*0.90f, Drawing.Height*0.58f, Color.Yellow,
                        "Q LaneClear : On");
                }
                else
                {
                    Drawing.DrawText(Drawing.Width*0.90f, Drawing.Height*0.58f, Color.DarkRed,
                        "Q LaneClear : Off");
                }

                if (ItemHandler.Muramana.IsReady())
                {
                    if (MenuHandler._uMenu.Item("useMura").GetValue<KeyBind>().Active)
                    {
                        Drawing.DrawText(Drawing.Width*0.90f, Drawing.Height*0.64f, Color.Yellow,
                            "Auto Muramana : On");
                    }
                    else
                    {
                        Drawing.DrawText(Drawing.Width*0.90f, Drawing.Height*0.64f, Color.DarkRed,
                            "Auto Muramana : Off");
                    }
                }
                else
                {
                    Drawing.DrawText(Drawing.Width*0.90f, Drawing.Height*0.64f, Color.DarkRed,
                        "Muramana unavailable");
                }

                if (MenuHandler._uMenu.Item("autoR").GetValue<bool>() == true ||
                    MenuHandler._uMenu.Item("autoInt").GetValue<bool>() == true)
                {
                    Drawing.DrawText(Drawing.Width*0.90f, Drawing.Height*0.62f, Color.Yellow,
                        "Auto R : On");
                }
                else
                {
                    Drawing.DrawText(Drawing.Width*0.90f, Drawing.Height*0.62f, Color.DarkRed,
                        "Auto R : Off");
                }

                if (MenuHandler._uMenu.Item("hitbye").GetValue<bool>() == true)
                {
                    var target = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Physical);

                    foreach (
                        var obj in
                            ObjectManager.Get<Obj_AI_Hero>()
                                .Where(obj => obj.IsValidTarget(2000) && obj.HasBuff("urgotcorrosivedebuff", true)))

                    {
                        Utility.DrawCircle(target.Position, 100, Color.GreenYellow);
                    }

                    var DrawR = MenuHandler._uMenu.Item("drawR").GetValue<Circle>();
                    if (DrawR.Active && SkillHandler.R.IsReady())
                    {
                        Utility.DrawCircle(Player.Position, SkillHandler.R.Range, DrawR.Color);
                    }
                }
            }
        }
    }
}
