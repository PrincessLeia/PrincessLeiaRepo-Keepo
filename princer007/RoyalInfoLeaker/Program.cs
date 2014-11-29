using System;
using LeagueSharp;
using LeagueSharp.Common;
using System.Collections.Generic;
using SharpDX;
using Color = System.Drawing.Color;

namespace RoyalInfoLeaker
{
    class Program
    {
        private static readonly Obj_AI_Hero player = ObjectManager.Player;
        //private static Menu menu;
        private static List<Bitch> bitchez = new List<Bitch>();
        

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }
        private static void Game_OnGameLoad(EventArgs args)
        {

            foreach (Obj_AI_Hero obj in ObjectManager.Get<Obj_AI_Hero>())
                if(!obj.IsMe)
                    bitchez.Add(new Bitch(obj));
            Game.OnGameSendPacket += OnSendPacket;
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            Game.PrintChat("Hey, psss. Wanna find out some cheaters?");
        }

        private static void OnDraw(EventArgs args)
        {
            foreach (Bitch b in bitchez)
            {
                if (b.total > 20 && b.total < 50)
                {
                    Drawing.DrawText(b.bitch.HPBarPosition.X + 10, b.bitch.HPBarPosition.Y - 15, Color.GreenYellow, "Possibly cheater");
                    Drawing.DrawText(b.bitch.HPBarPosition.X + 10, b.bitch.HPBarPosition.Y - 3, Color.GreenYellow, "Suspicious moves: " + b.total);
                }
                else if (b.total >= 50)
                    Drawing.DrawText(b.bitch.HPBarPosition.X + 10, b.bitch.HPBarPosition.Y - 3, Color.IndianRed, "Cheater");

            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            foreach (Bitch b in bitchez)
                if (b.bitch.IsVisible && b.total < 50)
                {
                    List<Vector2> wp = b.bitch.GetWaypoints();
                    Vector2 bb = wp[wp.Count - 1];
                    if (bb != b.lastWP)
                    {
                        if (b.second == (int)Game.Time)
                        {
                            b.average++;
                            b.lastWP = bb;
                        }
                        else
                        {
                            b.second = (int)Game.Time;
                            if (b.average >= 10) b.total++;
                            b.average = 0;
                        }
                    }
                }
        }

        private static void OnSendPacket(GamePacketEventArgs args)
        {

        }
        /*
        private static void LoadMenu()
        {
            // Initialize the menu
            menu = new Menu("Royal Info Leaker", "lel", true);
            menu.AddItem(new MenuItem("draw", "Drawerino Changerino").SetValue(true));

            // Finalize menu
            menu.AddToMainMenu();
            Console.WriteLine("Menu finalized");
        }*/
    }
    class Bitch
    {
        public int average = 0;
        public int total;
        public int second;
        public Obj_AI_Hero bitch;
        public Vector2 lastWP = new Vector2();

        public Bitch(Obj_AI_Hero bitch)
        {
            this.bitch = bitch;
        }
    }
}
