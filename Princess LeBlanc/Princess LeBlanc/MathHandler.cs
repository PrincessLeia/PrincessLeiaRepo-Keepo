using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_LeBlanc
{
    class MathHandler
    {
        public static float ComboDamage(Obj_AI_Base enemy)
        {
            var t = TargetSelector.GetTarget(SkillHandler.E.Range, TargetSelector.DamageType.Magical);
            var ap = ObjectManager.Player.BaseAbilityDamage + ObjectManager.Player.FlatMagicDamageMod;
            var dmg = 0d;

            if (SkillHandler.Q.IsReady() && SkillHandler.W.IsReady() || SkillHandler.R.IsReady() || (SkillHandler.E.IsReady() && SkillHandler.E.GetPrediction(t).Hitchance >= HitChance.Medium))
            {
                dmg += ((30 + (25 * SkillHandler.Q.Level)) + (ap * 0.4)) * 2;
            }
            else if (SkillHandler.Q.IsReady() && !SkillHandler.W.IsReady() && !SkillHandler.R.IsReady() && !SkillHandler.E.IsReady() || (SkillHandler.E.IsReady() && SkillHandler.E.GetPrediction(t).Hitchance <= HitChance.Low))
            {
                dmg += (30 + (25 * SkillHandler.Q.Level)) + (ap * 0.4);
            }

            if (SkillHandler.W.IsReady())
            {
                dmg += (45 + (40 * SkillHandler.W.Level)) + (ap * 0.6);
            }

            if (SkillHandler.E.IsReady())
            {
                dmg += (30 + (50 * SkillHandler.E.Level)) + ap;
            }

            switch (FightHandler.StatusR())
            {
                case "Q":
                    {
                        if (SkillHandler.Q.IsReady() || SkillHandler.W.IsReady() || (SkillHandler.E.IsReady() && SkillHandler.E.GetPrediction(t).Hitchance >= HitChance.Medium))
                        {
                            dmg += (100 * SkillHandler.R.Level) + (ap * 0.65) + (30 + 25 * SkillHandler.Q.Level) + (ap * 0.4);
                        }
                        else if (!SkillHandler.Q.IsReady() && !SkillHandler.W.IsReady() && !SkillHandler.E.IsReady() ||
                                 (SkillHandler.E.IsReady() && SkillHandler.E.GetPrediction(t).Hitchance <= HitChance.Low))
                        {
                            dmg += (100 * SkillHandler.R.Level) + (ap * 0.65);
                        }
                        break;
                    }
                case "W":
                    {
                        dmg += 150 * SkillHandler.R.Level + (ap * 0.975);
                        break;
                    }
            }

            if (ItemManager.Dfg.IsReady() && ItemManager.Dfg.IsInRange(t))
            {
                dmg += ObjectManager.Player.GetItemDamage(enemy, Damage.DamageItems.Dfg);
                dmg = dmg * 1.2;
            }

            if (ObjectManager.Player.GetSpellSlot("SummonerIgnite") != SpellSlot.Unknown)
            {
                dmg += ObjectManager.Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            }
            dmg += ObjectManager.Player.GetAutoAttackDamage(enemy, true) * 1;
            dmg -= (enemy.FlatMagicReduction - ((enemy.FlatMagicReduction / 100) * ObjectManager.Player.PercentMagicPenetrationMod)) - ObjectManager.Player.FlatMagicPenetrationMod;

            return (float)dmg;
        }

        public static float RwDamage(Obj_AI_Base enemy)
        {
            var dmg = 0d;

            dmg += 150 * SkillHandler.R.Level + (ObjectManager.Player.BaseAbilityDamage + ObjectManager.Player.FlatMagicDamageMod) * 0.975;

            return (float)dmg;
        }

        public static float RqDamage(Obj_AI_Base enemy)
        {
            var t = TargetSelector.GetTarget(SkillHandler.E.Range, TargetSelector.DamageType.Magical);
            var ap = ObjectManager.Player.BaseAbilityDamage + ObjectManager.Player.FlatMagicDamageMod;
            var dmg = 0d;
            if (SkillHandler.Q.IsReady() || SkillHandler.W.IsReady() || (SkillHandler.E.IsReady() && SkillHandler.E.GetPrediction(t).Hitchance >= HitChance.Medium))
            {
                dmg += (100 * SkillHandler.R.Level) + (ap * 0.65) + (30 + 25 * SkillHandler.Q.Level) + (ap * 0.4);
            }
            else if (!SkillHandler.Q.IsReady() && !SkillHandler.W.IsReady() && !SkillHandler.E.IsReady() || (SkillHandler.E.IsReady() && SkillHandler.E.GetPrediction(t).Hitchance <= HitChance.Low))
            {
                dmg += (100 * SkillHandler.R.Level) + (ap * 0.65);
            }

            return (float)dmg;
        }
    }
}
