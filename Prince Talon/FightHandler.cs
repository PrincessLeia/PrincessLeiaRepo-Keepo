using System;
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
        private static Obj_AI_Hero target
        {
            get { return TargetSelector.GetTarget(1200, TargetSelector.DamageType.Physical); }
        }

        public static bool PacketCast;

        static Obj_AI_Hero GetEnemy(float vDefaultRange = 0, TargetSelector.DamageType vDefaultDamageType = TargetSelector.DamageType.Physical)
        {
            if (Math.Abs(vDefaultRange) < 0.00001)
                vDefaultRange = SkillHandler.Q.Range;

            if (!MenuHandler.TalonConfig.Item("AssassinActive").GetValue<bool>())
                return TargetSelector.GetTarget(vDefaultRange, vDefaultDamageType);

            var assassinRange = MenuHandler.TalonConfig.Item("AssassinSearchRange").GetValue<Slider>().Value;

            var vEnemy = ObjectManager.Get<Obj_AI_Hero>()
                .Where(
                    enemy =>
                        enemy.Team != ObjectManager.Player.Team && !enemy.IsDead && enemy.IsVisible &&
                        MenuHandler.TalonConfig.Item("Assassin" + enemy.ChampionName) != null &&
                        MenuHandler.TalonConfig.Item("Assassin" + enemy.ChampionName).GetValue<bool>() &&
                        ObjectManager.Player.Distance(enemy) < assassinRange);

            if (MenuHandler.TalonConfig.Item("AssassinSelectOption").GetValue<StringList>().SelectedIndex == 1)
            {
                vEnemy = (from vEn in vEnemy select vEn).OrderByDescending(vEn => vEn.MaxHealth);
            }

            Obj_AI_Hero[] objAiHeroes = vEnemy as Obj_AI_Hero[] ?? vEnemy.ToArray();

            Obj_AI_Hero t = !objAiHeroes.Any()
                ? TargetSelector.GetTarget(vDefaultRange, vDefaultDamageType)
                : objAiHeroes[0];

            return t;
        }

        public static void Combo()
        {
            var t = GetEnemy(SkillHandler.E.Range, TargetSelector.DamageType.Magical);
            var useW = SkillHandler.W.IsReady() && MenuHandler.TalonConfig.SubMenu("Combo").Item("useW").GetValue<bool>() && SkillHandler.Q.IsInRange(t);
            var useE = SkillHandler.E.IsReady() && MenuHandler.TalonConfig.SubMenu("Combo").Item("useE").GetValue<bool>() && SkillHandler.W.IsInRange(t);
            var useR = SkillHandler.R.IsReady() && MenuHandler.TalonConfig.SubMenu("Combo").Item("useR").GetValue<bool>() && SkillHandler.E.IsInRange(t);
            var useBlade = ItemHandler.Blade.IsReady() && MenuHandler.TalonConfig.SubMenu("Items").Item("useBotrK").GetValue<bool>() && ItemHandler.Blade.IsInRange(t);
            var useBilge = ItemHandler.Bilgewater.IsReady() && MenuHandler.TalonConfig.SubMenu("Items").Item("useBilge").GetValue<bool>() && ItemHandler.Bilgewater.IsInRange(t);
            var useYoumuu = ItemHandler.Youmuu.IsReady() && MenuHandler.TalonConfig.SubMenu("Items").Item("useYoumuu").GetValue<bool>() && t.Distance(Player.Position) < SkillHandler.E.Range + 100;
            var useTiamat = ItemHandler.Tiamat.IsReady() && MenuHandler.TalonConfig.SubMenu("Items").Item("useTiamat").GetValue<bool>() && ItemHandler.Tiamat.IsInRange(t);
            var useHydra = ItemHandler.Hydra.IsReady() && MenuHandler.TalonConfig.SubMenu("Items").Item("useHydra").GetValue<bool>() && ItemHandler.Hydra.IsInRange(t);

            //Items
            if (useBlade)
            {
                ItemHandler.Blade.Cast(t);
            }
            if (useBilge)
            {
                ItemHandler.Bilgewater.Cast(t);
            }
            if (useYoumuu)
            {
                ItemHandler.Youmuu.Cast();
            }
            if (useTiamat)
            {
                ItemHandler.Tiamat.Cast();
            }
            if (useHydra)
            {
                ItemHandler.Hydra.Cast();
            }
            if (ItemHandler.IgniteSlot != SpellSlot.Unknown &&
            Player.Spellbook.CanUseSpell(ItemHandler.IgniteSlot) == SpellState.Ready)
            {
                if (t.Health <= MathHandler.ComboDamage(t))
                {
                    Player.Spellbook.CastSpell(ItemHandler.IgniteSlot, t);
                }
            }

            //Spells
            if (useE)
            {
                SkillHandler.E.Cast(t, PacketCast);
            }
            if (useW)
            {
                SkillHandler.W.CastIfHitchanceEquals(t, HitChance.Medium, PacketCast);
            }
            if (useR)
            {
                SkillHandler.R.Cast(PacketCast);
            }
        }

        public static void AfterAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            var Target = TargetSelector.GetTarget(SkillHandler.Q.Range, TargetSelector.DamageType.Physical);
            var useQ = SkillHandler.Q.IsReady() && MenuHandler.TalonConfig.SubMenu("Combo").Item("useQ").GetValue<bool>() && SkillHandler.Q.IsInRange(Target);
            var useQh = SkillHandler.Q.IsReady() && MenuHandler.TalonConfig.SubMenu("Harass").Item("haraQ").GetValue<bool>() && SkillHandler.Q.IsInRange(Target);

            if (MenuHandler.Orb.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (useQh && Player.ManaPercentage() > MenuHandler.TalonConfig.Item("HarassManaPercent").GetValue<Slider>().Value)
                {
                    SkillHandler.Q.Cast(PacketCast);
                }
            }
            if (MenuHandler.Orb.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (useQ)
                {
                    SkillHandler.Q.Cast(PacketCast);
                }
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
            var farmLocation = MinionManager.GetBestCircularFarmLocation(MinionManager.GetMinions(SkillHandler.W.Range, MinionTypes.All, MinionTeam.Enemy).Select(m => m.ServerPosition.To2D()).ToList(), SkillHandler.W.Width, SkillHandler.W.Range);
            if (mana)
            {
                foreach (var minion in minions)
                {
                    if (useQ && minion.IsValidTarget(SkillHandler.Q.Range))
                    {
                        SkillHandler.Q.Cast(Player, PacketCast);
                    }
                }
                if (farmLocation.MinionsHit >= 1 && useW)
                {
                    SkillHandler.W.Cast(farmLocation.Position, PacketCast);
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
            var farmLocation = MinionManager.GetBestCircularFarmLocation(MinionManager.GetMinions(SkillHandler.W.Range, MinionTypes.All, MinionTeam.Enemy).Select(m => m.ServerPosition.To2D()).ToList(), SkillHandler.W.Width, SkillHandler.W.Range);
            var minionHit = farmLocation.MinionsHit >=
                            MenuHandler.TalonConfig.SubMenu("ClearL").Item("LaneClearWHit").GetValue<Slider>().Value;
            var mana = Player.ManaPercentage() >
                       MenuHandler.TalonConfig.SubMenu("ClearL").Item("LaneClearManaPercent").GetValue<Slider>().Value;
            if (mana)
            {
                foreach (var minion in minions)
                {
                    if (useQ && minion.IsValidTarget(SkillHandler.Q.Range))
                    {
                        SkillHandler.Q.Cast(Player, PacketCast);
                    }
                    if (useW && minion.IsValidTarget() && minionHit)
                    {
                        SkillHandler.W.Cast(farmLocation.Position, PacketCast);
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
            var target = TargetSelector.GetTarget(SkillHandler.E.Range, TargetSelector.DamageType.Physical);
            var mana = Player.ManaPercentage() > MenuHandler.TalonConfig.Item("HarassManaPercent").GetValue<Slider>().Value;
            var useW = MenuHandler.TalonConfig.SubMenu("Harass").Item("haraW").GetValue<bool>() &&
                       SkillHandler.W.IsReady();
            var useE = MenuHandler.TalonConfig.SubMenu("Harass").Item("haraE").GetValue<bool>() &&
                       SkillHandler.E.IsReady();

            if (!mana)
            {
                return;
            }
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (useW && SkillHandler.W.IsInRange(target))
            {
                SkillHandler.W.Cast(target.Position, PacketCast);
            }
            if (useE && SkillHandler.E.IsInRange(target))
            {
                SkillHandler.E.Cast(target, PacketCast);
            }

        }
    }
}