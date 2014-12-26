﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_Diana
{
    internal class FightHandler
    {
        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        public static void Combo1()
        {
            var target = TargetSelector.GetTarget(SkillHandler.Q.Range, TargetSelector.DamageType.Magical);
            var dfg = ItemHandler.Dfg;


            if (Player.Distance(target) <= dfg.Range && MenuHandler.DianaConfig.Item("useDfg").GetValue<bool>() && dfg.IsReady() && target.Health <= MathHandler.ComboDamage(target))
            {
                dfg.Cast(target);
            }

            if (ItemHandler.IgniteSlot != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(ItemHandler.IgniteSlot) == SpellState.Ready)
            {
                if (target.Health <= MathHandler.ComboDamage(target))
                {
                    Player.Spellbook.CastSpell(ItemHandler.IgniteSlot, target);
                }
            }

            if (Player.Distance(target) <= SkillHandler.Q.Range && MenuHandler.DianaConfig.Item("CombouseQ").GetValue<bool>() && SkillHandler.Q.IsReady() && SkillHandler.Q.GetPrediction(target).Hitchance >= HitChance.High)
            {
                SkillHandler.Q.CastIfHitchanceEquals(target, HitChance.High, Packeting());
            }
            if (Player.Distance(target) <= SkillHandler.R.Range && MenuHandler.DianaConfig.Item("CombouseR").GetValue<bool>() && SkillHandler.R.IsReady() && target.HasBuff("dianamoonlight", true))
            {
                SkillHandler.R.Cast(target, Packeting());
            }
            if (Player.Distance(target) <= SkillHandler.W.Range && MenuHandler.DianaConfig.Item("CombouseW").GetValue<bool>() && SkillHandler.W.IsReady() /*&& !SkillHandler.Q.IsReady()*/)
            {
                SkillHandler.W.Cast();
            }
            if (Player.Distance(target) <= SkillHandler.E.Range && Player.Distance(target) >= SkillHandler.W.Range || (!Orbwalking.InAutoAttackRange(Player) && Player.HasBuff("dianaarcready")) && MenuHandler.DianaConfig.Item("CombouseE").GetValue<bool>() && SkillHandler.W.IsReady() && SkillHandler.E.IsReady() && !SkillHandler.W.IsReady())
            {
                SkillHandler.E.Cast();
            }
            if (Player.Distance(target) <= SkillHandler.R.Range && SkillHandler.R.IsReady() && !SkillHandler.W.IsReady() && !SkillHandler.Q.IsReady())
            {
                if (MenuHandler.DianaConfig.Item("Combouse2RKill").GetValue<bool>() && target.Health <= Player.GetSpellDamage(target, SpellSlot.R))
                {
                    SkillHandler.R.Cast(target, Packeting());
                }
                else
                {
                    SkillHandler.R.Cast(target, Packeting());  
                }
            }
        }

        public static void Combo2()
        {
            var target = TargetSelector.GetTarget(SkillHandler.Q.Range, TargetSelector.DamageType.Magical);
            var dfg = ItemHandler.Dfg;

                if (Player.Distance(target) <= dfg.Range && MenuHandler.DianaConfig.Item("useDfg").GetValue<bool>() &&
                    dfg.IsReady() && target.Health <= MathHandler.ComboDamage(target))
                {
                    dfg.Cast(target);
                }

                if (ItemHandler.IgniteSlot != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(ItemHandler.IgniteSlot) == SpellState.Ready)
                {
                    if (target.Health <= MathHandler.ComboDamage(target))
                    {
                        Player.Spellbook.CastSpell(ItemHandler.IgniteSlot, target);
                    }
                }

                if (Player.Distance(target) <= SkillHandler.R.Range &&
                    MenuHandler.DianaConfig.Item("CombouseR").GetValue<bool>() && SkillHandler.R.IsReady())
                {
                    SkillHandler.R.Cast(target, Packeting());
                }
                if (Player.Distance(target) <= SkillHandler.Q.Range &&
                    MenuHandler.DianaConfig.Item("CombouseQ").GetValue<bool>() && SkillHandler.Q.IsReady())
                {
                    SkillHandler.Q.Cast(target, Packeting());
                }
                if (Player.Distance(target) <= SkillHandler.W.Range &&
                    MenuHandler.DianaConfig.Item("CombouseW").GetValue<bool>() && SkillHandler.W.IsReady()
                    /*&& !SkillHandler.Q.IsReady()*/)
                {
                    SkillHandler.W.Cast();
                }
                if (Player.Distance(target) <= SkillHandler.E.Range &&
                    (((!Orbwalking.InAutoAttackRange(Player)) && Player.HasBuff("dianaarcready")) ||
                     Player.Distance(target) >= SkillHandler.W.Range) &&
                    MenuHandler.DianaConfig.Item("CombouseE").GetValue<bool>() && SkillHandler.W.IsReady() &&
                    SkillHandler.E.IsReady() && !SkillHandler.W.IsReady())
                {
                    SkillHandler.E.Cast();
                }
                if (Player.Distance(target) <= SkillHandler.R.Range && SkillHandler.R.IsReady() &&
                    !SkillHandler.W.IsReady() && !SkillHandler.Q.IsReady())
                {
                        SkillHandler.R.Cast(target, Packeting());
                }
        }

        public static void Combo3()
        {
            var target = TargetSelector.GetTarget(SkillHandler.Q.Range, TargetSelector.DamageType.Magical);
            var dfg = ItemHandler.Dfg;

                if (Player.Distance(target) <= dfg.Range && MenuHandler.DianaConfig.Item("useDfg").GetValue<bool>() &&
                    dfg.IsReady() && target.Health <= MathHandler.ComboDamage(target))
                {
                    dfg.Cast(target);
                }

                if (ItemHandler.IgniteSlot != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(ItemHandler.IgniteSlot) == SpellState.Ready)
                {
                    if (target.Health <= MathHandler.ComboDamage(target))
                    {
                        Player.Spellbook.CastSpell(ItemHandler.IgniteSlot, target);
                    }
                }

                if (Player.Distance(target) <= SkillHandler.Q.Range &&
                    MenuHandler.DianaConfig.Item("CombouseQ").GetValue<bool>() && SkillHandler.Q.IsReady() &&
                    SkillHandler.Q.GetPrediction(target).Hitchance >= HitChance.VeryHigh)
                {
                    SkillHandler.Q.CastIfHitchanceEquals(target, HitChance.VeryHigh, Packeting());
                }
                if (Player.Distance(target) <= SkillHandler.R.Range &&
                    MenuHandler.DianaConfig.Item("CombouseR").GetValue<bool>() && SkillHandler.R.IsReady())
                {
                    SkillHandler.R.Cast(target, Packeting());
                }
                if (Player.Distance(target) <= SkillHandler.W.Range &&
                    MenuHandler.DianaConfig.Item("CombouseW").GetValue<bool>() && SkillHandler.W.IsReady()
                    /*&& !SkillHandler.Q.IsReady()*/)
                {
                    SkillHandler.W.Cast();
                }
                if (Player.Distance(target) <= SkillHandler.E.Range &&
                    (((!Orbwalking.InAutoAttackRange(Player)) && Player.HasBuff("dianaarcready")) ||
                     Player.Distance(target) >= SkillHandler.W.Range) &&
                    MenuHandler.DianaConfig.Item("CombouseE").GetValue<bool>() && SkillHandler.W.IsReady() &&
                    SkillHandler.E.IsReady() && !SkillHandler.W.IsReady())
                {
                    SkillHandler.E.Cast();
                }
                if (Player.Distance(target) <= SkillHandler.R.Range && SkillHandler.R.IsReady() &&
                    !SkillHandler.W.IsReady() && !SkillHandler.Q.IsReady())
                {
                    if (MenuHandler.DianaConfig.Item("Combouse2RKill").GetValue<bool>() &&
                        target.Health <= Player.GetSpellDamage(target, SpellSlot.R))
                    {
                        SkillHandler.R.Cast(target, Packeting());
                    }
                    else
                    {
                        SkillHandler.R.Cast(target, Packeting());
                    }
                }
            }

        public static void GapCloseKill()
    {
        //if (!_menu.Item("useMinionGapclose").GetValue<bool>()) return;
            
        var target = TargetSelector.GetTarget(SkillHandler.Q.Range + SkillHandler.R.Range, TargetSelector.DamageType.Physical);
        if (!target.IsValidTarget() || target == null) return;

        foreach (
            var minion in
                MinionManager.GetMinions(ObjectManager.Player.ServerPosition, SkillHandler.R.Range)
                    .Where(minion => minion.ServerPosition.Distance(target.ServerPosition) < SkillHandler.Q.Range)
                    .Where(minion => minion.IsValidTarget(SkillHandler.R.Range))
                    .Where(minion => SkillHandler.Q.IsReady() && SkillHandler.R.IsReady()))
        {
            SkillHandler.R.Cast(minion, Packeting());
            SkillHandler.Q.Cast(target, Packeting());
            if (ItemHandler.IgniteSlot != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(ItemHandler.IgniteSlot) == SpellState.Ready)
            {
                if (target.Health <= MathHandler.GapCloseKill1Damage(target))
                {
                    Player.Spellbook.CastSpell(ItemHandler.IgniteSlot, target);
                }
            }
            break;
        }
    }

        public static void GapCloseKill2()
        {
            //if (!_menu.Item("useMinionGapclose").GetValue<bool>()) return;

            var target = TargetSelector.GetTarget(SkillHandler.Q.Range + SkillHandler.R.Range, TargetSelector.DamageType.Physical);
            if (!target.IsValidTarget() || target == null) return;

            foreach (
                var minion in
                    MinionManager.GetMinions(ObjectManager.Player.ServerPosition, SkillHandler.Q.Range)
                        .Where(minion => ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q) < minion.Health &&
                                         minion.ServerPosition.Distance(target.ServerPosition) < SkillHandler.R.Range)
                                         .Where(minion => minion.IsValidTarget(SkillHandler.Q.Range))
                        .Where(minion => SkillHandler.Q.IsReady() && SkillHandler.R.IsReady()))
            {
                SkillHandler.Q.Cast(minion, Packeting());

                if (minion.HasBuff("dianamoonlight", true))
                {
                    SkillHandler.R.Cast(minion, Packeting());
                }
                SkillHandler.R.Cast(target, Packeting());
                SkillHandler.W.Cast();

                if (ItemHandler.IgniteSlot != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(ItemHandler.IgniteSlot) == SpellState.Ready)
                {
                    if (target.Health <= MathHandler.GapCloseKill2Damage(target))
                    {
                        Player.Spellbook.CastSpell(ItemHandler.IgniteSlot, target);
                    }
                }
                break;
            }
        }

        public static void KillSteal()
        {
            foreach (
                var target in
                    ObjectManager.Get<Obj_AI_Hero>().Where(target => target.IsValidTarget(SkillHandler.R.Range)))
            {
                if (MenuHandler.DianaConfig.Item("KSi").GetValue<bool>() && ItemHandler.IgniteSlot != SpellSlot.Unknown &&
                    Player.Spellbook.CanUseSpell(ItemHandler.IgniteSlot) == SpellState.Ready &&
                    Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) >= target.Health)
                    Player.Spellbook.CastSpell(ItemHandler.IgniteSlot, target);

                if (MenuHandler.DianaConfig.Item("KSq").GetValue<bool>() && SkillHandler.Q.IsReady() &&
                    SkillHandler.Q.GetDamage(target) >= target.Health && Player.Distance(target) <= SkillHandler.Q.Range)
                {
                    SkillHandler.Q.Cast(target, Packeting());
                }
                else if (MenuHandler.DianaConfig.Item("KSr").GetValue<bool>() &&
                         SkillHandler.R.GetDamage(target) >= target.Health &&
                         Player.Distance(target) <= SkillHandler.R.Range)
                {
                    SkillHandler.R.Cast(target, Packeting());
                }
                
                if (MenuHandler.DianaConfig.Item("KSr").GetValue<bool>() && target.HasBuff("dianamoonlight", true) && SkillHandler.R.IsReady() && SkillHandler.R.GetDamage(target) >= target.Health && Player.Distance(target) <= SkillHandler.R.Range)
                {
                        SkillHandler.R.Cast(target, Packeting());
                }
            }
        }

        public static void Harass()
        {
            var target = TargetSelector.GetTarget(SkillHandler.Q.Range, TargetSelector.DamageType.Magical);
            var mana = Player.Mana > Player.MaxMana * MenuHandler.DianaConfig.Item("HaraManaPercent").GetValue<Slider>().Value / 100;
            if (MenuHandler.DianaConfig.Item("haraQ").GetValue<bool>() && SkillHandler.Q.IsReady() && target.IsValidTarget(SkillHandler.Q.Range) && mana)
            {
                SkillHandler.Q.CastIfHitchanceEquals(target, HitChance.VeryHigh, Packeting());
            }
        }

        public static void LaneClear()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, SkillHandler.Q.Range, MinionTypes.All);
            var useQ = MenuHandler.DianaConfig.Item("LCQ").GetValue<bool>();
            var useW = MenuHandler.DianaConfig.Item("LCW").GetValue<bool>();
            foreach (var minion in allMinions)
            {
                if (useQ && SkillHandler.Q.IsReady() && Player.Distance(minion) < SkillHandler.Q.Range &&
                    minion.Health < 0.95 * Player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    SkillHandler.Q.Cast(minion, Packeting());
                }

                if (SkillHandler.W.IsReady() && useW && Player.Distance(minion) < SkillHandler.W.Range &&
                    minion.Health < 0.95 * Player.GetSpellDamage(minion, SpellSlot.W))
                {
                    SkillHandler.W.Cast();
                }
            }
        }

        public static bool Packeting()
        {
            return MenuHandler.DianaConfig.Item("packets").GetValue<bool>();
        }

        public static void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
            {
                if (SkillHandler.E.IsReady() && unit.IsEnemy && unit.IsValidTarget(SkillHandler.E.Range) && MenuHandler.DianaConfig.Item("InterE").GetValue<bool>())
                {
                    SkillHandler.E.Cast();
                } 
            }

        public static void AntiGapCloser(ActiveGapcloser gapcloser)
        {
            if (SkillHandler.W.IsReady() && gapcloser.Sender.IsValidTarget(SkillHandler.W.Range) && MenuHandler.DianaConfig.Item("gapcloW").GetValue<bool>())
            {
                SkillHandler.W.Cast();
            }
        }
    }
}