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
        private static Obj_AI_Hero Target
        {
            get { return TargetSelector.GetTarget(1200, TargetSelector.DamageType.Physical); }
        }
        public static bool PacketCast;


        public static void Combo()
        {
            var assassinRange = MenuHandler.TalonConfig.Item("AssassinRange").GetValue<Slider>().Value;
            Obj_AI_Hero vTarget = null;
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>()
                .Where(enemy => enemy.Team != Player.Team
                    && !enemy.IsDead && enemy.IsVisible
                    && MenuHandler.TalonConfig.Item("Assassin" + enemy.ChampionName) != null
                    && MenuHandler.TalonConfig.Item("Assassin" + enemy.ChampionName).GetValue<bool>())
                    .OrderBy(enemy => enemy.Distance(Game.CursorPos))
                    )
                if (MenuHandler.TalonConfig.SubMenu("Common_TargetSelector").SubMenu("AssassinManager").Item("AssassinActive").GetValue<bool>())
                {
                    vTarget = Player.Distance(enemy) < assassinRange ? enemy : null;
                }
                else if (!MenuHandler.TalonConfig.SubMenu("Common_TargetSelector").SubMenu("AssassinManager").Item("AssassinActive").GetValue<bool>())
                {
                    vTarget = Target;
                }

            var useW = SkillHandler.W.IsReady() && MenuHandler.TalonConfig.SubMenu("Combo").Item("useW").GetValue<bool>();
            var useE = SkillHandler.E.IsReady() && MenuHandler.TalonConfig.SubMenu("Combo").Item("useE").GetValue<bool>();
            var useR = SkillHandler.R.IsReady() && MenuHandler.TalonConfig.SubMenu("Combo").Item("useR").GetValue<bool>();
            var useBlade = ItemHandler.Blade.IsReady() && MenuHandler.TalonConfig.SubMenu("Items").Item("useBotrK").GetValue<bool>();
            var useBilge = ItemHandler.Bilgewater.IsReady() && MenuHandler.TalonConfig.SubMenu("Items").Item("useBilge").GetValue<bool>();
            var useYoumuu = ItemHandler.Youmuu.IsReady() && MenuHandler.TalonConfig.SubMenu("Items").Item("useYoumuu").GetValue<bool>();
            var useTiamat = ItemHandler.Tiamat.IsReady() && MenuHandler.TalonConfig.SubMenu("Items").Item("useTiamat").GetValue<bool>();
            var useHydra = ItemHandler.Hydra.IsReady() && MenuHandler.TalonConfig.SubMenu("Items").Item("useHydra").GetValue<bool>();
            var targetW = vTarget.Distance(Player.Position) < SkillHandler.W.Range;
            var targetE = vTarget.Distance(Player.Position) < SkillHandler.E.Range;
            var targetR = vTarget.Distance(Player.Position) < SkillHandler.R.Range;
            var targetBlade = vTarget.Distance(Player.Position) < ItemHandler.Blade.Range;
            var targetBilge = vTarget.Distance(Player.Position) < ItemHandler.Bilgewater.Range;
            var targetYoumuu = vTarget.Distance(Player.Position) < SkillHandler.E.Range + 100;
            var targetTiamat = vTarget.Distance(Player.Position) < ItemHandler.Tiamat.Range;
            var targetHydra = vTarget.Distance(Player.Position) < ItemHandler.Hydra.Range;

            //Items
            if (useBlade && targetBlade)
            {
                ItemHandler.Blade.Cast(vTarget);
            }
            if (useBilge && targetBilge)
            {
                ItemHandler.Bilgewater.Cast(vTarget);
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
                if (Target.Health <= MathHandler.ComboDamage(Target))
                {
                    Player.Spellbook.CastSpell(ItemHandler.IgniteSlot, vTarget);
                }
            }

            //Spells
            if (useE && targetE)
            {
                SkillHandler.E.Cast(vTarget, PacketCast);
            }
            if (useW && targetW)
            {
                SkillHandler.W.CastIfHitchanceEquals(vTarget, HitChance.Medium, PacketCast);
            }
            if (useR && targetR)
            {
                SkillHandler.R.Cast(PacketCast);
            }
        }

        public static void AfterAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            var targetq = Target.Distance(Player.Position) < SkillHandler.Q.Range;
            var useQ = SkillHandler.Q.IsReady() && MenuHandler.TalonConfig.SubMenu("Combo").Item("useQ").GetValue<bool>();
            var useQh = SkillHandler.Q.IsReady() && MenuHandler.TalonConfig.SubMenu("Harass").Item("haraQ").GetValue<bool>();

            if (MenuHandler.Orb.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (useQh && targetq && Player.ManaPercentage() > MenuHandler.TalonConfig.Item("HarassManaPercent").GetValue<Slider>().Value)
                {
                    SkillHandler.Q.Cast(PacketCast);
                }
            }
            if (MenuHandler.Orb.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (useQ && targetq)
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
            if (useW && SkillHandler.W.InRange(Target))
            {
                SkillHandler.W.Cast(Target.Position, PacketCast);
            }
            if (useE && SkillHandler.E.InRange(Target))
            {
                SkillHandler.E.Cast(Target, PacketCast);
            }

        }
    }
}