using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lisb.Entity
{
    public class User
    {
        public User()
        {
            RoomIds=new List<string>();
        }
        public string Id { get; set; }
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
        public string UserGroup { get; set; }
        public int AdminCode { get; set; }
        //if Freeflag==0 ==> Busy
        //if Freeflag==1 ==> Free
        public int Freeflag { get; set; }
        //if Tpflag==2 ==> User Admin
        //if Tpflag==0 ==> User Member
        //if Tpflag==1 ==> Admin
        public int Tpflag { get; set; }        
        public int AdminId { get; set; }
        public List<string> RoomIds { get; set; } 
    }
}
