using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_Leia
{
    internal class HarassClass
    {
        public static Menu HarassMenu;
        public static void Init()
        {
            Game.OnGameUpdate += Harass;
        }

        public static void Menu()
        {
            Menu harassMenu = new Menu("harass", "Harass");
            harassMenu.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            harassMenu.AddItem(new MenuItem("use2W", "Use Second W").SetValue(true));
            harassMenu.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            harassMenu.AddItem(new MenuItem("HarassManaPercent", "Minimum Mana Percent").SetValue(new Slider(30)));
            harassMenu.AddItem(new MenuItem("haraKey", "Harass Toggle").SetValue(new KeyBind('J', KeyBindType.Toggle)));

            HarassMenu = harassMenu;
        }

        private static void Harass(EventArgs args)
        {
            if (Program.Orbwalker.ActiveMode.ToString() == "Mixed" ||
                HarassMenu.Item("haraKey").GetValue<KeyBind>().Active)
            {
                var target = TargetSelector.GetTarget(SpellClass.E.Range, TargetSelector.DamageType.Magical);
                var mana = ObjectManager.Player.ManaPercentage() > HarassMenu.Item("HarassManaPercent").GetValue<Slider>().Value;
                var useQ = HarassMenu.Item("useQ").GetValue<bool>();
                var useW = HarassMenu.Item("useW").GetValue<bool>();
                var useE = HarassMenu.Item("useE").GetValue<bool>();

                if (!mana) { return; }

                SpellClass.CastE(target, HitChance.High, useE);
                SpellClass.CastQ(target, useQ);
                SpellClass.CastW(target, useW);
                SpellClass.CastSecondW(ObjectManager.Player, HarassMenu.Item("use2W").GetValue<bool>());
            }
        }
    }
}
