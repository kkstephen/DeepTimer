using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CFS.Net;

namespace DeepCore
{
    public class DeepClient : CFClient
    { 
        public string Json { get; set; } 
        
        private readonly string EOL = "\r\n";
        public string ClientName { get; set; }

        private string data; 

        public DeepClient()      
        { 
        }

        public void Hello()
        {
            data = this.Receive();

            this.sendLine(this.ClientName);

            data = this.Receive();
        }

        public override void KeepAlive()
        { 
            this.sendLine("RACE");

            data = this.Receive();

            this.Json = StringHelper.Unzip(data); 
        } 

        public override void Logout()
        {
            this.sendLine("QUIT");
        }

        public void sendLine(string data)
        {
            this.Send(data + EOL);
        }
    }
}
