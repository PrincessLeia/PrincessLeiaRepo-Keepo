using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Prince_Urgot
{
    internal class Program
    {

        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        internal static Orbwalking.Orbwalker Orbwalker;
        public static void Main(string[] args)
        {
            Game.PrintChat("---------------------------");
            Game.PrintChat("[<font color='#FF0000'>v4</font>]<font color='#7A6EFF'>Twilight's Auto Carry:</font> <font color='#86E5E1'>Kalista</font>");
            CustomEvents.Game.OnGameLoad += Load;
        }

        public static void Load(EventArgs args)
        {
            if (Player.ChampionName != "Urgot")
                return;

            SkillHandler.Init();
            ItemHandler.Init();
            MenuHandler.Init();
            DrawingHandler.Init();

            Interrupter.OnPossibleToInterrupt += FightHandler.Interrupter_OnPossibleToInterrupt;
            CustomEvents.Unit.OnLevelUp += FightHandler.OnLevelUp;
            Game.OnGameUpdate += OnGameUpdateModes;

            Game.PrintChat("Prince " + Player.ChampionName + " Loaded");

        }
        public static void OnGameUpdateModes(EventArgs args)
        {
            if (Player.IsDead)
                return;

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                FightHandler.CastLogic();
                FightHandler.ActivateMura();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                FightHandler.LaneClear();
                FightHandler.DeActivateMura();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                FightHandler.LastHit();
                FightHandler.DeActivateMura();
                    break;
                default:
                    break;
            }

            if (MenuHandler._uMenu.Item("HarassActive").GetValue<KeyBind>().Active || MenuHandler._uMenu.Item("HarassToggle").GetValue<KeyBind>().Active)
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
