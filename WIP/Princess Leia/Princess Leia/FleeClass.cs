using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_Leia
{
    internal class FleeClass
    {
        public static Menu FleeMenu;
        public static void Init()
        {
            Game.OnGameUpdate += Flee;
        }

        public static void Menu()
        {
            Menu fleeMenu = new Menu("Flee", "Flee");
            fleeMenu.SubMenu("Flee").AddItem(new MenuItem("FleeK", "Key").SetValue(new KeyBind('A', KeyBindType.Press)));
            fleeMenu.SubMenu("Flee").AddItem(new MenuItem("FleeW", "Use W").SetValue(true));
            fleeMenu.SubMenu("Flee").AddItem(new MenuItem("FleeE", "Use E").SetValue(true));
            fleeMenu.SubMenu("Flee").AddItem(new MenuItem("FleeR", "Use R").SetValue(true));
            fleeMenu.SubMenu("Flee").AddItem(new MenuItem("FleeSecondW", "Use Second W if Cursor over it").SetValue(true));

            FleeMenu = fleeMenu;
        }

        public static void Flee(EventArgs args)
        {
            if (!FleeMenu.Item("FleeK").GetValue<KeyBind>().Active) { return; }

            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var target = TargetSelector.GetTarget(SpellClass.E.Range, TargetSelector.DamageType.Magical);
            var useW = FleeMenu.Item("FleeW").GetValue<bool>();
            var useE = FleeMenu.Item("FleeE").GetValue<bool>();
            var useR = FleeMenu.Item("FleeW").GetValue<bool>();

            if (SpellClass.W.IsReady() && useW && SpellClass.W.Instance.Name == "LeblancSlide")
            {
                SpellClass.W.Cast(Game.CursorPos, MiscClass.PacketCast);
            }
            if (SpellClass.R.IsReady() && useR && SpellClass.R.Instance.Name == "LeblancSlideM")
            {
                SpellClass.R.Cast(Game.CursorPos, MiscClass.PacketCast);
            }
            SpellClass.CastE(target, HitChance.Medium, useE);

            var wposex = MiscClass.SecondW.Pos.Extend(Game.CursorPos, 100);
            var fleepos = wposex.Distance(MiscClass.SecondW.Pos) > Game.CursorPos.Distance(MiscClass.SecondW.Pos);
            var useSecondW = FleeMenu.Item("FleeSecondW").GetValue<bool>();
            if (fleepos)
            {
                SpellClass.CastSecondW(ObjectManager.Player, useSecondW);
            }

        }
    }
}
