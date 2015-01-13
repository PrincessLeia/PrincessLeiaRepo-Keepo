using System;
using System.Net;
using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_LeBlanc
{
    class ItemManager
    {
        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }
        public static Items.Item Dfg;
        public static Items.Item HealthPod;
        public static Items.Item ManaPod;
        public static Items.Item Flask;
        public static Items.Item CookiePod;
        public static Items.Item Zhonyas;
        public static SpellSlot Igniteslot;
        public static void Init()
        {
            if (Game.MapId.Equals(1))
            {
                Dfg = new Items.Item(3128, 750);
                Zhonyas = new Items.Item(3157);
            }
            else
            {
                Dfg = new Items.Item(3188, 750);
                Zhonyas = new Items.Item(3090);
            }
            Igniteslot = ObjectManager.Player.GetSpellSlot("SummonerDot");
            HealthPod = new Items.Item(2003);
            ManaPod = new Items.Item(2004);
            Flask = new Items.Item(2041);
            CookiePod = new Items.Item(2009);
            Menu();
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            UseItems();
        }

        private static void UseItems()
        {
            if (ObjectManager.Player.HasBuff("Recall") || ObjectManager.Player.InFountain() && ObjectManager.Player.InShop())
                return;

            if (MenuHandler.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var t = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Magical);
                var useIgnite = Igniteslot != SpellSlot.Unknown && MenuHandler.LeBlancConfig.SubMenu("Items").Item("useIgnite").GetValue<bool>() &&
                    Player.Spellbook.CanUseSpell(Igniteslot) == SpellState.Ready && Player.ServerPosition.Distance(t.ServerPosition) < 600;
                var useDfg = Dfg.IsReady() && Dfg.IsInRange(t) && MenuHandler.LeBlancConfig.SubMenu("Items").Item("useDfg").GetValue<bool>();

                if (t.Health <= MathHandler.ComboDamage(t) && useDfg)
                {
                    Dfg.Cast(t);
                }

                if (t.Health <= MathHandler.ComboDamage(t) && useIgnite)
                {
                    Player.Spellbook.CastSpell(Igniteslot, t);
                }
            }

            if (MenuHandler.LeBlancConfig.Item("useZho").GetValue<bool>() && Player.HealthPercentage() <= MenuHandler.LeBlancConfig.Item("zhoHp").GetValue<Slider>().Value && Player.CountEnemysInRange(Player.AttackRange) >= 1)
            {
                Zhonyas.Cast(Player);
            }

            if (MenuHandler.LeBlancConfig.Item("HealthPotion").GetValue<bool>())
            {
                if (Player.HealthPercentage() <= MenuHandler.LeBlancConfig.Item("HealthPercent").GetValue<Slider>().Value)
                {
                    HealthPod.Cast(Player);
                    CookiePod.Cast(Player);
                    Flask.Cast(Player);
                }
            }
            if (MenuHandler.LeBlancConfig.Item("ManaPotion").GetValue<bool>())
            {
                if (ObjectManager.Player.ManaPercentage() <= MenuHandler.LeBlancConfig.Item("ManaPercent").GetValue<Slider>().Value)
                {
                    ManaPod.Cast(Player);
                    CookiePod.Cast(Player);
                    Flask.Cast(Player);
                }
            }
        }

        private static void Menu()
        {
            MenuHandler.LeBlancConfig.AddSubMenu(new Menu("Items", "Items"));
            MenuHandler.LeBlancConfig.SubMenu("Items").AddSubMenu(new Menu("Offensive", "offensive"));
            MenuHandler.LeBlancConfig.SubMenu("Items").SubMenu("offensive").AddItem(new MenuItem("useDfg", "Use DFG").SetValue(true));
            MenuHandler.LeBlancConfig.SubMenu("Items").SubMenu("offensive").AddItem(new MenuItem("useIgnite", "Use Ignite").SetValue(true));

            MenuHandler.LeBlancConfig.SubMenu("Items").AddSubMenu(new Menu("Defensive", "defensive"));
            MenuHandler.LeBlancConfig.SubMenu("Items").SubMenu("defensive").AddSubMenu(new Menu("Zhoniyas", "Zhoniyas"));
            MenuHandler.LeBlancConfig.SubMenu("Items").SubMenu("defensive").SubMenu("Zhoniyas").AddItem(new MenuItem("useZho", "Use Zhoniyas").SetValue(true));
            MenuHandler.LeBlancConfig.SubMenu("Items").SubMenu("defensive").SubMenu("Zhoniyas").AddItem(new MenuItem("zhoHp", "Min Player HP%").SetValue(new Slider(5)));
            MenuHandler.LeBlancConfig.SubMenu("Items").SubMenu("defensive").AddSubMenu(new Menu("Health Potion", "Health"));
            MenuHandler.LeBlancConfig.SubMenu("Items").SubMenu("defensive").SubMenu("Health").AddItem(new MenuItem("HealthPotion", "Use Health Potion").SetValue(true));
            MenuHandler.LeBlancConfig.SubMenu("Items").SubMenu("defensive").SubMenu("Health").AddItem(new MenuItem("HealthPercent", "HP Trigger Percent").SetValue(new Slider(30)));

            MenuHandler.LeBlancConfig.SubMenu("Items").SubMenu("defensive").AddSubMenu(new Menu("Mana Potion", "Mana"));
            MenuHandler.LeBlancConfig.SubMenu("Items").SubMenu("defensive").SubMenu("Mana").AddItem(new MenuItem("ManaPotion", "Use Mana Potion").SetValue(true));
            MenuHandler.LeBlancConfig.SubMenu("Items").SubMenu("defensive").SubMenu("Mana").AddItem(new MenuItem("ManaPercent", "MP Trigger Percent").SetValue(new Slider(30)));
        }
    }
}
