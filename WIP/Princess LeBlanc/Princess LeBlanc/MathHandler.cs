﻿using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_LeBlanc
{
    internal class MathHandler
    {
        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }

        public static float ComboDamage(Obj_AI_Base enemy)
        {
            var dmg = 0d;

            if (SkillHandler.Q.IsReady())
            {
                dmg += 70 + (50 * SkillHandler.Q.Level) + (0.8 * ObjectManager.Player.BaseAbilityDamage + ObjectManager.Player.FlatMagicDamageMod);
            }

            if (SkillHandler.W.IsReady())
            {
                dmg += Player.GetSpellDamage(enemy, SpellSlot.W) * 2;
            }

            if (SkillHandler.E.IsReady())
            {
                dmg += 30 + (50 * SkillHandler.E.Level) + ObjectManager.Player.BaseAbilityDamage + ObjectManager.Player.FlatMagicDamageMod;
            }

            if (SkillHandler.R.IsReady() && SkillHandler.Q.IsReady() && SkillHandler.W.IsReady() || SkillHandler.E.IsReady())
            {
                dmg += 70 + (50 * SkillHandler.Q.Level) + (0.8 * ObjectManager.Player.BaseAbilityDamage + ObjectManager.Player.FlatMagicDamageMod);
            }

            if (Items.HasItem(3128))
            {
                dmg += Player.GetItemDamage(enemy, Damage.DamageItems.Dfg);
                dmg = dmg * 1.2;
            }

            if (ObjectManager.Player.GetSpellSlot("SummonerIgnite") != SpellSlot.Unknown)
            {
                dmg += Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            }
            dmg += Player.GetAutoAttackDamage(enemy, true) * 2;

            return (float) dmg;
        }
    }
}
