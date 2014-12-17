using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_Zyra
{
   internal class SkillHandler
    {
        public static Spell Q, W, E, R, Passive;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 800);
            Q.SetSkillshot(0.8f, 60f, float.MaxValue, false, SkillshotType.SkillshotCircle); // small width for better hits

            W = new Spell(SpellSlot.W, 825);

            E = new Spell(SpellSlot.E, 1100);
            E.SetSkillshot(0.5f, 70f, 1400f, false, SkillshotType.SkillshotLine);

            R = new Spell(SpellSlot.R, 700);
            R.SetSkillshot(0.5f, 500f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Passive = new Spell(SpellSlot.Q, 1470);
            Passive.SetSkillshot(0.5f, 70f, 1400f, false, SkillshotType.SkillshotLine);
        }
    }
}