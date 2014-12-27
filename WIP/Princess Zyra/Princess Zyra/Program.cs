using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_Zyra
{
    internal class Program
    {

        internal static Orbwalking.Orbwalker Orbwalker;

        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Load;
        }

        public static void Load(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Zyra")
            {
                return;
            }

            SkillHandler.Init();
            MenuHandler.Init();
            DrawingHandler.Init();

            Game.OnGameUpdate += OnGameUpdateModes;

            Game.PrintChat("Princess " + ObjectManager.Player.ChampionName);
        }

        public static bool ZyraisZombie()
        {
            return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name ==
                   ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Name ||
                   ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name ==
                   ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name;
        }

        public static void OnGameUpdateModes(EventArgs args)
        {

            Orbwalker.SetAttack(true);
            SkillHandler.E.Range = 700 + 800*SkillHandler.E.Level;

            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (ObjectManager.Player.HasBuff("Recall"))
            {
                return;
            }

            if (ZyraisZombie())
            {
                FightHandler.CastPassive();
                return;
            }
            switch (Orbwalker.ActiveMode)
            {

                case Orbwalking.OrbwalkingMode.Combo:
                    if (MenuHandler.ZyraConfig.Item("useQ_TeamFight").GetValue<bool>())
                        FightHandler.CastQEnemy();
                    if (MenuHandler.ZyraConfig.Item("useE_TeamFight").GetValue<bool>())
                        FightHandler.CastEEnemy();
                    if (MenuHandler.ZyraConfig.Item("useR_TeamFight_willhit").GetValue<Slider>().Value >= 1)
                        FightHandler.CastREnemy();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    if (MenuHandler.ZyraConfig.Item("useQ_Harass").GetValue<bool>())
                        FightHandler.CastQEnemy();
                    if (MenuHandler.ZyraConfig.Item("useE_Harass").GetValue<bool>())
                        FightHandler.CastEEnemy();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    if (MenuHandler.ZyraConfig.Item("useQ_LaneClear").GetValue<bool>())
                        FightHandler.CastQMinion();
                    if (MenuHandler.ZyraConfig.Item("useE_LaneClear").GetValue<bool>())
                        FightHandler.CastEMinion();
                    break;
            }
        }
    }
}
