using LeagueSharp;
using LeagueSharp.Common;

namespace PrinceTalon
{
    internal class ItemHandler
    {
        public static Items.Item Bilgewater;
        public static Items.Item Blade;
        public static Items.Item Youmuu;
        public static Items.Item Tiamat;
        public static Items.Item Hydra;
        public static SpellSlot IgniteSlot;

        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }
        public static void Init()
        {
            Bilgewater = new Items.Item(3144, 450);
            Blade = new Items.Item(3153, 450);
            Youmuu = new Items.Item(3142, 0);
            Tiamat = new Items.Item(3077, 400);
            Hydra = new Items.Item(3074, 400);
            IgniteSlot = Player.GetSpellSlot("SummonerDot");






        }
    }
}
