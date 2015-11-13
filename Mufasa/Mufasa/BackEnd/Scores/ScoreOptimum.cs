using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mufasa.BackEnd.Designer;

//Copyright (C) 2015 Jakub Bartoszewicz (if not stated otherwise)
namespace Mufasa.BackEnd.Scores
{
    class ScoreOptimum : Score
    {
        /// <summary>
        /// ScoreMean constructor.
        /// </summary>
        public ScoreOptimum(double targetTm)
        {
            this.Label = "So";
            this.Description = "Deviation from optimal Tm";
            this.TargetTm = targetTm;
        }


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
        /// ScoreOptimum copying constructor.
        /// </summary>
        /// <param name="s">Score to copy.</param>
        public ScoreOptimum(ScoreOptimum s)
        {
            this.RawScore = s.RawScore;
            this.NormalizedScore = s.NormalizedScore;
        }

        /// <summary>
        /// Scoring function.
        /// </summary>
        /// <param name="overlaps">Overlap list.</param>
        /// <param name="targetTm">Target melting temperature.</param>
        override public void Rescore(List<Overlap> overlaps)
        {
            this.RawScore = 0.0;

            foreach (Overlap o in overlaps)
            {
                RawScore += Math.Abs(o.MeltingTemperature - this.TargetTm);
            }
            
            this.NormalizedScore = this.RawScore / overlaps.Count;
        }

        /// <summary>
        /// Target melting temperature.
        /// </summary>
        public double TargetTm { get; set; }
    }
}
