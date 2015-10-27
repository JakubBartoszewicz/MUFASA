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

        public static ScoreTotal Inacceptable = new ScoreTotal();

        /// <summary>
        /// Score constructor.
        /// </summary>
        /// <param name="overlaps">Overlap list.</param>
        /// <param name="targetTm">Target melting temperature.</param>
        public ScoreTotal(List<Overlap> overlaps, double targetTm)
        {
            this.Label = "Total score";
            this.Description = "Total score";
            this.TargetTm = targetTm;
            this.Sm = new ScoreMean();
            this.So = new ScoreOptimum(targetTm);
            Rescore(overlaps);
        }

        /// <summary>
        /// Empty Score constructor for inacceptable solutions.
        /// </summary>
        public ScoreTotal()
        {
            this.Label = "inacceptable solution";
            this.Description = "inacceptable solution";
            this.NormalizedScore = Double.PositiveInfinity;
            this.RawScore = Double.PositiveInfinity;
        }

        /// <summary>
        /// Empty Score constructor for unscored solutions.
        /// </summary>
        public ScoreTotal(double targetTm)
        {
            this.Label = "Total score";
            this.Description = "Total score";
            this.NormalizedScore = Double.PositiveInfinity;
            this.RawScore = Double.PositiveInfinity;
            this.TargetTm = targetTm;
            this.Sm = new ScoreMean();
            this.So = new ScoreOptimum(targetTm);
        }

        /// <summary>
        /// ScoreMean partial score.
        /// </summary>
        public ScoreMean Sm { get; private set; }

        /// <summary>
        /// ScoreOptimum partial score.
        /// </summary>
        public ScoreOptimum So { get; private set; }

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
            Overlap.CalculateHeterodimers(overlaps);
            this.Sm.Rescore(overlaps);
            this.So.Rescore(overlaps);
            this.NormalizedScore = (0.5 * Sm.NormalizedScore) + (0.5 * So.NormalizedScore);
            this.RawScore = (0.5 * Sm.RawScore) + (0.5 * So.RawScore);
        }
    }
}
