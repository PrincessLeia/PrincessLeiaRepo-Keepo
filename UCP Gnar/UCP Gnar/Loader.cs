using System;
using System.Drawing;
using LeagueSharp;
using System.Windows.Forms;

namespace UCP_Gnar
{
    internal class Loader
    {
        public static void WellCome()
        {
            if (IsChampionSupported())
            {
                Game.PrintChat(
                    "<font color ='{0}'>Loaded UCP </font> <font color ='{1}'>" + ObjectManager.Player.ChampionName + "</font>",
                    Chat.HtmlColor.Cyan, Chat.HtmlColor.Gold);

            }
            else
                /*Game.PrintChat(
                    "<font color ='{0}'>" + ObjectManager.Player.ChampionName +
                    "</font> <font color ='{1}'>Currently not Fully Supported / bugged / or just sucks</font>", Chat.HtmlColor.Gold,
                    Chat.HtmlColor.Cyan);*/
                    return;
        }


        private static bool IsChampionSupported()
        {
            try
            {//sync, sync where?
                // ReSharper disable once AssignNullToNotNullAttribute
                var handle = Activator.CreateInstance(null, "UCP_Gnar." + ObjectManager.Player.ChampionName);
                var champion = (Champion)handle.Unwrap();
                return true;
            }
            // ReSharper disable once EmptyGeneralCatchClause
            //catch (Exception ex)
            catch (Exception)
            {
                //Game.PrintChat(ex.Message);
                return false;
            }
        }

        }
    }
