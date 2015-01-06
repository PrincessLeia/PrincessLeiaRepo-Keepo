using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Princess_LeBlanc
{
    internal class FightHandler
    {
        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }
        public static readonly float[] WPosition = new float[3];
        public static float CloneTime;
        public static Obj_AI_Hero CTarget;
        public static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name.Contains("LeBlanc_MirrorImagePoff.troy") && sender.IsMe)
            {
                CloneTime = 8 + Game.Time;
            }

            if (sender.Name == "LeBlanc_Base_W_return_indicator.troy")
            {
                WPosition[0] = sender.Position.X;
                WPosition[1] = sender.Position.Y;
                WPosition[2] = sender.Position.Z;
            }

        }
        public static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name == "LeBlanc_Base_W_return_indicator.troy")
            {
                WPosition[0] = 0;
                WPosition[1] = 0;
                WPosition[2] = 0;
            }

        }
        public static void Flee()
        {
            if (MenuHandler.LeBlancConfig.Item("FleeK").GetValue<KeyBind>().Active)
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                if (MenuHandler.LeBlancConfig.Item("FleeW").GetValue<bool>() && Wunused() && SkillHandler.W.IsReady())
                {
                    SkillHandler.W.Cast(Game.CursorPos);
                }

                if (MenuHandler.LeBlancConfig.Item("FleeW").GetValue<bool>() && StatusR() == "W" &&
                    SkillHandler.R.IsReady())
                {
                    SkillHandler.R.Cast(Game.CursorPos);
                }
            }
        }
        public static void Combo()
        {
            var assassinRange = MenuHandler.LeBlancConfig.Item("AssassinRange").GetValue<Slider>().Value;
            Obj_AI_Hero target = null;
            foreach (
                var enemy in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(
                            enemy =>
                                enemy.Team != Player.Team && !enemy.IsDead && enemy.IsVisible &&
                                MenuHandler.LeBlancConfig.Item("Assassin" + enemy.ChampionName) != null &&
                                MenuHandler.LeBlancConfig.Item("Assassin" + enemy.ChampionName).GetValue<bool>())
                        .OrderBy(enemy => enemy.Distance(Game.CursorPos)))
            {
                    target = Player.Distance(enemy) < assassinRange ? enemy : null;
            }
            switch (MenuHandler.LeBlancConfig.Item("AssassinActive").GetValue<bool>())
            {
                case true:
                    {
                        CTarget = target;
                        break;
                    }
                case false:
                {
                    CTarget = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Magical);
                    break;
                }
            }

            var useQ = SkillHandler.Q.IsReady() &&
                       MenuHandler.LeBlancConfig.SubMenu("Combo").Item("useQ").GetValue<bool>();
            var useW = SkillHandler.W.IsReady() &&
                       MenuHandler.LeBlancConfig.SubMenu("Combo").Item("useW").GetValue<bool>();
            var useE = SkillHandler.E.IsReady() &&
                       MenuHandler.LeBlancConfig.SubMenu("Combo").Item("useE").GetValue<bool>();
            var useR = SkillHandler.R.IsReady() &&
                       MenuHandler.LeBlancConfig.SubMenu("Combo").Item("useR").GetValue<bool>();
            var useIgnite = ItemHandler.IgniteSlot != SpellSlot.Unknown && MenuHandler.LeBlancConfig.SubMenu("Items").Item("useIgnite").GetValue<bool>() &&
                            Player.Spellbook.CanUseSpell(ItemHandler.IgniteSlot) == SpellState.Ready;
            var useDfg = ItemHandler.Dfg.IsReady() &&
                         MenuHandler.LeBlancConfig.SubMenu("Items").Item("useDfg").GetValue<bool>();
            var targetQ = SkillHandler.Q.InRange(CTarget.ServerPosition);
            var targetW = SkillHandler.W.InRange(CTarget.ServerPosition);
            var targetE = SkillHandler.E.InRange(CTarget.ServerPosition, 800);
            var targetR = SkillHandler.R.InRange(CTarget.ServerPosition);
            var targetDfg = CTarget.Distance(Player.Position) < ItemHandler.Dfg.Range;
            var wPriority = SkillHandler.W.Level > SkillHandler.Q.Level;

            //Items
            if (CTarget.Health <= MathHandler.ComboDamage(CTarget) && useDfg && targetDfg)
            {
                ItemHandler.Dfg.Cast(CTarget);
            }

            if (CTarget.Health <= MathHandler.ComboDamage(CTarget) && useIgnite)
            {
                Player.Spellbook.CastSpell(ItemHandler.IgniteSlot, CTarget);
            }

            //Spells
            if (useE && targetE && !useR && !useQ && (!useW || Wused()))
            {
                SkillHandler.E.CastIfHitchanceEquals(CTarget, HitChance.Medium);
            }


            switch (wPriority)
            {
                case true:
                    {
                        if (useQ && targetQ)
                        {
                            SkillHandler.Q.Cast(CTarget);
                        }
                        if (!useQ && useW && targetW && Wunused())
                        {
                            SkillHandler.W.Cast(CTarget);
                        }
                        if (useR && StatusR() == "W" && targetW)
                        {
                            SkillHandler.R.Cast(CTarget);
                            if (SkillHandler.R.Instance.Name == "leblancslidereturnm")
                            {
                                SkillHandler.R.Cast(CTarget);
                            }
                        }
                        else if (useR && StatusR() == "Q" && targetR && !useW)
                        {
                            SkillHandler.R.CastOnUnit(CTarget);
                        }
                        return;
                    }
                case false:
                    {
                        if (useQ && targetQ)
                        {
                            SkillHandler.Q.Cast(CTarget);
                        }
                        if (useR && StatusR() == "Q" && targetR)
                        {
                            SkillHandler.R.CastOnUnit(CTarget);
                        }
                        else if (useR && StatusR() == "W" && targetW && !useQ)
                        {
                            SkillHandler.R.Cast(CTarget);
                            if (SkillHandler.R.Instance.Name == "leblancslidereturnm")
                            {
                                SkillHandler.R.Cast(Player);
                            }
                        }
                        if (!useQ && !useR && useW && targetW && Wunused())
                        {
                            SkillHandler.W.Cast(CTarget);
                        }

                        return;
                    }
            }


        }
        public static void ComboLong()
        {
            if (Wused())
            {
                return;
            }

            SkillHandler.W.Cast(CTarget.ServerPosition);
            Combo();
        }
        public static void ComboTanky()
        {
            var useQ = SkillHandler.Q.IsReady() &&
                       MenuHandler.LeBlancConfig.SubMenu("Combo").Item("useQ").GetValue<bool>();
            var useW = SkillHandler.W.IsReady() &&
                       MenuHandler.LeBlancConfig.SubMenu("Combo").Item("useW").GetValue<bool>();
            var useE = SkillHandler.E.IsReady() &&
                       MenuHandler.LeBlancConfig.SubMenu("Combo").Item("useE").GetValue<bool>();
            var useR = SkillHandler.R.IsReady() &&
                       MenuHandler.LeBlancConfig.SubMenu("Combo").Item("useR").GetValue<bool>();
            var targetE = SkillHandler.E.InRange(CTarget.ServerPosition, 800);
            var targetQ = SkillHandler.Q.InRange(CTarget.ServerPosition);
            var targetW = SkillHandler.W.InRange(CTarget.ServerPosition);
            var targetBuff = CTarget.HasBuff("LeblancSoulShackle");

            if (useE && targetE)
            {
                SkillHandler.E.CastIfHitchanceEquals(CTarget, HitChance.Medium);
            }
            else if (useR && StatusR() == "E" && targetE)
            {
                if (CTarget.IsRooted)
                {
                    SkillHandler.R.Cast(CTarget);
                }
                else if (!targetBuff)
                {
                    SkillHandler.R.CastIfHitchanceEquals(CTarget, HitChance.Medium);
                }
            }
            else if (StatusR() != "E" || !useR)
            {
                if (useQ && targetQ)
                {
                    SkillHandler.Q.Cast(CTarget);
                }

                if (useW && targetW && !useQ && Wunused())
                {
                    SkillHandler.W.Cast(CTarget);
                }
            }

        }
        public static void Harass()
        {
            var target = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Magical);
            var mana = Player.ManaPercentage() > MenuHandler.LeBlancConfig.SubMenu("Harass").Item("HarassManaPercent").GetValue<Slider>().Value;
            var useQ = SkillHandler.Q.IsReady() &&
                       MenuHandler.LeBlancConfig.SubMenu("Harass").Item("useQ").GetValue<bool>();
            var useW = SkillHandler.W.IsReady() &&
                       MenuHandler.LeBlancConfig.SubMenu("Harass").Item("useW").GetValue<bool>();
            var useE = SkillHandler.E.IsReady() &&
                       MenuHandler.LeBlancConfig.SubMenu("Harass").Item("useE").GetValue<bool>();
            var targetQ = SkillHandler.Q.InRange(target);
            var targetW = SkillHandler.W.InRange(target);
            var targetE = SkillHandler.E.InRange(target);

            if (!mana) {return;}

            if (useE && targetE)
                {
                    SkillHandler.E.CastIfHitchanceEquals(target, HitChance.Medium);
                }
                if (useQ && targetQ)
                {
                    SkillHandler.Q.CastOnUnit(target);
                }
                if (!useQ && useW && targetW)
                {
                    SkillHandler.W.Cast(target);
                    if (Wused())
                    {
                        Utility.DelayAction.Add(100, () => SkillHandler.W.Cast(Player));
                    }
                }
        }
        public static string StatusR()
        {
            var name = Player.Spellbook.GetSpell(SpellSlot.R).Name;

            switch (name)
            {
                case "LeblancChaosOrbM":
                    return "Q";
                case "LeblancSlideM":
                    return "W";
                case "LeblancSoulShackleM":
                    return "E";
            }
            return "unkown";

        }
        public static void LaneClear()
        {
            var useQ = MenuHandler.LeBlancConfig.SubMenu("ClearL").Item("LaneClearQ").GetValue<bool>() &&
                       SkillHandler.Q.IsReady();
            var useW = MenuHandler.LeBlancConfig.SubMenu("ClearL").Item("LaneClearW").GetValue<bool>() &&
                       SkillHandler.W.IsReady();
            var minions = MinionManager.GetMinions(
                Player.ServerPosition, SkillHandler.Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            var farmLocation = MinionManager.GetBestCircularFarmLocation(MinionManager.GetMinions(SkillHandler.W.Range, MinionTypes.All, MinionTeam.Enemy).Select(m => m.ServerPosition.To2D()).ToList(), SkillHandler.W.Width, SkillHandler.W.Range);
            var mana = Player.ManaPercentage() >
                       MenuHandler.LeBlancConfig.SubMenu("ClearL").Item("LaneClearManaPercent").GetValue<Slider>().Value;
            var minionHit = farmLocation.MinionsHit >=
                            MenuHandler.LeBlancConfig.SubMenu("ClearL").Item("LaneClearWHit").GetValue<Slider>().Value;
            if (!mana)
            {
                return;
            }
            foreach (var minion in minions)
            {
                if (useQ && minion.IsValidTarget() && minion.Health <= SkillHandler.Q.GetDamage(minion))
                {
                    SkillHandler.Q.CastOnUnit(minion);
                }
            }
            if (minionHit && useW)
            {
                SkillHandler.W.Cast(farmLocation.Position);
            }
            if (Wused())
            {
                Utility.DelayAction.Add(100, () => SkillHandler.W.Cast(Player));
            }
        }
        public static void JungleClear()
        {
            var useQ = MenuHandler.LeBlancConfig.SubMenu("ClearJ").Item("JungleClearQ").GetValue<bool>() &&
                       SkillHandler.Q.IsReady();
            var useW = MenuHandler.LeBlancConfig.SubMenu("ClearJ").Item("JungleClearW").GetValue<bool>() &&
                       SkillHandler.W.IsReady();
            var minions = MinionManager.GetMinions(
                Player.ServerPosition, SkillHandler.Q.Range, MinionTypes.All, MinionTeam.Neutral);
            var farmLocation = MinionManager.GetBestCircularFarmLocation(MinionManager.GetMinions(SkillHandler.W.Range, MinionTypes.All, MinionTeam.Neutral).Select(m => m.ServerPosition.To2D()).ToList(), SkillHandler.W.Width, SkillHandler.W.Range);
            var mana = Player.ManaPercentage() >
                       MenuHandler.LeBlancConfig.SubMenu("ClearJ").Item("JungleClearManaPercent").GetValue<Slider>().Value;

            if (!mana)
            {
                return;
            }
            foreach (var minion in minions)
            {
                if (useQ && minion.IsValidTarget())
                {
                    SkillHandler.Q.CastOnUnit(minion);
                }
            }
            if (farmLocation.MinionsHit > 0 && useW)
            {
                SkillHandler.W.Cast(farmLocation.Position);
            }
            if (Wused())
            {
                Utility.DelayAction.Add(100, () => SkillHandler.W.Cast(Player));
            }
        }
        public static void CloneLogic()
        {
            if (Game.Time > CloneTime)
            {
                return;
            }

            var menu = MenuHandler.LeBlancConfig.SubMenu("Misc").Item("Clone").GetValue<StringList>().SelectedIndex;
            var target = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Magical);

            switch (menu)
            {
                case 0: //none
                    {
                        return;
                    }
                case 1: //toenemy
                    {
                        Player.IssueOrder(GameObjectOrder.AutoAttackPet, target);
                        break;
                    }
                case 2: //rlocation or maybe between player enemy
                    {
                        var newPosition = Player.Position.Extend(target.Position, 100f);
                        Player.IssueOrder(GameObjectOrder.MovePet, newPosition);
                        break;
                    }
                case 3: //toplayer
                    {
                        var play = Player.ServerPosition;
                        Utility.DelayAction.Add(100, () => { Player.IssueOrder(GameObjectOrder.MovePet, play); });
                        break;
                    }
                case 4: //tocursor
                    {
                        Player.IssueOrder(GameObjectOrder.MovePet, Game.CursorPos);
                        break;
                    }
            }
        }
        public static void WLogic()
        {
            var target = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Magical);
            var allOncd = !SkillHandler.Q.IsReady() && !SkillHandler.E.IsReady() &&
                          !SkillHandler.R.IsReady();
            var wPos = new Vector3(WPosition[0], WPosition[1], WPosition[2]);
            var countW = Utility.CountEnemysInRange(wPos, 200) == 0;
            if (!MenuHandler.LeBlancConfig.Item("backW").GetValue<bool>())
            {
                return;
            }

            if (Wused() && countW)
            {
                if (Player.Mana < Player.Spellbook.GetSpell(SpellSlot.Q).ManaCost && target.HealthPercentage() > 5)
                {
                    SkillHandler.W.Cast(Player);
                }
                else if (target.IsDead)
                {
                    SkillHandler.W.Cast(Player);
                }
                else if (target.HealthPercentage() > 40 && allOncd)
                {
                    SkillHandler.W.Cast(Player);
                }
            }
        }
        public static void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!MenuHandler.LeBlancConfig.Item("Interrupt").GetValue<bool>() || spell.DangerLevel != InterruptableDangerLevel.High)
                return;

            if (SkillHandler.E.IsReady())
            {
                SkillHandler.E.Cast(unit);
            }
            else if (SkillHandler.R.IsReady() && StatusR() == "E")
            {
                SkillHandler.R.Cast(unit);
            }
        }
        public static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!MenuHandler.LeBlancConfig.Item("Interrupt").GetValue<bool>())
                return;

            if (SkillHandler.E.IsReady())
            {
                SkillHandler.E.Cast(gapcloser.Sender);
            }
            else if (SkillHandler.R.IsReady() && StatusR() == "E")
            {
                SkillHandler.R.Cast(gapcloser.Sender);
            }
        }
        private static bool Wunused()
        {
            return SkillHandler.W.Instance.Name == "LeblancSlide";
        }
        private static bool Wused()
        {
            return SkillHandler.W.Instance.Name == "leblancslidereturn";
        }
    }
}