using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace PrinceTalon
{
    class Program
    {
        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }

        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        public static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Talon")
                return;

            SkillHandler.Init();
            ItemHandler.Init();
            MenuHandler.Init();
            DrawingHandler.Init();

            Game.OnGameUpdate += OnGameUpdateModes;
            Orbwalking.BeforeAttack += FightHandler.AfterAttack;

            Game.PrintChat("<b><font color =\"#FFFFFF\">Prince Talon</font></b><font color =\"#FFFFFF\"> by </font><b><font color=\"#FF66FF\">Leia</font></b><font color =\"#FFFFFF\"> loaded!</font>");
        }

        public static void OnGameUpdateModes(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (ObjectManager.Player.HasBuff("Recall"))
            {
                return;
            }

            FightHandler.PacketCast = MenuHandler.TalonConfig.SubMenu("Misc").Item("UsePacket").GetValue<bool>();

            switch (MenuHandler.Orb.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                {
                    FightHandler.Combo();
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
                case Orbwalking.OrbwalkingMode.LastHit:
                {
                    FightHandler.Harass();
                    break;
                }
                case Orbwalking.OrbwalkingMode.None:
                {
                    break;
                }
            }
        }
    }
}
