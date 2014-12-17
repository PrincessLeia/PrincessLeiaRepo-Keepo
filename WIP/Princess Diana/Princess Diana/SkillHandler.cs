using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using SharpDX;
namespace Princess_Diana
{
    class SkillHandler
    {
        public static Spell Q, W, E, R;
        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 830f);
            W = new Spell(SpellSlot.W, 200f);
            E = new Spell(SpellSlot.E, 450f);
            R = new Spell(SpellSlot.R, 825f);
            Q.SetSkillshot(0.35f, 200f, 1800, false, SkillshotType.SkillshotCircle);
        }
    }
}