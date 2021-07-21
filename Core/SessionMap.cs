using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mina.Core.Buffer;
using Mina.Core.Session;
using TN.Mobike.ToolLock.Settings;

namespace TN.Mobike.ToolLock.Core
{
    class KeySession
    {
        public long Key { get; set; }
        public IoSession Session { get; set; }

        public KeySession()
        {
            
        }

        public KeySession(long key, IoSession session)
        {
            Key = key;
            Session = session;
        }

        public override string ToString()
        {
            return $"Key:{Key} | Session: {Session.RemoteEndPoint}";
        }
    }
    class SessionMap
    {
        private static SessionMap sessionMap = null;
        public static readonly int STATUS_SEND_SESSION_NULL = 0;
        public static readonly int STATUS_SEND_SUCCESSFULLY = 1;
        public static readonly int STATUS_SEND_FAILING = 2;
        private static KeySession keySession = new KeySession();
        public static Dictionary<long, IoSession> map = new Dictionary<long, IoSession>();

        public static List<KeySession> listKey = new List<KeySession>();
        
        public static List<string> List = new List<string>();

        public SessionMap()
        {
        }

        public static SessionMap NewInstance()
        {
            return sessionMap ?? (sessionMap = new SessionMap());
        }


        public void AddSession(long key, IoSession session)
        {
            try
            {
                RichTextBox rtb = Form1.RtbMessage;

                if (!map.TryGetValue(key, out var value))
                {
                    map.Add(key, session);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"KEY ADD SESSION = {key} - SUCCESSFUL !");
                    Console.ForegroundColor = ConsoleColor.White;

                    keySession = new KeySession(key, session);

                    listKey.Add(keySession);

                    List.Add($"{key}");

                    Console.WriteLine(keySession.ToString());

                    //Form1.ListBoxP.Items.AddRange(listKey.ToArray());

                    Utilities.SetInvokeRTB(rtb, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : KEY ADD SESSION = {key} - SUCCESSFUL !", Color.Green);

                    Utilities.WriteOperationLog("MinaOmni.AddSession", $"Imei: {key} | Add session has this key Successful !");
                }
                else
                {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"KEY ADD SESSION = {key} - EXIST");
                    Console.ForegroundColor = ConsoleColor.White;

                    return;
                }
            }
            catch (Exception e)
            {
                Utilities.WriteErrorLog("MinaOmni.AddSession", $"Imei: {key} | Error : {e.Message}");
            }
        }

        public IoSession GetSession(long key)
        {
            map.TryGetValue(key, out var session);

            return session;
        }

        public bool ConstantKey(long key)
        {
            return map.ContainsKey(key);
        }

        public void RemoveSession(IoSession session)
        {
            long key = 0;
            try
            {
                key = map.FirstOrDefault(x => x.Value == session).Key;

                if (key != 0)
                {
                    map.Remove(key);
                    Utilities.WriteOperationLog("MinaOmni.RemoveSession", $"Imei: {key} | Remove session successful !");
                }
                else
                {
                    Utilities.WriteOperationLog("MinaOmni.RemoveSession", $"Imei: {key} | Session has not exist with key !");
                }
            }
            catch (Exception e)
            {
                Utilities.WriteErrorLog("MinaOmni.RemoveSession", $"Imei: {key} | Error : {e.Message}");
            }
        }

        public int SendMessage(long key, string message, bool isDebug = false)
        {
            //IoSession session = GetSession(key);

            //if (session == null)
            //{

            //    Console.WriteLine($"IMEI: {key} has not connected the service");


            //    Utilities.WriteErrorLog("MinaOmni.Unlock.1", $"Imei: {key} | Error: This lock has not connected the services !");
            //    return STATUS_SEND_SESSION_NULL;
            //}

            //if (isDebug)
            //{
            //    Console.WriteLine($"DEBUG write IP = {session.RemoteEndPoint.Serialize().Family}");
            //    Console.WriteLine($"DEBUG write IP = {session.RemoteEndPoint.AddressFamily}");
            //}

            //session.Write(message);
            //return STATUS_SEND_SUCCESSFULLY;

            var command = MinaSocket.AddBytes(new byte[] { (byte)0xFF, (byte)0xFF }, Encoding.ASCII.GetBytes(message));

            Console.WriteLine(Encoding.ASCII.GetString(command));

            return SendMessageArray(key, command, isDebug);
        }

        public int SendMessageArray(long key, byte[] messBytes, bool isDebug = false)
        {
            IoSession session = GetSession(key);
            if (session == null)
            {
                Console.WriteLine($"IMEI: {key} has not connected the service");
                Utilities.WriteErrorLog("MinaOmni.Unlock.1", $"Imei: {key} | Error: This lock has not connected the services !");

                return STATUS_SEND_SESSION_NULL;
            }

            if (isDebug)
            {
                Console.WriteLine($"DEBUG write IP = {session.RemoteEndPoint.Serialize().Family}");
                Console.WriteLine($"DEBUG write IP = {session.RemoteEndPoint.AddressFamily}");
            }

            session.Write(IoBuffer.Wrap(messBytes));
            return STATUS_SEND_SUCCESSFULLY;
        }

        public int SendMessage(long key, byte[] messBytes, bool isDebug = false)
        {
            return SendMessageArray(key, messBytes, isDebug);
        }
    }
}
