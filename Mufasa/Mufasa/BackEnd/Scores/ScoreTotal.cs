using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mufasa.BackEnd.Designer;

namespace Mufasa.BackEnd.Scores
{
    class ScoreTotal : Score
    {

        public ScoreTotal(List<Overlap> overlaps, double targetTm)
        {
            this.Label = "Total score";
            this.Description = "Total score";
            this.TargetTm = targetTm;
            Rescore(overlaps);
        }

        /// <summary>
        /// ScoreMean partial score.
        /// </summary>
        public ScoreMean Sm { get { return _sm; } }

        /// <summary>
        /// ScoreOptimum partial score.
        /// </summary>
        public ScoreOptimum So { get { return _so; } }

        /// <summary>
        /// ScoreMean partial score.
        /// </summary>
        private ScoreMean _sm;

        /// <summary>
        /// ScoreOptimum partial score.
        /// </summary>
        private ScoreOptimum _so;


        /// <summary>
        /// Target melting temperature.
        /// </summary>
        public double TargetTm { get; set; }

        /// <summary>
        /// Scoring function.
        /// </summary>
        /// <param name="overlaps">Overlap list.</param>
        override public void Rescore(List<Overlap> overlaps)
        {
            this._sm = new ScoreMean(overlaps);
            this._so = new ScoreOptimum(overlaps, this.TargetTm);
            this._normalizedScore = (0.5 * _sm.NormalizedScore) + (0.5 * _so.NormalizedScore);
            this._score = (0.5 * _sm.RawScore) + (0.5 * _so.RawScore);
        }
    }
}
