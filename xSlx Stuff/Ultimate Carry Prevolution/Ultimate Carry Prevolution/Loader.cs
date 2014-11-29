using System;
using System.IO;
using LeagueSharp;

namespace Ultimate_Carry_Prevolution
{
	class Loader
	{
		public static bool IsBetaTester;
		public const string VersionNumber = "1.6";

		public Loader()
		{
			Game.OnGameInput += OnInput;
			try
			{
				var fileName = Path.Combine(Environment.GetFolderPath(
					Environment.SpecialFolder.ApplicationData), "LeagueSharp\\Betatester.xslx");
				if (File.Exists(fileName))
				{
					IsBetaTester = true;
					Game.PrintChat("BetaTests enabled.");
				}
			
			}
			catch(Exception)
			{
				Game.PrintChat("BetaTests currently not available.");
			}
			Chat.WellCome();
		}

		private void OnInput(GameInputEventArgs args)
		{
			var fileName = Path.Combine(Environment.GetFolderPath(
					   Environment.SpecialFolder.ApplicationData), "LeagueSharp\\Betatester.xslx");
			if (args.Input == ".ucp-beta-on")
			{
				File.Create(fileName);
				Game.PrintChat("Beta enabled, pls reload F8");
				args.Process = false;
			}
			if(args.Input == ".ucp-beta-off")
			{
				File.Delete(fileName);
				Game.PrintChat("Beta disabled, pls reload F8");
				args.Process = false;
			}
		}
	}
}
