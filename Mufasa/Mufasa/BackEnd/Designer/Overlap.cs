using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bio;
using Bio.IO;

namespace Mufasa.BackEnd.Designer
{
    /// <remarks>
    /// Overlap class.
    /// </remarks>
    class Overlap : Fragment
    {
        /// <summary>
        /// Overlap constructor.
        /// </summary>
        /// <param name="name">Overlap name.</param>
        /// <param name="sequence">Overlap sequence.</param>
        public Overlap(String name, ISequence sequence)
        {
            this.Name = name;
            this.Sequence = sequence;
        }

        /// <value>
        /// Overlap's temperature.
        /// </value>
        public int Temperature { get; set; }

        /// <summary>
        /// Prints the overlap in a human-readable format.
        /// </summary>
        /// <returns>String represanting the overlap.</returns>
        public override string ToString()
        {
            String result = this.Name + "," + this.Sequence + "," + this.Temperature;
            return result;
        }


    }
}
