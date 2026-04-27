using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotMonitor_3.Utilities
{
    public class MessageSender : ValueChangedMessage<string>
    {
        public MessageSender(string value) : base(value) { }
    }

    public class KeyboardDataSender : ValueChangedMessage<string>
    {
        public KeyboardDataSender(string value) : base(value) { }
    }

    public class DisplayDataSender : ValueChangedMessage<string>
    {
        public DisplayDataSender(string value) : base(value) { }
    }

    public class LoginBool : ValueChangedMessage<bool>
    {
        public LoginBool(bool value) : base(value) { }
    }
}
