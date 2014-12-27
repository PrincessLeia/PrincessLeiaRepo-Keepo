using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Prince_Warwick.Annotations;

namespace Prince_Warwick
{
    internal class Program
    {

        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Load;
        }
        public static void Load(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Warwick")
            {
                return;
            }

            SkillHandler.Init();
            ItemHandler.Init();
            MenuHandler.Init();
            DrawingHandler.Init();

            Game.OnGameUpdate += OnGameUpdateModes;

            Game.PrintChat("Prince " + ObjectManager.Player.ChampionName);

            AntiGapcloser.OnEnemyGapcloser += FightHandler.AntiGapCloser;
            Interrupter.OnPossibleToInterrupt += FightHandler.Interrupter_OnPossibleToInterrupt;
        }
        public static void OnGameUpdateModes(EventArgs args)
        {

            SkillHandler.E.Range = 700 + 800 * SkillHandler.E.Level;

            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (ObjectManager.Player.HasBuff("Recall"))
            {
                return;
            }

            if (MenuHandler.WarwickConfig.Item("Orbwalk").GetValue<KeyBind>().Active)
            {
                FightHandler.Combo();
            }
            else if (MenuHandler.WarwickConfig.Item("Farm").GetValue<KeyBind>().Active)
            {
            }
            else if (MenuHandler.WarwickConfig.Item("LaneClear").GetValue<KeyBind>().Active)
            {
                FightHandler.LaneClear();
                FightHandler.JungleClear();
            }
            else
            {
                if (MenuHandler.WarwickConfig.Item("autoULT").GetValue<KeyBind>().Active)
                {
                    FightHandler.UltonClick();
                }
            }

            if (MenuHandler.WarwickConfig.Item("KSi").GetValue<bool>() ||
                MenuHandler.WarwickConfig.Item("KSq").GetValue<bool>())
            {
                FightHandler.KillSteal();
            }
        }
    }
}
