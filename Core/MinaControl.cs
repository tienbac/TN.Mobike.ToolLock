using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mina.Core.Service;
using Mina.Core.Session;
using Mina.Filter.Codec;
using Mina.Filter.Codec.TextLine;
using Mina.Filter.Executor;
using Mina.Filter.Logging;
using Mina.Transport.Socket;
using TN.Mobike.ToolLock.Settings;

namespace TN.Mobike.ToolLock.Core
{
    class MinaControl
    {
        private static readonly int PORT = AppSettings.PortService;
        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static IoAcceptor acceptor;

        private static IPAddress ipAddress;

        public static void StartServer(Button btnConnect, RichTextBox rtbStatus, Button btnDisconnect, RichTextBox rtb)
        {
            try
            {
                acceptor = new AsyncSocketAcceptor();

                acceptor.FilterChain.AddLast("logger", new LoggingFilter());
                acceptor.FilterChain.AddLast("codec", new ProtocolCodecFilter(new TextLineCodecFactory(Encoding.UTF8)));

                acceptor.ExceptionCaught +=
                    (o, e) =>
                    {

                        

                        Utilities.WriteErrorLog("MinaControl.StartServer", $"Error: {e.Exception}");
                    };

                acceptor.SessionIdle +=
                    (o, e) =>
                    {

                        Console.WriteLine("IDLE " + e.Session.GetIdleCount(e.IdleStatus));

                        Utilities.WriteOperationLog("MinaControl.StartServer", $"IDLE: {e.Session.GetIdleCount(e.IdleStatus)}");
                    };

                acceptor.FilterChain.AddLast("exceutor", new ExecutorFilter());
                acceptor.Handler = new MinaSocket();
                acceptor.SessionConfig.ReadBufferSize = 2048;
                acceptor.SessionConfig.SetIdleTime(IdleStatus.ReaderIdle, 10 * 60);

                string hostName = Dns.GetHostName();

                ipAddress = IPAddress.Parse(AppSettings.HostService);
                //var ipAddress = Dns.GetHostByName(hostName).AddressList[0];

                acceptor.Bind(new IPEndPoint(ipAddress, PORT));

                Utilities.SetInvokeBtn(btnConnect, Color.Green, false, "CONNECTED", Color.White);
                Utilities.SetInvokeBtn(btnDisconnect, Color.DarkGray, true, "DISCONNECT",Color.Black);

                Utilities.SetInvokeRTB(rtb, $"Start on: {ipAddress} | Listening on port: {PORT}", Color.Green, true);

                Utilities.WriteOperationLog("MinaOmni.StartServer", $"Start on: {ipAddress} | Listening on port: {PORT}");
            }
            catch (Exception e)
            {
                Utilities.WriteErrorLog("MinaOmni.StartServer", $"Listening on port: {PORT} | Error: {e.Message}");
            }
        }

        public static void StopServer(Button btnDisconnect, Button btnConnect, RichTextBox rtb)
        {
            try
            {
                acceptor?.Unbind();

                Utilities.SetInvokeBtn(btnDisconnect, Color.Red, false, "DISCONNECTED", Color.White);
                Utilities.SetInvokeBtn(btnConnect, Color.DarkGray, true, "CONNECT", Color.Black);

                Utilities.SetInvokeRTB(rtb, $"Stop sv: {ipAddress} | Stop on port: {PORT}", Color.Red, true);

                Utilities.WriteOperationLog("MinaOmni.StopServer", $"Stop server Mina Omni successful !");
            }
            catch (Exception e)
            {
                Utilities.WriteErrorLog("MinaOmni.StopServer", $"Error: {e.Message}");
            }
        }

        public static bool UnLock(string imei)
        {
            var key = "L0";
            int uid = 0;
            long timestamp = (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
            var message = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},{key},0,{uid},{timestamp}#";

            //var message = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},D0#<LF>";
            //var message = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},S5#<LF>";
            //var message = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},S8,5,0#<LF>";
            //var message = $"*CMDS,XX,{imei},{DateTime.Now:yyMMddHHmmss},G0#<LF>";
            //var message = $"*CMDS,XX,{imei},{DateTime.Now:yyMMddHHmmss},W0#<LF>";
            //var message = $"*CMDS,XX,{imei},{DateTime.Now:yyMMddHHmmss},I0#<LF>";x

            var command = MinaSocket.AddBytes(new byte[] { (byte)0xFF, (byte)0xFF }, Encoding.ASCII.GetBytes(message));

            var result = SessionMap.NewInstance().SendMessage(Convert.ToInt64(imei), command, false);
            //var result = SessionMap.NewInstance().SendMessage(Convert.ToInt64(imei), message, false);

            var status = result == 1 ? $"Send message {message} successful !" : $"Send message {message} fail !";


            Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Message : {message}");
            Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : {status}");
            Console.WriteLine("====================================================================================================");


            Utilities.WriteOperationLog("MinaOmni.Unlock", $"Imei: {imei} | Status: {status}");
            return Convert.ToBoolean(result);
        }

        public static bool UnLock(RichTextBox rtb, string key,string imei, string messageIn, bool check = true)
        {
            //var imei = "861123053530935";
            int uid = 0;
            long timestamp = (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;

            var message = "";

            switch (key.ToUpper())
            {
                case "L0": // MỞ KHÓA
                    message = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},L0,0,{uid},{timestamp}#<LF>\n";
                    break;
                case "D0": // KIỂM TRA VỊ TRÍ
                    //message = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},D0#<LF>\n";
                    message = $"*CMDS,OM,{imei},000000000000,D0#<LF>\n";
                    break;
                case "D1": // CÀI ĐẶT KHÓA TỰ ĐỘNG TRACKING VỊ TRÍ
                    //message = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},D1,0#<LF>\n";
                    message = $"*CMDS,OM,{imei},000000000000,D1,{AppSettings.TimeTrackingLocation}#<LF>\n";
                    break;
                case "S5": // KIỂM TRA THÔNG TIN KHÓA
                    message = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},S5#<LF>\n";
                    break;
                case "S8": // TÌM KIẾM XE ĐẠP
                    message = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},S8,5,0#<LF>\n";
                    break;
                case "G0": // KIỂM TRA PHIÊN BẢN PHẦN MỀM
                    message = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},G0#<LF>\n";
                    break;
                case "I0": // NHẬN SỐ ICCID ĐÃ GẮN TRÊN SIM
                    message = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},I0#<LF>\n";
                    break;
                case "M0": // NHẬN ĐỊA CHỈ MAC BLUETOOTH
                    message = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},M0#<LF>\n";
                    break;
                case "S0": // TẮT THIẾT BỊ
                    message = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},S0#<LF>\n";
                    break;
                case "S1": // KHỞI ĐỘNG LẠI THIẾT BỊ
                    message = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},S1#<LF>\n";
                    break;
                case "C0": // MỞ KHÓA BẰNG MÃ RFID
                    message = $"*CMDR ,OM,{imei},{DateTime.Now:yyMMddHHmmss},C0,0,0,000000001A2B3C4D#<LF>\n";
                    break;
                case "C1": // CÀI ĐẶT MÃ SỐ CHO KHÓA
                    message = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},C1,0,000000001A2B3C4D#<LF>\n";
                    break;
                default:
                    message = messageIn;
                    break;
            }

            Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Message : {message}");

            Utilities.SetInvoke(Form1.lblMessageP, message);

            var command = MinaSocket.AddBytes(new byte[] { (byte)0xFF, (byte)0xFF }, Encoding.ASCII.GetBytes(message));

            var result = SessionMap.NewInstance().SendMessage(Convert.ToInt64(imei), command, false);
            //var result = SessionMap.NewInstance().SendMessage(Convert.ToInt64(imei), message, false);

            Console.WriteLine(result == 1 ? $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : STATUS : Success | {message}" : $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : STATUS : Fail | {message}");

            if (result == 1)
            {
                Utilities.SetInvokeRTB(rtb, $"Send OK message: {message}", Color.Green);
            }
            else
            {
                Utilities.SetInvokeRTB(rtb, $"Send FAIL message: {message}", Color.Red);
            }

            Utilities.WriteOperationLog("Send", result == 1 ? $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : STATUS : Success | {message}" : $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : STATUS : Fail | {message}");
            Console.WriteLine("====================================================================================================");
            return Convert.ToBoolean(result);
        }
    }
}
