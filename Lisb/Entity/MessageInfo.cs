using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lisb.Entity
{
    public class MessageInfo
    {
        public string UserName { get; set; }

        public string Message { get; set; }

        public string UserGroup { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime MsgDate { get; set; }
    }
}
