using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_LeBlanc
{
    internal class ItemHandler
    {
        public static Items.Item Dfg;
        public static SpellSlot Igniteslot;

        public static void Init()
        {
            Dfg = new Items.Item(3128, 750);
            Igniteslot = ObjectManager.Player.GetSpellSlot("SummonerDot");
        }
    }
}
