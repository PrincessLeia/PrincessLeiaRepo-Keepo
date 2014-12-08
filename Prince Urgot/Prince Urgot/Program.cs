using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using xSLx_Orbwalker;
using SharpDX;   
using Color = System.Drawing.Color;


class Program
{
    private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
    public static xSLxOrbwalker Orbwalker;
    private static Spell Q, Q2, W, E, R;
    public static SpellSlot IgniteSlot;
    private static Menu Menu;
    public static readonly StringList HitChanceList = new StringList(new []{ "Low", "Medium", "High", "Very High" });
    public static Items.Item Muramana;

    static void Main(string[] args)
    {
        CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
    }

    private static void Game_OnGameLoad(EventArgs args)
    {
        if (Player.ChampionName != "Urgot")
            return;

        Q = new Spell(SpellSlot.Q, 1000); 
        Q2 = new Spell(SpellSlot.Q, 1200); 
        W = new Spell(SpellSlot.W); 
        E = new Spell(SpellSlot.E, 900);  
        R = new Spell(SpellSlot.R, 550);

        Q.SetSkillshot(0.10f, 100f, 1600f, true, SkillshotType.SkillshotLine);
        Q2.SetSkillshot(0.10f, 100f, 1600f, false, SkillshotType.SkillshotLine);
        E.SetSkillshot(0.283f, 0f, 1750f, false, SkillshotType.SkillshotCircle);


        IgniteSlot = Player.GetSpellSlot("SummonerDot");
        Muramana = new Items.Item(3042, 0);

        Menu = new Menu("Prince " + Player.ChampionName, Player.ChampionName, true);

        Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("xSLx Orbwalker", "Orbwalkert1"));

        Orbwalker = new xSLxOrbwalker();
        xSLxOrbwalker.AddToMenu(orbwalkerMenu);

        Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector")); ;

        SimpleTs.AddToMenu(ts);

        Menu laneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
        laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q").SetValue(true));
        laneClearMenu.AddItem(new MenuItem("LaneClearQManaPercent", "Minimum Q Mana Percent").SetValue(new Slider(30, 0, 100)));

        Menu lastHitMenu = Menu.AddSubMenu(new Menu("LastHit", "LastHit"));
        lastHitMenu.AddItem(new MenuItem("lastHitQ", "Use Q").SetValue(true));
        lastHitMenu.AddItem(new MenuItem("lastHitQManaPercent", "Minimum Q Mana Percent").SetValue(new Slider(30, 0, 100)));

        Menu harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
        harassMenu.AddItem(new MenuItem("HarassActive", "Harass").SetValue(new KeyBind('C', KeyBindType.Press)));
        harassMenu.AddItem(new MenuItem("HarassToggle", "Harass").SetValue(new KeyBind('T', KeyBindType.Toggle)));
        harassMenu.AddItem(new MenuItem("haraQ", "Auto Q on E Debuff").SetValue(true));
        harassMenu.AddItem(new MenuItem("haraE", "Auto E for Debuff").SetValue(true));
        harassMenu.AddItem(new MenuItem("HaraManaPercent", "Minimum Mana Percent").SetValue(new Slider(30, 0, 100)));

        Menu itemsMenu = Menu.AddSubMenu(new Menu("Items", "Items"));
        itemsMenu.AddItem(new MenuItem("useMura", "enable Auto Muramana").SetValue(new KeyBind('A', KeyBindType.Toggle)));

        Menu preMenu = Menu.AddSubMenu(new Menu("Prediction", "Prediction"));
        preMenu.AddItem(new MenuItem("preE", "HitChance E").SetValue(HitChanceList));
        preMenu.AddItem(new MenuItem("preQ", "HitChance Q").SetValue(HitChanceList));

        Menu drawMenu = Menu.AddSubMenu(new Menu("Draw", "Drawing"));
        drawMenu.AddItem(new MenuItem("drawQ", "Draw Q").SetValue(new Circle(true, Color.FromArgb(100, Color.Aqua))));
        drawMenu.AddItem(new MenuItem("drawE", "Draw extended Q range if hit by E").SetValue(new Circle(true, Color.FromArgb(100, Color.Aqua))));
        drawMenu.AddItem(new MenuItem("HUD", "Heads-up Display").SetValue(true));
        drawMenu.AddItem(new MenuItem("hitbye", "Draw Circle on Enemy if hit by E").SetValue(true));
        drawMenu.AddItem(new MenuItem("drawR", "Draw R").SetValue(new Circle(true, Color.FromArgb(100, Color.Red))));

        Menu miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
        miscMenu.AddItem(new MenuItem("autoR", "Auto R if under Tower BETA").SetValue(false));
        miscMenu.AddItem(new MenuItem("autoInt", "Auto Interrupt with R").SetValue(false));
        miscMenu.AddItem(new MenuItem("KillQ", "KillSteal with Q?").SetValue(true));
        miscMenu.AddItem(new MenuItem("KillI", "KillSteal with Ignite?").SetValue(true));

        Menu.AddItem(new MenuItem("Packet", "Packet Casting").SetValue(true));

        Menu.AddItem(new MenuItem("madebyme", "PrincessLeia :)").DontSave());

        Menu.AddToMainMenu();


        Interrupter.OnPossibleToInterrupt += Interrupter_OnPossibleToInterrupt; 
        CustomEvents.Unit.OnLevelUp += OnLevelUp; 
        Drawing.OnDraw += Drawing_OnDraw;
        Game.OnGameUpdate += Game_OnGameUpdate;


        Game.PrintChat("Prince " + Player.ChampionName + " Loaded");
    }


    private static void Game_OnGameUpdate(EventArgs args)
    {

 
        if (Player.IsDead)
            return;

        if (xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Combo)
        {
            //NCC();
            //Hunter();
            CastLogic();
            activateMura();
        }

        if (xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Harass)
        {

        }

        if (xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.LaneClear)
        {
            laneClear();
            deActivateMura();
        }
        if (xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Lasthit)
        {
            lastHit();
            deActivateMura();
        }
        if (Menu.Item("HarassActive").GetValue<KeyBind>().Active || Menu.Item("HarassToggle").GetValue<KeyBind>().Active)
        {
            Harass();
        }
        if (Menu.Item("autoR").GetValue<bool>() && R.IsReady())
        {
            AutoR();
        }


        KillSteal();


    }


    private static float GetIgniteDamage(Obj_AI_Hero enemy)
    {
        if (IgniteSlot == SpellSlot.Unknown || Player.SummonerSpellbook.CanUseSpell(IgniteSlot) != SpellState.Ready) return 0f;
        return (float)Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
    }

    private static void Drawing_OnDraw(EventArgs args)
    {
        if (Player.IsDead)
            return;

        var DrawE = Menu.Item("drawE").GetValue<Circle>();
        if (DrawE.Active)
        {
            var target = SimpleTs.GetTarget(1500, SimpleTs.DamageType.Physical);

            foreach (var obj in ObjectManager.Get<Obj_AI_Hero>().Where(obj => obj.IsValidTarget(1500) && obj.HasBuff("urgotcorrosivedebuff", true)))
            {
                Utility.DrawCircle(Player.Position, 1110, DrawE.Color);
            }
        }

        var DrawQ = Menu.Item("drawQ").GetValue<Circle>();
        if (DrawQ.Active)
        {
            Utility.DrawCircle(Player.Position, Q.Range, DrawQ.Color);
        }

        if (Menu.Item("HUD").GetValue<bool>())
        {
            if (Menu.Item("HarassActive").GetValue<KeyBind>().Active || Menu.Item("HarassToggle").GetValue<KeyBind>().Active)
                Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.68f, System.Drawing.Color.Yellow,
                    "Auto Harass : On");
            else
                Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.68f, System.Drawing.Color.DarkRed,
                    "Auto Harass : Off");

            if (Menu.Item("KillI").GetValue<bool>() == true || Menu.Item("KillQ").GetValue<bool>() == true)
                Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.66f, System.Drawing.Color.Yellow, "Auto KS : On");
            else
                Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.66f, System.Drawing.Color.DarkRed,
                    "Auto KS : Off");

            if (Menu.Item("lastHitQ").GetValue<bool>() == true)
                Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.60f, System.Drawing.Color.Yellow, "Q LastHit : On");
            else
                Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.60f, System.Drawing.Color.DarkRed,
                    "Q LastHit : Off");

            if (Menu.Item("LaneClearQ").GetValue<bool>() == true)
                Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.58f, System.Drawing.Color.Yellow, "Q LaneClear : On");
            else
                Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.58f, System.Drawing.Color.DarkRed,
                    "Q LaneClear : Off");

            if (Muramana.IsReady())
            {
                if (Menu.Item("useMura").GetValue<KeyBind>().Active)
                    Drawing.DrawText(Drawing.Width*0.90f, Drawing.Height*0.64f, System.Drawing.Color.Yellow,
                        "Auto Muramana : On");
                else
                    Drawing.DrawText(Drawing.Width*0.90f, Drawing.Height*0.64f, System.Drawing.Color.DarkRed,
                        "Auto Muramana : Off");
            }
            else
                Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.64f, System.Drawing.Color.DarkRed,
                    "Muramana unavailable");
        }

        if (Menu.Item("autoR").GetValue<bool>() == true || Menu.Item("autoInt").GetValue<bool>() == true)
            Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.62f, System.Drawing.Color.Yellow, "Auto R : On");
        else
            Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.62f, System.Drawing.Color.DarkRed,
                "Auto R : Off");

        if (Menu.Item("hitbye").GetValue<bool>() == true)
        {
            var target = SimpleTs.GetTarget(2000, SimpleTs.DamageType.Physical);

            foreach (var obj in ObjectManager.Get<Obj_AI_Hero>().Where(obj => obj.IsValidTarget(2000) && obj.HasBuff("urgotcorrosivedebuff", true)))

                Utility.DrawCircle(target.Position, 100, Color.GreenYellow);
        }

        var DrawR = Menu.Item("drawR").GetValue<Circle>();
        if (DrawR.Active)
        {
            Utility.DrawCircle(Player.Position, R.Range, DrawR.Color);
        }

    }



            public static void OnLevelUp(Obj_AI_Base sender, CustomEvents.Unit.OnLevelUpEventArgs args) 
         { 
             if (!sender.IsValid || !sender.IsMe) 
                 return; 
             if (args.NewLevel == 11) 
                 R = new Spell(SpellSlot.R, 700); 
             if (args.NewLevel == 16) 
                 R = new Spell(SpellSlot.R, 850); 
         } 
 
 
         public static void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell) 
         { 
             if (Menu.Item("autoInt").GetValue<bool>() && R.IsReady() && unit.IsEnemy && unit.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player))) 
             {                 R.CastOnUnit(unit); 
            } 
         } 


         private static void Hunter()
         {
             var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
             if (Q.IsReady() && target.IsValidTarget(target.HasBuff("urgotcorrosivedebuff", true) ? Q2.Range : Q.Range))
             {
                 Q.Cast(target);
             }
         }
         private static void SmartQ()
         {

             foreach (var obj in
                 ObjectManager.Get<Obj_AI_Hero>()
                     .Where(obj => obj.IsValidTarget(Q2.Range) && obj.HasBuff("urgotcorrosivedebuff", true)))
             {
                 W.Cast();
                 Q2.Cast(obj.ServerPosition);
             }
         }

         private static void CastLogic()
         {
             SmartQ();

             var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
             if (target == null)
             {
                 return;
             }

             NCC();
             Hunter();
         }

         private static void NCC()
         {
             if (!E.IsReady())
             {
                 return;
             }

             var hitchance = (HitChance)(Menu.Item("preE").GetValue<StringList>().SelectedIndex + 3);
             var target = SimpleTs.GetTarget(1400, SimpleTs.DamageType.Physical);

             if (target.IsValidTarget(E.Range))
             {
                 E.CastIfHitchanceEquals(target, hitchance);
             }
             else
             {
                 E.CastIfHitchanceEquals(SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical), HitChance.High);
             }
         }



    private static void Harass()
    {
        var mana = Player.Mana > Player.MaxMana*Menu.Item("HaraManaPercent").GetValue<Slider>().Value/100;
        var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
        
            if (Menu.Item("haraQ").GetValue<bool>() == true)
            {
                SmartQ();
            }

            if (Menu.Item("haraE").GetValue<bool>() == true && mana)
            {

                if (target == null)
                {
                    return;
                }

                NCC();

            }

    }


    private static void KillSteal()
    {
        var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
        if (target == null) return;

        if (target.IsValidTarget(Q.Range) && Q.IsReady() && Menu.Item("KillQ").GetValue<bool>() == true &&
            ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q) > target.Health)
        {
           Hunter();
        }

        if (Menu.Item("KillI").GetValue<bool>() == true)
        {
            if (IgniteSlot != SpellSlot.Unknown &&
                Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                if (target.Health + 20 <= GetIgniteDamage(target))
                {
                    Player.SummonerSpellbook.CastSpell(IgniteSlot, target);
                }
            }
        }
    }


    private static void laneClear()
    {
                if (Menu.Item("LaneClearQ").GetValue<bool>() == true)
        {
            if (Player.Mana > Player.MaxMana*Menu.Item("LaneClearQManaPercent").GetValue<Slider>().Value/100)
            {
                var minion = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All,
                    MinionTeam.NotAlly);

                MinionManager.FarmLocation qPosition = Q.GetLineFarmLocation(minion);

                if (Q.IsReady() && qPosition.MinionsHit >= 1)

                {
                    Q.Cast(qPosition.Position, Menu.Item("Packet").GetValue<bool>());
                }
            }
        }
    }

    private static void lastHit()
    {
        if (Menu.Item("lastHitQ").GetValue<bool>() == true)
        {
            if (Player.Mana > Player.MaxMana*Menu.Item("LaneClearQManaPercent").GetValue<Slider>().Value/100)

            {
                var myMinions = MinionManager.GetMinions(Player.ServerPosition, Q.Range);

                if (Q.IsReady())
                {
                    foreach (var minion in myMinions.Where(minion => Player.GetSpellDamage(minion, SpellSlot.Q) >= HealthPrediction.GetHealthPrediction(minion, (int)(Q.Delay * 1000))))
                    {
                        Q.Cast(minion.ServerPosition, Menu.Item("Packet").GetValue<bool>());
                    }
                }             
            }
        }
    }

    public static void activateMura()
    {
        if (Menu.Item("useMura").GetValue<KeyBind>().Active)
        {
            if (Player.Buffs.Count(buf => buf.Name == "Muramana") == 0)
                Muramana.Cast();
        }
    }
    public static void deActivateMura()
    {
        if (Menu.Item("useMura").GetValue<KeyBind>().Active)
        {
            if (Player.Buffs.Count(buf => buf.Name == "Muramana") == 1)
                Muramana.Cast();
        }
    }

    public static void AutoR()
    {
        var target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Physical);

        var turret = ObjectManager.Get<Obj_AI_Turret>().First(obj => obj.IsAlly && obj.Distance(Player) <= 775f);

        if (turret != null && target != null)
        {
            R.Cast(target, true);
        }
    }


}