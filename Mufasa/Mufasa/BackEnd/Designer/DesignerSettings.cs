using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mufasa.BackEnd.TmThal;
using Mufasa.BackEnd.Lea;

namespace Mufasa.BackEnd.Designer
{
    /// <remarks>
    /// Design settings class.
    /// </remarks>
    class DesignerSettings
    {
        /// <value>
        /// Path to Primer3's thermodynamic parameters folder.
        /// </value>
        public String TmThalParamPath { get; set; }

        /// <value>
        /// Minimal length of the 3' ("gene-specific") part of an overlap.
        /// </value>
        public int MinLen_3 { get; set; }

        /// <value>
        /// Maximal length of the 3' ("gene-specific") part of an overlap.
        /// </value>
        public int MaxLen_3 { get; set; }

        /// <value>
        /// Minimal length of the 5' ("overhang") part of an overlap.
        /// </value>
        public int MinLen_5 { get; set; }

        /// <value>
        /// Maximal length of the 5' ("overhang") part of an overlap.
        /// </value>
        public int MaxLen_5 { get; set; }

        /// <value>
        /// CPEC/Gibson assembly reaction volume.
        /// </value>
        public double ReactionVolume { get; set; }

        /// <value>
        /// Target overlap melting temperature.
        /// </value>
        public double TargetTm { get; set; }

        /// <summary>
        /// TmThal settings
        /// </summary>
        public TmThalSettings TmThalSettings;

        /// <summary>
        /// Lea settings
        /// </summary>
        public LeaSettings LeaSettings;

        /// <value>
        /// Max hairpin melting temperature.
        /// </value>
        public double MaxTh { get; set; }

        /// <value>
        /// Max duplex melting temperature.
        /// </value>
        public double MaxTd { get; set; }

        /// <summary>
        /// Designer settings constructor.
        /// </summary>
        public DesignerSettings()
        {
            this.TmThalParamPath = ".\\tmthal_config\\";
            this.MinLen_3 = 18;
            this.MaxLen_3 = 25;
            this.MinLen_5 = 20;
            this.MaxLen_5 = 30;
            this.TargetTm = 65.0;
            this.MaxTh = 55.0;
            this.MaxTd= 55.0;
            this.ReactionVolume = 50.0;
            this.TmThalSettings = new TmThalSettings();
            this.LeaSettings = new LeaSettings();
        }

    }
}
