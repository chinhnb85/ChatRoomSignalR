using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lisb.Entity
{
    public class Room
    {
        public Room()
        {
            RoomId = Guid.NewGuid().ToString();
            Users=new List<User>();
        }

        public string RoomId { get; set; }
        public string InitiatedBy { get; set; }
        public string InitiatedTo { get; set; }
        public List<User> Users { get; set; } 
    }
}
