using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace MauiApp1.Constants
{
    internal class Urls
    {
        private readonly Dictionary<string, string> tiles;
        private readonly Dictionary<string, string> coreDependency;

        public Urls()
        {
            tiles = new Dictionary<string, string> {
                {"witeboard", "https://witeboard.com/" },
                {"watchDuty", "https://app.watchduty.org/"},
                {"esriSatellite", "https://www.arcgis.com/apps/mapviewer/index.html?layers=10df2279f9684e4a9f6a7f08febac2a9"},
                {"googleSatellite", "https://www.google.com/maps/"},
                {"google", "https://www.google.com/"},
                {"windy", "https://www.windy.com/-Waves-waves"},
                {"macroWeather", "https://earth.nullschool.net/"}
            };

            coreDependency = new Dictionary<string, string> {
                {"florianWebPhila", "https://phila.florian.app/About" },
                {"florianWebQa", "https://qa.florian.app/About"},
                {"florianWebDemo", "https://demo.florian.app/About"},
                {"florianWebProd", "https://florian.app/About"},
                {"bingMaps", "https://www.bing.com/maps"},
                {"googlePlay", "http://play.google.com"},
                {"nextNav", "https://api.nextnav.io/"}
            };
        }
        public Dictionary<string, string> getTiles()
        {
            return tiles;
        }
        public Dictionary<string, string> getCoreDependency()
        {
            return coreDependency;
        }
    }
}
