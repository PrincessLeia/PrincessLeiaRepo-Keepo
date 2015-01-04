using System;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Princess_LeBlanc
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Load;
        }
        public static void Load(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Leblanc")
            {
                return;
            }

            SkillHandler.Init();
            ItemHandler.Init();
            MenuHandler.Init();
            DrawingHandler.Init();

            Interrupter.OnPossibleToInterrupt += FightHandler.Interrupter_OnPossibleToInterrupt;
            AntiGapcloser.OnEnemyGapcloser += FightHandler.AntiGapcloser_OnEnemyGapcloser;
            Game.OnGameUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += FightHandler.GameObject_OnCreate;
            GameObject.OnDelete += FightHandler.GameObject_OnDelete;

            Game.PrintChat("Princess " + ObjectManager.Player.ChampionName);
        }

        public static void Game_OnGameUpdate(EventArgs args)
        {

            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (ObjectManager.Player.HasBuff("Recall"))
            {
                return;
            }


            if (MenuHandler.LeBlancConfig.Item("Orbwalk").GetValue<KeyBind>().Active)
            {
                var target = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Magical);
                var targetExtendet = target.Distance(ObjectManager.Player.Position) < SkillHandler.W.Range + SkillHandler.Q.Range - 100;
                var targetQ = SkillHandler.Q.InRange(target.ServerPosition);
                var useW = SkillHandler.W.IsReady() &&  ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "LeblancSlide" &&
                           MenuHandler.LeBlancConfig.SubMenu("Combo").Item("useW").GetValue<bool>();

                if (targetExtendet && !targetQ && useW && ObjectManager.Player.Level > 1
                     && target.Health < MathHandler.ComboDamage(target) - ObjectManager.Player.GetSpellDamage(target, SpellSlot.W))
                {
                    FightHandler.ComboLong();
                }
                else if (((target.Health - MathHandler.ComboDamage(target)) / target.MaxHealth) * 100 >
                    (target.Health * 50) / 100)
                {
                    FightHandler.ComboTanky();
                }
                else
                {
                    FightHandler.Combo();
                }

                if (MenuHandler.LeBlancConfig.Item("backW").GetValue<bool>())
                {
                    FightHandler.WLogic();
                }

            }
            else if (MenuHandler.LeBlancConfig.Item("Farm").GetValue<KeyBind>().Active)
            {
                
            }
            else if (MenuHandler.LeBlancConfig.Item("LaneClear").GetValue<KeyBind>().Active)
            {
                FightHandler.LaneClear();
                FightHandler.JungleClear();
            }

            if (MenuHandler.LeBlancConfig.Item("FleeK").GetValue<KeyBind>().Active)
            {
                FightHandler.Flee();
            }
        }
    }
}
