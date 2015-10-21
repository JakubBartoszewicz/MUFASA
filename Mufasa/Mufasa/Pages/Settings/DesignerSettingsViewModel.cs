using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mufasa.BackEnd.Designer;

namespace Mufasa.Pages.Settings
{
    /// <summary>
    /// Wraps DesignerSettings class and provide notification of changes
    /// </summary>
    class DesignerSettingsViewModel : NotifyPropertyChanged
    {
        /// <summary>
        /// DesignerSettingsViewModel constructor.
        /// </summary>
        public DesignerSettingsViewModel()
        {
            this.Model = new DesignerSettings();
        }

        /// <summary>
        /// DesignerSettingsViewModel constructor.
        /// </summary>
        /// <param name="m">DesignerSettings model.</param>
        public DesignerSettingsViewModel(DesignerSettings m)
        {
            this.Model = m;
        }

        /// <summary>
        /// Designer settings model.
        /// </summary>
        public DesignerSettings Model { get; private set; }
        
        /// <value>
        /// Minimal length of the 3' ("gene-specific") part of an overlap.
        /// </value>
        public int MinLen_3
        {
            get { return this.Model.MinLen_3; }
            set
            {
                this.Model.MinLen_3 = value;
                OnPropertyChanged("MinLen_3");
            }
        }

        /// <value>
        /// Maximal length of the 3' ("gene-specific") part of an overlap.
        /// </value>
        public int MaxLen_3
        {
            get { return this.Model.MaxLen_3; }
            set
            {
                this.Model.MaxLen_3 = value;
                OnPropertyChanged("MaxLen_3");
            }
        }

        /// <value>
        /// Minimal length of the 5' ("overhang") part of an overlap.
        /// </value>
        public int MinLen_5
        {
            get { return this.Model.MinLen_5; }
            set
            {
                this.Model.MinLen_5 = value;
                OnPropertyChanged("MinLen_5");
            }
        }

        /// <value>
        /// Maximal length of the 5' ("overhang") part of an overlap.
        /// </value>
        public int MaxLen_5
        {
            get { return this.Model.MaxLen_5; }
            set
            {
                this.Model.MaxLen_5 = value;
                OnPropertyChanged("MaxLen_5");
            }
        }

        /// <value>
        /// CPEC/Gibson assembly reaction volume.
        /// </value>
        public double ReactionVolume     
        {
            get { return this.Model.ReactionVolume ; }
            set
            {
                this.Model.ReactionVolume  = value;
                OnPropertyChanged("ReactionVolume");
            }
        }

        /// <value>
        /// Target overlap melting temperature.
        /// </value>
        public double TargetTm
        {
            get { return this.Model.TargetTm; }
            set
            {
                this.Model.TargetTm = value;
                OnPropertyChanged("TargetTm");
            }
        }

        /// <value>
        /// Max hairpin melting temperature.
        /// </value>
        public double MaxTh  
        {
            get { return this.Model.MaxTh; }
            set
            {
                this.Model.MaxTh = value;
                OnPropertyChanged("MaxTh");
            }
        }

        /// <value>
        /// Max duplex melting temperature.
        /// </value>
        public double MaxTd
        {
            get { return this.Model.MaxTd; }
            set
            {
                this.Model.MaxTd = value;
                OnPropertyChanged("MaxTd");
            }
        }

    }
}
