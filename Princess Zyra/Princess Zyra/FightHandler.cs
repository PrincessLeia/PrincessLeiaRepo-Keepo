using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Princess_Zyra
{
    class FightHandler
    {
        private static Obj_AI_Hero Player
            {
              get { return ObjectManager.Player; }
            }



        public static void CastEMinion()
        {
            if (!SkillHandler.E.IsReady())
                return;
            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, SkillHandler.E.Range, MinionTypes.All, MinionTeam.NotAlly);
            if (minions.Count == 0)
                return;
            var castPostion = MinionManager.GetBestLineFarmLocation(minions.Select(minion => minion.ServerPosition.To2D()).ToList(), SkillHandler.E.Width, SkillHandler.E.Range);
            SkillHandler.E.Cast(castPostion.Position, Packets());
            if (!MenuHandler.ZyraConfig.Item("useW_Passive").GetValue<bool>())
                return;
            var pos = castPostion.Position.To3D();
            Utility.DelayAction.Add(50, () => SkillHandler.W.Cast(new Vector3(pos.X - 5, pos.Y - 5, pos.Z), Packets()));
            Utility.DelayAction.Add(150, () => SkillHandler.W.Cast(new Vector3(pos.X + 5, pos.Y + 5, pos.Z), Packets()));
        }

        public static void CastQMinion()
        {
            if (!SkillHandler.Q.IsReady())
                return;
            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, SkillHandler.Q.Range + (SkillHandler.Q.Width / 2), MinionTypes.All, MinionTeam.NotAlly);
            if (minions.Count == 0)
                return;
            var castPostion = MinionManager.GetBestCircularFarmLocation(minions.Select(minion => minion.ServerPosition.To2D()).ToList(), SkillHandler.Q.Width, SkillHandler.Q.Range);
            SkillHandler.Q.Cast(castPostion.Position, Packets());
            if (MenuHandler.ZyraConfig.Item("useW_Passive").GetValue<bool>())
            {
                var pos = castPostion.Position.To3D();
                Utility.DelayAction.Add(50, () => SkillHandler.W.Cast(new Vector3(pos.X - 5, pos.Y - 5, pos.Z), Packets()));
                Utility.DelayAction.Add(150, () => SkillHandler.W.Cast(new Vector3(pos.X + 5, pos.Y + 5, pos.Z), Packets()));
            }
        }

        public static void CastREnemy()
        {
            if (!SkillHandler.R.IsReady())
                return;
            var minHit = MenuHandler.ZyraConfig.Item("useR_TeamFight_willhit").GetValue<Slider>().Value;
            if (minHit == 0)
                return;
            var target = SimpleTs.GetTarget(SkillHandler.R.Range, SimpleTs.DamageType.Magical);
            if (!target.IsValidTarget(SkillHandler.R.Range))
                return;
            SkillHandler.R.CastIfWillHit(target, minHit - 1, Packets());
        }

        public static void CastQEnemy()
        {
            if (!SkillHandler.Q.IsReady())
                return;
            var target = SimpleTs.GetTarget(SkillHandler.Q.Range + (SkillHandler.Q.Width / 2), SimpleTs.DamageType.Magical);
            if (!target.IsValidTarget(SkillHandler.Q.Range))
                return;
            SkillHandler.Q.CastIfHitchanceEquals(target, HitChance.High, Packets());
            if (MenuHandler.ZyraConfig.Item("useW_Passive").GetValue<bool>())
            {
                var pos = SkillHandler.Q.GetPrediction(target).CastPosition;
                Utility.DelayAction.Add(50, () => SkillHandler.W.Cast(new Vector3(pos.X - 2, pos.Y - 2, pos.Z), Packets()));
                Utility.DelayAction.Add(150, () => SkillHandler.W.Cast(new Vector3(pos.X + 2, pos.Y + 2, pos.Z), Packets()));
            }
        }

        public static void CastEEnemy()
        {
            if (!SkillHandler.E.IsReady())
                return;
            var target = SimpleTs.GetTarget(SkillHandler.E.Range, SimpleTs.DamageType.Magical);
            if (!target.IsValidTarget(SkillHandler.E.Range))
                return;
            SkillHandler.E.CastIfHitchanceEquals(target, HitChance.High, Packets());
            if (MenuHandler.ZyraConfig.Item("useW_Passive").GetValue<bool>())
            {
                var pos = SkillHandler.E.GetPrediction(target).CastPosition;
                Utility.DelayAction.Add(50, () => SkillHandler.W.Cast(new Vector3(pos.X - 5, pos.Y - 5, pos.Z), Packets()));
                Utility.DelayAction.Add(150, () => SkillHandler.W.Cast(new Vector3(pos.X + 5, pos.Y + 5, pos.Z), Packets()));
            }
        }

        public static void CastPassive()
        {
            if (!SkillHandler.Passive.IsReady())
                return;
            var target = SimpleTs.GetTarget(SkillHandler.Passive.Range, SimpleTs.DamageType.Magical);
            if (!target.IsValidTarget(SkillHandler.E.Range))
                return;
            SkillHandler.Passive.CastIfHitchanceEquals(target, HitChance.High, Packets());
        }

        public static bool Packets()
        {
            return MenuHandler.ZyraConfig.Item("packets").GetValue<bool>();
        }


    }
}