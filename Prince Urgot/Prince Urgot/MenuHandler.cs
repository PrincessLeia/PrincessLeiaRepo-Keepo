using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using xSLx_Orbwalker;
using Color = System.Drawing.Color;

namespace Prince_Urgot
{
    internal class MenuHandler
    {
        public static Menu _uMenu;
        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        public static readonly StringList HitChanceList = new StringList(new[] { "Low", "Medium", "High", "Very High" });
        public static xSLxOrbwalker Orbwalker;
        public static void Init()
        {
            _uMenu = new Menu("Prince " + Player.ChampionName, Player.ChampionName, true);

            Menu orbwalkerMenu = _uMenu.AddSubMenu(new Menu("xSLx Orbwalker", "Orbwalkert1"));

            Orbwalker = new xSLxOrbwalker();
            xSLxOrbwalker.AddToMenu(orbwalkerMenu);

            Menu ts = _uMenu.AddSubMenu(new Menu("Target Selector", "Target Selector")); ;

            SimpleTs.AddToMenu(ts);

            Menu comboMenu = _uMenu.AddSubMenu(new Menu("Combo", "Combo"));
            comboMenu.AddItem(new MenuItem("ComboQ", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("ComboW", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("ComboE", "Use E").SetValue(true));

            Menu laneClearMenu = _uMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q").SetValue(true));
            laneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E").SetValue(false));
            laneClearMenu.AddItem(new MenuItem("LaneClearQManaPercent", "Minimum Q Mana Percent").SetValue(new Slider(30, 0, 100)));

            Menu lastHitMenu = _uMenu.AddSubMenu(new Menu("LastHit", "LastHit"));
            lastHitMenu.AddItem(new MenuItem("lastHitQ", "Use Q").SetValue(true));
            lastHitMenu.AddItem(new MenuItem("lastHitQManaPercent", "Minimum Q Mana Percent").SetValue(new Slider(30, 0, 100)));

            Menu harassMenu = _uMenu.AddSubMenu(new Menu("Harass", "Harass"));
            harassMenu.AddItem(new MenuItem("HarassActive", "Harass").SetValue(new KeyBind('C', KeyBindType.Press)));
            harassMenu.AddItem(new MenuItem("HarassToggle", "Harass").SetValue(new KeyBind('T', KeyBindType.Toggle)));
            harassMenu.AddItem(new MenuItem("haraQ", "Auto Q on E Debuff").SetValue(true));
            harassMenu.AddItem(new MenuItem("haraE", "Auto E for Debuff").SetValue(true));
            harassMenu.AddItem(new MenuItem("HaraManaPercent", "Minimum Mana Percent").SetValue(new Slider(30, 0, 100)));

            Menu itemsMenu = _uMenu.AddSubMenu(new Menu("Items", "Items"));
            itemsMenu.AddItem(new MenuItem("useMura", "enable Auto Muramana").SetValue(new KeyBind('A', KeyBindType.Toggle)));

            Menu preMenu = _uMenu.AddSubMenu(new Menu("Prediction", "Prediction"));
            preMenu.AddItem(new MenuItem("preE", "HitChance E").SetValue(HitChanceList));
            preMenu.AddItem(new MenuItem("preQ", "HitChance Q").SetValue(HitChanceList));

            Menu drawMenu = _uMenu.AddSubMenu(new Menu("Draw", "Drawing"));
            drawMenu.AddItem(new MenuItem("drawQ", "Draw Q").SetValue(new Circle(true, Color.FromArgb(100, Color.Aqua))));
            drawMenu.AddItem(new MenuItem("drawEe", "Draw E").SetValue(new Circle(true, Color.FromArgb(100, Color.Aqua))));
            drawMenu.AddItem(new MenuItem("drawE", "Draw extended Q range if hit by E").SetValue(new Circle(true, Color.FromArgb(100, Color.Aqua))));
            drawMenu.AddItem(new MenuItem("HUD", "Heads-up Display").SetValue(true));
            drawMenu.AddItem(new MenuItem("hitbye", "Draw Circle on Enemy if hit by E").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawR", "Draw R").SetValue(new Circle(true, Color.FromArgb(100, Color.Red))));

            Menu miscMenu = _uMenu.AddSubMenu(new Menu("Misc", "Misc"));
            miscMenu.AddItem(new MenuItem("autoR", "Auto R if under Tower BETA").SetValue(false));
            miscMenu.AddItem(new MenuItem("autoInt", "Auto Interrupt with R").SetValue(false));
            miscMenu.AddItem(new MenuItem("KillQ", "KillSteal with Q?").SetValue(true));
            miscMenu.AddItem(new MenuItem("KillI", "KillSteal with Ignite?").SetValue(true));

            _uMenu.AddItem(new MenuItem("Packet", "Packet Casting").SetValue(true));

            _uMenu.AddItem(new MenuItem("madebyme", "PrincessLeia :)").DontSave());

            _uMenu.AddToMainMenu();
    }

    }
}
