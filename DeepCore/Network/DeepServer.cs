using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CFS.Net;

namespace DeepCore
{
    public class DeepServer : CFServer
    {
        
        public DeepServer()
        {
            this.OnConnect += DeepServer_OnConnect;
        }

        private async void DeepServer_OnConnect(object sender, ClientConnectEventArgs e)
        {
            await this.AcceptSocket(e.EndPoint);
        }

        private async Task AcceptSocket(Socket socket)
        {
            DCSession session = new DCSession(socket);

            session.Timeout = 20;

            session.WelcomeMessage = "DeepTimer Server";

            if (this.Sessions.TryAdd(session.ID, session))
            {
                session.OnOpen += Session_Open;
                session.OnClose += Session_Close;
                session.OnError += Session_OnError;

                session.Start();
            }
        }

        private void Session_OnError(object sender, CFErrorEventArgs e)
        {
            this.Client_Error(this, e);
        }

        private void Session_Open(object sender, SessionOpenEventArgs e)
        {
            var session = sender as DCSession;
          
            Thread th = new Thread(new ThreadStart(session.Begin));
            th.Start();             
        }

        private void Session_Close(object sender, SessionCloseEventArgs e)
        {
            ICFSession session = null;

            if (this.Sessions.TryRemove(e.ID, out session))
            {
                this.Client_Disconnect(new ClientDisconnectEventArgs(session.Host, session.Port));

                if (session != null)
                {
                    var conn = session as DCSession;

                    conn.OnOpen -= Session_Open;
                    conn.OnClose -= Session_Close;
                    conn.OnError -= Session_OnError;

                    conn.Dispose();
                    conn = null;
                }
            }
            else
            {
                this.Server_Error(this, new CFErrorEventArgs("Session remove fail: " + e.ID));
            }
        }
    }
}
