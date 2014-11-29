using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using xSLx_Orbwalker;
using Color = System.Drawing.Color;

namespace Ultimate_Carry_Prevolution.Plugin
{
    class Akali_2 : Champion
    {

        public Akali_2()
        {
            Game.PrintChat("Akali Beta Test Loaded");
            SetSpells();
            LoadMenu();
        }

        private void SetSpells()
        {
            Q = new Spell(SpellSlot.Q, 600);

            W = new Spell(SpellSlot.W, 700);

            E = new Spell(SpellSlot.E, 325);

            R = new Spell(SpellSlot.R, 800);
        }

        private void LoadMenu()
        {
            var champMenu = new Menu("Akali Plugin", "Akali");
            {
                var spellMenu = new Menu("SpellMenu", "SpellMenu");
                {
                    var wMenu = new Menu("WMenu", "WMenu");
                    {
                        wMenu.AddItem(new MenuItem("useW_enemyCount", "Use W if x Enemys Arround")).SetValue(new Slider(1, 1, 5));
                        wMenu.AddItem(new MenuItem("useW_Health", "Use W if health below").SetValue(new Slider(25)));
                        spellMenu.AddSubMenu(wMenu);
                    }

                    var eMenu = new Menu("EMenu", "EMenu");
                    {
                        eMenu.AddItem(new MenuItem("E_On_Killable", "E to KS").SetValue(true));
                        eMenu.AddItem(new MenuItem("E_Wait_Q", "Wait For Q").SetValue(true));
                        spellMenu.AddSubMenu(eMenu);
                    }

                    var rMenu = new Menu("RMenu", "RMenu");
                    {
                        rMenu.AddItem(new MenuItem("R_Wait_For_Q", "Wait for Q Mark").SetValue(false));
                        rMenu.AddItem(new MenuItem("R_If_Killable", "R If Enemy Is killable").SetValue(true));
                        spellMenu.AddSubMenu(rMenu);
                    }

                    champMenu.AddSubMenu(spellMenu);
                }

                var comboMenu = new Menu("Combo", "Combo");
                {
                    comboMenu.AddItem(new MenuItem("Combo_mode", "Combo Mode").SetValue(new StringList(new[] { "Normal", "Q-R-AA-Q-E", "Q-Q-R-E-AA" }, 0)));
                    comboMenu.AddItem(new MenuItem("Combo_Switch", "Switch mode Key").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                    AddSpelltoMenu(comboMenu, "Q", true);
                    AddSpelltoMenu(comboMenu, "W", true);
                    AddSpelltoMenu(comboMenu, "E", true);
                    AddSpelltoMenu(comboMenu, "R", true);
                    AddSpelltoMenu(comboMenu, "Use Ignite", true);
                    AddSpelltoMenu(comboMenu, "Use Bilge and HexTech", true);
                    champMenu.AddSubMenu(comboMenu);
                }
                var harassMenu = new Menu("Harass", "Harass");
                {
                    AddSpelltoMenu(harassMenu, "Q", true);
                    AddSpelltoMenu(harassMenu, "E", true);
                    champMenu.AddSubMenu(harassMenu);
                }
                var laneClearMenu = new Menu("LaneClear", "LaneClear");
                {
                    AddSpelltoMenu(laneClearMenu, "Q", true);
                    AddSpelltoMenu(laneClearMenu, "E", true);
                    laneClearMenu.AddItem(new MenuItem("LaneClear_useE_minHit", "Use E if min. hit").SetValue(new Slider(2, 1, 6)));
                    champMenu.AddSubMenu(laneClearMenu);
                }
                var drawMenu = new Menu("Drawing", "Drawing");
                {
                    drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All").SetValue(false));
                    drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q").SetValue(true));
                    drawMenu.AddItem(new MenuItem("Draw_W", "Draw W").SetValue(true));
                    drawMenu.AddItem(new MenuItem("Draw_E", "Draw E").SetValue(true));
                    drawMenu.AddItem(new MenuItem("Draw_R", "Draw R").SetValue(true));
                    drawMenu.AddItem(new MenuItem("Current_Mode", "Draw current Mode").SetValue(true));

                    var drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage").SetValue(true);
                    drawMenu.AddItem(drawComboDamageMenu);
                    Utility.HpBarDamageIndicator.DamageToUnit = GetComboDamage;
                    Utility.HpBarDamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
                    drawComboDamageMenu.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                    };

                    champMenu.AddSubMenu(drawMenu);
                }
            }

            Menu.AddSubMenu(champMenu);
            Menu.AddToMainMenu();

        }

        private float GetComboDamage(Obj_AI_Base target)
        {
            var rStacks = GetRStacks();
            var comboDamage = 0d;
            int mode = Menu.Item("Combo_mode").GetValue<StringList>().SelectedIndex;

            if (mode == 0)
            {
                if (Q.IsReady())
                    comboDamage += (MyHero.GetSpellDamage(target, SpellSlot.Q) +
                                    MyHero.CalcDamage(target, Damage.DamageType.Magical,
                                        (45 + 35 * Q.Level + 0.5 * MyHero.FlatMagicDamageMod)));
            }
            else if (Q.IsReady())
            {
                comboDamage += (MyHero.GetSpellDamage(target, SpellSlot.Q) + MyHero.CalcDamage(target, Damage.DamageType.Magical, (45 + 35 * Q.Level + 0.5 * MyHero.FlatMagicDamageMod))) * 2;
            }

            if (E.IsReady())
                comboDamage += MyHero.GetSpellDamage(target, SpellSlot.E);

            if (HasBuff(target, "AkaliMota"))
                comboDamage += MyHero.CalcDamage(target, Damage.DamageType.Magical, (45 + 35 * Q.Level + 0.5 * MyHero.FlatMagicDamageMod));

            comboDamage += MyHero.CalcDamage(target, Damage.DamageType.Magical, CalcPassiveDmg());

            if (Items.CanUseItem(Bilge.Id))
                comboDamage += MyHero.GetItemDamage(target, Damage.DamageItems.Bilgewater);

            if (Items.CanUseItem(Hex.Id))
                comboDamage += MyHero.GetItemDamage(target, Damage.DamageItems.Hexgun);

            if (Ignite != SpellSlot.Unknown && MyHero.SummonerSpellbook.CanUseSpell(Ignite) == SpellState.Ready)
                comboDamage += MyHero.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

            if (rStacks > 0)
                comboDamage += MyHero.GetSpellDamage(target, SpellSlot.R) * rStacks;

            return (float)(comboDamage + MyHero.GetAutoAttackDamage(target));
        }

        public override void OnDraw()
        {
            if (Menu.Item("Draw_Disabled").GetValue<bool>())
            {
                xSLxOrbwalker.DisableDrawing();
                return;
            }
            xSLxOrbwalker.EnableDrawing();

            if (Menu.Item("Draw_Q").GetValue<bool>())
                if (Q.Level > 0)
                    Utility.DrawCircle(MyHero.Position, Q.Range, Q.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_W").GetValue<bool>())
                if (W.Level > 0)
                    Utility.DrawCircle(MyHero.Position, W.Range - 2, W.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_E").GetValue<bool>())
                if (E.Level > 0)
                    Utility.DrawCircle(MyHero.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_R").GetValue<bool>())
                if (R.Level > 0)
                    Utility.DrawCircle(MyHero.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Current_Mode").GetValue<bool>())
            {
                Vector2 wts = Drawing.WorldToScreen(MyHero.Position);
                int mode = Menu.Item("Combo_mode").GetValue<StringList>().SelectedIndex;
                if (mode == 0)
                    Drawing.DrawText(wts[0] - 20, wts[1], Color.White, "Normal");
                else if (mode == 1)
                    Drawing.DrawText(wts[0] - 20, wts[1], Color.White, "Q-R-AA-Q-E");
                else if (mode == 2)
                    Drawing.DrawText(wts[0] - 20, wts[1], Color.White, "Q-Q-R-E-AA");
            }

        }

        private int lasttick;
        public override void OnPassive()
        {
            int mode = Menu.Item("Combo_mode").GetValue<StringList>().SelectedIndex;
            int lasttime = Environment.TickCount - lasttick;

            if (Menu.Item("Combo_Switch").GetValue<KeyBind>().Active && lasttime > Game.Ping)
            {
                if (mode == 0)
                {
                    Menu.Item("Combo_mode").SetValue(new StringList(new[] { "Normal", "Q-R-AA-Q-E", "Q-Q-R-E-AA" }, 1));
                    lasttick = Environment.TickCount + 300;
                }
                else if (mode == 1)
                {
                    Menu.Item("Combo_mode").SetValue(new StringList(new[] { "Normal", "Q-R-AA-Q-E", "Q-Q-R-E-AA" }, 2));
                    lasttick = Environment.TickCount + 300;
                }
                else
                {
                    Menu.Item("Combo_mode").SetValue(new StringList(new[] { "Normal", "Q-R-AA-Q-E", "Q-Q-R-E-AA" }, 0));
                    lasttick = Environment.TickCount + 300;
                }
            }
        }
        public override void OnCombo()
        {
            int mode = Menu.Item("Combo_mode").GetValue<StringList>().SelectedIndex;
            switch (mode)
            {
                case 0:
                    if (IsSpellActive("Q"))
                        Cast_Q(true);
                    if (IsSpellActive("E"))
                        Cast_E(true);
                    if (IsSpellActive("W"))
                        Cast_W();
                    if (IsSpellActive("R"))
                        Cast_R(0);
                    break;
                case 1:
                    if (IsSpellActive("Q"))
                        Cast_Q(true, 1);
                    if (IsSpellActive("R"))
                        Cast_R(1);
                    if (IsSpellActive("E"))
                        Cast_E(true, 1);
                    if (IsSpellActive("W"))
                        Cast_W();
                    break;
                case 2:
                    if (IsSpellActive("Q"))
                        Cast_Q(true, 2);
                    if (IsSpellActive("R"))
                        Cast_R(2);
                    if (IsSpellActive("E"))
                        Cast_E(true, 2);
                    if (IsSpellActive("W"))
                        Cast_W();
                    break;
            }

            var qTarget = SimpleTs.GetTarget(650, SimpleTs.DamageType.Physical);
            if (qTarget != null)
            {
                if (GetComboDamage(qTarget) >= qTarget.Health && IsSpellActive("Use Ignite") &&
                    Ignite != SpellSlot.Unknown && MyHero.Distance(qTarget) < 650 &&
                    MyHero.SummonerSpellbook.CanUseSpell(Ignite) == SpellState.Ready)
                    Use_Ignite(qTarget);

                if (IsSpellActive("Use Bilge and HexTech"))
                {
                    if (GetComboDamage(qTarget) >= qTarget.Health &&
                        !qTarget.HasBuffOfType(BuffType.Slow))
                        Use_Bilge(qTarget);

                    if (GetComboDamage(qTarget) >= qTarget.Health &&
                        !qTarget.HasBuffOfType(BuffType.Slow))
                        Use_Hex(qTarget);
                }
            }

        }

        public override void OnHarass()
        {
            if (IsSpellActive("Q"))
                Cast_Q(true);
            if (IsSpellActive("E"))
                Cast_E(true);
        }

        public override void OnLaneClear()
        {
            if (IsSpellActive("Q"))
                Cast_Q(false);
            if (IsSpellActive("E"))
                Cast_E(false);
        }

        private Obj_AI_Hero CheckMark(float range)
        {
            return ObjectManager.Get<Obj_AI_Hero>().FirstOrDefault(x => x.IsValidTarget(range) && HasBuff(x, "AkaliMota") && x.IsVisible);
        }

        private void Cast_Q(bool combo, int mode = 0)
        {
            if (!Q.IsReady())
                return;
            if (combo)
            {
                var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
                if (!target.IsValidTarget(Q.Range))
                    return;

                if (CheckMark(Q.Range) != null)
                    target = CheckMark(Q.Range);

                if (mode == 0)
                {
                    Q.Cast(target, UsePackets());
                }
                else if (mode == 1)
                {
                    if (!HasBuff(target, "AkaliMota"))
                        Q.Cast(target);
                }
                else if (mode == 2)
                {
                    Q.Cast(target);
                    if (HasBuff(target, "AkaliMota"))
                        Q.LastCastAttemptT = Environment.TickCount + 400;
                }
            }
            else
            {
                foreach (var minion in MinionManager.GetMinions(MyHero.Position, Q.Range).Where(minion => HasBuff(minion, "AkaliMota") && xSLxOrbwalker.InAutoAttackRange(minion)))
                    xSLxOrbwalker.ForcedTarget = minion;

                foreach (var minion in MinionManager.GetMinions(MyHero.Position, Q.Range).Where(minion => HealthPrediction.GetHealthPrediction(minion,
                        (int)(E.Delay + (minion.Distance(MyHero) / E.Speed)) * 1000) <
                                                             MyHero.GetSpellDamage(minion, SpellSlot.Q) &&
                                                             HealthPrediction.GetHealthPrediction(minion,
                                                                 (int)(E.Delay + (minion.Distance(MyHero) / E.Speed)) * 1000) > 0 &&
                                                             xSLxOrbwalker.InAutoAttackRange(minion)))
                    Q.Cast(minion);

                foreach (var minion in MinionManager.GetMinions(MyHero.Position, Q.Range).Where(minion => HealthPrediction.GetHealthPrediction(minion,
                        (int)(Q.Delay + (minion.Distance(MyHero) / Q.Speed))) <
                                                             MyHero.GetSpellDamage(minion, SpellSlot.Q)))
                    Q.Cast(minion);

                foreach (var minion in MinionManager.GetMinions(MyHero.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).Where(minion => MyHero.Distance(minion) <= Q.Range))
                    Q.Cast(minion);
            }
        }

        private void Cast_W()
        {
            if (Menu.Item("useW_enemyCount").GetValue<Slider>().Value > Utility.CountEnemysInRange(400) &&
                Menu.Item("useW_Health").GetValue<Slider>().Value < (int)(MyHero.Health / MyHero.MaxHealth * 100))
                return;
            W.Cast(MyHero.Position, UsePackets());
        }

        private void Cast_E(bool combo, int mode = 0)
        {
            if (!E.IsReady())
                return;
            if (combo)
            {
                var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);
                if (target == null || !target.IsValidTarget(E.Range))
                    return;

                if (CheckMark(E.Range) != null)
                    target = CheckMark(Q.Range);

                if (mode == 0)
                {
                    if (HasBuff(target, "AkaliMota") && !Q.IsReady())
                        E.Cast();
                    else if (E.IsKillable(target) && Menu.Item("E_On_Killable").GetValue<bool>())
                        E.Cast();
                    else if (!Menu.Item("E_Wait_Q").GetValue<bool>())
                        E.Cast();
                }
                else if (mode == 1)
                {
                    if (HasBuff(target, "AkaliMota") && xSLxOrbwalker.InAutoAttackRange(target))
                        xSLxOrbwalker.ForcedTarget = target;
                    else if (HasBuff(target, "AkaliMota") && !Q.IsReady())
                        E.Cast();
                    else if (E.IsKillable(target) && Menu.Item("E_On_Killable").GetValue<bool>())
                        E.Cast();
                    else if (!Menu.Item("E_Wait_Q").GetValue<bool>())
                        E.Cast();
                }
                else if (mode == 2)
                {
                    if (HasBuff(target, "AkaliMota"))
                    {
                        E.Cast();
                        Menu.Item("Combo_mode").SetValue(new StringList(new[] { "Normal", "Q-R-AA-Q-E", "Q-Q-R-E-AA" }, 0));
                    }
                    else if (E.IsKillable(target) && Menu.Item("E_On_Killable").GetValue<bool>())
                        E.Cast();
                }
            }
            else
            {
                if (MinionManager.GetMinions(MyHero.Position, E.Range).Count >= Menu.Item("LaneClear_useE_minHit").GetValue<Slider>().Value)
                    E.Cast();
                foreach (var minion in MinionManager.GetMinions(MyHero.ServerPosition, Q.Range, MinionTypes.All,
                    MinionTeam.Neutral, MinionOrderTypes.MaxHealth).Where(minion => MyHero.Distance(minion) <= E.Range))
                    E.Cast();
            }
        }

        private double getSimpleDmg(Obj_AI_Hero target)
        {
            double dmg = 0;

            if (Q.IsReady())
                dmg += MyHero.GetSpellDamage(target, SpellSlot.Q) + MyHero.GetSpellDamage(target, SpellSlot.Q, 1);
            if (HasBuff(target, "AkaliMota"))
                dmg += MyHero.GetSpellDamage(target, SpellSlot.Q, 1);
            if (E.IsReady())
                dmg += MyHero.GetSpellDamage(target, SpellSlot.E);
            if (R.IsReady())
                dmg += MyHero.GetSpellDamage(target, SpellSlot.R)*GetRStacks();
            
            return dmg;
        }

        private void Cast_R(int mode)
        {
            var target = SimpleTs.GetTarget(R.Range + MyHero.BoundingRadius, SimpleTs.DamageType.Magical);
            if (target == null)
                return;

            if (CheckMark(Q.Range) != null)
                target = CheckMark(R.Range);

            if (target.IsValidTarget(R.Range) && R.IsReady())
            {
                if (R.IsKillable(target) && Menu.Item("R_If_Killable").GetValue<bool>())
                    R.Cast(target, UsePackets());
                else if (getSimpleDmg(target) > target.Health)
                    R.Cast(target, UsePackets());

                if (mode == 0)
                {
                    if (Menu.Item("R_Wait_For_Q").GetValue<bool>() && HasBuff(target, "AkaliMota"))
                        R.Cast(target, UsePackets());
                    else
                        R.Cast(target, UsePackets());
                }
                else if (mode == 1)
                {
                    if (HasBuff(target, "AkaliMota") && Q.IsReady())
                    {
                        R.Cast(target, UsePackets());
                        Menu.Item("Combo_mode").SetValue(new StringList(new[] { "Normal", "Q-R-AA-Q-E", "Q-Q-R-E-AA" }, 0));
                    }
                }
                else if (mode == 2)
                {
                    if (HasBuff(target, "AkaliMota") && Environment.TickCount - Q.LastCastAttemptT < Game.Ping)
                    {
                        R.Cast(target, UsePackets());
                    }
                }
            }
        }

        private double CalcPassiveDmg()
        {
            return (0.06 + 0.01 * (MyHero.FlatMagicDamageMod / 6)) * (MyHero.FlatPhysicalDamageMod + MyHero.BaseAttackDamage);
        }

        private int GetRStacks()
        {
            return (from buff in MyHero.Buffs
                    where buff.Name == "AkaliShadowDance"
                    select buff.Count).FirstOrDefault();
        }
    }
}
