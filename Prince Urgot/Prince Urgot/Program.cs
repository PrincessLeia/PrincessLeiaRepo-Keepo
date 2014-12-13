#region

using System;
using LeagueSharp;
using LeagueSharp.Common;
using xSLx_Orbwalker;

#endregion

namespace Prince_Urgot
{
    internal class Program
    {
        internal static xSLxOrbwalker Orbwalker;

        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Load;
        }

        public static void Load(EventArgs args)
        {
            if (Player.ChampionName != "Urgot")
            {
                return;
            }

            SkillHandler.Init();
            ItemHandler.Init();
            MenuHandler.Init();
            DrawingHandler.Init();

            Interrupter.OnPossibleToInterrupt += FightHandler.Interrupter_OnPossibleToInterrupt;
            Game.OnGameUpdate += OnGameUpdateModes;

            Game.PrintChat("Prince " + Player.ChampionName + " Loaded");
        }

        public static void OnGameUpdateModes(EventArgs args)
        {
            SkillHandler.R.Range = 400 + 150 * SkillHandler.R.Level;

            if (Player.IsDead)
            {
                return;
            }

            if (xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Combo)
            {
                FightHandler.CastLogic();
                FightHandler.ActivateMura();
            }

            if (xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Harass) { }

            if (xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.LaneClear)
            {
                FightHandler.LaneClear();
                FightHandler.DeActivateMura();
            }
            if (xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Lasthit)
            {
                FightHandler.LastHit();
                FightHandler.DeActivateMura();
            }
            if (MenuHandler._uMenu.Item("HarassActive").GetValue<KeyBind>().Active ||
                MenuHandler._uMenu.Item("HarassToggle").GetValue<KeyBind>().Active)
            {
                FightHandler.Harass();
            }
            if (MenuHandler._uMenu.Item("autoR").GetValue<bool>() && SkillHandler.R.IsReady())
            {
                FightHandler.AutoR();
            }


            FightHandler.KillSteal();
        }
    }
}