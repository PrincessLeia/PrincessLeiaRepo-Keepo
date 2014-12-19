using LeagueSharp;
using LeagueSharp.Common;

namespace Princess_LeBlanc
{
    internal class ItemHandler
    {
        public static Items.Item Dfg;
        public static SpellSlot IgniteSlot;

        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }
        public static void Init()
        {
            Dfg = new Items.Item(3128, 750);
            IgniteSlot = Player.GetSpellSlot("SummonerDot");
        }
    }
}
