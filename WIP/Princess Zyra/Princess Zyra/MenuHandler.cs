using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_Zyra
{
    class MenuHandler
    {
        public static Menu ZyraConfig;
        internal static Orbwalking.Orbwalker Orb;
        public static void Init()
        {
        ZyraConfig = new Menu(ObjectManager.Player.ChampionName, ObjectManager.Player.ChampionName, true);

        Menu orbwalker = new Menu("Orbwalker", "orbwalker");

        Orb = new Orbwalking.Orbwalker(orbwalker);
        ZyraConfig.AddSubMenu(orbwalker);

        var targetselectormenu = new Menu("Target Selector", "Common_TargetSelector");
        TargetSelector.AddToMenu(targetselectormenu);
        ZyraConfig.AddSubMenu(targetselectormenu);

       ZyraConfig.AddSubMenu(new Menu("TeamFight", "TeamFight"));
       ZyraConfig.SubMenu("TeamFight").AddItem(new MenuItem("useQ_TeamFight", "Use Q").SetValue(true));
       ZyraConfig.SubMenu("TeamFight").AddItem(new MenuItem("useE_TeamFight", "Use E").SetValue(true));
       ZyraConfig.SubMenu("TeamFight").AddItem(new MenuItem("useR_TeamFight_willhit", "Use R if hit").SetValue(new Slider(2, 5, 0)));

       ZyraConfig.AddSubMenu(new Menu("Harass", "Harass"));
       ZyraConfig.SubMenu("Harass").AddItem(new MenuItem("useQ_Harass", "Use Q").SetValue(true));
       ZyraConfig.SubMenu("Harass").AddItem(new MenuItem("useE_Harass", "Use E").SetValue(true));

       ZyraConfig.AddSubMenu(new Menu("LaneClear", "LaneClear"));
       ZyraConfig.SubMenu("LaneClear").AddItem(new MenuItem("useQ_LaneClear", "Use Q").SetValue(true));
       ZyraConfig.SubMenu("LaneClear").AddItem(new MenuItem("useE_LaneClear", "Use E").SetValue(true));

       ZyraConfig.AddSubMenu(new Menu("Passive", "Passive"));
       ZyraConfig.SubMenu("Passive").AddItem(new MenuItem("useW_Passive", "Plant on Spelllocations").SetValue(true));

       ZyraConfig.AddSubMenu(new Menu("SupportMode", "SupportMode"));
       ZyraConfig.SubMenu("SupportMode").AddItem(new MenuItem("hitMinions", "Hit Minions").SetValue(false));

       ZyraConfig.AddSubMenu(new Menu("Drawing", "Drawing"));
       ZyraConfig.SubMenu("Drawing").AddItem(new MenuItem("Draw_Disabled", "Disable All").SetValue(false));
       ZyraConfig.SubMenu("Drawing").AddItem(new MenuItem("Draw_Q", "Draw Q").SetValue(true));
       ZyraConfig.SubMenu("Drawing").AddItem(new MenuItem("Draw_W", "Draw W").SetValue(true));
       ZyraConfig.SubMenu("Drawing").AddItem(new MenuItem("Draw_E", "Draw E").SetValue(true));
       ZyraConfig.SubMenu("Drawing").AddItem(new MenuItem("Draw_R", "Draw R").SetValue(true));

       ZyraConfig.AddSubMenu(new Menu("Misc", "Misc"));
       ZyraConfig.SubMenu("Misc").AddItem(new MenuItem("packets", "Packets").SetValue(true));

        ZyraConfig.AddItem(new MenuItem("madebyme", "PrincessLeia :)").DontSave());
        ZyraConfig.AddToMainMenu();
    }

    }
}
