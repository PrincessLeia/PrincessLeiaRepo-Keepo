using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_Leia
{
    internal class LaneClearClass
    {
        public static Menu LaneClearMenu;
        public static void Init()
        {
            Game.OnGameUpdate += LaneClear;
        }

        public static void Menu()
        {
            Menu laneclearMenu = new Menu("laneclear", "LaneClear");
            laneclearMenu.AddItem(new MenuItem("laneClearActive", "Laning Mode").SetValue(new KeyBind(Program.LeBlancConfig.Item("LaneClear").GetValue<KeyBind>().Key, KeyBindType.Press)));
            laneclearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q").SetValue(true));
            laneclearMenu.AddItem(new MenuItem("LaneClearW", "Use W").SetValue(true));
            laneclearMenu.AddItem(new MenuItem("LaneClear2W", "Use Second W").SetValue(true));
            laneclearMenu.AddItem(new MenuItem("LaneClearWHit", "Min minions by W").SetValue(new Slider(2, 0, 5)));
            laneclearMenu.AddItem(new MenuItem("LaneClearManaPercent", "Minimum Mana Percent").SetValue(new Slider(30)));

            LaneClearMenu = laneclearMenu;
        }

        private static void LaneClear(EventArgs args)
        {
            if (Program.Orbwalker.ActiveMode.ToString() != "LaneClear" || ObjectManager.Player.IsDead || ObjectManager.Player.HasBuff("Recall"))
                return;

            var useQ = LaneClearMenu.Item("LaneClearQ").GetValue<bool>();
            var useW = LaneClearMenu.Item("LaneClearW").GetValue<bool>();
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, SpellClass.Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            var minionsJung = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, SpellClass.Q.Range, MinionTypes.All, MinionTeam.Neutral);
            var farmLocation = MinionManager.GetBestCircularFarmLocation(MinionManager.GetMinions(SpellClass.W.Range).Select(m => m.ServerPosition.To2D()).ToList(), SpellClass.W.Width, SpellClass.W.Range);
            var mana = ObjectManager.Player.ManaPercentage() > LaneClearMenu.Item("LaneClearManaPercent").GetValue<Slider>().Value;
            var minionHit = farmLocation.MinionsHit >= LaneClearMenu.Item("LaneClearWHit").GetValue<Slider>().Value;
            if (!mana)
            {
                return;
            }
            foreach (var minion in minions)
            {
                if (minion != null && minion.IsValidTarget() && minion.Health <= SpellClass.Q.GetDamage(minion))
                {
                    SpellClass.CastQ(minion, useQ);
                }
            }
            foreach (var minion in minionsJung)
            {
                if (minion != null && minion.IsValidTarget())
                {
                    SpellClass.CastQ(minion, useQ);
                }
            }

            if (minionHit && useW && SpellClass.W.IsReady() && SpellClass.W.Instance.Name == "LeblancSlide")
            {
                SpellClass.W.Cast(farmLocation.Position, MiscClass.PacketCast);
            }
            if (farmLocation.MinionsHit > 0 && useW && SpellClass.W.IsReady() && SpellClass.W.Instance.Name == "LeblancSlide")
            {
                SpellClass.W.Cast(farmLocation.Position, MiscClass.PacketCast);
            }

                SpellClass.CastSecondW(ObjectManager.Player, LaneClearMenu.Item("LaneClear2W").GetValue<bool>());
        }
    }
}
