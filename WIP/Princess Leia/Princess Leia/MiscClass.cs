﻿using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color=System.Drawing.Color;

namespace Princess_Leia
{
    internal class MiscClass
    {
        public static class SecondW
        {
            public static GameObject Object { get; set; }
            public  static Vector3 Pos { get; set; }
            public static double ExpireTime { get; set; }
        }

        public static Menu MiscMenu;
        public static bool PacketCast;
        public static void Init()
        {
            Drawing.OnDraw += OnDraw;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPossibleToInterrupt;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Game.OnGameUpdate += WLogic;
            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += OnDeleteObject;
        }

        public static void Menu()
        {
            Menu miscMenu = new Menu("Misc", "Misc");
            miscMenu.AddSubMenu(new Menu("Second W Logic", "backW"));
            miscMenu.SubMenu("backW").AddItem(new MenuItem("SWcountEnemy", "Minimum of Enemys around Second W Point").SetValue(new Slider(0, 0, 5)));
            miscMenu.SubMenu("backW").AddItem(new MenuItem("SWpos", "If Cursor is hover Second W").SetValue(true));
            miscMenu.AddItem(new MenuItem("Interrupt", "Interrupt with E").SetValue(true));
            miscMenu.AddItem(new MenuItem("Gapclose", "Anit Gapclose with E").SetValue(true));
            miscMenu.AddItem(new MenuItem("UsePacket", "Use Packets").SetValue(false));
            miscMenu.AddItem(new MenuItem("Clone", "Clone Logic").SetValue(false));

            MiscMenu = miscMenu;
        }

        private static void WLogic(EventArgs args)
        {
            PacketCast = MiscMenu.Item("UsePacket").GetValue<bool>();

            var countW = SecondW.Pos.CountEnemysInRange(200) > MiscMenu.SubMenu("Misc").SubMenu("backW").Item("SWcountEnemy").GetValue<Slider>().Value;
            var wposex = SecondW.Pos.Extend(Game.CursorPos, 100);
            var fleepos = wposex.Distance(SecondW.Pos) > Game.CursorPos.Distance(SecondW.Pos) &&  MiscMenu.SubMenu("Misc").SubMenu("backW").Item("SWpos").GetValue<bool>();

            if (countW) { return; }
            if (fleepos)
            {
                SpellClass.CastSecondW(ObjectManager.Player, true);
            }
        }

        private static void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!MiscMenu.Item("Interrupt").GetValue<bool>() || spell.DangerLevel != InterruptableDangerLevel.High)
                return;

            SpellClass.CastE(unit, HitChance.Medium ,true);
            SpellClass.CastRe(unit, HitChance.Medium, true);
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!MiscMenu.Item("Interrupt").GetValue<bool>())
                return;

            SpellClass.CastE(gapcloser.Sender, HitChance.Medium, true);
            SpellClass.CastRe(gapcloser.Sender, HitChance.Medium, true);
        }

        private static void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
                return;

            var wts = Drawing.WorldToScreen(ObjectManager.Player.Position);
            var timer = (SecondW.ExpireTime - Game.Time > 0) ? (SecondW.ExpireTime - Game.Time) : 0;
            Drawing.DrawText(wts[0] - 35, wts[1] + 10, Color.White, "Second W: " + timer.ToString("0.0"));

            Render.Circle.DrawCircle(SecondW.Pos, 100, Color.Red);
        }

        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {
            if (!(sender.Name.Contains("LeBlanc_Base_W_return_indicator.troy")))
            {
                return;
            }
        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (!sender.Name.Contains("LeBlanc_Base_W_return_indicator.troy") && !sender.IsMe)
            {
                return;
            }
            SecondW.Object = sender;
            SecondW.Pos = sender.Position;
            SecondW.ExpireTime = Game.Time + 4;
        }
    }
}
