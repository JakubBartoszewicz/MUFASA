using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mufasa.BackEnd.Designer;

namespace Mufasa.BackEnd.Scores
{
    abstract class Score
    {
        /// <summary>
        /// Raw score.
        /// </summary>
        public double RawScore { get { return _score; } }

        /// <summary>
        /// Normalized Score.
        /// </summary>
        public double NormalizedScore { get { return _normalizedScore; } }


        /// <summary>
        /// Raw score.
        /// </summary>
        protected double _score;

        /// <summary>
        /// Normalized Score.
        /// </summary>
        protected double _normalizedScore;

        /// <summary>
        /// Score label or name
        /// </summary>
        public String Label { get; set; }

        /// <summary>
        /// Score description
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Scoring function.
        /// </summary>
        /// <param name="overlaps">Overlap list.</param>
        abstract public void Rescore(List<Overlap> overlaps);

        /// <summary>
        /// Prints the overlap in a human-readable format.
        /// </summary>
        /// <returns>String represanting the overlap.</returns>
        public override string ToString()
        {
            String sep = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            String result = this.Label + sep + Math.Round(this._score, 2) + sep + Math.Round(this._normalizedScore, 2);
            return result;
        }
    }
}
