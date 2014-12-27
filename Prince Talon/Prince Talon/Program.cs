using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Prince_Talon
{
    internal class Program
    {


        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Load;
        }
        public static void Load(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Talon")
            {
                return;
            }

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
            else if (MenuHandler.TalonConfig.Item("Farm").GetValue<KeyBind>().Active)
            {
            }
            else if (MenuHandler.TalonConfig.Item("LaneClear").GetValue<KeyBind>().Active)
            {
                FightHandler.LaneClear();
                FightHandler.JungleClear();
            }

            if (MenuHandler.TalonConfig.Item("KSi").GetValue<bool>() ||
                MenuHandler.TalonConfig.Item("KSq").GetValue<bool>() ||
                MenuHandler.TalonConfig.Item("KSw").GetValue<bool>()
                && !MenuHandler.TalonConfig.Item("Orbwalk").GetValue<KeyBind>().Active)
            {
                FightHandler.KillSteal();
            }

            if (MenuHandler.TalonConfig.Item("HarassW").GetValue<bool>() && !MenuHandler.TalonConfig.Item("Orbwalk").GetValue<KeyBind>().Active)
            {
                FightHandler.Harass();
            }
        }
    }
}
