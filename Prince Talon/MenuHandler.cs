using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace PrinceTalon
{
    class MenuHandler
    {
        public static Menu TalonConfig;
        internal static Orbwalking.Orbwalker Orb;
        public static void Init()
        {
        TalonConfig = new Menu(ObjectManager.Player.ChampionName, ObjectManager.Player.ChampionName, true);

        Menu orbwalker = new Menu("Orbwalker", "orbwalker");

        Orb = new Orbwalking.Orbwalker(orbwalker);
        TalonConfig.AddSubMenu(orbwalker);

        var targetselectormenu = new Menu("Target Selector", "Common_TargetSelector");
        TargetSelector.AddToMenu(targetselectormenu);
        TalonConfig.AddSubMenu(targetselectormenu);

        TalonConfig.AddSubMenu(new Menu("Combo", "Combo"));
        TalonConfig.SubMenu("Combo").AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
        TalonConfig.SubMenu("Combo").AddItem(new MenuItem("useW", "Use W").SetValue(true));
        TalonConfig.SubMenu("Combo").AddItem(new MenuItem("useE", "Use E").SetValue(true));
        TalonConfig.SubMenu("Combo").AddItem(new MenuItem("useR", "Use R").SetValue(true));

        TalonConfig.AddSubMenu(new Menu("Lane Clear", "ClearL"));
        TalonConfig.SubMenu("ClearL").AddItem(new MenuItem("LaneClearQ", "Use Q").SetValue(true));
        TalonConfig.SubMenu("ClearL").AddItem(new MenuItem("LaneClearW", "Use W").SetValue(true));
        TalonConfig.SubMenu("ClearL").AddItem(new MenuItem("uselIt", "Use Items").SetValue(true));
        TalonConfig.SubMenu("ClearL").AddItem(new MenuItem("LaneClearManaPercent", "Minimum Mana Percent").SetValue(new Slider(30, 0, 100)));

        TalonConfig.AddSubMenu(new Menu("Jungle Clear", "ClearJ"));
        TalonConfig.SubMenu("ClearJ").AddItem(new MenuItem("usejQ", "Use Q").SetValue(true));
        TalonConfig.SubMenu("ClearJ").AddItem(new MenuItem("usejW", "Use W").SetValue(true));
        TalonConfig.SubMenu("ClearJ").AddItem(new MenuItem("usejIt", "Use Items").SetValue(true));
        TalonConfig.SubMenu("ClearJ").AddItem(new MenuItem("JungleClearManaPercent", "Minimum Mana Percent").SetValue(new Slider(30, 0, 100)));

        TalonConfig.AddSubMenu(new Menu("Harass", "Harass"));
        TalonConfig.SubMenu("Harass").AddItem(new MenuItem("HarassToggle", "Auto Harass Toggle").SetValue(new KeyBind('T', KeyBindType.Toggle)));
        TalonConfig.SubMenu("Harass").AddItem(new MenuItem("haraW", "Use W").SetValue(true));
        TalonConfig.SubMenu("Harass").AddItem(new MenuItem("HarassManaPercent", "Minimum Mana Percent").SetValue(new Slider(30, 0, 100)));

        TalonConfig.AddSubMenu(new Menu("Items", "Items"));
        TalonConfig.SubMenu("Items").AddItem(new MenuItem("useBilge", "Use Bilgewater Cutless").SetValue(true));
        TalonConfig.SubMenu("Items").AddItem(new MenuItem("useBortK", "Use BotrK").SetValue(true));
        TalonConfig.SubMenu("Items").AddItem(new MenuItem("useYoumuu", "Use Youmuu's").SetValue(true));
        TalonConfig.SubMenu("Items").AddItem(new MenuItem("useTiamat", "Use Tiamat").SetValue(true));
        TalonConfig.SubMenu("Items").AddItem(new MenuItem("useHydra", "Use Hydra").SetValue(true));

        TalonConfig.AddSubMenu(new Menu("KillSteal", "KS"));
        TalonConfig.SubMenu("KS").AddItem(new MenuItem("KSi", "Use Ignite").SetValue(true));
        TalonConfig.SubMenu("KS").AddItem(new MenuItem("KSq", "Use Q").SetValue(true));
        TalonConfig.SubMenu("KS").AddItem(new MenuItem("KSw", "Use W").SetValue(true));

        MenuItem dmgAfterComboItem = new MenuItem("DamageAfterCombo", "Draw damage after combo").SetValue(true);
        Utility.HpBarDamageIndicator.DamageToUnit = MathHandler.ComboDamage;
        Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
        dmgAfterComboItem.ValueChanged +=
            delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };

        TalonConfig.AddSubMenu(new Menu("Drawings", "Drawing"));
        TalonConfig.SubMenu("Drawing").AddItem(new MenuItem("HUD", "HUD").SetValue(true));
        TalonConfig.SubMenu("Drawing").AddItem(dmgAfterComboItem);
        TalonConfig.SubMenu("Drawing").AddItem(new MenuItem("drawW", "Draw W").SetValue(new Circle(true, Color.FromArgb(100, Color.Aqua))));
        TalonConfig.SubMenu("Drawing").AddItem(new MenuItem("drawE", "Draw E").SetValue(new Circle(true, Color.FromArgb(100, Color.Aqua))));
        TalonConfig.SubMenu("Drawing").AddItem(new MenuItem("drawR", "Draw R").SetValue(new Circle(true, Color.FromArgb(100, Color.Aqua))));

        TalonConfig.AddItem(new MenuItem("madebyme", "PrincessLeia :)").DontSave());
        TalonConfig.AddToMainMenu();
    }

    }
}
