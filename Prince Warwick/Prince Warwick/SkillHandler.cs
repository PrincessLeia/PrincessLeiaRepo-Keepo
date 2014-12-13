using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using SharpDX;
namespace Prince_Warwick
{
    class SkillHandler
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