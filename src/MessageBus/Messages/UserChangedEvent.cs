using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Messages
{
    public class UserChangedEvent
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Position { get; set; }

    }
}
