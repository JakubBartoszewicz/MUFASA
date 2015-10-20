﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mufasa.BackEnd.Exceptions
{
    /// <remarks>
    /// Exception thrown if failed to load Primer3's thermodynamic parameters.
    /// <see cref="BackEnd.Designer.Designer.cs"/>
    /// </remarks>
    [Serializable]
    class TmThalParamException : Exception
    {
        /// <summary>
        /// TmThalParamException constructor.
        /// </summary>
        public TmThalParamException()
            : base()
        {

        }

        /// <summary>
        /// TmThalParamException constructor.
        /// </summary>
        /// <param name="message">Message to send.</param>
        public TmThalParamException(string message)
            : base(message)
        {

        }
    }
}
