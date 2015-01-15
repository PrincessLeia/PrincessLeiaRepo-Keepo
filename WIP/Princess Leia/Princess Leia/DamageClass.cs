using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_Leia
{
    internal class DamageClass
    {
        public static float ComboDamage(Obj_AI_Base enemy)
        {
            var ap = ObjectManager.Player.BaseAbilityDamage + ObjectManager.Player.FlatMagicDamageMod;
            var dmg = 0d;
            var player = ObjectManager.Player;

            if (SpellClass.Q.IsReady())
            {
                dmg += player.GetSpellDamage(enemy, SpellSlot.Q);
            }

            if (enemy.HasBuff("leblancchaosorb", true))
            {
                dmg += player.GetSpellDamage(enemy, SpellSlot.Q);
            }

            if (SpellClass.W.IsReady())
            {
                dmg += player.GetSpellDamage(enemy, SpellSlot.W);
            }

            if (SpellClass.E.IsReady())
            {
                dmg += player.GetSpellDamage(enemy, SpellSlot.E);
            }

            if (SpellClass.R.IsReady())
            {
                var name = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name;
                var maxDmg = new[] { 200, 400, 600 }[SpellClass.R.Level] + 1.3 * ap;

                switch (name)
                {
                    case "LeblancChaosOrbM":
                    {
                        var qDmg = new[] { 100, 200, 300 }[SpellClass.R.Level] + 0.65 * ap;

                        dmg += player.CalcDamage(enemy, Damage.DamageType.Magical, qDmg);

                        if (player.CalcDamage(enemy, Damage.DamageType.Magical, qDmg) > maxDmg)
                        {
                            dmg += player.CalcDamage(enemy, Damage.DamageType.Magical, maxDmg);
                        }
                        break;
                    }
                    case "LeblancSlideM":
                    {
                        var wDmg = new[] { 150, 300, 450 }[SpellClass.R.Level] + 0.975 * ap;
                        dmg += player.CalcDamage(enemy, Damage.DamageType.Magical, wDmg);
                        break;
                    }
                    case "LeblancSoulShackleM":
                    {
                        var eDmg = new[] { 100, 200, 300 }[SpellClass.R.Level] + 0.65 * ap;

                        dmg += player.CalcDamage(enemy, Damage.DamageType.Magical, eDmg);

                        if (player.CalcDamage(enemy, Damage.DamageType.Magical, eDmg) > maxDmg)
                        {
                            dmg += player.CalcDamage(enemy, Damage.DamageType.Magical, maxDmg);
                        }
                        break;
                    }
                }
            }

            if (enemy.HasBuff("leblancchaosorbm", true))
            {
                var qDmg = new[] { 100, 200, 300 }[SpellClass.R.Level] + 0.65 * ap;
                var maxDmg = new[] { 200, 400, 600 }[SpellClass.R.Level] + 1.3 * ap;

                dmg += player.CalcDamage(enemy, Damage.DamageType.Magical, qDmg);

                if (player.CalcDamage(enemy, Damage.DamageType.Magical, qDmg) > maxDmg)
                {
                    dmg += player.CalcDamage(enemy, Damage.DamageType.Magical, maxDmg);
                }
            }

            if (ItemClass.Dfg.IsReady())
            {
                dmg += player.GetItemDamage(enemy, Damage.DamageItems.Dfg);
                dmg = dmg * 1.2;
            }

            if (player.GetSpellSlot("SummonerIgnite") != SpellSlot.Unknown)
            {
                dmg += player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            }
            dmg += player.GetAutoAttackDamage(enemy, true) * 1;

            return (float)dmg;
        }
    }
}