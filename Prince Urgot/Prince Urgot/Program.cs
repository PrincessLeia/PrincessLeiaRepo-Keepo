using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using xSLx_Orbwalker;   
using Color = System.Drawing.Color;

class Program
{
    // declare shorthandle to access the player object
    // Properties http://msdn.microsoft.com/en-us/library/aa288470%28v=vs.71%29.aspx 
    private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }

    // declare orbwalker class
    public static xSLxOrbwalker Orbwalker;

    // declare  list of spells
    private static Spell Q, Q2, W, E, R;

    public static SpellSlot IgniteSlot;

    // declare menu
    private static Menu Menu;

    /// <summary>
    /// Default programm entrypoint, gets called once on programm creation
    /// </summary>
    static void Main(string[] args)
    {
        // Events http://msdn.microsoft.com/en-us/library/edzehd2t%28v=vs.110%29.aspx
        // OnGameLoad event, gets fired after loading screen is over
        CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
    }

    /// <summary>
    /// Game Loaded Method
    /// </summary>
    private static void Game_OnGameLoad(EventArgs args)
    {
        if (Player.ChampionName != "Urgot") // check if the current champion is Nunu
            return; // stop programm

        // the Spell class provides methods to check and cast Spells
        // Constructor Spell(SpellSlot slot, float range)
        Q = new Spell(SpellSlot.Q, 1000); // create Q spell with a range of 125 units
        Q2 = new Spell(SpellSlot.Q, 1200);
        W = new Spell(SpellSlot.W, 0); // create W spell with a range of 700 units
        E = new Spell(SpellSlot.E, 900); // create E spell with a range of 550 units
        R = new Spell(SpellSlot.R, float.MaxValue); // create R spell with a range of 650 units

        // set spells prediction values, not used on Nunu
        // Method Spell.SetSkillshot(float delay, float width, float speed, bool collision, SkillshotType type)
        // Q.SetSkillshot(0.25f, 80f, 1800f, false, SkillshotType.SkillshotLine);
        Q.SetSkillshot(0.10f, 100f, 1600f, true, SkillshotType.SkillshotLine);
        Q2.SetSkillshot(0.10f, 100f, 1600f, false, SkillshotType.SkillshotLine);
        E.SetSkillshot(0.283f, 0f, 1750f, false, SkillshotType.SkillshotCircle);

        IgniteSlot = Player.GetSpellSlot("SummonerDot");

        // create root menu
        // Constructor Menu(string displayName, string name, bool root)
        Menu = new Menu("Prince " + Player.ChampionName, Player.ChampionName, true);

        // create and add submenu 'Orbwalker'
        // Menu.AddSubMenu(Menu menu) returns added Menu
        Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("xSLx Orbwalker", "Orbwalkert1"));

        // creates Orbwalker object and attach to orbwalkerMenu
        // Constructor Orbwalking.Orbwalker(Menu menu);
        Orbwalker = new xSLxOrbwalker();
        xSLxOrbwalker.AddToMenu(orbwalkerMenu);
        //Menu.AddSubMenu(orbwalkerMenu);

        // create submenu for TargetSelector used by Orbwalker
        Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector")); ;

        // attach
        SimpleTs.AddToMenu(ts);

        // Menu.AddItem(MenuItem item) returns added MenuItem
        // Constructor MenuItem(string name, string displayName)
        // .SetValue(true) on/off button
        Menu laneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
        laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q").SetValue(true));
        laneClearMenu.AddItem(new MenuItem("LaneClearQManaPercent", "Minimum Q Mana Percent").SetValue(new Slider(30, 0, 100)));

        Menu lastHitMenu = Menu.AddSubMenu(new Menu("LastHit", "LastHit"));
        lastHitMenu.AddItem(new MenuItem("lastHitQ", "Use Q").SetValue(true));
        lastHitMenu.AddItem(new MenuItem("lastHitQManaPercent", "Minimum Q Mana Percent").SetValue(new Slider(30, 0, 100)));

        Menu drawMenu = Menu.AddSubMenu(new Menu("Draw", "Drawing"));
        drawMenu.AddItem(new MenuItem("drawQ", "Draw Q").SetValue(true));
        drawMenu.AddItem(new MenuItem("drawE", "Draw extended Q range if hit by E").SetValue(true));
        drawMenu.AddItem(new MenuItem("HUD", "Heads-up Display").SetValue(true));

        Menu harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
        harassMenu.AddItem(new MenuItem("HarassActive", "Harass").SetValue(new KeyBind('C', KeyBindType.Press)));
        harassMenu.AddItem(new MenuItem("HarassToggle", "Harass").SetValue(new KeyBind('T', KeyBindType.Toggle)));
        harassMenu.AddItem(new MenuItem("haraQ", "Harass Q").SetValue(true));
        harassMenu.AddItem(new MenuItem("haraE", "Harass QE").SetValue(true));
        harassMenu.AddItem(new MenuItem("HaraManaPercent", "Minimum Mana Percent").SetValue(new Slider(30, 0, 100)));

        Menu killMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
        killMenu.AddItem(new MenuItem("KillQ", "Steal with Q?").SetValue(true));
        killMenu.AddItem(new MenuItem("KillI", "Steal with Ignite?").SetValue(true));

        Menu.AddItem(new MenuItem("Packet", "Packet Casting").SetValue(true));

        // attach to 'Sift/F9' Menu
        Menu.AddToMainMenu();

        // subscribe to Drawing event
        Drawing.OnDraw += Drawing_OnDraw;


        // subscribe to Update event gets called every game update around 10ms
        Game.OnGameUpdate += Game_OnGameUpdate;

        // print text in local chat
        Game.PrintChat("Prince " + Player.ChampionName + "Loaded");
    }

    /// <summary>
    /// Main Update Method
    /// </summary>
    private static void Game_OnGameUpdate(EventArgs args)
    {
        // dont do stuff while dead
        if (Player.IsDead)
            return;

        // checks the current Orbwalker mode Combo/Mixed/LaneClear/LastHit
        if (xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Combo)
        {
            // combo to kill the enemy
            Hunter();
            NCC();
            Shield();
        }

        if (xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Harass)
        {
            // farm and harass
        }

        if (xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.LaneClear)
        {
            laneClear();
        }
        if (xSLxOrbwalker.CurrentMode == xSLxOrbwalker.Mode.Lasthit)
        {
            lastHit();
        }
        if (Menu.Item("HarassActive").GetValue<KeyBind>().Active || Menu.Item("HarassToggle").GetValue<KeyBind>().Active)
        {
            Harass();
        }

        KillSteal();

    }

    /// <summary>
    /// Get Ignite DMG
    /// </summary>

    private static float GetIgniteDamage(Obj_AI_Hero enemy)
    {
        if (IgniteSlot == SpellSlot.Unknown || Player.SummonerSpellbook.CanUseSpell(IgniteSlot) != SpellState.Ready) return 0f;
        return (float)Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
    }

    /// <summary>
    /// Main Draw Method
    /// </summary>
    private static void Drawing_OnDraw(EventArgs args)
    {
        // dont draw stuff while dead
        if (Player.IsDead)
            return;

        // check if E ready
        if (Menu.Item("drawE").GetValue<bool>() == true)
        {
            var target = SimpleTs.GetTarget(1500, SimpleTs.DamageType.Magical);

            foreach (var obj in ObjectManager.Get<Obj_AI_Hero>().Where(obj => obj.IsValidTarget(Q2.Range) && obj.HasBuff("urgotcorrosivedebuff", true)))
            // draw Aqua circle around the player
            { 
                Utility.DrawCircle(Player.Position, 1100, Color.Aqua);
}
        }

        if (Menu.Item("drawQ").GetValue<bool>() == true)
        {
            // draw Aqua circle around the player
            Utility.DrawCircle(Player.Position, Q.Range, Color.Aqua);
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
                Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.64f, System.Drawing.Color.Yellow, "Q LastHit : On");
            else
                Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.64f, System.Drawing.Color.DarkRed,
                    "Q LastHit : Off");

            if (Menu.Item("LaneClearQ").GetValue<bool>() == true)
                Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.62f, System.Drawing.Color.Yellow, "Q LaneClear : On");
            else
                Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.62f, System.Drawing.Color.DarkRed,
                    "Q LaneClear : Off");
        }
    }


    /// <summary>
    /// Consume logic
    /// </summary>
    private static void Hunter()
    {
        var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
        if (target == null) return;

        if (Q2.IsReady() && target.IsValidTarget(Q2.Range) && target.HasBuff("urgotcorrosivedebuff", true))
        {
            Q2.Cast(target.ServerPosition, Menu.Item("Packet").GetValue<bool>());
        }
        
        if (Q.IsReady() && target.IsValidTarget(Q.Range))
        {
            Q.Cast(target, Menu.Item("Packet").GetValue<bool>());
        }
    }

    /// <summary>
    /// Bloodboil logic
    /// </summary>
    private static void Shield()
    {
        var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
        if (target == null) return;

        if (target.IsValidTarget(Q.Range) && Q.IsReady() && W.IsReady())
        {
            W.Cast(Menu.Item("Packet").GetValue<bool>());
        }
    }

    /// <summary>
    /// Iceblast logic
    /// </summary>
    private static void NCC()
    {
        var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Magical);
        if (target == null) return;

        if (target.IsValidTarget(E.Range) && E.IsReady() && Q.IsReady())
        {
            E.Cast(target, Menu.Item("Packet").GetValue<bool>());
        }
    }

    private static void Harass()
    {
        if (Player.Mana > Player.MaxMana*Menu.Item("HaraManaPercent").GetValue<Slider>().Value/100)
        {
            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            if (target == null) return;

                   Hunter();

                if (Menu.Item("haraE").GetValue<bool>() == true)
                {
                    NCC();
                }
            }
        }


    private static void KillSteal()
    {
        var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
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
                foreach (
                    var minion in
                        MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly))

                    if (Q.IsReady() && Q.IsKillable(minion))
                    {
                        Q.Cast(minion.Position, Menu.Item("Packet").GetValue<bool>());
                    }
            }
        }
    }
}