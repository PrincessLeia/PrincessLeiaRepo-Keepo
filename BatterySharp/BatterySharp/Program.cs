using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using Color =System.Drawing.Color;

namespace BatterySharp
{
    class Program
    {

        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
            Drawing.OnDraw += Drawing_OnDraw;
        }

       public static void Game_OnGameLoad(EventArgs args)
        {
            Game.PrintChat("Battery loaded");
        }

       public static void Drawing_OnDraw(EventArgs args)
        {
             var p = SystemInformation.PowerStatus.BatteryLifePercent * 100;
           var b = p.ToString("");
           if (p < 33)
           {
               Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.07f, Color.Red, "Battery: " + b + " %");
           }
           else if (p < 66)
           {
               Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.07f, Color.Yellow, "Battery: " + b + " %");
           }
           else if (p <= 100)
           {
               {
                   Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.07f, Color.Green, "Battery: " + b + " %");
               }
           }
        }
    }
}
