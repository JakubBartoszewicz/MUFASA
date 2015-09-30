﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mufasa.BackEnd.Designer
{
    /// <remarks>
    /// Design settings class.
    /// </remarks>
    class DesignerSettings
    {
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
        public int ReactionVolume { get; set; }

        /// <value>
        /// Target overlap melting temperature.
        /// </value>
        public int TargetTm { get; set; }

        /// <summary>
        /// Designer settings constructor.
        /// </summary>
        public DesignerSettings()
        {
            this.MinLen_3 = 18;
            this.MaxLen_3 = 25;
            this.MinLen_5 = 20;
            this.MaxLen_5 = 30;
            this.TargetTm = 65;
            this.ReactionVolume = 50;
        }

    }
}
