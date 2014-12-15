using LeagueSharp;
using LeagueSharp.Common;
namespace Prince_Talon
{
   internal class SkillHandler
    {
        public static Spell Q, W, E, R;
        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, ObjectManager.Player.AttackRange);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 650);

            W.SetSkillshot(0.7f, 400f, 900f, false, SkillshotType.SkillshotCone);
        }
    }
}