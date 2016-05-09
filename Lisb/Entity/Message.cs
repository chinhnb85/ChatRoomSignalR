using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lisb.Entity
{
    public class Message
    {
        public string Id { get; set; }

        public string ConversationId { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }
        public string FullName { get; set; }

        public string MessageText { get; set; }

        public string UserGroup { get; set; }

        public DateTime MsgDate { get; set; }

        public string DisplayPrefix
        {
            get { return string.Format("[{0}] {1}: ",MsgDate.ToShortTimeString(),FullName); }            
        }
    }
}
