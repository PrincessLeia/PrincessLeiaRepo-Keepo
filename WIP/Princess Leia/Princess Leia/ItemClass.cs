using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_Leia
{
    internal class ItemClass
    {
        public static Menu ItemMenu;
        public static Items.Item Dfg = new Items.Item(ItemData.Deathfire_Grasp.Id, ItemData.Deathfire_Grasp.Range);
        public static Items.Item Zho = new Items.Item(ItemData.Zhonyas_Hourglass.Id);
        public static Items.Item Hp = new Items.Item(ItemData.Health_Potion.Id);
        public static Items.Item Mp = new Items.Item(ItemData.Mana_Potion.Id);
        public static Items.Item Flask = new Items.Item(ItemData.Crystalline_Flask.Id);

        public static void Init()
        {
            Game.OnGameUpdate += OnGameUpdate;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            UseItems();
        }
        public static void Menu()
        {
            Menu itemMenu = new Menu("Items", "items");
            itemMenu.AddItem(new MenuItem("useDfg", "Use DFG").SetValue(true));
            itemMenu.AddItem(new MenuItem("useZho", "Use Zhonyas").SetValue(true));
            itemMenu.AddItem(new MenuItem("minZho", "Zhonyas Min Health%").SetValue(new Slider(5)));
            itemMenu.AddSubMenu(new Menu("hp", "Health Potions"));
            itemMenu.SubMenu("hp").AddItem(new MenuItem("useHp", "Use Health Potion").SetValue(true));
            itemMenu.SubMenu("hp").AddItem(new MenuItem("useFlask", "Use Flask").SetValue(true));
            itemMenu.SubMenu("hp").AddItem(new MenuItem("minHp", "Minimum Health%").SetValue(new Slider(30)));
            itemMenu.AddSubMenu(new Menu("mp", "Mana Potions"));
            itemMenu.SubMenu("mp").AddItem(new MenuItem("useMp", "Use Mana Potion").SetValue(true));
            itemMenu.SubMenu("mp").AddItem(new MenuItem("useFlask", "Use Flask").SetValue(true));
            itemMenu.SubMenu("mp").AddItem(new MenuItem("minMp", "Minimum Mana%").SetValue(new Slider(30)));

            ItemMenu = itemMenu;
        }
        public static void CastDfg(Obj_AI_Base t, bool useDfg)
        {
            if (t.IsDead || t.IsInvulnerable || !t.IsTargetable || !Dfg.IsInRange(t) || !Dfg.IsReady() || !useDfg)
                return;

            Dfg.Cast(t);
        }
        public static void CastZho(bool useZho)
        {
            if (!Zho.IsReady() || !useZho)
                return;

            Zho.Cast();
        }
        public static void CastHp(bool useHp)
        {
            if (!Hp.IsReady() || !useHp)
                return;

            Hp.Cast();
        }
        public static void CastMp(bool useMp)
        {
            if (!Mp.IsReady() || !useMp)
                return;

            Mp.Cast();
        }
        public static void CastFlask(bool useFlask)
        {
            if (!Flask.IsReady() || !useFlask)
                return;

            Flask.Cast();
        }
        private static void UseItems()
        {
            var useHp = ItemMenu.SubMenu("hp").Item("useHp").GetValue<bool>();
            var useHFlask = ItemMenu.SubMenu("hp").Item("useFlask").GetValue<bool>();
            var minHp = ObjectManager.Player.HealthPercentage() < ItemMenu.SubMenu("hp").Item("minHp").GetValue<Slider>().Value;
            var useMp = ItemMenu.SubMenu("mp").Item("useMp").GetValue<bool>();
            var useMFlask = ItemMenu.SubMenu("mp").Item("useFlask").GetValue<bool>();
            var minMp = ObjectManager.Player.ManaPercentage() < ItemMenu.SubMenu("mp").Item("minmp").GetValue<Slider>().Value;
            var useFlask = useHFlask || useMFlask;
            var useZho = ItemMenu.Item("useZho").GetValue<bool>();
            var minZho = ItemMenu.Item("minZho").GetValue<Slider>().Value > ObjectManager.Player.HealthPercentage();

            if (ObjectManager.Player.HasBuff("Recall") || ObjectManager.Player.InFountain() && ObjectManager.Player.InShop())
                return;
            if (minHp)
            {
                CastHp(useHp);
                CastFlask(useFlask);
            }
            if (minMp)
            {
                CastMp(useMp);
                CastFlask(useFlask);
            }
            if (minZho)
            {
                CastZho(useZho);
            }
        }
    }
}