using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace Princess_Diana
{
    class MenuHandler
    {
        public static Menu DianaConfig;
        public static Orbwalking.Orbwalker orb;
        public static void Init()
        {

        DianaConfig = new Menu(ObjectManager.Player.ChampionName, ObjectManager.Player.ChampionName, true);

        Menu orbwalker = new Menu("Orbwalker", "orbwalker");
        orb = new Orbwalking.Orbwalker(orbwalker);
        DianaConfig.AddSubMenu(orbwalker);

        var targetselectormenu = new Menu("Target Selector", "Common_TargetSelector");
        SimpleTs.AddToMenu(targetselectormenu);
        DianaConfig.AddSubMenu(targetselectormenu);

        //Submenu Spells
        DianaConfig.AddSubMenu(new Menu("Combo", "skillUsageCombo"));
        DianaConfig.SubMenu("skillUsageCombo").AddItem(new MenuItem("CombouseQ", "Use Q").SetValue(true));
        DianaConfig.SubMenu("skillUsageCombo").AddItem(new MenuItem("CombouseW", "Use W").SetValue(true));
        DianaConfig.SubMenu("skillUsageCombo").AddItem(new MenuItem("CombouseE", "Use E").SetValue(true));
        DianaConfig.SubMenu("skillUsageCombo").AddItem(new MenuItem("CombouseR", "Use R").SetValue(true));
        DianaConfig.SubMenu("skillUsageCombo").AddItem(new MenuItem("Combouse2RKill", "Use second R only to Kill").SetValue(true));

        DianaConfig.AddSubMenu(new Menu("Harass", "Harass"));
        DianaConfig.SubMenu("Harass").AddItem(new MenuItem("HarassActive", "Harass").SetValue(new KeyBind('C', KeyBindType.Press)));
        DianaConfig.SubMenu("Harass").AddItem(new MenuItem("HarassToggle", "Harass").SetValue(new KeyBind('T', KeyBindType.Toggle)));
        DianaConfig.SubMenu("Harass").AddItem(new MenuItem("haraQ", "Use Q").SetValue(true));
        DianaConfig.SubMenu("Harass").AddItem(new MenuItem("HaraManaPercent", "Minimum Mana Percent").SetValue(new Slider(30, 0, 100)));

        DianaConfig.AddSubMenu(new Menu("LaneClear", "LaneClear"));
        DianaConfig.SubMenu("LaneClear").AddItem(new MenuItem("LCQ", "Use Q").SetValue(true));
        DianaConfig.SubMenu("LaneClear").AddItem(new MenuItem("LCW", "Use W").SetValue(true));

        DianaConfig.AddSubMenu(new Menu("Killsteal", "Killsteal"));
        DianaConfig.SubMenu("Killsteal").AddItem(new MenuItem("KSq", "Use Q").SetValue(true));
        DianaConfig.SubMenu("Killsteal").AddItem(new MenuItem("KSr", "Use R").SetValue(true));
        DianaConfig.SubMenu("Killsteal").AddItem(new MenuItem("KSi", "Use Ignite").SetValue(true));

        DianaConfig.AddSubMenu(new Menu("Items", "ItemUsage"));
        DianaConfig.SubMenu("ItemUsage").AddItem(new MenuItem("useDfg", "Use DFG").SetValue(true));

        MenuItem dmgAfterComboItem = new MenuItem("DamageAfterCombo", "Draw damage after combo").SetValue(true);
        Utility.HpBarDamageIndicator.DamageToUnit = MathHandler.ComboDamage;
        Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
        dmgAfterComboItem.ValueChanged +=
            delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };

        DianaConfig.AddSubMenu(new Menu("Drawing", "Drawings"));
        DianaConfig.SubMenu("Drawing").AddItem(new MenuItem("drawQ", "Draw Q").SetValue(new Circle(true, Color.FromArgb(100, Color.Aqua))));
        DianaConfig.SubMenu("Drawing").AddItem(new MenuItem("drawR", "Draw R").SetValue(new Circle(true, Color.FromArgb(100, Color.Aqua))));
        DianaConfig.SubMenu("Drawing").AddItem(new MenuItem("drawA", "Draw passive Auto").SetValue(new Circle(true, Color.FromArgb(100, Color.Red))));
        DianaConfig.SubMenu("Drawing").AddItem(dmgAfterComboItem);
        DianaConfig.SubMenu("Drawing").AddItem(new MenuItem("HUD", "Draw HUD").SetValue(true));

        DianaConfig.AddSubMenu(new Menu("Misc", "misc"));
        DianaConfig.SubMenu("misc").AddItem(new MenuItem("packets", "Use Packets").SetValue(true));
        DianaConfig.SubMenu("misc").AddItem(new MenuItem("gapcloW", "Gapclose with W").SetValue(true));
        DianaConfig.SubMenu("misc").AddItem(new MenuItem("interE", "Interrupt with E").SetValue(true));
            
        // attach to 'Sift/F9' Menu
        DianaConfig.AddToMainMenu();
    }

    }
}
