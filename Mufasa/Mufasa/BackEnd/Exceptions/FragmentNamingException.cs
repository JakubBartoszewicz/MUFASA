using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mufasa.BackEnd.Exceptions
{
    /// <remarks>
    /// Exception thrown if a fragment name is invalid.
    /// <see cref="Design.xaml.cs"/>
    /// </remarks>
    class FragmentNamingException : Exception
    {
        public FragmentNamingException() : base()
        {

        }

        public FragmentNamingException(string message) : base(message)
        {

        }
    }
}
