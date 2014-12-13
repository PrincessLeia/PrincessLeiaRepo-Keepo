using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace Princess_Diana
{   class DrawingHandler
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

            var DrawQ = MenuHandler.DianaConfig.Item("drawQ").GetValue<Circle>();
            if (DrawQ.Active)
            {
                Utility.DrawCircle(Player.Position, SkillHandler.Q.Range, DrawQ.Color);
            }

            var DrawR = MenuHandler.DianaConfig.Item("drawR").GetValue<Circle>();
            if (DrawR.Active)
            {
                Utility.DrawCircle(Player.Position, SkillHandler.R.Range, DrawR.Color);
            }

            var DrawA = MenuHandler.DianaConfig.Item("drawA").GetValue<Circle>();
            if (DrawA.Active && Player.HasBuff("dianaarcready"))
            {
                Utility.DrawCircle(Player.Position, 150f, DrawQ.Color);
            }

            if (MenuHandler.DianaConfig.Item("HUD").GetValue<bool>())
            {
                if (MenuHandler.DianaConfig.Item("HarassActive").GetValue<KeyBind>().Active || MenuHandler.DianaConfig.Item("HarassToggle").GetValue<KeyBind>().Active)
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.68f, System.Drawing.Color.Yellow,
                        "Auto Harass : On");
                else
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.68f, System.Drawing.Color.DarkRed,
                        "Auto Harass : Off");

                if (MenuHandler.DianaConfig.Item("KSi").GetValue<bool>() == true || MenuHandler.DianaConfig.Item("KSq").GetValue<bool>() == true)
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.66f, System.Drawing.Color.Yellow, "Auto KS : On");
                else
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.66f, System.Drawing.Color.DarkRed,
                        "Auto KS : Off");

                if (MenuHandler.DianaConfig.Item("LCQ").GetValue<bool>() == true)
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.64f, System.Drawing.Color.Yellow, "Q LaneClear : On");
                else
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.64f, System.Drawing.Color.DarkRed,
                        "Q LaneClear : Off");

                if (MenuHandler.DianaConfig.Item("LCW").GetValue<bool>() == true)
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.62f, System.Drawing.Color.Yellow, "W LaneClear : On");
                else
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.62f, System.Drawing.Color.DarkRed,
                        "W LaneClear : Off");

                if (ItemHandler.Dfg.IsReady())
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.60f, System.Drawing.Color.Yellow,
                            "DFG Available");
                    else
                        Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.60f, System.Drawing.Color.DarkRed,
                            "DFG OnCooldown");
            }

        }
    }
}
