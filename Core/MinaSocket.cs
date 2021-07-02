using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mina.Core.Service;
using Mina.Core.Session;
using TN.Mobike.ToolLock.Settings;

namespace TN.Mobike.ToolLock.Core
{
    class MinaSocket : IoHandlerAdapter
    {

        public override void ExceptionCaught(IoSession session, Exception cause)
        {

            Console.WriteLine(cause.ToString());

            Utilities.WriteErrorLog("MinaOmni.ExceptionCaught", $"Error : {cause.Message}");
        }

        public override void MessageReceived(IoSession session, object message)
        {
            try
            {
                string msg = message.ToString();


                Console.WriteLine($"Message : {msg}");

                Utilities.WriteOperationLog("MinaOmni.MessageReceived", $"Message : {msg}");

                if (msg != null && msg.Length > 10)
                {
                    if (msg.Contains("*CMDR") && msg.Contains("#"))
                    {
                        int firstIndex = msg.IndexOf("#", StringComparison.Ordinal);
                        int lastIndex = msg.LastIndexOf("#", StringComparison.Ordinal);

                        if (firstIndex == lastIndex)
                        {
                            int startIndex = msg.IndexOf("*CMDR", StringComparison.Ordinal);
                            string order = msg.Substring(startIndex, lastIndex + 1);
                            HandCommand(session, order);
                        }
                        else
                        {
                            string[] strMsg = msg.Split('#');
                            foreach (var m in strMsg)
                            {
                                int startIndex = m.IndexOf("*CMDR", StringComparison.Ordinal);
                                string oneComm = m.Substring(startIndex, m.Length);
                                HandCommand(session, $"{oneComm}#");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utilities.WriteErrorLog("MinaOmni.MessageReceived", $"Error : {e.Message}");
            }
        }

        public override void SessionIdle(IoSession session, IdleStatus status)
        {
            try
            {

                Console.WriteLine($"IDLE : {session.GetIdleCount(status)}");

                if (status == IdleStatus.ReaderIdle)
                {
                    Console.WriteLine("READER IDLE");

                }
                else if (status == IdleStatus.WriterIdle)
                {
                    Console.WriteLine("WRITER IDLE");
                }
                else
                {
                    Console.WriteLine("BOTH IDLE");
                }


                Utilities.WriteOperationLog("MinaOmni.SessionIdle", $"Status : {status.ToString()}");
            }
            catch (Exception e)
            {
                Utilities.WriteErrorLog("MinaOmni.SessionIdle", $"Error : {e.Message}");
            }
        }

        public new void InputClosed(IoSession session)
        {
            SessionMap.NewInstance().RemoveSession(session);
            InputClosed(session);
        }

        public override void SessionClosed(IoSession session)
        {
            session.CloseOnFlush();
        }

        public void HandCommand(IoSession session, string command)
        {
            RichTextBox Rtbstatus = Form1.RtbMessage;

            if (command.Length <= 1) return;

            var comm = command.Split(',').ToList();
            var comCode = comm[4];
            var imei = comm[2];
            var imeiL = Convert.ToInt64(imei);


            Console.WriteLine($"MESSAGE FROM IMEI: {imeiL} | {command}");


            SessionMap.NewInstance().AddSession(imeiL, session);

            var message = "";
            int status = 0;
            var pin = "0 %";
            var time = "";
            var gms = "";
            var timeActive = "";

            Utilities.WriteOperationLog("MinaOmni.HandCommand", $"Imei: {imei} | {command}");

            switch (comCode.ToUpper())
            {
                //
                case "Q0":

                    // *CMDR ,OM,123456789123456,200318123020,Q0,412#


                    time = $"{Utilities.ConvertDateTime(comm[3]):yyyy-MM-dd HH:mm:ss}";
                    pin = $"{Utilities.CheckPercent(comm[5].Replace("#", ""))} %";


                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Q0 - SIGN IN COMMAND");
                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Q0 - IMEI = {imei}");
                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER Q0 : {command}");


                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Thời gian : {time}");
                    Console.WriteLine($"Pin       : {pin}");
                    Console.ForegroundColor = ConsoleColor.White;

                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Q0 - SIGN IN COMMAND", Color.Blue);
                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Q0 - IMEI = {imei}", Color.Blue);
                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER Q0 : {command}", Color.Blue);
                    Utilities.SetInvokeRTB(Rtbstatus, "Thời gian : {time}", Color.Blue);
                    Utilities.SetInvokeRTB(Rtbstatus, $"Pin       : {pin}", Color.Blue);
                    Utilities.SetInvokeRTB(Rtbstatus, $"===========================================================================", Color.Black);

                    Utilities.WriteOperationLog("MinaOmni.HandCommand", $"Imei: {imei} | Q0 - SIGN IN COMMAND | Thời gian : {time} | Pin : {pin}");

                    break;
                case "H0":

                    // *CMDR ,OM,123456789123456,200318123020,H0,0,412,28#

                    time = $"{Utilities.ConvertDateTime(comm[3]):yyyy-MM-dd HH:mm:ss}";
                    pin = $"{Utilities.CheckPercent(comm[6])} %";
                    gms = Utilities.CheckGMS(comm[7].Replace("#", ""));


                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : H0 - HEARTBEAT COMMAND");
                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER H0 : {command}");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Thời gian : {time}");
                    Console.WriteLine(comm[5] == "0" ? $"Trạng thái : Đã mở khóa" : $"Trạng thái : Đã khóa");
                    Console.WriteLine($"Pin : {pin}");
                    Console.WriteLine($"Tín hiệu : {gms}");
                    Console.ForegroundColor = ConsoleColor.White;

                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : H0 - HEARTBEAT COMMAND", Color.Crimson);
                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER H0 : {command}", Color.Crimson);
                    Utilities.SetInvokeRTB(Rtbstatus, $"Thời gian : {time}", Color.Crimson);
                    Utilities.SetInvokeRTB(Rtbstatus, comm[5] == "0" ? $"Trạng thái : Đã mở khóa" : $"Trạng thái : Đã khóa", Color.Crimson);
                    Utilities.SetInvokeRTB(Rtbstatus, $"Pin : {pin}", Color.Crimson);
                    Utilities.SetInvokeRTB(Rtbstatus, $"Tín hiệu : {gms}", Color.Crimson);
                    Utilities.SetInvokeRTB(Rtbstatus, $"===========================================================================", Color.Black);

                    Utilities.WriteOperationLog("MinaOmni.HandCommand", $"Imei: {imei} | H0 - HEARTBEAT COMMAND | Thời gian : {time} | Pin : {pin} % | Lock (1: Lock \\ 0: Unlock) : {comm[5]} | GMS : {gms}");


                    break;
                case "L1":

                    // Receiver : *CMDR ,OM,123456789123456,200318123020,L1,1234,1497689816,3#

                    message = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},Re,L1#<LF>\n";
                    status = SessionMap.NewInstance().SendMessage(imeiL, message, false);

                    var messageD1Off = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},D1,0#<LF>\n";
                    var statusD1Off = SessionMap.NewInstance().SendMessage(imeiL, messageD1Off, false);

                    var messageS5 = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},S5#<LF>\n";
                    var statusS5 = SessionMap.NewInstance().SendMessage(imeiL, messageS5, false);



                    time = $"{Utilities.ConvertDateTime(comm[3]):yyyy-MM-dd HH:mm:ss}";
                    timeActive = Utilities.UnixTimeStampToDateTime(comm[6]);
                    var cycle = $"{comm[7].Replace("#", "")} phút";


                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : L1 -  LOCK COMMAND");
                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER L1 : {command}");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Thời gian : {time}");
                    Console.WriteLine($"USER ID   : {comm[5]}");
                    Console.WriteLine($"Thời gian hoạt động : {timeActive}");
                    Console.WriteLine($"Thời gian chu kỳ : {cycle}");
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.WriteLine($"Send message turn off tracking location status: {statusD1Off}");
                    Console.WriteLine($"Send message check lock status: {statusS5}");

                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RESEND : {message ?? "No Message"}");
                    Console.WriteLine(status == 1 ? $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : STATUS : Success" : $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : STATUS : Fail");

                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : L1 -  LOCK COMMAND", Color.DarkGreen);
                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER L1 : {command}", Color.DarkGreen);
                    Utilities.SetInvokeRTB(Rtbstatus, $"Thời gian : {time}", Color.DarkGreen); 
                    Utilities.SetInvokeRTB(Rtbstatus, $"USER ID   : {comm[5]}", Color.DarkGreen);
                    Utilities.SetInvokeRTB(Rtbstatus, $"Thời gian hoạt động : {timeActive}", Color.DarkGreen);
                    Utilities.SetInvokeRTB(Rtbstatus, $"Thời gian chu kỳ : {cycle}", Color.DarkGreen);
                    Utilities.SetInvokeRTB(Rtbstatus, $"===========================================================================", Color.Black);


                    Utilities.WriteOperationLog("MinaOmni.HandCommand", $"Imei: {imei} | L1 - LOCK COMMAND | Thời gian : {time} | User : {comm[5]} | Thời gian hoạt động : {timeActive} | Thời gian đi (Phút) : {cycle} | Định vị tự động : Tắt - {statusD1Off} | Gửi kiểm tra trạng thái : {statusS5}");
                    break;
                case "L0":

                    // Receiver : *CMDR ,OM,123456789123456,200318123020,L0,0,1234,1497689816#

                    // Resend
                    message = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},Re,L0#<LF>\n";
                    status = SessionMap.NewInstance().SendMessage(imeiL, message, false);

                    //var messageD1On = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},D1,{AppSettings.TimeTrackingLocation}#<LF>\n";
                    //var statusD1On = SessionMap.NewInstance().SendMessage(imeiL, messageD1On, false);

                    time = $"{Utilities.ConvertDateTime(comm[3]):yyyy-MM-dd HH:mm:ss}";
                    timeActive = $"{Utilities.UnixTimeStampToDateTime(comm[7].Replace("#", ""))}";


                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : L0 - UNLOCK COMMAND");
                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER L0 : {command}");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($" Thời gian : {time}");
                    Console.WriteLine(comm[5] == "0" ? $"Trạng thái : Mở thành công" : $"Trạng thái : Mở lỗi");
                    Console.WriteLine($" Customer ID : {comm[6]}");
                    Console.WriteLine($" Timestamp : {timeActive}");
                    Console.ForegroundColor = ConsoleColor.White;

                    //Console.WriteLine($"Send message turn on tracking location status: {statusD1On}");
                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RESEND : {message ?? "No Message"}");
                    Console.WriteLine(status == 1 ? $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : STATUS : Success" : $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : STATUS : Fail");


                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : L0 - UNLOCK COMMAND", Color.Tomato);
                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER L0 : {command}", Color.Tomato);
                    Utilities.SetInvokeRTB(Rtbstatus, $" Thời gian : {time}", Color.Tomato);
                    Utilities.SetInvokeRTB(Rtbstatus, comm[5] == "0" ? $"Trạng thái : Mở thành công" : $"Trạng thái : Mở lỗi", Color.Tomato);
                    Utilities.SetInvokeRTB(Rtbstatus, $" Customer ID : {comm[6]}", Color.Tomato);
                    Utilities.SetInvokeRTB(Rtbstatus, $" Timestamp : {timeActive}", Color.Tomato);
                    Utilities.SetInvokeRTB(Rtbstatus, $"===========================================================================", Color.Black);


                    //Utilities.WriteOperationLog("MinaOmni.HandCommand", $"Imei: {imei} | L0 - UNLOCK COMMAND | Thời gian : {time} | User : {comm[6]} | Thời gian hoạt động : {timeActive} | Lock (1: Lock \\ 0: Unlock) : {comm[5]} | Định vị tự động : Bật - {statusD1On}");
                    Utilities.WriteOperationLog("MinaOmni.HandCommand", $"Imei: {imei} | L0 - UNLOCK COMMAND | Thời gian : {time} | User : {comm[6]} | Thời gian hoạt động : {timeActive} | Lock (1: Lock \\ 0: Unlock) : {comm[5]}");



                    break;
                case "D0":

                    // *CMDR, OM, 123456789123456, 200318123020, D0, 0,124458.00, A,2237.7514, N,11408.6214, E,6, 0.21, 151216,10, M,A#

                    time = $"{Utilities.ConvertDateTime(comm[3]):yyyy-MM-dd HH:mm:ss}";

                    message = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},Re,D0#<LF>\n";
                    status = SessionMap.NewInstance().SendMessage(imeiL, message, false);

                    var data12 = comm[17];
                    var data11 = comm[16];
                    var data10 = comm[15];
                    var data9 = comm[14];
                    var data8 = comm[13];
                    var data7 = comm[12];
                    var data6 = comm[11];
                    var data5 = Utilities.getLatitudeLongitude(comm[10], false);
                    var data4 = comm[9];
                    var data3 = Utilities.getLatitudeLongitude(comm[8]);
                    var data2 = comm[7];
                    var data1 = comm[6];
                    var data0 = comm[5];

                    var location = $@"https://maps.google.com?q={data3},{data5}";


                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : D0 - POSITIONING COMMAND");
                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER D0 : {command}");

                    Console.ForegroundColor = ConsoleColor.Yellow;

                    Console.WriteLine($"<1> : {data0}");
                    Console.WriteLine($"<2> : {data1} (hhmmss / UTC)");

                    if (data2 == "V" || data2 == "VA")
                    {
                        Console.WriteLine($"<3> : Location status : {data2} = Định vị không hoạt động");
                    }
                    else if (data2 == "A")
                    {
                        Console.WriteLine($"<3> : Location status : {data2} = Định vị hoạt động");
                    }

                    Console.WriteLine($"<4> : {data3} | Vĩ độ ddmm.mmmm");
                    Console.WriteLine($"<5> : {data4} | N (Bán cầu Bắc) | S (Bán cầu Nam)");
                    Console.WriteLine($"<6> : {data5} | Kinh độ dddmm.mmmm");
                    Console.WriteLine($"<7> : {data6} | E (Đông) | W (Tây)");
                    Console.WriteLine($"<8> : {data7} | Vận tốc");
                    Console.WriteLine($"<9> : {data8} | HDOP độ chính sác định vị");
                    Console.WriteLine($"<10> : {data9} | Ngày UTC | ddmmyy");
                    Console.WriteLine($"<11> : {data10} | Độ cao  so với mực nước biển");
                    Console.WriteLine($"<12> : {data11} | Đơn vị chiều cao mét");

                    if (data12 == "N#")
                    {
                        Console.WriteLine("<13> : N = Dữ liệu định vị không hợp lệ");
                    }
                    else if (data12 == "A#")
                    {
                        Console.WriteLine("<13> : A = Định vị tự động");
                    }
                    else if (data12 == "D#")
                    {
                        Console.WriteLine("<13> : D = Khác");
                    }
                    else
                    {
                        Console.WriteLine("<13> : E = Ước lượng");
                    }

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RESEND : {message ?? "No Message"}");
                    Console.WriteLine(status == 1 ? $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : STATUS : Success" : $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : STATUS : Fail");


                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : D0 - POSITIONING COMMAND", Color.Chocolate);
                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER D0 : {command}", Color.Chocolate);
                    Utilities.SetInvokeRTB(Rtbstatus, $"Imei: {imei} | D0 - POSITIONING COMMAND | Thời gian: {time} | Trạng thái định vị (A: Active \\ V|VA: Not Active) : {data2} | Lat (Vĩ Độ): {data3} | Long (Kinh độ): {data5} | {data4} - {data6} | Point HDOP: {data8} | Altitude: {data10} | Mode (A: Định vị tự động\\ D: Khác\\ E: Ước lượng\\ N: Dữ liệu lỗi): {data12} | Resend : {status} | {location}", Color.Chocolate);
                    Utilities.SetInvokeRTB(Rtbstatus, $"===========================================================================", Color.Black);


                    Utilities.WriteOperationLog("MinaOmni.HandCommand", $"Imei: {imei} | D0 - POSITIONING COMMAND | Thời gian: {time} | Trạng thái định vị (A: Active \\ V|VA: Not Active) : {data2} | Lat (Vĩ Độ): {data3} | Long (Kinh độ): {data5} | {data4} - {data6} | Point HDOP: {data8} | Altitude: {data10} | Mode (A: Định vị tự động\\ D: Khác\\ E: Ước lượng\\ N: Dữ liệu lỗi): {data12} | Resend : {status} | {location}");
                    break;

                case "D1":

                    // *CMDR,OM,123456789123456,200318123020,D1,60#
                    time = $"{Utilities.ConvertDateTime(comm[3]):yyyy-MM-dd HH:mm:ss}";


                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : D1 - TRACKING LOCATION COMMAND");
                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER S5 : {command}");
                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Location tracking each : {comm[5].Replace("#", "")} giây");

                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : D1 - TRACKING LOCATION COMMAND", Color.Brown);
                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER S5 : {command}", Color.Brown);
                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : Location tracking each : {comm[5].Replace("#", "")} giây", Color.Brown);
                    Utilities.SetInvokeRTB(Rtbstatus, $"===========================================================================", Color.Black);

                    Utilities.WriteOperationLog("MinaOmni.HandCommand", $"Imei: {imei} | D1 - TRACKING LOCATION COMMAND | Thời gian: {time} | Bật tự động gửi vị trí mỗi {comm[5].Replace("#", "")} giây");



                    break;

                case "S5":

                    // *CMDR,OM,123456789123456,200318123020,S5,412,30,5,0,0#
                    time = $"{Utilities.ConvertDateTime(comm[3]):yyyy-MM-dd HH:mm:ss}";
                    gms = Utilities.CheckGMS(comm[6]);
                    pin = $"{Utilities.CheckPercent(comm[5].Replace("#", ""))} %";


                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : S5 - LOCK STATUS COMMAND");
                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER S5 : {command}");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Pin : {pin}");
                    Console.WriteLine($"Tín hiệu : {gms}");
                    Console.WriteLine($"Chỉ số GPS : {comm[7]}");
                    Console.WriteLine(comm[8] == "0" ? $"Trạng thái : Đang mở khóa" : $"Trạng thái : Đã khóa");
                    Console.ForegroundColor = ConsoleColor.White;

                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : S5 - LOCK STATUS COMMAND", Color.BlueViolet);
                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER S5 : {command}", Color.BlueViolet);
                    Utilities.SetInvokeRTB(Rtbstatus, $"===========================================================================", Color.Black);


                    Utilities.WriteOperationLog("MinaOmni.HandCommand", $"Imei: {imei} | S5 - LOCK STATUS COMMAND | Thời gian: {time} | Pin: {pin} | GMS: {gms} | GPS: {comm[7]} | Lock (1: Lock \\ 0: Unlock): {comm[8]}");



                    break;
                case "S8":

                    // *CMDR,OM,123456789123456,200318123020,S8,8,0#

                    time = $"{Utilities.ConvertDateTime(comm[3]):yyyy-MM-dd HH:mm:ss}";


                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : S8 - RINGING FOR FINDING A BIKE COMMAND");
                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER S8 : {command}");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Thời gian : {time}");
                    Console.WriteLine($"Đổ chuông : {comm[5]} lần");
                    Console.WriteLine($"Reservations : {comm[6]}");
                    Console.ForegroundColor = ConsoleColor.White;

                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : S8 - RINGING FOR FINDING A BIKE COMMAND", Color.Black);
                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER S8 : {command}", Color.Black);
                    Utilities.SetInvokeRTB(Rtbstatus, $"===========================================================================", Color.Black);


                    Utilities.WriteOperationLog("MinaOmni.HandCommand", $"Imei: {imei} | S8 - RINGING FOR FINDING A BIKE COMMAND | Thời gian: {time} | Số lần đổ chuông: {comm[5]} | Reservations: {comm[6]}");


                    break;
                case "G0":

                    // *CMDR,OM,123456789123456,200318123020,G0,XX_110,Jul 4 2018#

                    time = $"{Utilities.ConvertDateTime(comm[3]):yyyy-MM-dd HH:mm:ss}";


                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : G0 - QUERY FIRMWARE VERSION COMMAND");
                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER G0 : {command}");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Thời gian : {time}");
                    Console.WriteLine($"Phiên bản : {comm[5]}");
                    Console.WriteLine($"Thời gian cập nhật : {comm[6].Replace("#", "")}");
                    Console.ForegroundColor = ConsoleColor.White;

                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : G0 - QUERY FIRMWARE VERSION COMMAND", Color.Black);
                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER G0 : {command}", Color.Black);
                    Utilities.SetInvokeRTB(Rtbstatus, $"===========================================================================", Color.Black);


                    Utilities.WriteOperationLog("MinaOmni.HandCommand", $"Imei: {imei} | G0 - QUERY FIRMWARE VERSION COMMAND | Thời gian: {time} | Phiên bản: {comm[5]} | Thời gian cập nhật: {comm[6].Replace("#", "")}");


                    break;
                case "W0":

                    // *CMDR,OM,123456789123456,200318123020,W0,1#

                    message = $"*CMDS,OM,{imei},{DateTime.Now:yyMMddHHmmss},W0#<LF>\n";
                    status = SessionMap.NewInstance().SendMessage(imeiL, message, false);

                    time = $"{Utilities.ConvertDateTime(comm[3]):yyyy-MM-dd HH:mm:ss}";
                    var alarm = comm[5].Replace("#", "");
                    var statusAlarm = "";
                    if (alarm == "1")
                    {
                        statusAlarm = "Báo động : Chuyển động bất hợp pháp";
                    }
                    else if (alarm == "2")
                    {
                        statusAlarm = "Báo động : đổ xe";
                    }
                    else if (alarm == "6")
                    {
                        statusAlarm = "Báo động : Xe được dựng lên";
                    }


                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : W0 - ALARMING COMMAND");
                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER W0 : {command}");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Thời gian : {time}");
                    Console.WriteLine(statusAlarm);

                    Console.ForegroundColor = ConsoleColor.White;

                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RESEND : {message ?? "No Message"}");
                    Console.WriteLine(status == 1 ? $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : STATUS : Success" : $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : STATUS : Fail");

                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : W0 - ALARMING COMMAND", Color.Black);
                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER W0 : {command}", Color.Black);
                    Utilities.SetInvokeRTB(Rtbstatus, $"===========================================================================", Color.Black);

                    Utilities.WriteOperationLog("MinaOmni.HandCommand", $"Imei: {imei} | W0 - ALARMING COMMAND | Thời gian: {time} | Status: {statusAlarm}");

                    //status = SessionMap.NewInstance().SendMessage(imeiL, message, false);
                    //Console.WriteLine($"Resend status: {status}");

                    break;
                case "U0":

                    // *CMDR ,OM,123456789123456,200318123020,U0,A 1,110,1101#
                    time = $"{Utilities.ConvertDateTime(comm[3]):yyyy-MM-dd HH:mm:ss}";


                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : U0 - Detection upgrade/start upgrade");
                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER U0 : {command}");

                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : U0 - Detection upgrade/start upgrade", Color.Black);
                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER U0 : {command}", Color.Black);
                    Utilities.SetInvokeRTB(Rtbstatus, $"===========================================================================", Color.Black);


                    Utilities.WriteOperationLog("MinaOmni.HandCommand", $"Imei: {imei} | U0 - Detection upgrade/start upgrade | Thời gian: {time} | Code: {comm[5]} | Date: {comm[7].Replace("#", "")} | Ver: {comm[6]}");



                    break;

                case "U1":

                    // *CMDR ,OM,123456789123456,200318123020,U1,100,A1#

                    time = $"{Utilities.ConvertDateTime(comm[3]):yyyy-MM-dd HH:mm:ss}";


                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : U1 - Acquisition of upgrade data");
                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER U1 : {command}");

                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : U1 - Acquisition of upgrade data", Color.Black);
                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER U1 : {command}", Color.Black);
                    Utilities.SetInvokeRTB(Rtbstatus, $"===========================================================================", Color.Black);

                    Utilities.WriteOperationLog("MinaOmni.HandCommand", $"Imei: {imei} | U1 - Acquisition of upgrade data | Thời gian: {time} | Packages: {comm[5]} | Code: {comm[6].Replace("#", "")}");


                    break;

                case "U2":

                    // *CMDR ,OM,123456789123456,200318123020,U2,A1,0#
                    time = $"{Utilities.ConvertDateTime(comm[3]):yyyy-MM-dd HH:mm:ss}";


                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : U2 - Notification of upgrade results");
                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER U2 : {command}");

                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : U2 - Notification of upgrade results", Color.Black);
                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER U2 : {command}", Color.Black);
                    Utilities.SetInvokeRTB(Rtbstatus, $"===========================================================================", Color.Black);

                    Utilities.WriteOperationLog("MinaOmni.HandCommand", $"Imei: {imei} | U2 - Notification of upgrade results | Thời gian: {time} | Code: {comm[5]} | Status: {comm[6].Replace("#", "")}");



                    break;
                case "K0":

                    // *CMDR ,OM,123456789123456,200318123020,K0,yOTmK50z #
                    time = $"{Utilities.ConvertDateTime(comm[3]):yyyy-MM-dd HH:mm:ss}";


                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : K0 - set/get BLE 8 byte communication KEY");
                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER U2 : {command}");

                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : K0 - set/get BLE 8 byte communication KEY", Color.Black);
                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER U2 : {command}", Color.Black);
                    Utilities.SetInvokeRTB(Rtbstatus, $"===========================================================================", Color.Black);


                    Utilities.WriteOperationLog("MinaOmni.HandCommand", $"Imei: {imei} | K0 - set/get BLE 8 byte communication KEY | Thời gian: {time} | Key: {comm[5].Replace("#", "")}");



                    break;

                case "I0":

                    // *CMDR,OM,123456789123456,200318123020,I0,123456789AB123456789#

                    time = $"{Utilities.ConvertDateTime(comm[3]):yyyy-MM-dd HH:mm:ss}";
                    var ICCID = comm[5].Replace("#", "");


                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : I0 - OBTAIN SIM CARD ICCID CODE COMMAND");
                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER I0 : {command}");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Thời gian : {time}");
                    Console.WriteLine($"Mã ICCID  : {ICCID}");
                    Console.ForegroundColor = ConsoleColor.White;

                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : I0 - OBTAIN SIM CARD ICCID CODE COMMAND", Color.Black);
                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER I0 : {command}", Color.Black);
                    Utilities.SetInvokeRTB(Rtbstatus, $"===========================================================================", Color.Black);


                    Utilities.WriteOperationLog("MinaOmni.HandCommand", $"Imei: {imei} | I0 - OBTAIN SIM CARD ICCID CODE COMMAND | Thời gian: {time} | ICCID: {ICCID}");



                    break;
                case "M0":

                    // *CMDR,OM,123456789123456,200318123020,M0,12：34：56：78：90：AB#

                    time = $"{Utilities.ConvertDateTime(comm[3]):yyyy-MM-dd HH:mm:ss}";
                    var macAdd = comm[5].Replace("#", "");


                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : M0 - GET LOCK BLUETOOTH MAC ADDRESS");
                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER M0 : {command}");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Thời gian : {time}");
                    Console.WriteLine($"MAC address  : {macAdd}");
                    Console.ForegroundColor = ConsoleColor.White;

                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : M0 - GET LOCK BLUETOOTH MAC ADDRESS", Color.Black);
                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER M0 : {command}", Color.Black);
                    Utilities.SetInvokeRTB(Rtbstatus, $"===========================================================================", Color.Black);


                    Utilities.WriteOperationLog("MinaOmni.HandCommand", $"Imei: {imei} | M0 - GET LOCK BLUETOOTH MAC ADDRESS | Thời gian: {time} | Mac: {macAdd}");


                    break;
                case "C1":

                    // *CMDR ,OM,863725031194523,000000000000,C1,000000001A2B3C4D#
                    time = $"{Utilities.ConvertDateTime(comm[3]):yyyy-MM-dd HH:mm:ss}";
                    var key = comm[5].Replace("#", "");


                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : C1 - Management biked number setting");
                    Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER C1 : {command}");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Thời gian : {time}");
                    Console.WriteLine($"Key  : {key}");
                    Console.ForegroundColor = ConsoleColor.White;

                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : C1 - Management biked number setting", Color.Black);
                    Utilities.SetInvokeRTB(Rtbstatus, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : RECEIVER C1 : {command}", Color.Black);
                    Utilities.SetInvokeRTB(Rtbstatus, $"===========================================================================", Color.Black);


                    Utilities.WriteOperationLog("MinaOmni.HandCommand", $"Imei: {imei} | C1 - Management biked number setting | Thời gian: {time} | Key: {key}");


                    break;
                default:
                    break;
            }

            Console.WriteLine("====================================================================================================");
        }

        public static byte[] AddBytes(byte[] b1, byte[] b2)
        {
            byte[] b = new byte[b1.Length + b2.Length];
            Array.Copy(b1, 0, b, 0, b1.Length);
            Array.Copy(b2, 0, b, b1.Length, b2.Length);

            return b;
        }
    }
}
