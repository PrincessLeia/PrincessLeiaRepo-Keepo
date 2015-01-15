using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_Leia
{
    internal class SpellClass
    {
        public static Spell Q, W, E, R;
        static readonly bool PacketCast = MiscClass.PacketCast;
        public static void Init()
        {
            Game.OnGameUpdate += Spells;
        }

        private static void Spells(EventArgs args)
        {
            Q = new Spell(SpellSlot.Q, 700);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 970);
            R = new Spell(SpellSlot.R);

            W.SetSkillshot(0.5f, 220, 1500, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.366f, 70, 1600, true, SkillshotType.SkillshotLine);

            var name = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name;

            switch (name)
            {
                case "LeblancChaosOrbM":
                {
                    R = new Spell(SpellSlot.R, Q.Range);
                    break;
                }
                case "LeblancSlideM":
                {
                    R = new Spell(SpellSlot.R, W.Range);
                    R.SetSkillshot(0.5f, 220, 1500, false, SkillshotType.SkillshotCircle);
                    break;
                }
                case "LeblancSoulShackleM":
                {
                    R = new Spell(SpellSlot.R, E.Range);
                    R.SetSkillshot(0.366f, 70, 1600, true, SkillshotType.SkillshotLine);
                    break;
                }
            }
        }

        public static void CastQ(Obj_AI_Base t, bool useQ)
        {
            if (t.IsDead || t.IsInvulnerable || !t.IsTargetable || !Q.IsReady() || !Q.IsInRange(t) || !useQ)
                return;

            Q.CastOnUnit(t, PacketCast);
        }
        public static void CastW(Obj_AI_Base t, bool useW)
        {
            if (t.IsDead || t.IsInvulnerable || !t.IsTargetable || !W.IsReady() || !W.IsInRange(t) || !useW || W.Instance.Name != "LeblancSlide")
                return;

            W.Cast(t.ServerPosition, PacketCast);
        }
        public static void CastSecondW(Obj_AI_Base t, bool useSecondW)
        {
            if (!W.IsReady() || !useSecondW || W.Instance.Name == "LeblancSlide")
                return;

            W.Cast(t, PacketCast);
        }
        public static void CastE(Obj_AI_Base t, HitChance hitE, bool useE)
        {
            if (t.IsDead || t.IsInvulnerable || !t.IsTargetable || !E.IsReady() || !E.IsInRange(t) || !useE)
                return;

            E.CastIfHitchanceEquals(t, hitE, PacketCast);
        }
        public static void CastRq(Obj_AI_Base t, bool useR)
        {
            if (t.IsDead || t.IsInvulnerable || !t.IsTargetable || !R.IsReady() || !R.IsInRange(t) || !useR || ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name != "LeblancChaosOrbM")
                return;

            R.CastOnUnit(t, PacketCast);
        }
        public static void CastRw(Obj_AI_Base t, bool useR)
        {
            if (t.IsDead || t.IsInvulnerable || !t.IsTargetable || !R.IsReady() || !R.IsInRange(t) || !useR || ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name != "LeblancSlideM")
                return;

            R.Cast(t.ServerPosition, PacketCast);
        }
        public static void CastRe(Obj_AI_Base t,HitChance hitR, bool useR)
        {
            if (t.IsDead || t.IsInvulnerable || !t.IsTargetable || !R.IsReady() || !R.IsInRange(t) || !useR || ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name != "LeblancSoulShackleM")
                return;

            R.CastIfHitchanceEquals(t, hitR, PacketCast);
        }
    }
}
