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

            Utility.DelayAction.Add(2000, () => Game.PrintChat("<b><font color =\"#FFFFFF\">Princess LeBlanc</font></b><font color =\"#FFFFFF\"> by </font><b><font color=\"#FF66FF\">Leia</font></b><font color =\"#FFFFFF\"> loaded!</font>"));
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

            switch (MenuHandler.Orb.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                {
                    var target = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Magical);
                    var targetExtendet = target.Distance(ObjectManager.Player.Position) < SkillHandler.W.Range + SkillHandler.Q.Range - 100;
                    var targetQ = SkillHandler.Q.InRange(target.ServerPosition);
                    var useW = SkillHandler.W.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "LeblancSlide" &&
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
                    FightHandler.WLogic();
                    break;
                }
                case Orbwalking.OrbwalkingMode.LaneClear:
                {
                    FightHandler.LaneClear();
                    FightHandler.JungleClear();
                    break;
                }
            }
            FightHandler.Flee();
            FightHandler.Harass();
        }
    }
}
