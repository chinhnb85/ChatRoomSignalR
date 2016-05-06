using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lisb.Entity
{
    public class OnlineUsers
    {
        public OnlineUsers()
        {
            Users=new List<User>();
        }
        public List<User> Users { get; set; } 
    }
}
