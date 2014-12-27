using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_Diana
{
    internal class Program
    {

        internal static Orbwalking.Orbwalker Orbwalker;

        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }

        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Load;
        }

        public static void Load(EventArgs args)
        {
            if (Player.ChampionName != "Diana") return;

            SkillHandler.Init();
            ItemHandler.Init();
            MenuHandler.Init();
            DrawingHandler.Init();

            Game.OnGameUpdate += OnGameUpdateModes;

            Game.PrintChat("Welcome to Education Nunu");

            AntiGapcloser.OnEnemyGapcloser += FightHandler.AntiGapCloser;
            Interrupter.OnPossibleToInterrupt += FightHandler.Interrupter_OnPossibleToInterrupt; 
        }

        public static void OnGameUpdateModes(EventArgs args)
        {

            if (Player.IsDead) return;
            if (Player.HasBuff("Recall")) return;
            var target = TargetSelector.GetTarget(SkillHandler.Q.Range, TargetSelector.DamageType.Magical);

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {

                if (target.Health <= MathHandler.ComboDamage(target))
                {
                    FightHandler.Combo2();
                }

                if (SkillHandler.Q.GetPrediction(target).Hitchance >= HitChance.VeryHigh)
                {
                    FightHandler.Combo3();
                }

                if (target.Health <= MathHandler.GapCloseKill1Damage(target))
                {
                    FightHandler.GapCloseKill();
                }

                if (target.Health <= MathHandler.GapCloseKill2Damage(target))
                {
                    FightHandler.GapCloseKill2();
                }

                else
                {
                    FightHandler.Combo1();
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {

            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                FightHandler.LaneClear();
            }

            if (MenuHandler.DianaConfig.Item("HarassActive").GetValue<KeyBind>().Active || MenuHandler.DianaConfig.Item("HarassToggle").GetValue<KeyBind>().Active)
            {
                FightHandler.Harass();
            }
        }
    }
}
