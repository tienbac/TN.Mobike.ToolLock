using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TN.Mobike.ToolLock.Settings
{
    class AppSettings
    {
        // SERVICES NAME
        public static string ServiceName = ConfigurationManager.AppSettings["ServiceName"];
        public static string ServiceDisplayName = ConfigurationManager.AppSettings["ServiceDisplayName"];
        public static string ServiceDescription = ConfigurationManager.AppSettings["ServiceDescription"];

        //<!--PORT nhận dữ liệu từ khóa trả về-->
        public static int PortService = Convert.ToInt32(ConfigurationManager.AppSettings["PortService"]);
        public static string HostService = ConfigurationManager.AppSettings["HostService"];

        // <!--Tổng dung lượng pin-->
        public static int TotalPin = Convert.ToInt32(ConfigurationManager.AppSettings["TotalPin"]);

        // <!--Thời gian bật định vị vị trí khóa ( tính theo giây )-->
        public static int TimeTrackingLocation = Convert.ToInt32(ConfigurationManager.AppSettings["TimeTrackingLocation"]);
    }
}
