using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Prince_Talon
{
    internal class Program
    {

        internal static Orbwalking.Orbwalker Orbwalker;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Load;
        }
        public static void Load(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Talon")
            {
                return;
            }

            SkillHandler.Init();
            ItemHandler.Init();
            MenuHandler.Init();
            DrawingHandler.Init();

            Game.OnGameUpdate += OnGameUpdateModes;

            Game.PrintChat("Prince " + ObjectManager.Player.ChampionName);

            AntiGapcloser.OnEnemyGapcloser += FightHandler.AntiGapCloser;
        }
        public static void OnGameUpdateModes(EventArgs args)
        {

            Orbwalker.SetAttack(true);

            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (ObjectManager.Player.HasBuff("Recall"))
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    FightHandler.Combo();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    FightHandler.LaneClear();
                    FightHandler.JungleClear();
                    break;
            }
            if (MenuHandler.TalonConfig.Item("KSi").GetValue<bool>() ||
                MenuHandler.TalonConfig.Item("KSq").GetValue<bool>() ||
                MenuHandler.TalonConfig.Item("KSw").GetValue<bool>())
            {
                FightHandler.KillSteal();
            }

            if (MenuHandler.TalonConfig.Item("HarassW").GetValue<bool>())
            {
                FightHandler.Harass();
            }
        }
    }
}
