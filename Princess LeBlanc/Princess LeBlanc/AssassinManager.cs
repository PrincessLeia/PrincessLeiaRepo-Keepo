using System;
using System.Drawing;
using Color = System.Drawing.Color;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_LeBlanc
{
    internal class AssassinManager
    {
        public static void Init()
        {
            Load();
        }

        public static void Load()
        {
            MenuHandler.TargetSelectorMenu.AddSubMenu(new Menu("Assassin Manager", "MenuAssassin"));
            MenuHandler.TargetSelectorMenu.SubMenu("MenuAssassin").AddItem(new MenuItem("AssassinActive", "Active").SetValue(true));
            MenuHandler.TargetSelectorMenu.SubMenu("MenuAssassin").AddItem(new MenuItem("AssassinSelectOption", "Set: ").SetValue(new StringList(new[] { "Single Select", "Multi Select" })));
            MenuHandler.TargetSelectorMenu.SubMenu("MenuAssassin").AddItem(new MenuItem("AssassinSetClick", "Add/Remove with click").SetValue(true));
            MenuHandler.TargetSelectorMenu.SubMenu("MenuAssassin").AddItem(new MenuItem("AssassinReset", "Reset List").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));

            MenuHandler.TargetSelectorMenu.SubMenu("MenuAssassin").AddSubMenu(new Menu("Draw:", "Draw"));

            MenuHandler.TargetSelectorMenu.SubMenu("MenuAssassin").SubMenu("Draw").AddItem(new MenuItem("DrawSearch", "Search Range").SetValue(new Circle(true, Color.GreenYellow)));
            MenuHandler.TargetSelectorMenu.SubMenu("MenuAssassin").SubMenu("Draw").AddItem(new MenuItem("DrawActive", "Active Enemy").SetValue(new Circle(true, Color.GreenYellow)));
            MenuHandler.TargetSelectorMenu.SubMenu("MenuAssassin").SubMenu("Draw").AddItem(new MenuItem("DrawNearest", "Nearest Enemy").SetValue(new Circle(true, Color.DarkSeaGreen)));


            MenuHandler.TargetSelectorMenu.SubMenu("MenuAssassin").AddSubMenu(new Menu("Assassin List:", "AssassinMode"));
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != ObjectManager.Player.Team))
            {
                MenuHandler.TargetSelectorMenu.SubMenu("MenuAssassin")
                    .SubMenu("AssassinMode")
                    .AddItem(
                        new MenuItem("Assassin" + enemy.ChampionName, enemy.ChampionName).SetValue(
                            TargetSelector.GetPriority(enemy) > 3));
            }
            MenuHandler.TargetSelectorMenu.SubMenu("MenuAssassin")
                .AddItem(new MenuItem("AssassinSearchRange", "Search Range")).SetValue(new Slider(1000, 2000));
        }

        static void ClearAssassinList()
        {
            foreach (
                var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != ObjectManager.Player.Team))
            {
                MenuHandler.TargetSelectorMenu.Item("Assassin" + enemy.ChampionName).SetValue(false);
            }
        }

        public static void Game_OnWndProc(WndEventArgs args)
        {

            if (MenuHandler.TargetSelectorMenu.Item("AssassinReset").GetValue<KeyBind>().Active && args.Msg == 257)
            {
                ClearAssassinList();
                Game.PrintChat(
                    "<font color='#FFFFFF'>Reset Assassin List is Complete! Click on the enemy for Add/Remove.</font>");
            }

            if (args.Msg != 0x201)
            {
                return;
            }

            if (MenuHandler.TargetSelectorMenu.Item("AssassinSetClick").GetValue<bool>())
            {
                foreach (var objAiHero in from hero in ObjectManager.Get<Obj_AI_Hero>()
                                          where hero.IsValidTarget()
                                          select hero
                                              into h
                                              orderby h.Distance(Game.CursorPos) descending
                                              select h
                                                  into enemy
                                                  where enemy.Distance(Game.CursorPos) < 150f
                                                  select enemy)
                {
                    if (objAiHero != null && objAiHero.IsVisible && !objAiHero.IsDead)
                    {
                        var xSelect =
                            MenuHandler.TargetSelectorMenu.Item("AssassinSelectOption").GetValue<StringList>().SelectedIndex;

                        switch (xSelect)
                        {
                            case 0:
                                ClearAssassinList();
                                MenuHandler.TargetSelectorMenu.Item("Assassin" + objAiHero.ChampionName).SetValue(true);
                                Game.PrintChat(
                                    string.Format(
                                        "<font color='FFFFFF'>Added to Assassin List</font> <font color='#09F000'>{0} ({1})</font>",
                                        objAiHero.Name, objAiHero.ChampionName));
                                break;
                            case 1:
                                var menuStatus =
                                    MenuHandler.TargetSelectorMenu.Item("Assassin" + objAiHero.ChampionName)
                                        .GetValue<bool>();
                                MenuHandler.TargetSelectorMenu.Item("Assassin" + objAiHero.ChampionName)
                                    .SetValue(!menuStatus);
                                Game.PrintChat(
                                    string.Format("<font color='{0}'>{1}</font> <font color='#09F000'>{2} ({3})</font>",
                                        !menuStatus ? "#FFFFFF" : "#FF8877",
                                        !menuStatus ? "Added to Assassin List:" : "Removed from Assassin List:",
                                        objAiHero.Name, objAiHero.ChampionName));
                                break;
                        }
                    }
                }
            }
        }
        public static void Drawing_OnDraw(EventArgs args)
        {
            if (!MenuHandler.TargetSelectorMenu.Item("AssassinActive").GetValue<bool>())
            {
                return;
            }

            var drawSearch = MenuHandler.TargetSelectorMenu.Item("DrawSearch").GetValue<Circle>();
            var drawActive = MenuHandler.TargetSelectorMenu.Item("DrawActive").GetValue<Circle>();
            var drawNearest = MenuHandler.TargetSelectorMenu.Item("DrawNearest").GetValue<Circle>();

            var drawSearchRange = MenuHandler.TargetSelectorMenu.Item("AssassinSearchRange").GetValue<Slider>().Value;
            if (drawSearch.Active)
            {
                Utility.DrawCircle(ObjectManager.Player.Position, drawSearchRange, drawSearch.Color);
            }

            foreach (
                var enemy in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(enemy => enemy.Team != ObjectManager.Player.Team)
                        .Where(
                            enemy =>
                                enemy.IsVisible &&
                                MenuHandler.TargetSelectorMenu.Item("Assassin" + enemy.ChampionName) != null &&
                                !enemy.IsDead)
                        .Where(
                            enemy => MenuHandler.TargetSelectorMenu.Item("Assassin" + enemy.ChampionName).GetValue<bool>()))
            {
                if (ObjectManager.Player.Distance(enemy) < drawSearchRange)
                {
                    if (drawActive.Active)
                        Utility.DrawCircle(enemy.Position, 85f, drawActive.Color);
                }
                else if (ObjectManager.Player.Distance(enemy) > drawSearchRange &&
                         ObjectManager.Player.Distance(enemy) < drawSearchRange + 400)
                {
                    if (drawNearest.Active)
                        Utility.DrawCircle(enemy.Position, 85f, drawNearest.Color);
                }
            }
        }
    }
}
