using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mufasa.BackEnd.Exceptions
{
    /// <summary>
    /// Assbly exception
    /// </summary>
    [Serializable]
    class AssemblyException : Exception
    {
        /// <summary>
        /// AssemblyException constructor.
        /// </summary>
        public AssemblyException()
            : base("Unable to find any acceptable solutions. Try relaxing some of the conditions.")
        {

        }

        /// <summary>
        /// AssemblyException constructor.
        /// </summary>
        /// <param name="message">Message to send.</param>
        public AssemblyException(string message)
            : base("Unable to find any acceptable solutions. Try relaxing some of the conditions.\n" + message)
        {

        }
    }
}
