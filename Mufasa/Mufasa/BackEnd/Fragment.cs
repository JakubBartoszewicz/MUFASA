using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mufasa.BackEnd
{
    class Fragment
    {
        /// <value>
        /// Path to the file containing the fragment.
        /// </value>
        public String FileName { get; set; }
        /// <summary>
        /// Name of the fragment.
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// Fragment sequence.
        /// </summary>
        public String Sequence { get; set; }

        public Fragment(String filename, String name)
        {
            this.FileName = filename;
            this.Name = name;
        }
    }
}
