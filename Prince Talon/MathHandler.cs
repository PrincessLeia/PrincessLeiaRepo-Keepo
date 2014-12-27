﻿using LeagueSharp;
using LeagueSharp.Common;

namespace PrinceTalon
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

            if (ItemHandler.Blade.IsReady())
            {
                dmg += ObjectManager.Player.GetItemDamage(enemy, Damage.DamageItems.Botrk);
            }

            if (SkillHandler.Q.IsReady())
            {
                dmg += 40 * SkillHandler.Q.Level + ((ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod) * 1.3) + Player.GetAutoAttackDamage(enemy, true);
            }

            if (SkillHandler.W.IsReady())
            {
                dmg += Player.GetSpellDamage(enemy, SpellSlot.W);
            }

            if (SkillHandler.E.IsReady())
            {
                dmg += Player.GetSpellDamage(enemy, SpellSlot.E);
            }

            if (SkillHandler.R.IsReady())
            {
                dmg += 140 + (100 * SkillHandler.R.Level) + ((ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod) * 1.5);
            }

            if (ObjectManager.Player.GetSpellSlot("SummonerIgnite") != SpellSlot.Unknown)
            {
                dmg += Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            }

            dmg += Player.GetAutoAttackDamage(enemy, true);

            return (float) dmg;
        }
    }
}
