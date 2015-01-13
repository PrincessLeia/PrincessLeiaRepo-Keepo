using System;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX.Direct3D9;

namespace Princess_LeBlanc
{
    internal class Program
    {
        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }

        public static bool PacketCast;

        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Leblanc")
            {
                return;
            }
            SkillHandler.Init();
            MenuHandler.Init();
            ItemManager.Init();
            DrawingHandler.Init();
            Game.OnGameUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += FightHandler.GameObject_OnCreate;
            GameObject.OnDelete += FightHandler.GameObject_OnDelete;
            Interrupter.OnPossibleToInterrupt += FightHandler.Interrupter_OnPossibleToInterrupt;
            AntiGapcloser.OnEnemyGapcloser += FightHandler.AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += FightHandler.Obj_AI_Base_OnProcessSpellCast;

            Game.PrintChat("<b><font color =\"#FFFFFF\">Princess LeBlanc</font></b><font color =\"#FFFFFF\"> by </font><b><font color=\"#FF66FF\">Leia</font></b><font color =\"#FFFFFF\"> loaded!</font>");
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (ObjectManager.Player.HasBuff("Recall"))
            {
                return;
            }

            PacketCast = MenuHandler.LeBlancConfig.SubMenu("Misc").Item("UsePacket").GetValue<bool>();

            switch (MenuHandler.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                {
                    FightHandler.ComboLogic();
                    break;
                }
                case Orbwalking.OrbwalkingMode.LaneClear:
                {
                        switch (MenuHandler.LeBlancConfig.SubMenu("LaneMode").Item("LaneModeSwitch").GetValue<StringList>().SelectedIndex)
                        {
                            case 0:
                            {
                                FightHandler.LaneClear();
                                FightHandler.JungleClear();
                                break;
                            }
                            case 1:
                            {
                                FightHandler.Harass();
                                break;
                            }
                        }
                    break;
                }
                case Orbwalking.OrbwalkingMode.LastHit:
                {
                    break;
                }
                case Orbwalking.OrbwalkingMode.Mixed:
                {
                    FightHandler.Harass();
                    break;
                }
            }
            FightHandler.WLogic();
            FightHandler.Flee();
            FightHandler.CloneLogic();
            FightHandler.Timer();

            if (MenuHandler.LeBlancConfig.SubMenu("LaneMode").Item("LaneModeToggle").GetValue<KeyBind>().Active)
            {
                if (MenuHandler.LeBlancConfig.SubMenu("LaneMode")
                        .Item("LaneModeSwitch")
                        .GetValue<StringList>()
                        .SelectedIndex == 0)
                {
                        MenuHandler.LeBlancConfig.SubMenu("LaneMode").Item("LaneModeSwitch").SetValue(new StringList(new[] { "LaneClear", "Harass"}, 1));
                }
            }
            if (!MenuHandler.LeBlancConfig.SubMenu("LaneMode").Item("LaneModeToggle").GetValue<KeyBind>().Active)
            {
                if (MenuHandler.LeBlancConfig.SubMenu("LaneMode")
                        .Item("LaneModeSwitch")
                        .GetValue<StringList>()
                        .SelectedIndex == 1)
                {
                            MenuHandler.LeBlancConfig.SubMenu("LaneMode").Item("LaneModeSwitch").SetValue(new StringList(new[] { "LaneClear", "Harass" }, 0));
                }
            }
        }
    }
}
