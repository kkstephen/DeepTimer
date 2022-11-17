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
        public string WelcomeMessage { get; set; }
         
        private string message;
        private string clientName;

        private StringBuilder _sb;
        private DeepManager dm; 
        
        public DCSession(Socket socket) : base(socket)
        {
            this.ID = StringHelper.GetGUID();

            this.dm = DeepManager.Instance;

            this._sb = new StringBuilder();
        }

        public void Begin()
        {
            this.welcome();

            while (this.IsAlive)
            {
                try
                {
                    this.readLine();

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

            this.readLine();

            this.clientName = this.message;

            this.sendLine("OK");
        }

        private void ranking()
        { 
            string json = this.dm.Ranklist.Serialize();

            string data = StringHelper.GZip(json);
            
            this.sendLine(data);
        }

        private void readLine()
        { 
            this.message = this.Receive();
        }

        private void sendLine(string data)
        {
            this._sb.Clear();
            this._sb.AppendLine(data);
            
            this.Send(this._sb.ToString()); 
        }
    }
}
