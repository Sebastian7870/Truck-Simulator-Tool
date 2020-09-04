using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Json
{

    public class Rootobject_TFMdj
    {
        public string status { get; set; }
        public Result result { get; set; }
    }

    public class Result
    {
        public Slot slot { get; set; }
        public Dj dj { get; set; }
    }

    public class Slot
    {
        public string id { get; set; }
        public string dj { get; set; }
        public string about { get; set; }
        public string timestart { get; set; }
        public string timeend { get; set; }
        public string cover { get; set; }
        public string perm { get; set; }
    }

    public class Dj
    {
        public string id { get; set; }
        public string name { get; set; }
        public string twitter { get; set; }
        public string facebook { get; set; }
        public string instagram { get; set; }
        public string youtube { get; set; }
        public string twitch { get; set; }
        public string avatar { get; set; }
    }
    public class Rootobject_TFMsong
    {
        public int id { get; set; }
        public string artist { get; set; }
        public string title { get; set; }
        public int playcount { get; set; }
        public string created_at { get; set; }
        public int updated_at { get; set; }
        public string art { get; set; }
        public string link { get; set; }
    }
}
