using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;

namespace UCP_Gnar
{
    class Program
    {


        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {
            Game.OnGameStart += OnGameStart;
        }

        private static void OnGameStart(EventArgs args)
        {

            Loader.WellCome();
        }
    }
}