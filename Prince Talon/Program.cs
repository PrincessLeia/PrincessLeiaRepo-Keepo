using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace PrinceTalon
{
    class Program
    {
        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        public static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Talon")
                return;

            SkillHandler.Init();
            ItemHandler.Init();
            MenuHandler.Init();
            DrawingHandler.Init();

            Game.OnGameUpdate += OnGameUpdateModes;

            Game.PrintChat("Prince " + ObjectManager.Player.ChampionName);
        }

        public static void OnGameUpdateModes(EventArgs args)
        {

            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (ObjectManager.Player.HasBuff("Recall"))
            {
                return;
            }
            if (MenuHandler.TalonConfig.Item("Orbwalk").GetValue<KeyBind>().Active)
            {
                FightHandler.Combo();
            }
            else if (MenuHandler.TalonConfig.Item("Mixed").GetValue<KeyBind>().Active)
            {
                FightHandler.Harass();
            }
            else if (MenuHandler.TalonConfig.Item("LaneClear").GetValue<KeyBind>().Active)
            {
                FightHandler.LaneClear();
                FightHandler.JungleClear();
            }
            if (MenuHandler.TalonConfig.SubMenu("Harass").Item("HarassToggle").GetValue<KeyBind>().Active)
            {
                FightHandler.Harass();
            }
        }
    }
}
