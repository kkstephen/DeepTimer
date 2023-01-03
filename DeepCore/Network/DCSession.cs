using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using CFS.Net;

namespace DeepCore
{
    public class DCSession : CFSession, ICFSession
    {
        private static readonly string CRLF = "\r\n";
        public string WelcomeMessage { get; set; }
         
        private string message;
        private string clientName;
        private string json;
 
        private DeepManager dm; 
        
        public DCSession(Socket socket) : base(socket)
        {
            this.ID = StringHelper.GetGUID();

            this.dm = DeepManager.Instance; 
        }

        public void Begin()
        {
            this.welcome();

            while (this.IsAlive)
            {
                try
                {
                    this.message = this.Receive();

                    switch (this.message)
                    {
                        case "RACE":
                            this.ranking();
                            break;


                        case "QUIT":
                            this.End();
                            break;

                        default:
                            this.sendLine("Undefine command.");
                            break;
                    }
                }
                catch (CFException ce)
                {
                    this.sendLine(ce.Message);
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;

                    if (ex is SocketException)
                    {
                        var sx = ex as SocketException;

                        if (sx.ErrorCode == 10060)
                        {
                            msg = "Connection timeout.";
                        }
                    }

                    CFErrorEventArgs err = new CFErrorEventArgs("[" + this.clientName + "] Error: " + msg);

                    this.onSocketError(err);

                    break;
                } 
            }

            this.Close();
        }

        public void welcome()
        {
            this.sendLine(this.WelcomeMessage); 

            this.clientName = this.Receive();

            this.sendLine("OK");
        }

        private void ranking()
        { 
            this.json = this.dm.Ranklist.Serialize();

            string data = StringHelper.GZip(this.json);
            
            this.sendLine(data);
        } 

        private void sendLine(string data)
        {
            this.Send(data + CRLF);
        }
    }
}
