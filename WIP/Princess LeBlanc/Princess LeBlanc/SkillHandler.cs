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
            W = new Spell(SpellSlot.W, 720);
            E = new Spell(SpellSlot.E, 1000);
            R = new Spell(SpellSlot.R, 0);

            // Method Spell.SetSkillshot(float delay, float width, float speed, bool collision, SkillshotType 
            W.SetSkillshot(0.25f, 100f, 2000f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 95f, 1600f, true, SkillshotType.SkillshotLine);
        }
    }
}