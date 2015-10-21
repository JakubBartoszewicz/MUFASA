using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mufasa.BackEnd.TmThal;

namespace Mufasa.Pages.Settings
{
    /// <remarks>
    /// Wraps TmThalSettings class and provide notification of changes
    /// </remarks>
    class TmThalSettingsViewModel : NotifyPropertyChanged
    {
        /// <summary>
        /// DesignerSettingsViewModel constructor.
        /// </summary>
        public TmThalSettingsViewModel()
        {
            this.Model = new TmThalSettings();
        }

        /// <summary>
        /// DesignerSettingsViewModel constructor.
        /// </summary>
        /// <param name="m">DesignerSettings model.</param>
        public TmThalSettingsViewModel(TmThalSettings m)
        {
            this.Model = m;
        }

        /// <value>
        /// TmThal settings model.
        /// </value>
        public TmThalSettings Model { get; private set; }

        /// <value>
        /// Salt correction.
        /// </value>
        public Thermodynamics.p3_salt_correction_type SaltCorrectionMethod
        {
            get { return this.Model.SaltCorrectionMethod; }
            set
            {
                this.Model.SaltCorrectionMethod = value;
                OnPropertyChanged("SaltCorrectionMethod");
            }
        }

        /// <summary>
        /// Melting temperature computation method. See <see cref="Thermodynamics.p3_tm_method_type"/>
        /// </summary>
        public Thermodynamics.p3_tm_method_type TmMethod
        {
            get { return this.Model.TmMethod; }
            set
            {
                this.Model.TmMethod = value;
                OnPropertyChanged("TmMethod");
            }
        }

        /// <summary>
        /// Maximum size of loop to consider; longer than 30 bp are not allowed.
        /// </summary>
        public int MaxLoop
        {
            get { return this.Model.MaxLoop; }
            set
            {
                this.Model.MaxLoop = value;
                OnPropertyChanged("MaxLoop");
            }
        }

        /// <summary>
        /// Max oligo length for Nearest Neighbor-model computations.
        /// </summary>
        public int NnMaxLen
        {
            get { return this.Model.NnMaxLen; }
            set
            {
                this.Model.NnMaxLen = value;
                OnPropertyChanged("NnMaxLen");
            }
        }

        /// <summary>
        /// Temperature from which hairpin structures will be calculated.
        /// </summary>
        public double ReactionTemperature
        {
            get { return this.Model.ReactionTemperature; }
            set
            {
                this.Model.ReactionTemperature = value;
                OnPropertyChanged("ReactionTemperature");
            }
        }


        /// <summary>
        /// DNA concentration.
        /// </summary>
        public double DnaConcentration
        {
            get { return this.Model.DnaConcentration; }
            set
            {
                this.Model.DnaConcentration = value;
                OnPropertyChanged("DnaConcentration");
            }
        }

        /// <summary>
        /// dNTP concentration.
        /// </summary>
        public double DntpConcentration
        {
            get { return this.Model.DntpConcentration; }
            set
            {
                this.Model.DntpConcentration = value;
                OnPropertyChanged("DntpConcentration");
            }
        }

        /// <summary>
        /// Divalent [Mg2+] cations concentration.
        /// </summary>
        public double DivalentConcentration
        {
            get { return this.Model.DivalentConcentration; }
            set
            {
                this.Model.DivalentConcentration= value;
                OnPropertyChanged("DivalentConcentration");
            }
        }

        /// <summary>
        /// Monovalent [Na+/K+] cations concentration.
        /// </summary>
        public double MonovalentConcentration
        {
            get { return this.Model.MonovalentConcentration; }
            set
            {
                this.Model.MonovalentConcentration = value;
                OnPropertyChanged("MonovalentConcentration");
            }
        }

    }
}
