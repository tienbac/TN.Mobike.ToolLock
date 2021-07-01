using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mina.Core.Buffer;
using Mina.Core.Session;
using TN.Mobike.ToolLock.Settings;

namespace TN.Mobike.ToolLock.Core
{
    class SessionMap
    {
        private static SessionMap sessionMap = null;
        public static readonly int STATUS_SEND_SESSION_NULL = 0;
        public static readonly int STATUS_SEND_SUCCESSFULLY = 1;
        public static readonly int STATUS_SEND_FAILING = 2;

        public static ListBox listData;

        private Dictionary<long, IoSession> map = new Dictionary<long, IoSession>();

        public SessionMap(ListBox data)
        {
            listData = data;
        }

        public static SessionMap NewInstance()
        {
            return sessionMap ?? (sessionMap = new SessionMap());
        }

        public void AddSession(long key, IoSession session)
        {
            try
            {
                if (!map.TryGetValue(key, out var value))
                {
                    map.Add(key, session);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"KEY ADD SESSION = {key} - SUCCESSFUL !");
                    Console.ForegroundColor = ConsoleColor.White;



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

            session.Write(message);
            return STATUS_SEND_SUCCESSFULLY;
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
