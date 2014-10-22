using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mufasa.BackEnd.Designer
{
    class DesignerSettings
    {
        /// <value>
        /// Minimal length of the gene-specific part of a primer.
        /// </value>
        public int MinGeneSpecificLen { get; set; }

        /// <value>
        /// Maximal length of the gene-specific part of a primer.
        /// </value>
        public int MaxGeneSpecificLen { get; set; }

        /// <value>
        /// Minimal length of the overlapping part of a primer.
        /// </value>
        public int MinOverlapLen { get; set; }

        /// <value>
        /// Maximal length of the overlapping part of a primer.
        /// </value>
        public int MaxOverlapLen { get; set; }

        /// <value>
        /// CPEC/Gibson assembly reaction volume.
        /// </value>
        public int ReactionVolume { get; set; }

        /// <value>
        /// Target overlaps melting temperature.
        /// </value>
        public int TargetOverlapTm { get; set; }

        /// <value>
        /// Target primer melting temperature.
        /// </value>
        public int TargetPrimerTm { get; set; }

        /// <summary>
        /// Designer settings constructor.
        /// </summary>
        public DesignerSettings()
        {
            MinGeneSpecificLen = 18;
            MaxGeneSpecificLen = 25;
            MinOverlapLen = 20;
            MaxOverlapLen = 30;
            TargetOverlapTm = 60;
            TargetPrimerTm = 60;
            ReactionVolume = 50;
        }

    }
}
