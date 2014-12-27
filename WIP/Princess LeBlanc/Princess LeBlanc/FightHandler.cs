using System;
using System.Linq;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;

namespace Princess_LeBlanc
{
    internal class FightHandler
    {
        private static Obj_AI_Hero Player
            {
              get { return ObjectManager.Player; }
            }
        public static float WPositionX;
        public static float WPositionY;
        public static float WPositionZ;
        public static float RPositionX;
        public static float RPositionY;
        public static float RPositionZ;
        private static bool leBlancClone;

        public static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            leBlancClone = sender.Name.Contains("LeBlanc_MirrorImagePoff.troy");
        }
        public static bool LeBlancClone
        {
            get { return leBlancClone; }
            set { leBlancClone = value;}
        }
        public static void Flee()
        {
            if (MenuHandler.LeBlancConfig.Item("FleeK").GetValue<KeyBind>().Active)
            {
                if (MenuHandler.LeBlancConfig.Item("FleeW").GetValue<bool>() && Player.Spellbook.GetSpell(SpellSlot.W).Name != "leblancslidereturn" && SkillHandler.W.IsReady())
                {
                    SkillHandler.W.Cast(Game.CursorPos);
                }
                if (MenuHandler.LeBlancConfig.Item("FleeW").GetValue<bool>() && Player.Spellbook.GetSpell(SpellSlot.R).Name != "leblancslidereturnm" &&
                    SkillHandler.R.IsReady())
                {
                    
                }
            }
        }
        public static void Combo()
        {
            var target = TargetSelector.GetTarget(SkillHandler.W.Range*2, TargetSelector.DamageType.Magical);
            var q = SkillHandler.Q.IsReady() && Player.Distance(target) <= SkillHandler.Q.Range &&
                     MenuHandler.LeBlancConfig.Item("useQ").GetValue<bool>();
            var w = SkillHandler.W.IsReady() && Player.Distance(target) <= SkillHandler.W.Range &&
                     MenuHandler.LeBlancConfig.Item("useW").GetValue<bool>() && Player.Spellbook.GetSpell(SpellSlot.W).Name != "leblancslidereturn";
            var e = SkillHandler.E.IsReady() && Player.Distance(target) <= SkillHandler.E.Range &&
                     MenuHandler.LeBlancConfig.Item("useE").GetValue<bool>() && SkillHandler.E.GetPrediction(target).Hitchance >= HitChance.High;
            var r = MenuHandler.LeBlancConfig.Item("useR").GetValue<bool>() && SkillHandler.R.IsReady();
            var wPriority = SkillHandler.W.Level > SkillHandler.Q.Level;

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

            if (!wPriority)
            {
                if (e)
                {
                    SkillHandler.E.CastIfHitchanceEquals(target, HitChance.High, Packeting());
                    if (q)
                    {
                        SkillHandler.Q.Cast(target, Packeting());
                    }
                    if (r)
                    {
                        UseR();
                    }
                    if (w)
                    {
                        SkillHandler.W.Cast(target, Packeting());
                    }
                }
                else if (Player.Distance(target) <= SkillHandler.W.Range+SkillHandler.Q.Range)
                {
                    SkillHandler.W.Cast(target.Direction, Packeting());
                    if (q)
                    {
                        SkillHandler.Q.Cast(target, Packeting());
                    }
                    if (r)
                    {
                        UseR();
                    }
                    if (SkillHandler.E.IsReady() && Player.Distance(target) <= SkillHandler.E.Range &&
                     MenuHandler.LeBlancConfig.Item("useE").GetValue<bool>() && SkillHandler.E.GetPrediction(target).Hitchance >= HitChance.High)
                    {
                        SkillHandler.E.CastIfHitchanceEquals(target, HitChance.High, Packeting());
                    }
                }
            }
            else
            {
                if (e)
                {
                    SkillHandler.E.CastIfHitchanceEquals(target, HitChance.High, Packeting());
                    if (q)
                    {
                        SkillHandler.Q.Cast(target, Packeting());
                    }
                    if (w)
                    {
                        SkillHandler.W.Cast(target, Packeting());
                    }
                    if (r)
                    {
                        UseR();
                    }

                }
                else if (Player.Distance(target) <= SkillHandler.W.Range*2 + SkillHandler.Q.Range)
                {
                    SkillHandler.W.Cast(target.Direction, Packeting());
                    if (r)
                    {
                        UseR();
                    }
                    if (q)
                    {
                        SkillHandler.Q.Cast(target, Packeting());
                    }
                    if (SkillHandler.E.IsReady() && Player.Distance(target) <= SkillHandler.E.Range &&
                     MenuHandler.LeBlancConfig.Item("useE").GetValue<bool>() && SkillHandler.E.GetPrediction(target).Hitchance >= HitChance.High)
                    {
                        SkillHandler.E.CastIfHitchanceEquals(target, HitChance.High, Packeting());
                    }
                }
            }



        }
        public static void UseR()
        {
            var target = TargetSelector.GetTarget(SkillHandler.E.Range, TargetSelector.DamageType.Magical);
            var q = Player.Distance(target) <= SkillHandler.Q.Range &&
                     MenuHandler.LeBlancConfig.Item("useQ").GetValue<bool>();
            var w = Player.Distance(target) <= SkillHandler.W.Range &&  //add prediction
                     MenuHandler.LeBlancConfig.Item("useW").GetValue<bool>();
            var e = Player.Distance(target) <= SkillHandler.E.Range &&
                     MenuHandler.LeBlancConfig.Item("useE").GetValue<bool>();
            var name = Player.Spellbook.GetSpell(SpellSlot.R).Name;

            if (name == "leblancslidereturnm")
            {
                return;
            }

            if (q && name == "LeblancChaosOrbM")
            {
                SkillHandler.R.Cast(target, Packeting());
            }
            else if (w && name == "LeblancSlideM")
            {
                SkillHandler.R.Cast(target, Packeting());
            }
            else if (e && name == "LeblancSoulShackleM" 
                && SkillHandler.E.GetPrediction(target).Hitchance >= HitChance.High)
            {
                SkillHandler.R.CastIfHitchanceEquals(target, HitChance.High, Packeting());
            }
        }
        public static void KillSteal()
        {
            foreach (var target in
                    ObjectManager.Get<Obj_AI_Hero>().Where(target => target.IsValidTarget(SkillHandler.W.Range)))
            {
                if (MenuHandler.LeBlancConfig.Item("KSi").GetValue<bool>() &&
                    ItemHandler.IgniteSlot != SpellSlot.Unknown &&
                    Player.Spellbook.CanUseSpell(ItemHandler.IgniteSlot) == SpellState.Ready &&
                    Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) >= target.Health)
                {
                    Player.Spellbook.CastSpell(ItemHandler.IgniteSlot, target);
                }
                if (MenuHandler.LeBlancConfig.Item("KSw").GetValue<bool>() && MenuHandler.LeBlancConfig.Item("KSq").GetValue<bool>() 
                    && SkillHandler.W.IsReady() && SkillHandler.Q.IsReady() &&
                    MathHandler.KsComboDamage(target) >= target.Health 
                    && Player.Distance(target) <= SkillHandler.W.Range)
                {
                    SkillHandler.Q.Cast(target, Packeting());
                    SkillHandler.W.Cast(target.ServerPosition, Packeting());
                    if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "leblancslidereturn")
                    {
                        SkillHandler.W.Cast(Packeting());
                    }
                }
                else if (MenuHandler.LeBlancConfig.Item("KSw").GetValue<bool>() && SkillHandler.W.IsReady() &&
                    SkillHandler.W.GetDamage(target) >= target.Health && Player.Distance(target) <= SkillHandler.W.Range)
                {
                    SkillHandler.W.Cast(target.ServerPosition, Packeting());
                    if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "leblancslidereturn")
                    {
                        SkillHandler.W.Cast(Packeting());
                    }
                }

                else if (MenuHandler.LeBlancConfig.Item("KSq").GetValue<bool>() && SkillHandler.Q.IsReady() &&
                    SkillHandler.Q.GetDamage(target) >= target.Health && Player.Distance(target) <= SkillHandler.Q.Range)
                {
                    SkillHandler.Q.Cast(target, Packeting());
                }
                
            }
        }
        public static void LaneClear()
        {
            if (SkillHandler.W.IsReady() && MenuHandler.LeBlancConfig.Item("LaneClearW").GetValue<bool>() &&
                Player.ManaPercentage() >=
                MenuHandler.LeBlancConfig.Item("LaneClearManaPercent").GetValue<Slider>().Value)
            {
                var allMinionsQ =
                    MinionManager.GetMinions(Player.Position, SkillHandler.W.Range + SkillHandler.W.Width,
                        MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health).ToList();
                var allMinionsQNonPoisoned = allMinionsQ.Where(x => !x.HasBuffOfType(BuffType.Poison)).ToList();

                if (allMinionsQNonPoisoned.Any())
                {
                    var farmNonPoisoned = SkillHandler.W.GetCircularFarmLocation(allMinionsQNonPoisoned,
                        SkillHandler.W.Width*0.8f);
                    if (farmNonPoisoned.MinionsHit >= 3)
                    {
                        SkillHandler.W.Cast(farmNonPoisoned.Position, Packeting());
                        SkillHandler.W.Cast(Packeting()); //need delay
                    }
                }
            }
        }
        public static void CloneLogic()
        {
            if (leBlancClone == true)
            {
                var target = TargetSelector.GetTarget(SkillHandler.E.Range*2, TargetSelector.DamageType.Magical);
                var none = MenuHandler.LeBlancConfig.Item("Clone").GetValue<StringList>().SelectedIndex == 1;
                var toenemy = MenuHandler.LeBlancConfig.Item("Clone").GetValue<StringList>().SelectedIndex == 2;
                var rlocation = MenuHandler.LeBlancConfig.Item("Clone").GetValue<StringList>().SelectedIndex == 3;
                var tryescape = MenuHandler.LeBlancConfig.Item("Clone").GetValue<StringList>().SelectedIndex == 4;
                var tomouse = MenuHandler.LeBlancConfig.Item("Clone").GetValue<StringList>().SelectedIndex == 5;

                if (none)
                {
                    return;
                }
                if (toenemy)
                {

                }
                if (rlocation)
                {

                }
                if (tryescape)
                {

                }
                if (tomouse)
                {

                }

            }
        }
        public static void WLogic()
        {
            var target = TargetSelector.GetTarget(SkillHandler.E.Range, TargetSelector.DamageType.Magical);

            if (!MenuHandler.LeBlancConfig.Item("backW").GetValue<bool>())
            {
                return;
            }

            if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "leblancslidereturn" && Utility.CountEnemysInRange(new Vector3(WPositionX, WPositionY, WPositionZ) , 400) == 0)
            {
                if (Player.Mana < Player.Spellbook.GetSpell(SpellSlot.Q).ManaCost
                        || target.IsDead && !target.IsValid 
                        || target.Health > MathHandler.ComboDamage(target) && target.HealthPercentage() >= 15
                        || Player.HealthPercentage() <= 5)
                    {
                        SkillHandler.W.Cast(Packeting());
                    }
            }

            else if (Player.Spellbook.GetSpell(SpellSlot.R).Name == "leblancslidereturnm" && Utility.CountEnemysInRange(new Vector3(RPositionX, RPositionY, RPositionZ), 400) == 0)
                {
                    if (Player.Mana < Player.Spellbook.GetSpell(SpellSlot.Q).ManaCost
                        || target.IsDead && !target.IsValid
                        || target.Health > MathHandler.ComboDamage(target) && target.HealthPercentage() >= 15
                        || Player.HealthPercentage() <= 5)
                    {
                        SkillHandler.R.Cast(Packeting());
                    }
            }
        }
        public static void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!MenuHandler.LeBlancConfig.Item("Interrupt").GetValue<bool>() || spell.DangerLevel != InterruptableDangerLevel.High)
                return;

            if (SkillHandler.E.IsReady())
            {
                SkillHandler.E.Cast(unit, Packeting());
            }
            else if (SkillHandler.R.IsReady() && Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSoulShackleM")
            {
                SkillHandler.R.Cast(unit, Packeting());
            }
        }

        public static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!MenuHandler.LeBlancConfig.Item("Interrupt").GetValue<bool>())
                return;

            if (SkillHandler.E.IsReady())
            {
                SkillHandler.E.Cast(gapcloser.Sender, Packeting());
            }
            else if (SkillHandler.R.IsReady() && Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSoulShackleM")
            {
                SkillHandler.R.Cast(gapcloser.Sender, Packeting());
            }
        }
        public static bool Packeting()
        {
            return MenuHandler.LeBlancConfig.Item("packets").GetValue<bool>();
        }

        public static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (args.SData.Name == "LeblancSlide")
            {
                WPositionX = Player.Position.X;
                WPositionY = Player.Position.Y;
                WPositionZ = Player.Position.Z;
            }
            if (args.SData.Name == "LeblancSlideM")
            {
                RPositionX = Player.Position.X;
                RPositionY = Player.Position.Y;
                RPositionZ = Player.Position.Z;
            }
        }
    }
 }