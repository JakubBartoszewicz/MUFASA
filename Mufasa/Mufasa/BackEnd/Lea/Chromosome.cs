using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bio;
using Mufasa.BackEnd.Designer;
using Mufasa.BackEnd.Scores;

namespace Mufasa.BackEnd.Lea
{
    class Chromosome
    {
        /// <summary>
        /// Chromosome constructor.
        /// </summary>
        /// <param name="lengths_3">List of 3' lengths.</param>
        /// <param name="lengths_5">List of 5' lengths.</param>
        public Chromosome(List<int> lengths_3, List<int> lengths_5)
        {
            this.Lengths_3 = lengths_3;
            this.Lengths_5 = lengths_5;
        }

        /// <summary>
        /// Chromosome copying constructor.
        /// </summary>
        /// <param name="c"></param>
        public Chromosome(Chromosome c)
        {
            this.Lengths_3 = c.Lengths_3;
            this.Lengths_5 = c.Lengths_5;
            this.Score = c.Score;
        }

        /// <value>
        /// List of lengths of 3' (overhang) parts of the overlaps./> 
        /// </value>
        public List<int> Lengths_3 { get; set; }

        /// <value>
        /// List of lengths of 3' (overhang) parts of the overlaps./> 
        /// </value>        
        public List<int> Lengths_5 { get; set; }

        /// <value>
        /// Score of the chromosome./> 
        /// </value>        
        public ScoreTotal Score { get; private set; }

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
                Overlap temp;
                Sequence seq_3 = (Sequence)templates[i].Seq_3.GetSubSequence(0, this.Lengths_3[i]);

                if (templates[i].Seq_5.Count > 0)
                {
                    Sequence seq_5 = (Sequence)templates[i].Seq_5.GetSubSequence(templates[i].Seq_5.Count - this.Lengths_5[i], this.Lengths_5[i]);
                    temp = new Overlap(templates[i].Name, seq_5, seq_3, templates[i].Settings, templates[i].PairIndex);
                }
                else
                {
                    temp = new Overlap(templates[i].Name, seq_3, templates[i].Settings, templates[i].PairIndex);
                }

                overlaps.Add(temp);
            }

            return overlaps;
        }

        /// <summary>
        /// Converts the chromosome to its corresponding construct.
        /// </summary>
        /// <param name="templates">List of overlap templates.</param>
        /// <param name="sequence">Designer settings.</param>
        /// <returns>A construct represented by this chromosome.</returns>
        public Construct ToConstruct(List<Overlap> templates, ISequence sequence, DesignerSettings settings)
        {
            List<Overlap> overlaps = this.ToOverlaps(templates);
            return new Construct(overlaps, sequence, settings);
        }

        /// <summary>
        /// Evaluate the chromosome.
        /// </summary>
        /// <param name="templates">List of overlap templates.</param>
        /// <param name="settings">Designer settings.</param>
        /// <returns>Total score of the chromosome.</returns>
        public ScoreTotal Evaluate(List<Overlap> templates, DesignerSettings settings)
        {
            Construct c = this.ToConstruct(templates, new Sequence(Alphabets.AmbiguousDNA, ""), settings);
            this.Score = c.Evaluate();
            return this.Score;
        }        
    }
}
