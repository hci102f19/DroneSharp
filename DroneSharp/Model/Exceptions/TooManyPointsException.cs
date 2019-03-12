using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneSharp.Model.Exceptions
{
    public class TooManyPointsException : Exception
    {
        public TooManyPointsException(string message) : base(message)
        {
        }
    }
}
