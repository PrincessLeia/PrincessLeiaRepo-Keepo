using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using xSLx_Orbwalker;
using Color = System.Drawing.Color;

namespace Ultimate_Carry_Prevolution.Plugin
{
	internal class Thresh : Champion
	{
		public int QFollowTick = 0;
		public const int QFollowTime = 5000;
		public Thresh()
		{
			SetSpells();
			LoadMenu();
		}

		private void SetSpells()
		{
			Q = new Spell(SpellSlot.Q, 1000);
			Q.SetSkillshot(0.5f, 50f, 1900, true, SkillshotType.SkillshotLine);

			W = new Spell(SpellSlot.W, 950);

			E = new Spell(SpellSlot.E, 400);

			R = new Spell(SpellSlot.R, 400);
		}

		private void LoadMenu()
		{
			var champMenu = new Menu(MyHero.ChampionName + " Plugin", MyHero.ChampionName);
			{
				var comboMenu = new Menu("Combo", "Combo");
				{
					AddSpelltoMenu(comboMenu, "Q", true);
					AddSpelltoMenu(comboMenu, "Q Follow", true);
					AddSpelltoMenu(comboMenu, "W Shield at %", 40);
					AddSpelltoMenu(comboMenu, "W for Engage", true);
					AddSpelltoMenu(comboMenu, "E to Me till % health", 20);
					AddSpelltoMenu(comboMenu, "R if Hit", 2, 0, 5);
					champMenu.AddSubMenu(comboMenu);
				}
				var harassMenu = new Menu("Harass", "Harass");
				{
					AddSpelltoMenu(harassMenu, "Q", true);
					AddSpelltoMenu(harassMenu, "W to safe Friend", true);
					AddSpelltoMenu(harassMenu, "E to Me till % health", 50);
					AddManaManagertoMenu(harassMenu, 30);
					champMenu.AddSubMenu(harassMenu);
				}

				var laneClearMenu = new Menu("LaneClear", "LaneClear");
				{
					AddSpelltoMenu(laneClearMenu, "E", true);
					AddManaManagertoMenu(laneClearMenu, 0);
					champMenu.AddSubMenu(laneClearMenu);
				}

				var miscMenu = new Menu("Misc", "Misc");
				{
					AddMisc(miscMenu, "Q to Interrupt", true);
					AddMisc(miscMenu, "E to Interrupt", true);
					AddMisc(miscMenu, "E Anti Gapclose", true);
					champMenu.AddSubMenu(miscMenu);
				}

				var drawMenu = new Menu("Drawing", "Drawing");
				{
					drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All").SetValue(false));
					drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_W", "Draw W").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_E", "Draw E").SetValue(true));
					drawMenu.AddItem(new MenuItem("Draw_R", "Draw R").SetValue(true));

					var drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage").SetValue(true);
					drawMenu.AddItem(drawComboDamageMenu);
					Utility.HpBarDamageIndicator.DamageToUnit = GetComboDamage;
					Utility.HpBarDamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
					drawComboDamageMenu.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
					{
						Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
					};

					champMenu.AddSubMenu(drawMenu);
				}

				Menu.AddSubMenu(champMenu);
				Menu.AddToMainMenu();
			}
		}

		private float GetComboDamage(Obj_AI_Base enemy)
		{
			var damage = 0d;

			if(Q.IsReady())
				damage += MyHero.GetSpellDamage(enemy, SpellSlot.Q);

			if(E.IsReady())
				damage += MyHero.GetSpellDamage(enemy, SpellSlot.E) + MyHero.GetAutoAttackDamage(enemy);

			if(R.IsReady())
				damage += MyHero.GetSpellDamage(enemy, SpellSlot.R);

			damage += MyHero.GetAutoAttackDamage(enemy, true);

			return (float)damage;
		}

		public override void OnDraw()
		{
			if(Menu.Item("Draw_Disabled").GetValue<bool>())
			{
				xSLxOrbwalker.DisableDrawing();
				return;
			}
			xSLxOrbwalker.EnableDrawing();

			if(Menu.Item("Draw_Q").GetValue<bool>())
				if(Q.Level > 0)
					Utility.DrawCircle(MyHero.Position, Q.Range, Q.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_W").GetValue<bool>())
				if(W.Level > 0)
					Utility.DrawCircle(MyHero.Position, W.Range, W.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_E").GetValue<bool>())
				if(E.Level > 0)
					Utility.DrawCircle(MyHero.Position, E.Range - 5, E.IsReady() ? Color.Green : Color.Red);

			if(Menu.Item("Draw_R").GetValue<bool>())
				if(R.Level > 0)
					Utility.DrawCircle(MyHero.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);

		}

		public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
		{
			if(GetMiscBool("E to Interrupt") && unit.IsValidTarget(E.Range))
			{
				E.Cast(GetReversePosition(MyHero.Position, unit.Position), UsePackets());
				return;
			}
			if(!GetMiscBool("Q to Interrupt") || Environment.TickCount - QFollowTick <= QFollowTime || !unit.IsValidTarget(Q.Range))
				return;
			if(Q.CastIfHitchanceEquals(unit, HitChance.Medium, UsePackets()))
				QFollowTick = Environment.TickCount;
		}

		public override void OnGapClose(ActiveGapcloser gapcloser)
		{
			if(!GetMiscBool("E Anti Gapclose"))
				return;
			if(!(gapcloser.End.Distance(MyHero.ServerPosition) < E.Range))
				return;
			E.Cast(gapcloser.End, UsePackets());
		}

		public override void OnCombo()
		{
			if(IsSpellActive("Q") && Environment.TickCount - QFollowTick >= QFollowTime)
				if(Cast_BasicSkillshot_Enemy(Q,SimpleTs.DamageType.Magical,0,HitChance.High) != null)
					QFollowTick = Environment.TickCount;

			if(IsSpellActive("Q Follow") && QFollowTarget.ShouldCast() && Q.IsReady())
				Q.Cast();

			Cast_Shield_onFriend(W, GetValue("W Shield at %"), true);
			
			if(IsSpellActive("W for Engage"))
				EngageFriendLatern();

			if(GetValue("E to Me till % health") > 0)
				if(GetHealthPercent(MyHero) > GetValue("E to Me till % health"))
					Cast_E("ToMe");
				else
					Cast_E();

			if(GetValue("R if Hit") > 0)
				if(MyHero.CountEnemysInRange((int)R.Range) >= GetValue("R if Hit"))
					R.Cast();
			
		}

		public override void OnHarass()
		{
			if(IsSpellActive("Q") && Environment.TickCount - QFollowTick >= QFollowTime && ManaManagerAllowCast())
				if(Cast_BasicSkillshot_Enemy(Q) != null)
					QFollowTick = Environment.TickCount;

			if(GetValue("E to Me till % health") > 0)
				if(GetHealthPercent(MyHero) > GetValue("E to Me till % health"))
					Cast_E("ToMe");
				else
					Cast_E();
			if(IsSpellActive("W to safe Friend"))
				SafeFriendLatern();
		}

		public override void OnLaneClear()
		{
			if(IsSpellActive("E"))
				Cast_BasicSkillshot_AOE_Farm(E);
		}


		private void Cast_E(string mode = "")
		{
			if(!E.IsReady())
				return;
			var target = SimpleTs.GetTarget(E.Range - 10, SimpleTs.DamageType.Physical);
			if(target == null)
				return;
			E.Cast(mode == "ToMe" ? GetReversePosition(MyHero.Position, target.Position) : target.Position, UsePackets());
		}
		private void EngageFriendLatern()
		{
			if(!W.IsReady())
				return;
			var bestcastposition = new Vector3(0f, 0f, 0f);
			foreach(var friend in AllHerosFriend.Where(hero => !hero.IsMe && hero.Distance(MyHero) <= W.Range + 300 && hero.Distance(MyHero) <= W.Range - 300 && hero.Health / hero.MaxHealth * 100 >= 20 && EnemysinRange(150)))
			{
				var center = MyHero.Position;
				const int points = 36;
				var radius = W.Range;

				const double slice = 2 * Math.PI / points;
				for(var i = 0; i < points; i++)
				{
					var angle = slice * i;
					var newX = (int)(center.X + radius * Math.Cos(angle));
					var newY = (int)(center.Y + radius * Math.Sin(angle));
					var p = new Vector3(newX, newY, 0);
					if(p.Distance(friend.Position) <= bestcastposition.Distance(friend.Position))
						bestcastposition = friend.Position;
				}
				if(!(friend.Distance(MyHero) <= W.Range))
					continue;
				W.Cast(bestcastposition, UsePackets());
				return;
			}
			if(bestcastposition.Distance(new Vector3(0f, 0f, 0f)) >= 100)
				W.Cast(bestcastposition, UsePackets());
		}

		private void SafeFriendLatern()
		{
			if(!W.IsReady())
				return;
			var bestcastposition = new Vector3(0f, 0f, 0f);
			foreach(
				var friend in
					AllHerosFriend
						.Where(
							hero =>
								hero.Distance(MyHero) <= W.Range - 200 && hero.Health / hero.MaxHealth * 100 <= 20 && !hero.IsDead))
			{
				foreach(var enemy in AllHerosEnemy)
				{
					if(friend == null)
						continue;
					var center = ObjectManager.Player.Position;
					const int points = 36;
					var radius = W.Range;

					const double slice = 2 * Math.PI / points;
					for(var i = 0; i < points; i++)
					{
						var angle = slice * i;
						var newX = (int)(center.X + radius * Math.Cos(angle));
						var newY = (int)(center.Y + radius * Math.Sin(angle));
						var p = new Vector3(newX, newY, 0);
						if(p.Distance(friend.Position) <= bestcastposition.Distance(friend.Position))
							bestcastposition = p;
					}
					if(friend.Distance(ObjectManager.Player) <= W.Range)
					{
						W.Cast(W.GetPrediction(friend).CastPosition, UsePackets());
						return;
					}
				}
				if(bestcastposition.Distance(new Vector3(0f, 0f, 0f)) >= 100)
					W.Cast(bestcastposition, UsePackets());
			}
		}
		internal class QFollowTarget
		{
			public static bool Exist()
			{
				return ObjectManager.Get<Obj_AI_Base>().Any(unit => unit.HasBuff("ThreshQ") && !unit.IsMe);
			}

			public static Obj_AI_Base Get()
			{
				return ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(unit => unit.HasBuff("ThreshQ") && !unit.IsMe);
			}

			public bool InTower()
			{
				return IsInsideEnemyTower(Get().Position);
			}

			public static bool ShouldCast()
			{
				if(!Exist())
					return false;
				var buff = Get().Buffs.FirstOrDefault(buf => buf.Name == "ThreshQ");
				if(buff == null)
					return false;
				return buff.EndTime - Game.Time < 0.5;
			}
		}
	}
}
