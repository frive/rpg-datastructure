using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rpg
{
    public class DataStructureException: Exception
    {
        public DataStructureException() : base()
        {
        }

        public DataStructureException(string message) : base(message)
        {
        }

        public DataStructureException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DataStructureException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}
