using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_LeBlanc
{
    internal class Program
    {
        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }

        public static bool PacketCast;

        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Leblanc")
            {
                return;
            }
            SkillHandler.Init();
            MenuHandler.Init();
            ItemHandler.Init();
            DrawingHandler.Init();
            Game.OnGameUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += FightHandler.GameObject_OnCreate;
            GameObject.OnDelete += FightHandler.GameObject_OnDelete;
            Interrupter.OnPossibleToInterrupt += FightHandler.Interrupter_OnPossibleToInterrupt;
            AntiGapcloser.OnEnemyGapcloser += FightHandler.AntiGapcloser_OnEnemyGapcloser;

            Game.PrintChat("<b><font color =\"#FFFFFF\">Princess LeBlanc</font></b><font color =\"#FFFFFF\"> by </font><b><font color=\"#FF66FF\">Leia</font></b><font color =\"#FFFFFF\"> loaded!</font>");
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (ObjectManager.Player.HasBuff("Recall"))
            {
                return;
            }

            PacketCast = MenuHandler.LeBlancConfig.SubMenu("Misc").Item("UsePacket").GetValue<bool>();

            switch (MenuHandler.Orbwalker.ActiveMode)
            {
                case Orbwalking2.OrbwalkingMode.Combo:
                {
                    FightHandler.ComboLogic();
                    break;
                }
                case Orbwalking2.OrbwalkingMode.LaneClear:
                {
                    FightHandler.LaneClear();
                    FightHandler.JungleClear();
                    break;
                }
                case Orbwalking2.OrbwalkingMode.LastHit:
                {
                    break;
                }
                case Orbwalking2.OrbwalkingMode.Mixed:
                {
                    FightHandler.Harass();
                    break;
                }
            }
            FightHandler.WLogic();
            FightHandler.Flee();
            FightHandler.CloneLogic();
        }
    }
}
