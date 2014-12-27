using System.Linq;
using System.Runtime.InteropServices;
using LeagueSharp;
using LeagueSharp.Common;

namespace PrinceTalon
{
    class FightHandler
    {
        private static Obj_AI_Hero Player
            {
              get { return ObjectManager.Player; }
            }



        public static void Combo()
        {
            var useQ = SkillHandler.Q.IsReady() && MenuHandler.TalonConfig.SubMenu("Combo").Item("useQ").GetValue<bool>();
            var useW = SkillHandler.W.IsReady() && MenuHandler.TalonConfig.SubMenu("Combo").Item("useW").GetValue<bool>();
            var useE = SkillHandler.E.IsReady() && MenuHandler.TalonConfig.SubMenu("Combo").Item("useE").GetValue<bool>();
            var useR = SkillHandler.R.IsReady() && MenuHandler.TalonConfig.SubMenu("Combo").Item("useR").GetValue<bool>();
            var useBlade = ItemHandler.Blade.IsReady() && MenuHandler.TalonConfig.SubMenu("Items").Item("useBotrK").GetValue<bool>();
            var useBilge = ItemHandler.Bilgewater.IsReady() && MenuHandler.TalonConfig.SubMenu("Items").Item("useBilge").GetValue<bool>();
            var useYoumuu = ItemHandler.Youmuu.IsReady() && MenuHandler.TalonConfig.SubMenu("Items").Item("useYoumuu").GetValue<bool>();
            var useTiamat = ItemHandler.Tiamat.IsReady() && MenuHandler.TalonConfig.SubMenu("Items").Item("useTiamat").GetValue<bool>();
            var useHydra = ItemHandler.Hydra.IsReady() && MenuHandler.TalonConfig.SubMenu("Items").Item("useHydra").GetValue<bool>();
            var targetQ = TargetSelector.GetTarget(SkillHandler.Q.Range, TargetSelector.DamageType.Physical);
            var targetW = TargetSelector.GetTarget(SkillHandler.W.Range, TargetSelector.DamageType.Physical);
            var targetE = TargetSelector.GetTarget(SkillHandler.E.Range, TargetSelector.DamageType.Physical);
            var targetR = TargetSelector.GetTarget(SkillHandler.R.Range, TargetSelector.DamageType.Physical);
            var targetBlade = TargetSelector.GetTarget(ItemHandler.Blade.Range, TargetSelector.DamageType.Physical);
            var targetBilge = TargetSelector.GetTarget(ItemHandler.Bilgewater.Range, TargetSelector.DamageType.Physical);
            var targetYoumuu = TargetSelector.GetTarget(SkillHandler.E.Range + 100, TargetSelector.DamageType.Physical);
            var targetTiamat = TargetSelector.GetTarget(ItemHandler.Tiamat.Range, TargetSelector.DamageType.Physical);
            var targetHydra = TargetSelector.GetTarget(ItemHandler.Hydra.Range, TargetSelector.DamageType.Physical);

            //Items
            if (useBlade)
            {
                ItemHandler.Blade.Cast(targetBlade);
            }
            if (useBilge)
            {
                ItemHandler.Bilgewater.Cast(targetBilge);
            }
            if (useYoumuu && targetYoumuu.IsValid)
            {
                ItemHandler.Youmuu.Cast();
            }
            if (useTiamat && targetTiamat.IsValid)
            {
                ItemHandler.Tiamat.Cast();
            }
            if (useHydra && targetHydra.IsValid)
            {
                ItemHandler.Hydra.Cast();
            }
            if (ItemHandler.IgniteSlot != SpellSlot.Unknown &&
            Player.Spellbook.CanUseSpell(ItemHandler.IgniteSlot) == SpellState.Ready)
            {
                if (targetE.Health <= MathHandler.ComboDamage(targetE))
                {
                    Player.Spellbook.CastSpell(ItemHandler.IgniteSlot, targetE);
                }
            }

            //Spells
            if (useE)
            {
                SkillHandler.E.Cast(targetE);
            }
            if (useQ && targetQ.IsValid)
            {
                SkillHandler.Q.Cast();
            }
            if (useW)
            {
                SkillHandler.W.CastIfHitchanceEquals(targetW, HitChance.Medium);
            }
            if (useR && targetR.IsValid)
            {
                SkillHandler.R.Cast();
            }
        }

        public static void JungleClear()
        {
            var minions = MinionManager.GetMinions(
                Player.ServerPosition, SkillHandler.W.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var mana = Player.Mana >
                       Player.MaxMana * MenuHandler.TalonConfig.Item("JungleClearManaPercent").GetValue<Slider>().Value /
                       100;
            var useQ = MenuHandler.TalonConfig.SubMenu("ClearJ").Item("usejQ").GetValue<bool>() && SkillHandler.Q.IsReady();
            var useW = MenuHandler.TalonConfig.SubMenu("ClearJ").Item("usejW").GetValue<bool>() && SkillHandler.W.IsReady();
            var useHydra = MenuHandler.TalonConfig.SubMenu("ClearJ").Item("usejIt").GetValue<bool>() && ItemHandler.Hydra.IsReady();
            var useTiamat = MenuHandler.TalonConfig.SubMenu("ClearJ").Item("usejIt").GetValue<bool>() && ItemHandler.Tiamat.IsReady();

            if (mana)
            {
                if (useQ)
                {
                    foreach (var minion in minions)
                    {
                        if (minion.IsValidTarget(SkillHandler.Q.Range))
                        {
                            SkillHandler.Q.Cast();
                        }
                    }
                }

                if (useW)
                {
                    foreach (var minion in minions)
                    {
                        if (minion.IsValidTarget())
                        {
                            SkillHandler.W.Cast(minion.ServerPosition);
                        }
                    }
                }
            }

            if (useHydra)
            {
                 foreach (var minion in minions)
                    {
                        if (minion.IsValidTarget(ItemHandler.Hydra.Range))
                        {
                            ItemHandler.Hydra.Cast();
                        }
                    }
            }

            if (useTiamat)
            {
                foreach (var minion in minions)
                {
                    if (minion.IsValidTarget(ItemHandler.Tiamat.Range))
                    {
                        ItemHandler.Tiamat.Cast();
                    }
                }
            }
        }

        public static void LaneClear()
        {
            var mana = Player.Mana >
                       Player.MaxMana * MenuHandler.TalonConfig.Item("LaneClearManaPercent").GetValue<Slider>().Value /
                       100;
            var minions = MinionManager.GetMinions(
                Player.ServerPosition, SkillHandler.W.Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth);
            var useW = MenuHandler.TalonConfig.SubMenu("ClearL").Item("LaneClearW").GetValue<bool>() &&
                       SkillHandler.W.IsReady();
            var useQ = MenuHandler.TalonConfig.SubMenu("ClearL").Item("LaneClearQ").GetValue<bool>() &&
                       SkillHandler.Q.IsReady();
            var useHydra = MenuHandler.TalonConfig.SubMenu("ClearL").Item("uselIt").GetValue<bool>() &&
                           ItemHandler.Hydra.IsReady();
            var useTiamat = MenuHandler.TalonConfig.SubMenu("ClearL").Item("uselIt").GetValue<bool>() &&
               ItemHandler.Tiamat.IsReady();

            if (mana)
            {
                if (useQ)
                {
                    foreach (var minion in minions)
                    {
                        if (minion.IsValidTarget(SkillHandler.Q.Range))
                        {
                            SkillHandler.Q.Cast();
                        }
                    }
                }

                if (useW)
                {
                    foreach (var minion in minions)
                    {
                        if (minion.IsValidTarget())
                        {
                            SkillHandler.W.Cast(minion.ServerPosition);
                        }
                    }
                }
            }

            if (useHydra)
            {
                foreach (var minion in minions)
                {
                    if (minion.IsValidTarget(ItemHandler.Hydra.Range))
                    {
                        ItemHandler.Hydra.Cast();
                    }
                }
            }

            if (useTiamat)
            {
                foreach (var minion in minions)
                {
                    if (minion.IsValidTarget(ItemHandler.Tiamat.Range))
                    {
                        ItemHandler.Tiamat.Cast();
                    }
                }
            }
        }

        public static void KillSteal()
        {
            var useIgnite = MenuHandler.TalonConfig.Item("KSi").GetValue<bool>() &&
                            ItemHandler.IgniteSlot != SpellSlot.Unknown &&
                            Player.Spellbook.CanUseSpell(ItemHandler.IgniteSlot) == SpellState.Ready;
            var useQ = MenuHandler.TalonConfig.Item("KSq").GetValue<bool>() && SkillHandler.Q.IsReady();
            var useW = MenuHandler.TalonConfig.Item("KSw").GetValue<bool>() && SkillHandler.W.IsReady();

            foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(target => target.IsValidTarget(SkillHandler.W.Range)))
            {
                if (useIgnite && Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) >= target.Health)
                {
                    Player.Spellbook.CastSpell(ItemHandler.IgniteSlot, target);
                }

                if (useQ && SkillHandler.Q.GetDamage(target) + Player.GetAutoAttackDamage(target, true) >= target.Health && Player.Distance(target) <= SkillHandler.Q.Range)
                {
                    SkillHandler.Q.Cast();
                }

                if (useW && SkillHandler.W.GetDamage(target) >= target.Health && Player.Distance(target) <= SkillHandler.W.Range)
                {
                    SkillHandler.W.CastIfHitchanceEquals(target, HitChance.Medium);
                }
            }
        }

        public static void Harass()
        {
            var mana = Player.Mana >
                       Player.MaxMana * MenuHandler.TalonConfig.Item("HarassManaPercent").GetValue<Slider>().Value / 100;
            var useW = MenuHandler.TalonConfig.SubMenu("Harass").Item("haraW").GetValue<bool>() &&
                       SkillHandler.W.IsReady();
            var targetW = TargetSelector.GetTarget(SkillHandler.W.Range, TargetSelector.DamageType.Physical);

            if (mana)
            {
                if (useW)
                {
                    SkillHandler.W.CastIfHitchanceEquals(targetW, HitChance.Medium);
                }
           }

        }
    }
}