using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mufasa.BackEnd.Exceptions
{
    /// <remarks>
    /// Exception thrown by oligotm.
    /// <see cref="BackEnd.Tm_thal.cs"/>
    /// </remarks>
    [Serializable]
    public class TmException : Exception
    {

                /// <summary>
        /// TmException constructor.
        /// </summary>
        public TmException()
            : base()
        {

        }

        /// <summary>
        /// TmException constructor.
        /// </summary>
        /// <param name="message">Message to send.</param>
        public TmException(string message)
            : base(message)
        {

        }

    }
}
