using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casino
{
    public class FraudException : Exception
    {
        public FraudException()
            : base() { } //base is just grabbing from Exception, which we are inheriting from
        public FraudException(string message)
            : base(message) { }
    }
}
