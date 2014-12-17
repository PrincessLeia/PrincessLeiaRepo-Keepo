using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_LeBlanc
{
    internal class FightHandler
    {
        private static Obj_AI_Hero Player
            {
              get { return ObjectManager.Player; }
            }

        public static void Combo()
        {
            var target = SimpleTs.GetTarget(SkillHandler.E.Range, SimpleTs.DamageType.Magical);
            var qu = SkillHandler.Q.IsReady() && Player.Distance(target) <= SkillHandler.Q.Range &&
                     MenuHandler.LeBlancConfig.Item("useQ").GetValue<bool>();
            var wu = SkillHandler.W.IsReady() && Player.Distance(target) <= SkillHandler.W.Range &&
                     MenuHandler.LeBlancConfig.Item("useW").GetValue<bool>();
            var wru = SkillHandler.W.IsChargedSpell;
            var eu = SkillHandler.E.IsReady() && Player.Distance(target) <= SkillHandler.E.Range &&
                     MenuHandler.LeBlancConfig.Item("useE").GetValue<bool>();
            var ru = SkillHandler.R.IsReady() && !SkillHandler.R.IsChargedSpell && Player.Distance(target) <= SkillHandler.Q.Range &&
                     MenuHandler.LeBlancConfig.Item("useR").GetValue<bool>();
            if (!Player.Level.Equals(6))
            {
                if (qu)
                {
                    SkillHandler.Q.Cast(target, Packeting());
                }

                if (wu)
                {
                    SkillHandler.W.Cast(target.ServerPosition, Packeting());
                }

                if (eu)
                {
                    SkillHandler.E.Cast(target.ServerPosition, Packeting());
                }

                if (wru)
                {
                    SkillHandler.W.Cast(Packeting());
                }
            }

            else
            {
            if (target.Health <= MathHandler.ComboDamage(target) && ItemHandler.Dfg.IsReady() &&
                Player.Distance(target) <= ItemHandler.Dfg.Range)
            {
                ItemHandler.Dfg.Cast(target);
            }

            if (target.Health <= MathHandler.ComboDamage(target) && ItemHandler.IgniteSlot != SpellSlot.Unknown &&
                Player.Spellbook.CanUseSpell(ItemHandler.IgniteSlot) == SpellState.Ready)
            {
                Player.Spellbook.CastSpell(ItemHandler.IgniteSlot, target);
            }

            if (SkillHandler.W.IsReady() && Player.Distance(target) <= SkillHandler.W.Range * 2 &&
                     MenuHandler.LeBlancConfig.Item("useW").GetValue<bool>())
            {
                SkillHandler.W.Cast(target.ServerPosition, Packeting());
            }

            if (eu)
            {
                SkillHandler.E.Cast(target.ServerPosition, Packeting());
            }

            if (qu)
            {
                SkillHandler.Q.Cast(target, Packeting());
            }

            if (ru)
            {
                SkillHandler.R.Cast(Packeting());
                SkillHandler.W.Cast(target.ServerPosition, Packeting());
            }

            if (wru)
            {
                SkillHandler.W.Cast(Packeting());
            }
            }
    }
        public static void KillSteal()
        {
            foreach (var target in
                    ObjectManager.Get<Obj_AI_Hero>().Where(target => target.IsValidTarget(SkillHandler.Q.Range)))
            {
                if (MenuHandler.LeBlancConfig.Item("KSi").GetValue<bool>() &&
                    ItemHandler.IgniteSlot != SpellSlot.Unknown &&
                    Player.Spellbook.CanUseSpell(ItemHandler.IgniteSlot) == SpellState.Ready &&
                    Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) >= target.Health)
                {
                    Player.Spellbook.CastSpell(ItemHandler.IgniteSlot, target);
                }

                if (MenuHandler.LeBlancConfig.Item("KSq").GetValue<bool>() && SkillHandler.Q.IsReady() &&
                    SkillHandler.Q.GetDamage(target) >= target.Health && Player.Distance(target) <= SkillHandler.Q.Range)
                {
                    SkillHandler.Q.Cast(target, Packeting());
                }
                
            }
        }

        public static bool Packeting()
        {
            return MenuHandler.LeBlancConfig.Item("packets").GetValue<bool>();
        }
    }
}