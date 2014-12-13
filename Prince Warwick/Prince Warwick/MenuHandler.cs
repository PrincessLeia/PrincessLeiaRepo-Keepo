#region

using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;

#endregion

namespace Prince_Warwick
{
    internal class MenuHandler
    {
        public static Menu WarwickConfig;
        internal static Orbwalking.Orbwalker Orb;

        public static void Init()
        {
            WarwickConfig = new Menu(ObjectManager.Player.ChampionName, ObjectManager.Player.ChampionName, true);

            Menu orbwalker = new Menu("Orbwalker", "orbwalker");

            Orb = new Orbwalking.Orbwalker(orbwalker);
            WarwickConfig.AddSubMenu(orbwalker);

            var targetselectormenu = new Menu("Target Selector", "Common_TargetSelector");
            SimpleTs.AddToMenu(targetselectormenu);
            WarwickConfig.AddSubMenu(targetselectormenu);

            WarwickConfig.AddSubMenu(new Menu("Combo", "Combo"));
            WarwickConfig.SubMenu("Combo").AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            WarwickConfig.SubMenu("Combo").AddItem(new MenuItem("useW", "Use W").SetValue(true));
            WarwickConfig.SubMenu("Combo").AddItem(new MenuItem("useR", "Use R").SetValue(true));

            WarwickConfig.AddSubMenu(new Menu("Jungle Clear", "ClearJ"));
            WarwickConfig.SubMenu("ClearJ").AddItem(new MenuItem("usejQ", "Use Q").SetValue(true));
            WarwickConfig.SubMenu("ClearJ").AddItem(new MenuItem("usejW", "Use W").SetValue(true));
            WarwickConfig.SubMenu("ClearJ").AddItem(new MenuItem("usejIt", "Use Items").SetValue(true));
            WarwickConfig.SubMenu("ClearJ")
                .AddItem(
                    new MenuItem("JungleClearManaPercent", "Minimum Mana Percent").SetValue(new Slider(30, 0, 100)));

            WarwickConfig.AddSubMenu(new Menu("Lane Clear", "ClearL"));
            WarwickConfig.SubMenu("ClearL").AddItem(new MenuItem("LaneClearQ", "Use Q").SetValue(true));
            WarwickConfig.SubMenu("ClearL").AddItem(new MenuItem("LaneClearW", "Use W").SetValue(true));
            WarwickConfig.SubMenu("ClearL").AddItem(new MenuItem("uselIt", "Use Items").SetValue(true));
            WarwickConfig.SubMenu("ClearL")
                .AddItem(new MenuItem("LaneClearManaPercent", "Minimum Mana Percent").SetValue(new Slider(30, 0, 100)));

            WarwickConfig.AddSubMenu(new Menu("Items", "Items"));
            WarwickConfig.SubMenu("Items").AddItem(new MenuItem("useDfg", "Use DFG").SetValue(true));
            WarwickConfig.SubMenu("Items").AddItem(new MenuItem("useBilge", "Use Bilgewater Cutless").SetValue(true));
            WarwickConfig.SubMenu("Items").AddItem(new MenuItem("useBortK", "Use BotrK").SetValue(true));
            WarwickConfig.SubMenu("Items").AddItem(new MenuItem("useYoumuu", "Use Youmuu's").SetValue(true));
            WarwickConfig.SubMenu("Items").AddItem(new MenuItem("useTiamat", "Use Tiamat").SetValue(true));
            WarwickConfig.SubMenu("Items").AddItem(new MenuItem("useHydra", "Use Hydra").SetValue(true));
            WarwickConfig.SubMenu("Items").AddItem(new MenuItem("gapcloR", "Gapclose with Randuin's").SetValue(true));

            WarwickConfig.AddSubMenu(new Menu("KillSteal", "KS"));
            WarwickConfig.SubMenu("KS").AddItem(new MenuItem("KSi", "Use Ignite").SetValue(true));
            WarwickConfig.SubMenu("KS").AddItem(new MenuItem("KSq", "Use Q").SetValue(true));

            MenuItem dmgAfterComboItem = new MenuItem("DamageAfterCombo", "Draw damage after combo").SetValue(true);
            Utility.HpBarDamageIndicator.DamageToUnit = MathHandler.ComboDamage;
            Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
            dmgAfterComboItem.ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                };

            WarwickConfig.AddSubMenu(new Menu("Drawings", "Drawing"));
            WarwickConfig.SubMenu("Drawing").AddItem(new MenuItem("HUD", "HUD").SetValue(true));
            WarwickConfig.SubMenu("Drawing").AddItem(dmgAfterComboItem);
            WarwickConfig.SubMenu("Drawing")
                .AddItem(new MenuItem("drawQ", "Draw Q").SetValue(new Circle(true, Color.FromArgb(100, Color.Aqua))));
            WarwickConfig.SubMenu("Drawing")
                .AddItem(
                    new MenuItem("drawE", "Draw E passive").SetValue(new Circle(true, Color.FromArgb(100, Color.Aqua))));
            WarwickConfig.SubMenu("Drawing")
                .AddItem(new MenuItem("drawR", "Draw R").SetValue(new Circle(true, Color.FromArgb(100, Color.Aqua))));
            WarwickConfig.SubMenu("Drawing")
                .AddItem(
                    new MenuItem("drawQss", "Draw if Target has QSS").SetValue(
                        new Circle(true, Color.FromArgb(100, Color.Red))));

            WarwickConfig.AddSubMenu(new Menu("Misc", "Misc"));
            WarwickConfig.SubMenu("Misc")
                .AddItem(new MenuItem("autoULT", "ULT on LeftClick").SetValue(new KeyBind('T', KeyBindType.Toggle)));
            WarwickConfig.SubMenu("Misc").AddItem(new MenuItem("packets", "Packets").SetValue(true));
            WarwickConfig.SubMenu("Misc").AddItem(new MenuItem("InterR", "Interrupt with R").SetValue(true));
            WarwickConfig.SubMenu("Misc").AddItem(new MenuItem("notULT", "Not ULT on QSS").SetValue(true));

            WarwickConfig.AddItem(new MenuItem("madebyme", "PrincessLeia :)").DontSave());
            WarwickConfig.AddToMainMenu();
        }
    }
}