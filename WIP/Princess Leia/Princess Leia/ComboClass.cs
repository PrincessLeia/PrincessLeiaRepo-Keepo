using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_Leia
{
    internal class ComboClass
    {
        public static Menu ComboMenu;
        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }


        public static void Init()
        {
            Menu();
            Game.OnGameUpdate += OnGameUpdate;
        }

        public static void Menu()
        {
            Menu comboMenu = new Menu("Combo", "Combo");
            comboMenu.AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(Program.LeBlancConfig.Item("Orbwalk").GetValue<KeyBind>().Key, KeyBindType.Press)));
            comboMenu.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("useR", "Use R").SetValue(true));
            comboMenu.AddItem(new MenuItem("preE", "Minimum HitChance E").SetValue(new StringList((new[] { "Low", "Medium", "High", "Very High" }), 1)));
            comboMenu.AddItem(new MenuItem("preR", "Minimum HitChance R(E)").SetValue(new StringList((new[] { "Low", "Medium", "High", "Very High" }), 1)));

            ComboMenu = comboMenu;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            if (Program.Orbwalker.ActiveMode.ToString() != "Combo" || Player.IsDead || Player.HasBuff("Recall"))
                return;

            var t = GetEnemy(SpellClass.E.Range, TargetSelector.DamageType.Magical);
            Combo(t);
        }

        private static void Combo(Obj_AI_Base t)
        {
            var useQ = ComboMenu.Item("useQ").GetValue<bool>();
            var useW = ComboMenu.Item("useW").GetValue<bool>();
            var useE = ComboMenu.Item("useE").GetValue<bool>();
            var useR = ComboMenu.Item("useR").GetValue<bool>();
            var useDfg = ItemClass.ItemMenu.Item("useDfg").GetValue<bool>() && t.Health < DamageClass.ComboDamage(t);
            var hitE = (HitChance)(ComboMenu.Item("preE").GetValue<StringList>().SelectedIndex + 3);
            var hitR = (HitChance)(ComboMenu.Item("preE").GetValue<StringList>().SelectedIndex + 3);

            ItemClass.CastDfg(t, useDfg);
            SpellClass.CastE(t, hitE, useE);
            SpellClass.CastQ(t, useQ);
            SpellClass.CastW(t, useW);

            if (SpellClass.W.Level > SpellClass.Q.Level)
            {
                SpellClass.CastRw(t, useR);
            }
            else
            {
                SpellClass.CastRq(t, useR);
            }
            if (!SpellClass.Q.IsReady() && !SpellClass.W.IsReady() && !SpellClass.E.IsReady() &&
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSoulShackleM")
            {
                SpellClass.CastRe(t, hitR, useR);
            }
        }

        static Obj_AI_Hero GetEnemy(float vDefaultRange = 0, TargetSelector.DamageType vDefaultDamageType = TargetSelector.DamageType.Physical)
        {
            if (Math.Abs(vDefaultRange) < 0.00001)
                vDefaultRange = SpellClass.Q.Range;

            if (!Program.LeBlancConfig.Item("AssassinActive").GetValue<bool>())
                return TargetSelector.GetTarget(vDefaultRange, vDefaultDamageType);

            var assassinRange = Program.LeBlancConfig.Item("AssassinSearchRange").GetValue<Slider>().Value;

            var vEnemy = ObjectManager.Get<Obj_AI_Hero>()
                .Where(
                    enemy =>
                        enemy.Team != ObjectManager.Player.Team && !enemy.IsDead && enemy.IsVisible &&
                        Program.LeBlancConfig.Item("Assassin" + enemy.ChampionName) != null &&
                        Program.LeBlancConfig.Item("Assassin" + enemy.ChampionName).GetValue<bool>() &&
                        ObjectManager.Player.Distance(enemy) < assassinRange);

            if (Program.LeBlancConfig.Item("AssassinSelectOption").GetValue<StringList>().SelectedIndex == 1)
            {
                vEnemy = (from vEn in vEnemy select vEn).OrderByDescending(vEn => vEn.MaxHealth);
            }

            Obj_AI_Hero[] objAiHeroes = vEnemy as Obj_AI_Hero[] ?? vEnemy.ToArray();

            Obj_AI_Hero t = !objAiHeroes.Any()
                ? TargetSelector.GetTarget(vDefaultRange, vDefaultDamageType)
                : objAiHeroes[0];

            return t;
        }
       
    }
}
