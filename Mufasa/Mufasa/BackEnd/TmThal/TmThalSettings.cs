using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mufasa.BackEnd.TmThal
{
    class TmThalSettings
    {
        /// <summary>
        /// TmThalSettings constructor.
        /// </summary>
        public TmThalSettings()
        {
            this.TmSettings.dna_conc = 50.0;    //Assume primer3's default 
            this.TmSettings.salt_conc = 50.0;   //Phusion buffer
            this.TmSettings.divalent_conc = 1.5; //Phusion buffer
            this.TmSettings.dntp_conc = 0.8 ; //Qian & Tian, 2014: 0.8 mM.
            this.TmSettings.nn_max_len = 60; //or 36
            this.TmSettings.tm_method = Thermodynamics.p3_tm_method_type.p3_santalucia_auto;
            this.TmSettings.salt_corrections = Thermodynamics.p3_salt_correction_type.p3_santalucia;
        }

        /// <summary>
        /// TmThal.p3_seqtm settings
        /// </summary>
        public Thermodynamics.p3_tm_args TmSettings;

        /// <summary>
        /// TmThal.p3_thal settings
        /// </summary>
        public Thermodynamics.p3_thal_args ThalSettings;
    }
}
