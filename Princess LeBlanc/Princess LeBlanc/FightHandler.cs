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

        private static float _clonetime;
        private static Vector3 _wpos;

        static readonly bool PacketCast = Program.PacketCast;

        static Obj_AI_Hero GetEnemy(float vDefaultRange = 0, TargetSelector.DamageType vDefaultDamageType = TargetSelector.DamageType.Physical)
        {
            if (Math.Abs(vDefaultRange) < 0.00001)
                vDefaultRange = SkillHandler.Q.Range;

            if (!MenuHandler.LeBlancConfig.Item("AssassinActive").GetValue<bool>())
                return TargetSelector.GetTarget(vDefaultRange, vDefaultDamageType);

            var assassinRange = MenuHandler.LeBlancConfig.Item("AssassinSearchRange").GetValue<Slider>().Value;

            var vEnemy = ObjectManager.Get<Obj_AI_Hero>()
                .Where(
                    enemy =>
                        enemy.Team != ObjectManager.Player.Team && !enemy.IsDead && enemy.IsVisible &&
                        MenuHandler.LeBlancConfig.Item("Assassin" + enemy.ChampionName) != null &&
                        MenuHandler.LeBlancConfig.Item("Assassin" + enemy.ChampionName).GetValue<bool>() &&
                        ObjectManager.Player.Distance(enemy) < assassinRange);

            if (MenuHandler.LeBlancConfig.Item("AssassinSelectOption").GetValue<StringList>().SelectedIndex == 1)
            {
                vEnemy = (from vEn in vEnemy select vEn).OrderByDescending(vEn => vEn.MaxHealth);
            }

            Obj_AI_Hero[] objAiHeroes = vEnemy as Obj_AI_Hero[] ?? vEnemy.ToArray();

            Obj_AI_Hero t = !objAiHeroes.Any()
                ? TargetSelector.GetTarget(vDefaultRange, vDefaultDamageType)
                : objAiHeroes[0];

            return t;
        }

        public static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name.Contains("LeBlanc_MirrorImagePoff.troy") && sender.IsMe)
            {
                _clonetime = 8 + Game.Time;
            }

            if (sender.Name == "LeBlanc_Base_W_return_indicator.troy")
            {
                _wpos = sender.Position;
            }

        }

        public static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name == "LeBlanc_Base_W_return_indicator.troy")
            {
                _wpos = new Vector3(0, 0, 0);
            }

        }

        public static void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!MenuHandler.LeBlancConfig.Item("Interrupt").GetValue<bool>() || spell.DangerLevel != InterruptableDangerLevel.High)
                return;

            if (SkillHandler.E.IsReady())
            {
                SkillHandler.E.Cast(unit, PacketCast);
            }
            else if (SkillHandler.R.IsReady() && StatusR() == "E")
            {
                SkillHandler.R.Cast(unit, PacketCast);
            }
        }

        public static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!MenuHandler.LeBlancConfig.Item("Interrupt").GetValue<bool>())
                return;

            if (SkillHandler.E.IsReady())
            {
                SkillHandler.E.Cast(gapcloser.Sender, PacketCast);
            }
            else if (SkillHandler.R.IsReady() && StatusR() == "E")
            {
                SkillHandler.R.Cast(gapcloser.Sender, PacketCast);
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

        public static string StatusW()
        {
            var name = Player.Spellbook.GetSpell(SpellSlot.W).Name;

            if (name != "LeblancSlide")
            {
                return "return";
            }
            return "normal";

        }

        public static void CloneLogic()
        {
            if (Game.Time > _clonetime)
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

        public static void LaneClear()
        {
            var useQ = MenuHandler.LeBlancConfig.SubMenu("ClearL").Item("LaneClearQ").GetValue<bool>() && SkillHandler.Q.IsReady();
            var useW = MenuHandler.LeBlancConfig.SubMenu("ClearL").Item("LaneClearW").GetValue<bool>() && SkillHandler.W.IsReady();
            var minions = MinionManager.GetMinions(
                Player.ServerPosition, SkillHandler.Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            var farmLocation = MinionManager.GetBestCircularFarmLocation(MinionManager.GetMinions(SkillHandler.W.Range).Select(m => m.ServerPosition.To2D()).ToList(), SkillHandler.W.Width, SkillHandler.W.Range);
            var mana = Player.ManaPercentage() > MenuHandler.LeBlancConfig.SubMenu("ClearL").Item("LaneClearManaPercent").GetValue<Slider>().Value;
            var minionHit = farmLocation.MinionsHit >= MenuHandler.LeBlancConfig.SubMenu("ClearL").Item("LaneClearWHit").GetValue<Slider>().Value;
            if (!mana)
            {
                return;
            }
            foreach (var minion in minions)
            {
                if (minion != null && useQ && minion.IsValidTarget() && minion.Health <= SkillHandler.Q.GetDamage(minion))
                {
                    SkillHandler.Q.CastOnUnit(minion, PacketCast);
                }
            }
            if (minionHit && useW && StatusW() == "normal")
            {
                SkillHandler.W.Cast(farmLocation.Position, PacketCast);
            }
            if (StatusW() == "return")
            {
                SkillHandler.W.Cast(PacketCast);
            }
        }

        public static void JungleClear()
        {
            var useQ = MenuHandler.LeBlancConfig.SubMenu("ClearJ").Item("JungleClearQ").GetValue<bool>() && SkillHandler.Q.IsReady();
            var useW = MenuHandler.LeBlancConfig.SubMenu("ClearJ").Item("JungleClearW").GetValue<bool>() && SkillHandler.W.IsReady() && StatusW() == "normal";
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
                if (minion != null && useQ && minion.IsValidTarget())
                {
                    SkillHandler.Q.CastOnUnit(minion, PacketCast);
                }
            }
            if (farmLocation.MinionsHit > 0 && useW)
            {
                SkillHandler.W.Cast(farmLocation.Position);
            }
            if (StatusW() == "return")
            {
                SkillHandler.W.Cast(PacketCast);
            }
        }

        public static void Flee()
        {
            if (!MenuHandler.LeBlancConfig.Item("FleeK").GetValue<KeyBind>().Active) { return; }

            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var target = TargetSelector.GetTarget(SkillHandler.E.Range, TargetSelector.DamageType.Magical);

            if (MenuHandler.LeBlancConfig.Item("FleeW").GetValue<bool>() && StatusW() == "normal" && SkillHandler.W.IsReady())
            {
                SkillHandler.W.Cast(Game.CursorPos, PacketCast);
            }

            if (MenuHandler.LeBlancConfig.Item("FleeW").GetValue<bool>() && StatusR() == "W" && SkillHandler.R.IsReady() && SkillHandler.R.Instance.Name != "LeblancSlideReturnM")
            {
                SkillHandler.R.Cast(Game.CursorPos, PacketCast);
            }

            if (MenuHandler.LeBlancConfig.Item("FleeE").GetValue<bool>() && SkillHandler.E.IsReady() && SkillHandler.E.IsInRange(target))
            {
                SkillHandler.E.CastIfHitchanceEquals(target, HitChance.Medium, PacketCast);
            }
        }

        public static void Harass()
        {
            var target = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Magical);
            var mana = Player.ManaPercentage() > MenuHandler.LeBlancConfig.SubMenu("Harass").Item("HarassManaPercent").GetValue<Slider>().Value;
            var useQ = SkillHandler.Q.IsReady() && MenuHandler.LeBlancConfig.SubMenu("Harass").Item("useQ").GetValue<bool>();
            var useW = SkillHandler.W.IsReady() && MenuHandler.LeBlancConfig.SubMenu("Harass").Item("useW").GetValue<bool>();
            var useE = SkillHandler.E.IsReady() && MenuHandler.LeBlancConfig.SubMenu("Harass").Item("useE").GetValue<bool>();
            var targetQ = SkillHandler.Q.IsInRange(target);
            var targetW = SkillHandler.W.IsInRange(target);
            var targetE = SkillHandler.E.IsInRange(target);

            if (!mana) { return; }

            if (useE && targetE)
            {
                SkillHandler.E.CastIfHitchanceEquals(target, HitChance.Medium, PacketCast);
            }
            if (useQ && targetQ)
            {
                SkillHandler.Q.CastOnUnit(target, PacketCast);
            }
            if (!useQ && useW && targetW && StatusW() == "normal")
            {
                SkillHandler.W.Cast(target, PacketCast);
            }
            if (StatusW() == "return")
            {
                SkillHandler.W.Cast(PacketCast);
            }
        }

        public static void WLogic()
        {
            var t = GetEnemy(SkillHandler.E.Range, TargetSelector.DamageType.Magical);
            var countW = _wpos.CountEnemysInRange(200) >= MenuHandler.LeBlancConfig.SubMenu("Misc").SubMenu("backW").Item("SWcountEnemy").GetValue<Slider>().Value;
            var playerhealth = Player.HealthPercentage() <= MenuHandler.LeBlancConfig.SubMenu("Misc").SubMenu("backW").Item("SWplayerHp").GetValue<Slider>().Value;
            var useW = MenuHandler.LeBlancConfig.SubMenu("Misc").SubMenu("backW").Item("useSW").GetValue<bool>();
            var alloncd = !SkillHandler.Q.IsReady() && !SkillHandler.W.IsReady() && !SkillHandler.E.IsReady() &&
                          !SkillHandler.R.IsReady();
            var flee = MenuHandler.LeBlancConfig.Item("FleeK").GetValue<KeyBind>().Active;
            var playermana = Player.ManaPercentage() < 50;
            var targethealth = t.HealthPercentage() > 60;

            if (StatusW() == "return" && useW && !countW || flee || playerhealth || (alloncd && playermana && targethealth))
            {
                SkillHandler.W.Cast(PacketCast);
            }
        }

        private static void Combo()
        {
            var t = GetEnemy(SkillHandler.E.Range, TargetSelector.DamageType.Magical);
            var useQ = SkillHandler.Q.IsReady() && MenuHandler.LeBlancConfig.SubMenu("Combo").Item("useQ").GetValue<bool>() && SkillHandler.Q.IsInRange(t);
            var useW = SkillHandler.W.IsReady() && MenuHandler.LeBlancConfig.SubMenu("Combo").Item("useW").GetValue<bool>() && SkillHandler.W.IsInRange(t);
            var useE = SkillHandler.E.IsReady() && MenuHandler.LeBlancConfig.SubMenu("Combo").Item("useE").GetValue<bool>() && SkillHandler.E.IsInRange(t);
            var useR = SkillHandler.R.IsReady() && MenuHandler.LeBlancConfig.SubMenu("Combo").Item("useR").GetValue<bool>() && SkillHandler.R.IsInRange(t);
            var useIgnite = ItemHandler.Igniteslot != SpellSlot.Unknown && MenuHandler.LeBlancConfig.SubMenu("Items").Item("useIgnite").GetValue<bool>() &&
                            Player.Spellbook.CanUseSpell(ItemHandler.Igniteslot) == SpellState.Ready && Player.ServerPosition.Distance(t.ServerPosition) < 600;
            var useDfg = ItemHandler.Dfg.IsReady() && ItemHandler.Dfg.IsInRange(t) &&MenuHandler.LeBlancConfig.SubMenu("Items").Item("useDfg").GetValue<bool>();
            var prior = MathHandler.RqDamage(t) < MathHandler.RwDamage(t);

            //Items
            if (t.Health <= MathHandler.ComboDamage(t) && useDfg)
            {
                ItemHandler.Dfg.Cast(t);
            }

            if (t.Health <= MathHandler.ComboDamage(t) && useIgnite)
            {
                Player.Spellbook.CastSpell(ItemHandler.Igniteslot, t);
            }

            //Spells
            if (useE && (!SkillHandler.Q.IsReady() && !SkillHandler.W.IsReady() && !SkillHandler.R.IsReady()) || Player.ServerPosition.Distance(t.ServerPosition) > SkillHandler.Q.Range)
            {
                SkillHandler.E.CastIfHitchanceEquals(t, HitChance.Medium, PacketCast);
            }
            if (useQ)
            {
                SkillHandler.Q.CastOnUnit(t, PacketCast);
            }
            if (useW && !SkillHandler.Q.IsReady() && StatusW() == "normal")
            {
                SkillHandler.W.Cast(t.ServerPosition, PacketCast);
            }
            if (!SkillHandler.Q.IsReady() && !SkillHandler.W.IsReady() && !SkillHandler.E.IsReady() && useR &&
                StatusR() == "E")
            {
                SkillHandler.R.CastIfHitchanceEquals(t, HitChance.Medium, PacketCast);
            }
            switch (prior)
            {
                case true:
                    {
                        if (useR && StatusR() == "W")
                            SkillHandler.R.Cast(t, PacketCast);
                        break;
                    }
                case false:
                    {
                        if (useR && StatusR() == "Q")
                            SkillHandler.R.CastOnUnit(t, PacketCast);
                        break;
                    }
            }
            if (t.IsDead && StatusW() == "return" && MenuHandler.LeBlancConfig.SubMenu("Misc").SubMenu("backW").Item("SWtargetdead").GetValue<bool>() && MenuHandler.LeBlancConfig.SubMenu("Misc").SubMenu("backW").Item("useSW").GetValue<bool>())
                    {
                        SkillHandler.W.Cast(Player, PacketCast);
                    }
        }

        private static void ComboGapClose()
        {
            var t = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Magical);
            var useW = SkillHandler.W.IsReady() && MenuHandler.LeBlancConfig.SubMenu("Combo").Item("useW").GetValue<bool>() && StatusW() == "normal";

            if (useW)
            {
                SkillHandler.W.Cast(t.ServerPosition, PacketCast);
            }

            if (!SkillHandler.W.IsReady())
            {
                Combo();
            }
        }

        public static void ComboLogic()
        {
            var t = GetEnemy(1000, TargetSelector.DamageType.Magical);
            var tq = SkillHandler.W.IsInRange(t);

            if (!tq && t.Health < MathHandler.ComboDamage(t) - ((45 + (40 * SkillHandler.W.Level)) + ((Player.BaseAbilityDamage + Player.FlatMagicDamageMod) * 0.6)))
            {
                ComboGapClose();
            }
            else
            {
                Combo();
            }
        }
    }
}
