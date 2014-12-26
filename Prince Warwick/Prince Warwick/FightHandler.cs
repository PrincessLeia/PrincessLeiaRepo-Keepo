using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Prince_Warwick
{
    class FightHandler
    {
        private static Obj_AI_Hero Player
            {
              get { return ObjectManager.Player; }
            }



        public static void Combo()
        {
            var target = TargetSelector.GetTarget(SkillHandler.Q.Range, TargetSelector.DamageType.Magical);
            var dfg = ItemHandler.Dfg;

            if (Player.Distance(target) <= ItemHandler.Blade.Range &&

                MenuHandler.WarwickConfig.Item("useBotrK").GetValue<bool>() && ItemHandler.Blade.IsReady())
            {
                ItemHandler.Blade.Cast(target);
            }

            if (Player.Distance(target) <= ItemHandler.Bilgewater.Range &&
                MenuHandler.WarwickConfig.Item("useBilge").GetValue<bool>() && ItemHandler.Bilgewater.IsReady()) 
                {
                    ItemHandler.Bilgewater.Cast(target);
                }

            if (Player.Distance(target) <= SkillHandler.Q.Range + 100 &&
            MenuHandler.WarwickConfig.Item("useYoumuu").GetValue<bool>() && ItemHandler.Youmuu.IsReady())
            {
                ItemHandler.Youmuu.Cast();
            }

            if (Player.Distance(target) <= ItemHandler.Tiamat.Range &&
            MenuHandler.WarwickConfig.Item("useTiamat").GetValue<bool>() && ItemHandler.Tiamat.IsReady())
            {
                ItemHandler.Tiamat.Cast();
            }

            if (Player.Distance(target) <= ItemHandler.Hydra.Range &&
            MenuHandler.WarwickConfig.Item("useHydra").GetValue<bool>() && ItemHandler.Hydra.IsReady())
            {
                ItemHandler.Hydra.Cast();
            }

            if (Player.Distance(target) <= dfg.Range && MenuHandler.WarwickConfig.Item("useDfg").GetValue<bool>() &&
            dfg.IsReady() && target.Health <= MathHandler.ComboDamage(target))
            {
                dfg.Cast(target);
            }

            if (ItemHandler.IgniteSlot != SpellSlot.Unknown &&
            Player.Spellbook.CanUseSpell(ItemHandler.IgniteSlot) == SpellState.Ready)
            {
                if (target.Health <= MathHandler.ComboDamage(target))
                {
                    Player.Spellbook.CastSpell(ItemHandler.IgniteSlot, target);
                }
            }

            if (MenuHandler.WarwickConfig.Item("notULT").GetValue<bool>() &&
                target.InventoryItems.Equals(ItemHandler.Qss) || target.InventoryItems.Equals(ItemHandler.Mercurial))
            {
                if (MenuHandler.WarwickConfig.Item("useR").GetValue<bool>() &&
                    target.Health <= MathHandler.ComboDamage(target) && Player.Distance(target) <= SkillHandler.R.Range &&
                    SkillHandler.R.IsReady())
                {
                    SkillHandler.R.Cast(target, Packeting());
                }
            }

            if (MenuHandler.WarwickConfig.Item("useQ").GetValue<bool>() &&
                Player.Distance(target) <= SkillHandler.Q.Range && SkillHandler.Q.IsReady())
            {
                SkillHandler.Q.Cast(target, Packeting());
            }

            if (MenuHandler.WarwickConfig.Item("useW").GetValue<bool>() && Orbwalking.InAutoAttackRange(target) &&
                SkillHandler.W.IsReady())
            {
                SkillHandler.W.Cast(target, Packeting());
            }
        }

        public static void UltonClick()
        {
            var target = Hud.SelectedUnit;

            if (target.Type != GameObjectType.obj_AI_Hero || !((Obj_AI_Hero) target).IsValidTarget())
            {
                return;
            }

            if (MenuHandler.WarwickConfig.Item("autoULT").GetValue<KeyBind>().Active &&
            Player.Distance(target.Position) <= SkillHandler.R.Range && SkillHandler.R.IsReady())
            {
                SkillHandler.R.Cast(target.Position, Packeting());
            }
        }

        public static void JungleClear()
        {
            var minions = MinionManager.GetMinions(
                Player.ServerPosition, 400, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (Player.Mana >
                Player.MaxMana*MenuHandler.WarwickConfig.Item("JungleClearManaPercent").GetValue<Slider>().Value/
                100)
            {
                if (MenuHandler.WarwickConfig.Item("usejQ").GetValue<bool>() && SkillHandler.Q.IsReady())
                {
                    foreach (var minion in minions)
                    {
                        if (minion.IsValidTarget())
                        {
                            SkillHandler.Q.Cast(minion, Packeting());
                        }
                    }
                }

                if (MenuHandler.WarwickConfig.Item("usejW").GetValue<bool>() && SkillHandler.W.IsReady())
                {
                    foreach (var minion in minions)
                    {
                        if (minion.IsValidTarget())
                        {
                            SkillHandler.W.Cast();
                        }
                    }
                }
            }

            if (MenuHandler.WarwickConfig.Item("usejIt").GetValue<bool>() && ItemHandler.Hydra.IsReady())
            {
                foreach (var minion in minions.Where(minion => Player.Distance(minion) < ItemHandler.Hydra.Range))
                {
                        ItemHandler.Hydra.Cast();
                }
            }

            if (MenuHandler.WarwickConfig.Item("usejIt").GetValue<bool>() && ItemHandler.Tiamat.IsReady())
            {
                foreach (var minion in minions.Where(minion => Player.Distance(minion) < ItemHandler.Tiamat.Range))
                    {
                        ItemHandler.Tiamat.Cast();
                    }
            }
        }

        public static void LaneClear()
        {
            if (MenuHandler.WarwickConfig.Item("LaneClearW").GetValue<bool>())
            {
                if (Player.Mana >
                    Player.MaxMana * MenuHandler.WarwickConfig.Item("LaneClearManaPercent").GetValue<Slider>().Value /
                    100)
                {

                    var myMinions = MinionManager.GetMinions(
                        Player.ServerPosition, Player.AttackRange, MinionTypes.All, MinionTeam.NotAlly);

                    if (SkillHandler.W.IsReady())
                    {
                        foreach (
                            var minion in
                                myMinions.Where(minion => minion.IsValidTarget())
                                    .Where(minion => minion.IsValidTarget(Player.AttackRange)))
                            {
                                SkillHandler.W.Cast(Packeting());
                            }
                        }
                    }
                }

                if (MenuHandler.WarwickConfig.Item("LaneClearQ").GetValue<bool>())
                {
                    if (Player.Mana >
                        Player.MaxMana * MenuHandler.WarwickConfig.Item("LaneClearManaPercent").GetValue<Slider>().Value /
                        100)
                    {
                        var myMinions = MinionManager.GetMinions(
                             Player.ServerPosition, Player.AttackRange, MinionTypes.All, MinionTeam.NotAlly);

                        if (SkillHandler.Q.IsReady())
                        {
                            foreach (
                                var minion in
                                    myMinions.Where(minion => minion.IsValidTarget())
                                        .Where(minion => minion.IsValidTarget(SkillHandler.Q.Range)))
                                    SkillHandler.Q.Cast(minion, Packeting());
                            }
                        }
                    }

                var minions = MinionManager.GetMinions(
                           Player.ServerPosition, Player.AttackRange, MinionTypes.All, MinionTeam.NotAlly);
            if (MenuHandler.WarwickConfig.Item("uselIt").GetValue<bool>() && ItemHandler.Hydra.IsReady())
            {
                foreach (var minion in minions.Where(minion => Player.Distance(minion) < ItemHandler.Hydra.Range))
                {
                        ItemHandler.Hydra.Cast();
                }
            }

            if (MenuHandler.WarwickConfig.Item("uselIt").GetValue<bool>() && ItemHandler.Tiamat.IsReady())
            {
               foreach (var minion in minions.Where(minion => Player.Distance(minion) < ItemHandler.Tiamat.Range))
                    {
                        ItemHandler.Tiamat.Cast();
                    }
            }
        }

        public static void KillSteal()
        {
            foreach (var target in
                    ObjectManager.Get<Obj_AI_Hero>().Where(target => target.IsValidTarget(SkillHandler.Q.Range)))
            {
                if (MenuHandler.WarwickConfig.Item("KSi").GetValue<bool>() &&
                    ItemHandler.IgniteSlot != SpellSlot.Unknown &&
                    Player.Spellbook.CanUseSpell(ItemHandler.IgniteSlot) == SpellState.Ready &&
                    Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) >= target.Health)
                {
                    Player.Spellbook.CastSpell(ItemHandler.IgniteSlot, target);
                }

                if (MenuHandler.WarwickConfig.Item("KSq").GetValue<bool>() && SkillHandler.Q.IsReady() &&
                    SkillHandler.Q.GetDamage(target) >= target.Health && Player.Distance(target) <= SkillHandler.Q.Range)
                {
                    SkillHandler.Q.Cast(target, Packeting());
                }
                
            }
        }

        public static bool Packeting()
        {
            return MenuHandler.WarwickConfig.Item("packets").GetValue<bool>();
        }

        public static void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (SkillHandler.R.IsReady() && unit.IsEnemy && unit.IsValidTarget(SkillHandler.R.Range) &&
                MenuHandler.WarwickConfig.Item("InterR").GetValue<bool>())
            {
                SkillHandler.R.Cast(unit, Packeting());
            }
        }

        public static void AntiGapCloser(ActiveGapcloser gapcloser)
        {
           if (ItemHandler.Randuin.IsReady() && gapcloser.Sender.IsValidTarget(ItemHandler.Randuin.Range) &&
               MenuHandler.WarwickConfig.Item("gapcloR").GetValue<bool>())
            {
                ItemHandler.Randuin.Cast();
            }
        }


    }
}