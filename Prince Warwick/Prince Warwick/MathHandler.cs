﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Prince_Warwick
{
    class MathHandler
    {
        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        public static float ComboDamage(Obj_AI_Base enemy)
        {
            var dmg = 0d;

            if (ItemHandler.Blade.IsReady()) dmg += ObjectManager.Player.GetItemDamage(enemy, Damage.DamageItems.Botrk);

            if (SkillHandler.Q.IsReady()) dmg += Player.GetSpellDamage(enemy, SpellSlot.Q);

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
            dmg += Player.GetAutoAttackDamage(enemy, true) * 4;

            return (float)dmg;
        }
    }
}
