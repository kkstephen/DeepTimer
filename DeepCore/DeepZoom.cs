using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace DeepCore
{
    public class DeepZoom
    {
        public event EventHandler<EventArgs> OnUpdate;

        public IList<DeepMatch> Teams { get; set; }
        
        public DeepZoom()
        {
            this.Teams = new List<DeepMatch>();
        }

        public List<DeepMatch> Decode(string str)
        {
            return str.Deserialize<List<DeepMatch>>();
        }

        public void Refresh(IList<DeepMatch> data)
        {
            this.Teams.Clear();
            
            this.Teams.AddRange(data);

            if (OnUpdate != null)
            {
                OnUpdate(this, new EventArgs());
            }
        }
    }
}
