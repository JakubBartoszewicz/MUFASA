using Bio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mufasa.BackEnd.Designer
{
    /// <remarks>
    /// DNA fragment class.
    /// </remarks>
    class Fragment
    {
        /// <value>
        /// Path to the file or url containing the fragment.
        /// </value>
        public String Source { get; set; }
        /// <summary>
        /// Name of the fragment.
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// Fragment sequence.
        /// </summary>
        public ISequence Sequence { get; set; }

        /// <summary>
        /// Fragment constructor.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="name"></param>
        public Fragment(String source, String name, ISequence sequence)
        {
            this.Source = source;
            this.Name = name;
            this.Sequence = sequence;
        }

        /// <summary>
        /// Returns full fragment sequence as a string. Based on .NET Bio Programming Guide.
        /// </summary>
        /// <returns></returns>
        public string GetString()
        {
            char[] symbols = new char[this.Sequence.Count];
            for (long index = 0; index < this.Sequence.Count; index++)
            {
                symbols[index] = (char)this.Sequence[index]; 
            }
            return new String(symbols); 
        }
    }
}
