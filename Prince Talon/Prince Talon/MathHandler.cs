﻿using LeagueSharp;
using LeagueSharp.Common;

namespace Prince_Talon
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
                dmg += Player.GetSpellDamage(enemy, SpellSlot.Q) * 2;
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
                dmg += Player.GetSpellDamage(enemy, SpellSlot.R);
            }

            if (ObjectManager.Player.GetSpellSlot("SummonerIgnite") != SpellSlot.Unknown)
            {
                dmg += Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            }
            dmg += Player.GetAutoAttackDamage(enemy, true) * 4;

            return (float) dmg;
        }
    }
}
