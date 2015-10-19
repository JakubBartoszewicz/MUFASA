using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mufasa.BackEnd.Designer;

namespace Mufasa.BackEnd.Scores
{
    class ScoreOptimum : Score
    {
        /// <summary>
        /// ScoreMean constructor.
        /// </summary>
        public ScoreOptimum(List<Overlap> overlaps, double targetTm)
        {
            this.Label = "So";
            this.Description = "Deviation from optimal Tm";
            this.TargetTm = targetTm;
            Rescore(overlaps);
        }

        /// <summary>
        /// Scoring function.
        /// </summary>
        /// <param name="overlaps">Overlap list.</param>
        /// <param name="targetTm">Target melting temperature.</param>
        override public void Rescore(List<Overlap> overlaps)
        {
            this._score = 0.0;

            foreach (Overlap o in overlaps)
            {
                _score += Math.Abs(o.MeltingTemperature - this.TargetTm);
            }
            
            this._normalizedScore = this._score / overlaps.Count;
        }

        /// <summary>
        /// Target melting temperature.
        /// </summary>
        public double TargetTm { get; set; }
    }
}
