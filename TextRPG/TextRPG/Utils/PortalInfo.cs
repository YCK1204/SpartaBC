using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG.Utils
{
    public class PortalInfo
    {
        public int[] spawn_pos { get; set; }
        public int[] pos { get; set; }
        public string to_map { get; set; }
        public int to_id { get; set; }
        public string name { get; set; }
    }

    public class MapInfoRoot
    {
        public Dictionary<string, PortalInfo[]>[] portals { get; set; }
    }
}