using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Prince_Talon
{
    class FightHandler
    {
        private static Obj_AI_Hero Player
            {
              get { return ObjectManager.Player; }
            }



        public static void Combo()
        {
            var target = TargetSelector.GetTarget(SkillHandler.E.Range, TargetSelector.DamageType.Physical);

            if (Player.Distance(target) <= ItemHandler.Blade.Range &&

                MenuHandler.TalonConfig.Item("useBotrK").GetValue<bool>() && ItemHandler.Blade.IsReady())
            {
                ItemHandler.Blade.Cast(target);
            }

            if (Player.Distance(target) <= ItemHandler.Bilgewater.Range &&
                MenuHandler.TalonConfig.Item("useBilge").GetValue<bool>() && ItemHandler.Bilgewater.IsReady()) 
                {
                    ItemHandler.Bilgewater.Cast(target);
                }

            if (Player.Distance(target) <= SkillHandler.E.Range + 100 &&
            MenuHandler.TalonConfig.Item("useYoumuu").GetValue<bool>() && ItemHandler.Youmuu.IsReady())
            {
                ItemHandler.Youmuu.Cast();
            }

            if (Player.Distance(target) <= ItemHandler.Tiamat.Range &&
            MenuHandler.TalonConfig.Item("useTiamat").GetValue<bool>() && ItemHandler.Tiamat.IsReady())
            {
                ItemHandler.Tiamat.Cast();
            }

            if (Player.Distance(target) <= ItemHandler.Hydra.Range &&
            MenuHandler.TalonConfig.Item("useHydra").GetValue<bool>() && ItemHandler.Hydra.IsReady())
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

            if (MenuHandler.TalonConfig.Item("useE").GetValue<bool>() &&
                Player.Distance(target) <= SkillHandler.E.Range &&
               SkillHandler.E.IsReady())
            {
                SkillHandler.E.Cast(target, Packeting());
            }

            if (MenuHandler.TalonConfig.Item("useQ").GetValue<bool>() && Orbwalking.InAutoAttackRange(target)&& SkillHandler.Q.IsReady())
            {
                SkillHandler.Q.Cast(Packeting());
            }

            if (MenuHandler.TalonConfig.Item("useW").GetValue<bool>() &&
               Player.Distance(target) <= SkillHandler.W.Range &&
              SkillHandler.W.IsReady())
            {
                SkillHandler.W.Cast(target, Packeting());
            }

            if (MenuHandler.TalonConfig.Item("useR").GetValue<bool>() &&
               Player.Distance(target) <= SkillHandler.R.Range &&
              SkillHandler.R.IsReady() && target.Health <= SkillHandler.R.GetDamage(target) || target.Health <= MathHandler.ComboDamage(target))
            {
                SkillHandler.R.Cast(Packeting());
            }

        }

        public static void JungleClear()
        {
            var minions = MinionManager.GetMinions(
                Player.ServerPosition, SkillHandler.W.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (Player.Mana >
                Player.MaxMana*MenuHandler.TalonConfig.Item("JungleClearManaPercent").GetValue<Slider>().Value/
                100)
            {
                if (MenuHandler.TalonConfig.Item("usejQ").GetValue<bool>() && SkillHandler.Q.IsReady())
                {
                    foreach (var minion in minions)
                    {
                        if (minion.IsValidTarget(SkillHandler.Q.Range))
                        {
                            SkillHandler.Q.Cast(Packeting());
                        }
                    }
                }

                if (MenuHandler.TalonConfig.Item("usejW").GetValue<bool>() && SkillHandler.W.IsReady())
                {
                    foreach (var minion in minions)
                    {
                        if (minion.IsValidTarget())
                        {
                            SkillHandler.W.Cast(minion.ServerPosition, Packeting());
                        }
                    }
                }
            }

            if (MenuHandler.TalonConfig.Item("usejIt").GetValue<bool>() && ItemHandler.Hydra.IsReady())
            {
                foreach (var minion in minions.Where(minion => Player.Distance(minion) < ItemHandler.Hydra.Range))
                {
                        ItemHandler.Hydra.Cast();
                }
            }

            if (MenuHandler.TalonConfig.Item("usejIt").GetValue<bool>() && ItemHandler.Tiamat.IsReady())
            {
                foreach (var minion in minions.Where(minion => Player.Distance(minion) < ItemHandler.Tiamat.Range))
                    {
                        ItemHandler.Tiamat.Cast();
                    }
            }
        }

        public static void LaneClear()
        {
            if (MenuHandler.TalonConfig.Item("LaneClearW").GetValue<bool>())
            {
                if (Player.Mana >
                    Player.MaxMana * MenuHandler.TalonConfig.Item("LaneClearManaPercent").GetValue<Slider>().Value /
                    100)
                {

                    var myMinions = MinionManager.GetMinions(
                        Player.ServerPosition, SkillHandler.W.Range, MinionTypes.All, MinionTeam.NotAlly);

                    if (SkillHandler.W.IsReady())
                    {
                        foreach (
                            var minion in
                                myMinions.Where(minion => minion.IsValidTarget())
                                    .Where(minion => minion.IsValidTarget(Player.AttackRange)))
                            {
                                SkillHandler.W.Cast(minion.ServerPosition, Packeting());
                            }
                        }
                    }
                }

                if (MenuHandler.TalonConfig.Item("LaneClearQ").GetValue<bool>())
                {
                    if (Player.Mana >
                        Player.MaxMana * MenuHandler.TalonConfig.Item("LaneClearManaPercent").GetValue<Slider>().Value /
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
                                    SkillHandler.Q.Cast(Packeting());
                            }
                        }
                    }

                var minions = MinionManager.GetMinions(
                           Player.ServerPosition, Player.AttackRange, MinionTypes.All, MinionTeam.NotAlly);
            if (MenuHandler.TalonConfig.Item("uselIt").GetValue<bool>() && ItemHandler.Hydra.IsReady())
            {
                foreach (var minion in minions.Where(minion => Player.Distance(minion) < ItemHandler.Hydra.Range))
                {
                        ItemHandler.Hydra.Cast();
                }
            }

            if (MenuHandler.TalonConfig.Item("uselIt").GetValue<bool>() && ItemHandler.Tiamat.IsReady())
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
                    ObjectManager.Get<Obj_AI_Hero>().Where(target => target.IsValidTarget(SkillHandler.W.Range)))
            {
                if (MenuHandler.TalonConfig.Item("KSi").GetValue<bool>() &&
                    ItemHandler.IgniteSlot != SpellSlot.Unknown &&
                    Player.Spellbook.CanUseSpell(ItemHandler.IgniteSlot) == SpellState.Ready &&
                    Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) >= target.Health)
                {
                    Player.Spellbook.CastSpell(ItemHandler.IgniteSlot, target);
                }

                if (MenuHandler.TalonConfig.Item("KSq").GetValue<bool>() && SkillHandler.Q.IsReady() &&
                    SkillHandler.Q.GetDamage(target) >= target.Health && Player.Distance(target) <= ObjectManager.Player.AttackRange)
                {
                    SkillHandler.Q.Cast(Packeting());
                }

                if (MenuHandler.TalonConfig.Item("KSw").GetValue<bool>() && SkillHandler.W.IsReady() &&
                    SkillHandler.W.GetDamage(target) >= target.Health && Player.Distance(target) <= SkillHandler.W.Range)
                {
                    SkillHandler.W.Cast(target.ServerPosition, Packeting());
                }
                
            }
        }

        public static void Harass()
        {
            if (Player.Mana >
                Player.MaxMana*MenuHandler.TalonConfig.Item("HarassManaPercent").GetValue<Slider>().Value/
                100)
            {
                if (MenuHandler.TalonConfig.Item("haraQ").GetValue<KeyBind>().Active)
                {
                    var target = TargetSelector.GetTarget(ObjectManager.Player.AttackRange, TargetSelector.DamageType.Physical);
                    if (SkillHandler.Q.IsReady() && Player.Distance(target) <= SkillHandler.Q.Range)
                    {
                        SkillHandler.Q.Cast(Packeting());
                    }
                }

                if (MenuHandler.TalonConfig.Item("haraW").GetValue<KeyBind>().Active)
                {
                    var target = TargetSelector.GetTarget(SkillHandler.W.Range, TargetSelector.DamageType.Physical);
                    if (SkillHandler.W.IsReady() && Player.Distance(target) <= SkillHandler.W.Range)
                    {
                        SkillHandler.W.Cast(target.ServerPosition, Packeting());
                    }
                }
            }
        }

        public static bool Packeting()
        {
            return MenuHandler.TalonConfig.Item("packets").GetValue<bool>();
        }

        public static void AntiGapCloser(ActiveGapcloser gapcloser)
        {
           if (SkillHandler.W.IsReady() && gapcloser.Sender.IsValidTarget(SkillHandler.W.Range) &&
               MenuHandler.TalonConfig.Item("gapcloW").GetValue<bool>())
            {
               SkillHandler.W.Cast(gapcloser.Sender.ServerPosition);
            }
        }


    }
}