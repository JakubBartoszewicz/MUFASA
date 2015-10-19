using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mufasa.BackEnd.Designer;

namespace Mufasa.BackEnd.Scores
{
    class ScoreMean : Score
    {
        /// <summary>
        /// ScoreMean constructor.
        /// </summary>
        public ScoreMean(List<Overlap> overlaps) 
        {
            this.Label = "Sm";
            this.Description = "Deviation from mean Tm";
            Rescore(overlaps);
        }

        /// <summary>
        /// Scoring function.
        /// </summary>
        /// <param name="overlaps">Overlap list.</param>
        override public void Rescore(List<Overlap> overlaps)
        {
            this._score = 0.0;
            double sum = 0.0;

            foreach (Overlap o in overlaps)
            {
                sum += o.MeltingTemperature;
            }

            double mean = sum / overlaps.Count;

            foreach (Overlap o in overlaps)
            {
                _score += Math.Abs(o.MeltingTemperature - mean);
            }
            
            this._normalizedScore = this._score / overlaps.Count;
        }
    }
}
