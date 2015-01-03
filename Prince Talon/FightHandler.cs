using System.Linq;
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
            var target = TargetSelector.GetTarget(1200, TargetSelector.DamageType.Physical);
            var targetQ = target.Distance(Player.Position) < SkillHandler.Q.Range;
            var targetW = target.Distance(Player.Position) < SkillHandler.W.Range;
            var targetE = target.Distance(Player.Position) < SkillHandler.E.Range;
            var targetR = target.Distance(Player.Position) < SkillHandler.R.Range;
            var targetBlade = target.Distance(Player.Position) < ItemHandler.Blade.Range;
            var targetBilge = target.Distance(Player.Position) < ItemHandler.Bilgewater.Range;
            var targetYoumuu = target.Distance(Player.Position) < SkillHandler.E.Range + 100;
            var targetTiamat = target.Distance(Player.Position) < ItemHandler.Tiamat.Range;
            var targetHydra = target.Distance(Player.Position) < ItemHandler.Hydra.Range;

            //Items
            if (useBlade && targetBlade)
            {
                ItemHandler.Blade.Cast(target);
            }
            if (useBilge && targetBilge)
            {
                ItemHandler.Bilgewater.Cast(target);
            }
            if (useYoumuu && targetYoumuu)
            {
                ItemHandler.Youmuu.Cast();
            }
            if (useTiamat && targetTiamat)
            {
                ItemHandler.Tiamat.Cast();
            }
            if (useHydra && targetHydra)
            {
                ItemHandler.Hydra.Cast();
            }
            if (ItemHandler.IgniteSlot != SpellSlot.Unknown &&
            Player.Spellbook.CanUseSpell(ItemHandler.IgniteSlot) == SpellState.Ready)
            {
                if (target.Health <= MathHandler.ComboDamage(target))
                {
                    Player.Spellbook.CastSpell(ItemHandler.IgniteSlot, target);
                }
            }

            //Spells
            if (useE && targetE)
            {
                SkillHandler.E.Cast(target);
            }
            if (useQ && targetQ)
            {
                SkillHandler.Q.Cast();
            }
            if (useW && targetW)
            {
                SkillHandler.W.CastIfHitchanceEquals(target, HitChance.Medium);
            }
            if (useR && targetR)
            {
                SkillHandler.R.Cast();
            }
        }

        public static void JungleClear()
        {
            var useQ = MenuHandler.TalonConfig.SubMenu("ClearJ").Item("JungleClearQ").GetValue<bool>() &&
                                   SkillHandler.Q.IsReady();
            var useW = MenuHandler.TalonConfig.SubMenu("ClearJ").Item("JungleClearW").GetValue<bool>() &&
                       SkillHandler.W.IsReady();
            var useHydra = MenuHandler.TalonConfig.SubMenu("ClearJ").Item("uselIt").GetValue<bool>() &&
               ItemHandler.Hydra.IsReady();
            var useTiamat = MenuHandler.TalonConfig.SubMenu("ClearJ").Item("uselIt").GetValue<bool>() &&
               ItemHandler.Tiamat.IsReady();
            var minions = MinionManager.GetMinions(
                Player.ServerPosition, SkillHandler.W.Range, MinionTypes.All, MinionTeam.Neutral);
            var mana = Player.ManaPercentage() >
                       MenuHandler.TalonConfig.SubMenu("ClearJ").Item("JungleClearManaPercent").GetValue<Slider>().Value;
            if (mana)
            {
                foreach (var minion in minions)
                {
                    if (useQ && minion.IsValidTarget(SkillHandler.Q.Range))
                    {
                        SkillHandler.Q.Cast(Player);
                    }
                    if (useW && minion.IsValidTarget())
                    {
                        SkillHandler.W.Cast(minion.Position);
                    }
                }
            }

            foreach (var minion in minions)
            {
                if (useTiamat && minion.IsValidTarget(ItemHandler.Tiamat.Range))
                {
                    ItemHandler.Tiamat.Cast(Player);
                }
                if (useHydra && minion.IsValidTarget(ItemHandler.Hydra.Range))
                {
                    ItemHandler.Hydra.Cast(Player);
                }
            }
        }

        public static void LaneClear()
        {
            var useQ = MenuHandler.TalonConfig.SubMenu("ClearL").Item("LaneClearQ").GetValue<bool>() &&
                                   SkillHandler.Q.IsReady();
            var useW = MenuHandler.TalonConfig.SubMenu("ClearL").Item("LaneClearW").GetValue<bool>() &&
                       SkillHandler.W.IsReady();
            var useHydra = MenuHandler.TalonConfig.SubMenu("ClearL").Item("uselIt").GetValue<bool>() &&
               ItemHandler.Hydra.IsReady();
            var useTiamat = MenuHandler.TalonConfig.SubMenu("ClearL").Item("uselIt").GetValue<bool>() &&
               ItemHandler.Tiamat.IsReady();
            var minions = MinionManager.GetMinions(
                Player.ServerPosition, SkillHandler.W.Range, MinionTypes.All, MinionTeam.NotAlly);
            var wHit = SkillHandler.W.GetCircularFarmLocation(minions, SkillHandler.W.Width);
            var mana = Player.ManaPercentage() >
                       MenuHandler.TalonConfig.SubMenu("ClearL").Item("LaneClearManaPercent").GetValue<Slider>().Value;
            if (mana)
            {
                foreach (var minion in minions)
                {
                    if (useQ && minion.IsValidTarget(SkillHandler.Q.Range))
                    {
                        SkillHandler.Q.CastOnUnit(Player);
                    }
                    if (useW && minion.IsValidTarget() && wHit.MinionsHit >= 1)
                    {
                        SkillHandler.W.Cast(wHit.Position);
                    }
                }
            }

            foreach (var minion in minions)
            {
                if (useTiamat && minion.IsValidTarget(ItemHandler.Tiamat.Range) && minions.Count() > 1)
                {
                    ItemHandler.Tiamat.Cast(Player);
                }
                if (useHydra && minion.IsValidTarget(ItemHandler.Hydra.Range) && minions.Count() > 1)
                {
                    ItemHandler.Hydra.Cast(Player);
                }
            }
        }

        public static void Harass()
        {
            var mana = Player.ManaPercentage() > MenuHandler.TalonConfig.Item("HarassManaPercent").GetValue<Slider>().Value;
            var useW = MenuHandler.TalonConfig.SubMenu("Harass").Item("haraW").GetValue<bool>() &&
                       SkillHandler.W.IsReady();
            var target = TargetSelector.GetTarget(SkillHandler.W.Range, TargetSelector.DamageType.Physical);

            if (!mana || !MenuHandler.TalonConfig.SubMenu("Harass").Item("HarassToggle").GetValue<KeyBind>().Active)
            {
                return;
            }
            if (useW && SkillHandler.W.InRange(target))
            {
                SkillHandler.W.Cast(target.Position);
            }

        }
    }
}