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
            //this.TmSettings = new Thermodynamics.p3_tm_args();
            this.TmSettings.dna_conc = 50.0;
            this.TmSettings.salt_conc = 50.0;
            this.TmSettings.divalent_conc = 0.0; //Phusion buffer
            this.TmSettings.dntp_conc = 0.0; //Qian & Tian, 2014
            this.TmSettings.nn_max_len = 36;
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
