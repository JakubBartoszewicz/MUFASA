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
        public Chromosome(List<int> lengths_3, List<int> lengths_5, double targetTm)
        {
            this.Lengths_3 = lengths_3;
            this.Lengths_5 = lengths_5;
            this.Score = new ScoreTotal(targetTm);
            this.Overlaps = new List<Overlap>();
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
            this.Overlaps = c.Overlaps;
        }

        public Chromosome(List<Overlap> preoptimized, double targetTm)
        {
            this.Lengths_3 = new List<int>();
            this.Lengths_5 = new List<int>();

            foreach (Overlap o in preoptimized)
            {
                this.Lengths_3.Add((int)o.Seq_3.Count);
                this.Lengths_5.Add((int)o.Seq_5.Count);
            }

            this.Score = new ScoreTotal(targetTm);
            this.Overlaps = preoptimized;
        }

        /// <summary>
        /// Overlap list.
        /// </summary>
        public List<Overlap> Overlaps { get; set; }


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
            Overlaps.Clear();

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

                Overlaps.Add(temp);
            }

            return Overlaps;
        }

        /// <summary>
        /// Evaluate the chromosome.
        /// </summary>
        /// <param name="templates">List of overlap templates.</param>
        /// <param name="settings">Designer settings.</param>
        /// <returns>Total score of the chromosome.</returns>
        public ScoreTotal Evaluate(List<Overlap> templates, DesignerSettings settings, bool ignoreHeterodimers)
        {
            this.Score.Rescore(this.ToOverlaps(templates));
            return this.Score;
        }        
    }
}
