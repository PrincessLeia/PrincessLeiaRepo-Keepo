using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_Leia
{
    internal class Program
    {
        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }

        public static Menu LeBlancConfig;
        public static Menu TargetSelectorMenu;
        public static Orbwalking.Orbwalker Orbwalker;

        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Leblanc")
            {
                return;
            }
            
            ItemClass.Init();
            SpellClass.Init();
            ComboClass.Init();
            LaneClearClass.Init();
            HarassClass.Init();
            FleeClass.Init();
            MiscClass.Init();
            MainMenu();

            Game.PrintChat("<b><font color =\"#FFFFFF\">Princess LeBlanc</font></b><font color =\"#FFFFFF\"> by </font><b><font color=\"#FF66FF\">Leia</font></b><font color =\"#FFFFFF\"> loaded!</font>");
        }

        private static void MainMenu()
        {
            LeBlancConfig = new Menu("Prince " + ObjectManager.Player.ChampionName, "Prince" + ObjectManager.Player.ChampionName, true);

            LeBlancConfig.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(LeBlancConfig.SubMenu("Orbwalking"));

            TargetSelectorMenu = new Menu("Target Selector", "Common_TargetSelector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            LeBlancConfig.AddSubMenu(TargetSelectorMenu);

            new AssassinManager();

            ComboClass.Menu();
            LeBlancConfig.AddSubMenu(ComboClass.ComboMenu);

            LaneClearClass.Menu();
            LeBlancConfig.AddSubMenu(LaneClearClass.LaneClearMenu);

            HarassClass.Menu();
            LeBlancConfig.AddSubMenu(HarassClass.HarassMenu);

            FleeClass.Menu();
            LeBlancConfig.AddSubMenu(FleeClass.FleeMenu);

            ItemClass.Menu();
            LeBlancConfig.AddSubMenu(ItemClass.ItemMenu);

            MiscClass.Menu();
            LeBlancConfig.AddSubMenu(MiscClass.MiscMenu);
            


            MenuItem dmgAfterComboItem = new MenuItem("DamageAfterCombo", "Draw damage after combo").SetValue(true);
            Utility.HpBarDamageIndicator.DamageToUnit = DamageClass.ComboDamage;
            Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
            dmgAfterComboItem.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };
            LeBlancConfig.SubMenu("Drawing").AddItem(dmgAfterComboItem);

            LeBlancConfig.AddToMainMenu();
        }
    }
}
