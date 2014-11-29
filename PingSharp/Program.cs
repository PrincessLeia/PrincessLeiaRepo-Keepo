using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace PingSharp
{
    internal class Program
    {
        public static Menu Pingtime;
        public static String ping;
        public static int OffsetX = 0;
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
            Drawing.OnDraw += Drawing_OnDraw;
        }

          private static void Game_OnGameLoad (EventArgs args)
            {
                Pingtime = new Menu("Ping", "Ping", true);
                Pingtime.AddItem(new MenuItem("Activate", "Activate")).SetValue(true);
                Pingtime.AddItem(new MenuItem("Color", "Color")).SetValue(new Circle(true, Color.White));
                Pingtime.AddItem(new MenuItem("offX2", "Offset for width").SetValue(new Slider(0, -50, 50)));
                Pingtime.AddItem(new MenuItem("offY2", "Offset for height").SetValue(new Slider(0, -50, 50)));
                Pingtime.AddToMainMenu();
                Game.PrintChat("Ping loaded");
            }
          private static void Drawing_OnDraw(EventArgs args)
          {

              if (Pingtime.Item("Activate").GetValue<bool>())
              {

                  ping = Game.Ping.ToString("0");
                  OffsetX = Pingtime.Item("offX2").GetValue<Slider>().Value - 12;
                  
                  Drawing.DrawText((Drawing.Width - (Drawing.Width * 0.045f)) + OffsetX, (Drawing.Height * 0.065f) + Pingtime.Item("offY2").GetValue<Slider>().Value, Pingtime.Item("Color").GetValue<Circle>().Color, ping);
              }

          }
        }
    }   