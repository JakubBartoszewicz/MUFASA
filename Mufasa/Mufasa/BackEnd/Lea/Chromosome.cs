using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bio;
using Mufasa.BackEnd.Designer;

namespace Mufasa.BackEnd.Lea
{
    class Chromosome
    {
        public Chromosome(List<int> lengths_3, List<int> lengths_5)
        {
            this.Lengths_3 = lengths_3;
            this.Lengths_5 = lengths_5;
        }

        /// <value>
        /// List of lengths of 3' (overhang) parts of the overlaps./> 
        /// </value>
        public List<int> Lengths_3 { get; set; }

        /// <value>
        /// List of lengths of 3' (overhang) parts of the overlaps./> 
        /// </value>        
        public List<int> Lengths_5 { get; set; }

        /// <summary>
        /// Converts the chromosome to its corresponding overlap list.
        /// </summary>
        /// <param name="templates">List of overlap templates.</param>
        /// <returns>List of overlaps represented by this chromosome.</returns>
        public List<Overlap> ToOverlaps(List<Overlap> templates)
        {
            List<Overlap> overlaps = new List<Overlap>();

            for (int i = 0; i < templates.Count; i++)
            {
                Sequence seq_5 = (Sequence)templates[i].Seq_5.GetSubSequence(templates[i].Seq_5.Count - this.Lengths_5[i], this.Lengths_5[i]);
                Sequence seq_3 = (Sequence)templates[i].Seq_3.GetSubSequence(0, this.Lengths_3[i]);

                Overlap temp = new Overlap(templates[i].Name, seq_5, seq_3, templates[i].Settings, templates[i].PairIndex);
            }

            return overlaps;
        }
    }
}
