using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_LeBlanc
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
            if (ObjectManager.Player.ChampionName != "LeBlanc")
            {
                return;
            }

            SkillHandler.Init();
            ItemHandler.Init();
            MenuHandler.Init();
            DrawingHandler.Init();

            Interrupter.OnPossibleToInterrupt += FightHandler.Interrupter_OnPossibleToInterrupt;
            AntiGapcloser.OnEnemyGapcloser += FightHandler.AntiGapcloser_OnEnemyGapcloser;
            Game.OnGameUpdate += OnGameUpdateModes;
            Obj_AI_Base.OnProcessSpellCast += FightHandler.Obj_AI_Hero_OnProcessSpellCast;
            GameObject.OnCreate += FightHandler.GameObject_OnCreate;

            Game.PrintChat("Princess " + ObjectManager.Player.ChampionName);
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
                    break;
            }
                FightHandler.KillSteal();
                FightHandler.WLogic();
        }
    }
}
