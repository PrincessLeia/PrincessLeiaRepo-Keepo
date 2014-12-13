#region

using LeagueSharp;
using LeagueSharp.Common;

#endregion

namespace Prince_Warwick
{
    internal class SkillHandler
    {
        public static Spell Q, W, E, R;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 400);
            W = new Spell(SpellSlot.W, 0);
            E = new Spell(SpellSlot.E, 0); // 1500 / 2300 / 3100 / 3900 / 4700
            R = new Spell(SpellSlot.R, 700);
        }
    }
}