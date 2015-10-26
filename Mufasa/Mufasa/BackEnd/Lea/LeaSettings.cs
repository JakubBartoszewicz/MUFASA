using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mufasa.BackEnd.Lea
{
    /// <remarks>
    /// Lea settings.
    /// </remarks>
    class LeaSettings
    {
        /// <summary>
        /// Lea settings constructor.
        /// </summary>
        public LeaSettings()
        {
            this.PopulationSize = 20;
            this.TournamentSize = 5;
            this.CrossoverRate = 1.0;
            this.MutationRate = 0.8;
            this.LearningRate = 1.0;
            this.Epsilon = 0.01;
            this.IgnoreHeterodimers = false;
        }

        public bool IgnoreHeterodimers { get; set; }

        /// <value>
        /// Starting population size.
        /// </value>
        public int PopulationSize { get; set; }

        /// <value>
        /// Tournament size.
        /// </value>
        public int TournamentSize { get; set; }

        /// <value>
        /// Crossover rate.
        /// </value>
        public double CrossoverRate { get; set; }

        /// <value>
        /// Mutation rate.
        /// </value>
        public double MutationRate { get; set; }

        /// <summary>
        /// Local search chance.
        /// </summary>
        public double LearningRate { get; set; }

        /// <summary>
        /// Stopping criterion. Best solutions variance across generations threshold.
        /// </summary>
        /// <remarks>
        /// Variance of best solutions across generations must be lower than epsilon for the algorithm to stop.
        /// </remarks>
        public double Epsilon { get; set; }
    }
}
