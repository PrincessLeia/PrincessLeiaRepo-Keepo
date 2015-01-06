using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_LeBlanc
{
    class Program
    {
        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        public static void OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Leblanc")
            {
                return;
            }

            SkillHandler.Init();
            ItemHandler.Init();
            MenuHandler.Init();
            AssassinManager.Init();

            Drawing.OnDraw += AssassinManager.Drawing_OnDraw;
            Drawing.OnDraw += DrawingHandler.OnDraw;
            Game.OnWndProc += AssassinManager.Game_OnWndProc;
            Interrupter.OnPossibleToInterrupt += FightHandler.Interrupter_OnPossibleToInterrupt;
            AntiGapcloser.OnEnemyGapcloser += FightHandler.AntiGapcloser_OnEnemyGapcloser;
            Game.OnGameUpdate += OnGameUpdate;
            GameObject.OnCreate += FightHandler.GameObject_OnCreate;
            GameObject.OnDelete += FightHandler.GameObject_OnDelete;

            Game.PrintChat("<b><font color =\"#FFFFFF\">Princess LeBlanc</font></b><font color =\"#FFFFFF\"> by </font><b><font color=\"#FF66FF\">Leia</font></b><font color =\"#FFFFFF\"> loaded!</font>");
        }

        public static void OnGameUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (ObjectManager.Player.HasBuff("Recall"))
            {
                return;
            }

            switch (MenuHandler.Orb.ActiveMode)
            {
               case Orbwalking.OrbwalkingMode.Combo:
                {
                    FightHandler.TargetSelect();
                    FightHandler.ComboLogic();
                    FightHandler.WLogic();
                    break;
                }
               case Orbwalking.OrbwalkingMode.LaneClear:
                {
                    FightHandler.LaneClear();
                    FightHandler.JungleClear();
                    break;
                }
               case Orbwalking.OrbwalkingMode.Mixed:
                {
                    FightHandler.Harass();
                    break;
                }
            }
            FightHandler.Flee();
        }
    }
}
