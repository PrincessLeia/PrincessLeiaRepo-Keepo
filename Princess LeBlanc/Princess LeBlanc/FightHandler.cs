using System;
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

        public static string RStatus;
        public static readonly float[] WPosition = new float[3];
        public static Obj_AI_Base Clone;

        public static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name.Contains("LeBlanc_MirrorImagePoff.troy") && sender.IsMe)
            {
                Clone = sender as Obj_AI_Base;
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
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

                if (MenuHandler.LeBlancConfig.Item("FleeW").GetValue<bool>() && Wunused() && SkillHandler.W.IsReady())
                {
                    SkillHandler.W.Cast(Game.CursorPos);
                }

                if (MenuHandler.LeBlancConfig.Item("FleeW").GetValue<bool>() && RStatus == "W" &&
                    SkillHandler.R.IsReady())
                {
                    SkillHandler.R.Cast(Game.CursorPos);
                }
        }

        public static void Combo()
        {
            var useQ = SkillHandler.Q.IsReady() &&
                       MenuHandler.LeBlancConfig.SubMenu("Combo").Item("useQ").GetValue<bool>();
            var useW = SkillHandler.W.IsReady() &&
                       MenuHandler.LeBlancConfig.SubMenu("Combo").Item("useW").GetValue<bool>();
            var useE = SkillHandler.E.IsReady() &&
                       MenuHandler.LeBlancConfig.SubMenu("Combo").Item("useE").GetValue<bool>();
            var useR = SkillHandler.R.IsReady() &&
                       MenuHandler.LeBlancConfig.SubMenu("Combo").Item("useR").GetValue<bool>();
            var useIgnite = ItemHandler.IgniteSlot != SpellSlot.Unknown &&
                            Player.Spellbook.CanUseSpell(ItemHandler.IgniteSlot) == SpellState.Ready;
            var useDfg = ItemHandler.Dfg.IsReady() &&
                         MenuHandler.LeBlancConfig.SubMenu("Items").Item("useDfg").GetValue<bool>();
            var target = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Magical);
            var targetQ = SkillHandler.Q.InRange(target.ServerPosition);
            var targetW = SkillHandler.W.InRange(target.ServerPosition);
            var targetE = SkillHandler.E.InRange(target.ServerPosition, 800);
            var targetR = SkillHandler.R.InRange(target.ServerPosition);
            var targetDfg = target.Distance(Player.Position) < ItemHandler.Dfg.Range;
            var wPriority = SkillHandler.W.Level > SkillHandler.Q.Level;

            //Items
            if (target.Health <= MathHandler.ComboDamage(target) && useDfg && targetDfg)
            {
                ItemHandler.Dfg.Cast(target);
            }

            if (target.Health <= MathHandler.ComboDamage(target) && useIgnite)
            {
                Player.Spellbook.CastSpell(ItemHandler.IgniteSlot, target);
            }

            //Spells
            if (useE && targetE && !targetQ)
            {
                SkillHandler.E.CastIfHitchanceEquals(target, HitChance.Medium);
            }
            else if (useR && !useQ && !useW || Wused())
            {
                if (useE && targetE)
                {
                    SkillHandler.E.CastIfHitchanceEquals(target, HitChance.Medium);
                }
            }


            switch (wPriority)
            {
                case true:
                {
                    if (useQ && targetQ)
                    {
                        SkillHandler.Q.Cast(target);
                    }
                    if (!useQ && useW && targetW && Wunused())
                    {
                        SkillHandler.W.Cast(target);
                    }
                    if (useR && RStatus == "W" && targetW)
                    {
                        SkillHandler.R.Cast(target);
                        if (SkillHandler.R.Instance.Name == "leblancslidereturnm")
                        {
                            SkillHandler.R.Cast(Player);
                        }
                    }
                    else if (useR && RStatus == "Q" && targetR && !useW || (Player.Distance(target) > SkillHandler.W.Range + 30))
                    {
                        SkillHandler.R.Cast(target);
                    }
                    return;
                }
                case false:
                {
                    if (useQ && targetQ)
                    {
                        SkillHandler.Q.Cast(target);
                    }
                    if (useR && RStatus == "Q" && targetR)
                    {
                        SkillHandler.R.Cast(target);
                    }
                    else if (useR && RStatus == "W" && targetW && !useQ)
                    {
                        SkillHandler.R.Cast(target);
                        if (SkillHandler.R.Instance.Name == "leblancslidereturnm")
                        {
                            SkillHandler.R.Cast(Player);
                        }
                    }
                    if (!useQ && !useR && useW && targetW && Wunused())
                    {
                        SkillHandler.W.Cast(target);
                    }

                    return;
                }
            }

            
        }

        public static void ComboLong()
        {
            var target = TargetSelector.GetTarget(SkillHandler.W.Range + SkillHandler.Q.Range, TargetSelector.DamageType.Magical);

            if (Wused())
            {
                return;
            }

            SkillHandler.W.Cast(target.Position);
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
            var target = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Magical);
            var targetE = SkillHandler.E.InRange(target.ServerPosition, 800);
            var targetQ = SkillHandler.Q.InRange(target.ServerPosition);
            var targetW = SkillHandler.W.InRange(target.ServerPosition);
            var targetBuff = target.HasBuff("LeblancSoulShackle");

            if (useE && targetE)
                {
                    SkillHandler.E.CastIfHitchanceEquals(target, HitChance.Medium);
                }
            if (useR && RStatus == "E" && targetE)
            {
                if (target.IsRooted)
                {
                    SkillHandler.R.Cast(target);
                }
                else if (!useE && !targetBuff)
                {
                    SkillHandler.R.CastIfHitchanceEquals(target, HitChance.Medium);
                }
                return;
            }
            if (RStatus != "E" || !useR)
            {
                if (useQ && targetQ)
                {
                    SkillHandler.Q.Cast(target);
                }

                if (useW && targetW && !useQ)
                {
                    SkillHandler.W.Cast(target);
                }
            }

        }


        public static string Ult()
        {
            var nameR = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name;

            switch (nameR)
            {
                case "LeblancChaosOrbM":
                    return RStatus = "Q";
                case "LeblancSlideM":
                    return RStatus = "W";
                case "LeblancSoulShackleM":
                    return RStatus = "E";
                default:
                    return RStatus = "Unknown";
            }
        }

        public static void LaneClear()
        {
            var useQ = MenuHandler.LeBlancConfig.SubMenu("ClearL").Item("LaneClearQ").GetValue<bool>() &&
                       SkillHandler.Q.IsReady();
            var useW = MenuHandler.LeBlancConfig.SubMenu("ClearL").Item("LaneClearW").GetValue<bool>() &&
                       SkillHandler.W.IsReady();
            var minions = MinionManager.GetMinions(
                Player.ServerPosition, SkillHandler.W.Range, MinionTypes.All, MinionTeam.NotAlly);
            var wHit = SkillHandler.W.GetCircularFarmLocation(minions, SkillHandler.W.Width);
            var mana = Player.ManaPercentage() >
                       MenuHandler.LeBlancConfig.SubMenu("ClearL").Item("LaneClearManaPercent").GetValue<Slider>().Value;
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
                    if (useW && minion.IsValidTarget() && wHit.MinionsHit >= 1)
                    {
                        SkillHandler.W.Cast(wHit.Position);
                    }
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
                Player.ServerPosition, SkillHandler.W.Range, MinionTypes.All, MinionTeam.Neutral);
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
                if (useW && minion.IsValidTarget())
                {
                    SkillHandler.W.Cast(minion);
                }
            }
            if (Wused())
            {
                Utility.DelayAction.Add(100, () => SkillHandler.W.Cast(Player));
            }
        }
        public static void CloneLogic()
        {
            if (Clone == null || !Clone.IsValid)
            {
                return;
            }

            var menu = MenuHandler.LeBlancConfig.Item("Clone").GetValue<StringList>().SelectedIndex;
            var target = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Magical);

            switch (menu)
            {
                case 0: //none
                {
                   return;
                }
                case 1: //toenemy
                {
                    if (Clone.CanAttack && !Clone.IsWindingUp)
                    {
                        Clone.IssueOrder(GameObjectOrder.AutoAttackPet, target);
                    }
                    break;
                }
                case 2: //rlocation or maybe between player enemy
                {
                    var rnd = new Random();
                    var x = rnd.Next(1, 19000);
                    var y = rnd.Next(1, 19000);

                    Clone.IssueOrder(GameObjectOrder.MovePet, new Vector3(x, y, 0));
                    break;
                }
                case 3: //toplayer
                {
                    var play = Player.ServerPosition;
                    Utility.DelayAction.Add(100, () => { Clone.IssueOrder(GameObjectOrder.MovePet, play); });
                    break;
                }
                case 4: //tocursor
                {
                    Clone.IssueOrder(GameObjectOrder.MovePet, Game.CursorPos);
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
                else if (target.HealthPercentage() > 30 && allOncd)
                {
                    SkillHandler.W.Cast(Player);
                }
                else if (Player.HealthPercentage() <= 5)
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
            else if (SkillHandler.R.IsReady() && RStatus == "E")
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
            else if (SkillHandler.R.IsReady() && RStatus == "E")
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