using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Messages
{
    public class UserPositionSetEvent
    {
        public string UserId { get; set; }
        public string Position {  get; set; }
    }
}
