using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lisb.Entity
{
    public class Conversation
    {               
        public string MsgId { get; set; }

        public string Msg { get; set; }

        public DateTime MsgDate { get; set; }

        public string ConversationId { get; set; }

        public string InitiatedBy { get; set; }

        public string InitiatedTo { get; set; }

        public string AppFrom { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string DisplayPrefix
        {
            get { return string.Format("[{0}] {1} ", MsgDate.ToShortTimeString(), FullName); }
        }
    }
}
