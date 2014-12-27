using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using System.Collections.Generic;

namespace Prince_Urgot
{
    internal class ItemHandler
    {
        public static Items.Item Muramana;
        public static SpellSlot IgniteSlot;
        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        public static void Init()
        {
            IgniteSlot = Player.GetSpellSlot("SummonerDot");
            Muramana = new Items.Item(3042, 0);
        }
        public static float GetIgniteDamage(Obj_AI_Hero enemy)
        {
            if (IgniteSlot == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(IgniteSlot) != SpellState.Ready) return 0f;
            return (float)Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
        }
    }
}
