using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mufasa.BackEnd.Lea;

namespace Mufasa.Pages.Settings
{
    /// <remarks>
    /// Wraps LeaSettings class and provides notification of changes
    /// </remarks>
    class LeaSettingsViewModel : NotifyPropertyChanged
    {
        /// <summary>
        /// DesignerSettingsViewModel constructor.
        /// </summary>
        public LeaSettingsViewModel()
        {
            this.Model = new LeaSettings();
        }

        /// <summary>
        /// DesignerSettingsViewModel constructor.
        /// </summary>
        /// <param name="m">DesignerSettings model.</param>
        public LeaSettingsViewModel(LeaSettings m)
        {
            this.Model = m;
        }


        /// <value>
        /// Lea settings model.
        /// </value>
        public LeaSettings Model { get; private set; }

        /// <value>
        /// Starting population size.
        /// </value>
        public int PopulationSize
        {
            get { return this.Model.PopulationSize; }
            set
            {
                this.Model.PopulationSize = value;
                OnPropertyChanged("PopulationSize");
            }
        }

        /// <value>
        /// Tournament size.
        /// </value>
        public int TournamentSize
        {
            get { return this.Model.TournamentSize; }
            set
            {
                this.Model.TournamentSize = value;
                OnPropertyChanged("TournamentSize");
            }
        }

        /// <value>
        /// Crossover rate.
        /// </value>
        public double CrossoverRate
        {
            get { return this.Model.CrossoverRate; }
            set
            {
                this.Model.CrossoverRate = value;
                OnPropertyChanged("CrossoverRate");
            }
        }

        /// <value>
        /// Mutation rate.
        /// </value>
        public double MutationRate
        {
            get { return this.Model.MutationRate; }
            set
            {
                this.Model.MutationRate = value;
                OnPropertyChanged("MutationRate");
            }
        }

        /// <summary>
        /// Local search chance.
        /// </summary>
        public double LearningRate
        {
            get { return this.Model.LearningRate; }
            set
            {
                this.Model.LearningRate = value;
                OnPropertyChanged("LearningRate");
            }
        }
    }
}
