using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Mufasa.BackEnd
{
    static class Tm_thal
    {
        /*
           For olgigotm() and seqtm()

           Both functions return the melting temperature of the given oligo
           calculated as specified by user, but oligotm _should_ only be used on
           DNA sequences of length <= MAX_PRIMER_LENGTH (which is defined
           elsewhere).  seqtm uses oligotm for sequences of length <=
           MAX_PRIMER_LENGTH, and a different, G+C% based formula for longer
           sequences.  For oligotm(), no error is generated on sequences
           longer than MAX_PRIMER_LENGTH, but the formula becomes less
           accurate as the sequence grows longer.  Caveat emptor.

           We use the folowing typedefs:
        */
        public enum p3_tm_method_type
        {
            p3_breslauer_auto = 0,
            p3_santalucia_auto = 1,
        };

        public enum p3_salt_correction_type
        {
            p3_schildkraut = 0,
            p3_santalucia = 1,
            p3_owczarzy = 2,
        };

        public enum p3_thal_alignment_type
        {
            thal_any = 1,
            thal_end1 = 2,
            thal_end2 = 3,
            thal_hairpin = 4,
        };

        /* Structure for passing arguments to THermodynamic ALignment calculation */
        public struct p3_thal_args
        {
            int debug; /* if non zero, print debugging info to stderr */
            p3_thal_alignment_type type; /* one of the
							          1 THAL_ANY, (by default)
							          2 THAL_END1,
							          3 THAL_END2,
							          4 THAL_HAIRPIN */
            int maxLoop;  /* maximum size of loop to consider; longer than 30 bp are not allowed */
            double mv; /* concentration of monovalent cations */
            double dv; /* concentration of divalent cations */
            double dntp; /* concentration of dNTP-s */
            double dna_conc; /* concentration of oligonucleotides */
            double temp; /* temperature from which hairpin structures will be calculated */
            int temponly; /* if non zero, print only temperature to stderr */
            int dimer; /* if non zero, dimer structure is calculated */
        } ;


        /* Structure for receiving results from the thermodynamic alignment calculation */

        public unsafe struct p3_thal_results
        {
            fixed char msg[255];
            double temp;
            int align_end_1;
            int align_end_2;
        };


        [DllImport("Tm_thal.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern double p3_seqtm(char* seq,  /* The sequence. */
        double dna_conc,   /* DNA concentration (nanomolar). */
        double salt_conc,  /* Concentration of divalent cations (millimolar). */
        double divalent_conc, /* Concentration of divalent cations (millimolar) */
        double dntp_conc,     /* Concentration of dNTPs (millimolar) */
        int nn_max_len,  /* The maximum sequence length for
						    using the nearest neighbor model
						    (as implemented in oligotm.  For
						    sequences longer than this, seqtm
						    uses the "GC%" formula implemented
						    in long_seq_tm.
						    */

                            p3_tm_method_type tm_method,       /* See description above. */
                            p3_salt_correction_type salt_corrections /* See description above. */
                            );
        [DllImport("Tm_thal.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void p3_thal(char* oligo_f,
        char* oligo_r,
        p3_thal_args* a,
        p3_thal_results* o);

    }
}
