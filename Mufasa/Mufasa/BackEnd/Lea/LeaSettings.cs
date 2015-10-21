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
            this.PopulationSize = 2000;
            this.TournamentSize = 10;
            this.CrossoverRate = 1.0;
            this.MutationRate = 1.0;
            this.LearningRate = 1.0;
        }

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
    }
}
