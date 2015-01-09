using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_LeBlanc
{
    internal class SkillHandler
    {
        public static Spell Q, W, E, R;
        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 700);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 970);
            R = new Spell(SpellSlot.R);

            // Method Spell.SetSkillshot(float delay, float width, float speed, bool collision, SkillshotType 
            W.SetSkillshot(0.5f, 220, 1500, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.366f, 70, 1600, true, SkillshotType.SkillshotLine);
            switch (FightHandler.StatusR())
            {
                case "Q":
                {
                    R = new Spell(SpellSlot.R, Q.Range);
                    break;
                }
                case "W":
                {
                    R = new Spell(SpellSlot.R, W.Range);
                    R.SetSkillshot(0.5f, 220, 1500, false, SkillshotType.SkillshotCircle);
                    break;
                }
                case "E":
                {
                    R = new Spell(SpellSlot.R, E.Range);
                    R.SetSkillshot(0.366f, 70, 1600, true, SkillshotType.SkillshotLine);
                    break;
                }
            }
        }
    }
}
