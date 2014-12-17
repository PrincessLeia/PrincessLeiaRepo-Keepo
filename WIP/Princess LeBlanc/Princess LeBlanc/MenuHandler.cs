using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace Princess_LeBlanc
{
    class MenuHandler
    {
        public static Menu LeBlancConfig;
        internal static Orbwalking.Orbwalker Orb;
        public static void Init()
        {
        LeBlancConfig = new Menu(ObjectManager.Player.ChampionName, ObjectManager.Player.ChampionName, true);

        Menu orbwalker = new Menu("Orbwalker", "orbwalker");

        Orb = new Orbwalking.Orbwalker(orbwalker);
        LeBlancConfig.AddSubMenu(orbwalker);

        var targetselectormenu = new Menu("Target Selector", "Common_TargetSelector");
        SimpleTs.AddToMenu(targetselectormenu);
        LeBlancConfig.AddSubMenu(targetselectormenu);

        LeBlancConfig.AddSubMenu(new Menu("Combo", "Combo"));
        LeBlancConfig.SubMenu("Combo").AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
        LeBlancConfig.SubMenu("Combo").AddItem(new MenuItem("useW", "Use W").SetValue(true));
        LeBlancConfig.SubMenu("Combo").AddItem(new MenuItem("useE", "Use E").SetValue(true));
        LeBlancConfig.SubMenu("Combo").AddItem(new MenuItem("useR", "Use R").SetValue(true));

        LeBlancConfig.AddSubMenu(new Menu("Lane Clear", "ClearL"));
        LeBlancConfig.SubMenu("ClearL").AddItem(new MenuItem("LaneClearQ", "Use Q").SetValue(true));
        LeBlancConfig.SubMenu("ClearL").AddItem(new MenuItem("LaneClearW", "Use W").SetValue(true));
        LeBlancConfig.SubMenu("ClearL").AddItem(new MenuItem("uselIt", "Use Items").SetValue(true));
        LeBlancConfig.SubMenu("ClearL").AddItem(new MenuItem("LaneClearManaPercent", "Minimum Mana Percent").SetValue(new Slider(30, 0, 100)));

        LeBlancConfig.AddSubMenu(new Menu("Items", "Items"));
        LeBlancConfig.SubMenu("Items").AddItem(new MenuItem("useDfg", "Use DFG").SetValue(true));

        LeBlancConfig.AddSubMenu(new Menu("KillSteal", "KS"));
        LeBlancConfig.SubMenu("KS").AddItem(new MenuItem("KSi", "Use Ignite").SetValue(true));
        LeBlancConfig.SubMenu("KS").AddItem(new MenuItem("KSq", "Use Q").SetValue(true));
        LeBlancConfig.SubMenu("KS").AddItem(new MenuItem("KSw", "Use W").SetValue(true));

        MenuItem dmgAfterComboItem = new MenuItem("DamageAfterCombo", "Draw damage after combo").SetValue(true);
        Utility.HpBarDamageIndicator.DamageToUnit = MathHandler.ComboDamage;
        Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
        dmgAfterComboItem.ValueChanged +=
            delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };

        LeBlancConfig.AddSubMenu(new Menu("Drawings", "Drawing"));
        LeBlancConfig.SubMenu("Drawing").AddItem(new MenuItem("HUD", "HUD").SetValue(true));
        LeBlancConfig.SubMenu("Drawing").AddItem(dmgAfterComboItem);
        LeBlancConfig.SubMenu("Drawing").AddItem(new MenuItem("drawQ", "Draw Q").SetValue(new Circle(true, Color.FromArgb(100, Color.Aqua))));
        LeBlancConfig.SubMenu("Drawing").AddItem(new MenuItem("drawW", "Draw W").SetValue(new Circle(true, Color.FromArgb(100, Color.Aqua))));
        LeBlancConfig.SubMenu("Drawing").AddItem(new MenuItem("drawE", "Draw E passive").SetValue(new Circle(true, Color.FromArgb(100, Color.Aqua))));

        LeBlancConfig.AddSubMenu(new Menu("Misc", "Misc"));
        LeBlancConfig.SubMenu("Misc").AddItem(new MenuItem("packets", "Packets").SetValue(true));

        LeBlancConfig.AddItem(new MenuItem("madebyme", "PrincessLeia :)").DontSave());
        LeBlancConfig.AddToMainMenu();
    }

    }
}
