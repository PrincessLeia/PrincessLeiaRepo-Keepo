﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_Diana
{
    class MathHandler
    {
        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        public static float ComboDamage(Obj_AI_Base enemy)
        {
            var dmg = 0d;

            if (SkillHandler.Q.IsReady()) dmg += Player.GetSpellDamage(enemy, SpellSlot.Q) * 2;

            if (SkillHandler.W.IsReady()) dmg += Player.GetSpellDamage(enemy, SpellSlot.W);

            if (SkillHandler.R.IsReady()) dmg += Player.GetSpellDamage(enemy, SpellSlot.R);

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

            if (Player.HasBuff("dianaarcready"))
            {
                dmg += 20 + 5 * ObjectManager.Player.Level;
            }

            return (float)dmg;
        }

        public static float GapCloseKill1Damage(Obj_AI_Base enemy)
        {
            var dmg = 0d;

            if (SkillHandler.Q.IsReady()) dmg += Player.GetSpellDamage(enemy, SpellSlot.Q);

            if (SkillHandler.R.IsReady()) dmg += Player.GetSpellDamage(enemy, SpellSlot.R);

            if (ObjectManager.Player.GetSpellSlot("SummonerIgnite") != SpellSlot.Unknown)
            {
                dmg += Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            }

            return (float)dmg;
        }

        public static float GapCloseKill2Damage(Obj_AI_Base enemy)
        {
            var dmg = 0d;

            if (SkillHandler.R.IsReady()) dmg += Player.GetSpellDamage(enemy, SpellSlot.R) *2;

            if (SkillHandler.W.IsReady()) dmg += Player.GetSpellDamage(enemy, SpellSlot.W);

            if (ObjectManager.Player.GetSpellSlot("SummonerIgnite") != SpellSlot.Unknown)
            {
                dmg += Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            }
            dmg += Player.GetAutoAttackDamage(enemy, true) * 2;

            return (float)dmg;
        }
    }
}
