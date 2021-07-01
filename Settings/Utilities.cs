using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TN.Mobike.ToolLock.Settings
{
    class Utilities
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("TN.Mobike.LookOmniServices");

        public static bool ActiveOperationLog = true;
        public static bool ActiveDebugLog = true;
        public static long ConvertToUnixTime(DateTime datetime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

            return (long)(datetime - sTime).TotalSeconds;
        }

        public static string UnixTimeStampToDateTime(string unixTimeStamp)
        {
            try
            {
                var tsp = Convert.ToDouble(unixTimeStamp);
                // Unix timestamp is seconds past epoch
                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
                dtDateTime = dtDateTime.AddSeconds(tsp).ToLocalTime();
                return dtDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception)
            {
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public static string UtcToLocalTime(string utcTime)
        {
            var hour = Convert.ToDouble(utcTime.Substring(0, 2));
            var minutes = Convert.ToDouble(utcTime.Substring(2, 2));
            var second = Convert.ToDouble(utcTime.Substring(4, 2));

            var time = DateTime.Now.AddHours(hour).AddMinutes(minutes).AddSeconds(second).ToLocalTime();



            return "";
        }

        public static double getLatitudeLongitude(string LatitudeLongitude, bool isLat = true)
        {
            try
            {
                if (String.IsNullOrEmpty(LatitudeLongitude))
                {
                    return 0;
                }
                else
                {
                    double dd = 0;
                    double mm = 0;
                    if (isLat)
                    {
                        dd = Convert.ToDouble(LatitudeLongitude.Substring(0, 2));
                        mm = Convert.ToDouble(LatitudeLongitude.Substring(2, LatitudeLongitude.Length - 2));
                    }
                    else
                    {
                        dd = Convert.ToDouble(LatitudeLongitude.Substring(0, 3));
                        mm = Convert.ToDouble(LatitudeLongitude.Substring(3, LatitudeLongitude.Length - 3));
                    }

                    double data = dd + (mm / 60);
                    return data;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static void StartLog(bool activeOperationLog, bool activeDebugLog)
        {
            ActiveOperationLog = activeOperationLog;
            ActiveDebugLog = activeDebugLog;
        }

        public static void CloseLog()
        {
            foreach (log4net.Appender.IAppender app in log.Logger.Repository.GetAppenders())
            {
                app.Close();
            }
        }

        public static void WriteErrorLog(string logtype, string logcontent)
        {
            try
            {
                log.Error($"{logtype} \t {logcontent}");
            }
            catch
            {
                // ignored
            }
        }

        public static void WriteOperationLog(string logtype, string logcontent)
        {
            if (!ActiveOperationLog)
                return;

            try
            {
                log.Info($"{logtype} \t {logcontent}");
            }
            catch
            {
                // ignored
            }
        }

        public static void WriteDebugLog(string logtype, string logcontent)
        {
            if (!ActiveDebugLog)
                return;

            try
            {
                log.Debug($"{logtype} \t {logcontent}");
            }
            catch
            {
                // ignored
            }
        }

        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public static string GetBitStr(byte[] data)
        {
            BitArray bits = new BitArray(data);

            string strByte = string.Empty;
            for (int i = 0; i <= bits.Count - 1; i++)
            {
                if (i % 8 == 0)
                {
                    strByte += " ";
                }
                strByte += (bits[i] ? "1" : "0");
            }

            return strByte;
        }

        public static bool ValidateIPv4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }


        public static void WriteFile(string data, string fileName)
        {
            var path = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(path, fileName);
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            File.WriteAllText(filePath, $"{data}");
        }

        public static int GetCount(string fileName)
        {
            var path = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(path, fileName);
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            var data = File.ReadAllText(filePath);

            return string.IsNullOrEmpty(data) ? 0 : Convert.ToInt32(data);
        }

        public static List<string> ReadFileMapping(string fileName)
        {
            var path = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(path, fileName);
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            var data = File.ReadAllLines(filePath).ToList();

            return data;
        }

        public static DateTime ReadLastTime(string fileName)
        {
            var path = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(path, fileName);
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            DateTime lastTime = DateTime.Now.AddDays(-2);
            var data = File.ReadAllLines(filePath).ToList();
            if (data.Count > 0)
            {
                lastTime = DateTime.ParseExact(data.FirstOrDefault(), "yyyy-MM-dd HH:mm:ss.fff",
                    CultureInfo.CurrentCulture);
            }

            return lastTime;
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static DateTime ConvertDateTime(string time)
        {
            try
            {
                var dateTime = DateTime.ParseExact(time, "yyMMddHHmmss", CultureInfo.CurrentCulture);
                if (dateTime == DateTime.MinValue)
                {
                    dateTime = DateTime.Now;
                }
                return dateTime;
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }

        public static double CheckPercent(string voltage)
        {
            try
            {
                var vol = Convert.ToDouble(voltage);
                var percent = Math.Round(((vol / AppSettings.TotalPin) * 100), 3);

                return percent;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static string CheckGMS(string gms)
        {
            var gmsInt = Convert.ToInt32(gms);
            var gmsStr = "Tín hiệu yếu";
            if (gmsInt <= 8)
            {
                gmsStr = "Tín hiệu yếu";
            }
            else if (gmsInt > 8 && gmsInt <= 16)
            {
                gmsStr = "Tín hiệu trung bình";
            }
            else if (gmsInt > 16 && gmsInt <= 24)
            {
                gmsStr = "Tín hiệu tạm ổn";
            }
            else
            {
                gmsStr = "Tín hiệu khỏe";
            }

            return gmsStr;
        }

        public static void SetInvoke(Label label, string txt)
        {
            if (label.InvokeRequired)
            {
                label.BeginInvoke((MethodInvoker)delegate () { label.Text = $"{txt}"; });
            }
            else
            {
                label.Text = $"{txt}";
            }
        }

        public static void SetInvokeBtn(Button button, Color colorBg, bool isEnable, string text, Color colorTxt)
        {
            if (button.InvokeRequired)
            {
                button.BeginInvoke((MethodInvoker) delegate()
                {
                    button.BackColor = colorBg;
                    button.ForeColor = colorTxt;
                    button.Text = text;
                    button.Enabled = isEnable;
                });
            }
            else
            {
                button.BackColor = colorBg;
                button.ForeColor = colorTxt;
                button.Text = text;
                button.Enabled = isEnable;
            }
        }

        public static void SetInvokeRTB(RichTextBox status, string text, Color color)
        {
            if (status.InvokeRequired)
            {
                status.BeginInvoke((MethodInvoker) delegate()
                {
                    ExportInformation(status, text, color);
                });
            }
            else
            {
                ExportInformation(status, text, color);
            }
        }

        public static void ExportInformation(RichTextBox status, string text, Color color)
        {
            status.Clear();
        }
    }
}
