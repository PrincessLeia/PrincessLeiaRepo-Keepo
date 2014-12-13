#region

using LeagueSharp;
using LeagueSharp.Common;

#endregion

namespace Prince_Warwick
{
    internal class ItemHandler
    {
        public static Items.Item Dfg;
        public static Items.Item Bilgewater;
        public static Items.Item Blade;
        public static Items.Item Qss;
        public static Items.Item Youmuu;
        public static Items.Item Tiamat;
        public static Items.Item Hydra;
        public static Items.Item Randuin;
        public static Items.Item Mercurial;
        public static SpellSlot IgniteSlot;

        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }

        public static void Init()
        {
            Dfg = new Items.Item(3128, 750);
            Bilgewater = new Items.Item(3144, 450);
            Blade = new Items.Item(3153, 450);
            Qss = new Items.Item(3140, 0);
            Youmuu = new Items.Item(3142, 0);
            Tiamat = new Items.Item(3077, 400);
            Hydra = new Items.Item(3074, 400);
            Randuin = new Items.Item(3143, 500);
            Mercurial = new Items.Item(3139, 0);
            IgniteSlot = Player.GetSpellSlot("SummonerDot");
        }
    }
}