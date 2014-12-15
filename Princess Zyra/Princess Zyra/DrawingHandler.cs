using System;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace Princess_Zyra
{
    internal class DrawingHandler
    {
        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        public static void Init()
        {
            Drawing.OnDraw += OnDraw;
        }

        private static void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
                return;

            if (MenuHandler.ZyraConfig.Item("Draw_Disabled").GetValue<bool>())
                return;

            if (Program.ZyraisZombie())
            {
                Utility.DrawCircle(ObjectManager.Player.Position, SkillHandler.Passive.Range, SkillHandler.Passive.IsReady() ? Color.Green : Color.Red);
                return;
            }
            if (MenuHandler.ZyraConfig.Item("Draw_Q").GetValue<bool>())
                if (SkillHandler.Q.Level > 0)
                    Utility.DrawCircle(ObjectManager.Player.Position, SkillHandler.Q.Range, SkillHandler.Q.IsReady() ? Color.Green : Color.Red);

            if (MenuHandler.ZyraConfig.Item("Draw_W").GetValue<bool>())
                if (SkillHandler.W.Level > 0)
                    Utility.DrawCircle(ObjectManager.Player.Position, SkillHandler.W.Range, SkillHandler.W.IsReady() ? Color.Green : Color.Red);

            if (MenuHandler.ZyraConfig.Item("Draw_E").GetValue<bool>())
                if (SkillHandler.E.Level > 0)
                    Utility.DrawCircle(ObjectManager.Player.Position, SkillHandler.E.Range, SkillHandler.E.IsReady() ? Color.Green : Color.Red);

            if (MenuHandler.ZyraConfig.Item("Draw_R").GetValue<bool>())
                if (SkillHandler.R.Level > 0)
                    Utility.DrawCircle(ObjectManager.Player.Position, SkillHandler.R.Range, SkillHandler.R.IsReady() ? Color.Green : Color.Red);
        }

    }
}
