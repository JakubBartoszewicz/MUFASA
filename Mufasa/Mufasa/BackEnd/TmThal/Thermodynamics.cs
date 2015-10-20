using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Mufasa.BackEnd.TmThal
{
    static class Thermodynamics
    {
        /// <summary>
        /// Melting temperature computation method.
        /// </summary>
        /// <remarks>
        /// If tm_method==santalucia_auto, then the table of
        /// nearest-neighbor thermodynamic parameters and method for Tm
        /// calculation in the paper [SantaLucia JR (1998) &quot;A unified view of
        /// polymer, dumbbell and oligonucleotide DNA nearest-neighbor
        /// thermodynamics&quot;, Proc Natl Acad Sci 95:1460-65
        /// http://dx.doi.org/10.1073/pnas.95.4.1460] is used.
        /// *THIS IS THE RECOMMENDED VALUE*.
        /// If tm_method==breslauer_auto, then method for Tm
        /// calculations in the paper [Rychlik W, Spencer WJ and Rhoads RE
        /// (1990) &quot;Optimization of the annealing temperature for DNA
        /// amplification in vitro&quot;, Nucleic Acids Res 18:6409-12
        /// http://www.pubmedcentral.nih.gov/articlerender.fcgi?tool=pubmed&amp;pubmedid=2243783].
        /// and the thermodynamic parameters in the paper [Breslauer KJ, Frank
        /// R, Blöcker H and Marky LA (1986) &quot;Predicting DNA duplex stability
        /// from the base sequence&quot; Proc Natl Acad Sci 83:4746-50
        /// http://dx.doi.org/10.1073/pnas.83.11.3746], are is used.  This is
        /// the method and the table that primer3 used up to and including
        /// version 1.0.1
        /// </remarks>
        public enum p3_tm_method_type
        {
            p3_breslauer_auto = 0,
            p3_santalucia_auto = 1,
        };

        /// <summary>
        /// Salt correction method.
        /// </summary>
        /// <remarks>
        /// If salt_corrections==schildkraut, then formula for
        /// salt correction in the paper [Schildkraut, C, and Lifson, S (1965)
        /// &quot;Dependence of the melting temperature of DNA on salt
        /// concentration&quot;, Biopolymers 3:195-208 (not available on-line)] is
        /// used.  This is the formula that primer3 used up to and including
        /// version 1.0.1.
        ///
        /// If salt_corrections==santalucia, then formula for
        /// salt correction suggested by the paper [SantaLucia JR (1998) "A
        /// unified view of polymer, dumbbell and oligonucleotide DNA
        /// nearest-neighbor thermodynamics", Proc Natl Acad Sci 95:1460-65
        /// http://dx.doi.org/10.1073/pnas.95.4.1460] is used.
        ///
        /// *THIS IS THE RECOMMENDED VALUE*. 
        ///
        /// If salt_corrections==owczarzy, then formula for
        /// salt correction in the paper [Owczarzy, R., Moreira, B.G., You, Y., 
        /// Behlke, M.A., and Walder, J.A. (2008) &quot;Predicting stability of DNA 
        /// duplexes in solutions containing magnesium and monovalent cations&quot;, 
        /// Biochemistry 47:5336-53 http://dx.doi.org/10.1021/bi702363u] is used.
        /// </remarks>
        public enum p3_salt_correction_type
        {
            p3_schildkraut = 0,
            p3_santalucia = 1,
            p3_owczarzy = 2,
        };

        /// <summary>
        /// Alignment type.
        /// 1 THAL_ANY, (by default)
        /// 2 THAL_END1,
        /// 3 THAL_END2,
        /// 4 THAL_HAIRPIN
        /// </summary>
        public enum p3_thal_alignment_type
        {
            thal_any = 1,
            thal_end1 = 2,
            thal_end2 = 3,
            thal_hairpin = 4,
        };

        /// <summary>
        /// Structure for passing arguments to THermodynamic ALignment calculation.
        /// </summary>
        public struct p3_thal_args
        {
            /// <summary>
            /// If non zero, print debugging info to stderr. 
            /// </summary>
            public int debug;

            /// <summary>
            /// Alignment type. THAL_ANY, by default. See <see cref="p3_thal_alignment_type"/>
            /// </summary>
            public p3_thal_alignment_type type;

            /// <summary>
            /// Maximum size of loop to consider; longer than 30 bp are not allowed.
            /// </summary>
            public int maxLoop;

            /// <summary>
            /// Concentration of monovalent cations. 
            /// </summary>
            public double mv;

            /// <summary>
            /// Concentration of divalent cations.
            /// </summary>
            public double dv;

            /// <summary>
            /// Concentration of dNTP-s.
            /// </summary>
            public double dntp;

            /// <summary>
            /// Concentration of oligonucleotides.
            /// </summary>
            public double dna_conc;

            /// <summary>
            /// Temperature from which hairpin structures will be calculated.
            /// </summary>
            public double temp;

            /// <summary>
            /// If non zero, print only temperature to stderr.
            /// </summary>
            public int temponly;

            /// <summary>
            /// If non zero, dimer structure is calculated. 
            /// </summary>
            public int dimer;

        } ;


        /// <summary>
        /// Structure for receiving results from the thermodynamic alignment calculation.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct thal_results
        {
            /// <summary>
            /// Message.
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst=255, ArraySubType=UnmanagedType.I1)]
            public char [] msg;

            /// <summary>
            /// Melting temperature.
            /// </summary>
            public double temp;

            /// <summary>
            /// Alignment end 1.
            /// </summary>
            public int align_end_1;

            /// <summary>
            /// Alignment end 2.
            /// </summary>
            public int align_end_2;
        };


        /// <summary>
        /// Primer3's thal arguments structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct p3_tm_args
        {
            /// <summary>
            /// DNA concentration (nanomolar).
            /// </summary>
            public double dna_conc;

            /// <summary>
            /// Concentration of divalent cations (millimolar).
            /// </summary>
            public double salt_conc;

            /// <summary>
            /// Concentration of divalent cations (millimolar).
            /// </summary>
            public double divalent_conc;

            /// <summary>
            /// Concentration of dNTPs (millimolar).
            /// </summary>
            public double dntp_conc;

            /// <summary>
            /// The maximum sequence length for
            /// using the nearest neighbor model
            /// (as implemented in oligotm.  For
            /// sequences longer than this, seqtm
            /// uses the "GC%" formula implemented
            /// in long_seq_tm.
            /// </summary>
            public int nn_max_len;

            /// <summary>
            /// Melting temperature computation method. See <see cref="p3_tm_method_type"/>
            /// </summary>
            public p3_tm_method_type tm_method;

            /// <summary>
            /// Melting themperature method. See <see cref="p3_salt_correction_type"/>
            /// </summary>
            public p3_salt_correction_type salt_corrections; /* See description above. */

        }

        /// <summary>
        /// Primer3's general melting temperature calulation function.
        /// </summary>
        /// <param name="seq">The sequence.</param>
        /// <param name="dna_conc">DNA concentration (nanomolar).</param>
        /// <param name="salt_conc">Concentration of divalent cations (millimolar).</param>
        /// <param name="divalent_conc">Concentration of divalent cations (millimolar).</param>
        /// <param name="dntp_conc">concentration of dNTPs (millimolar).</param>
        /// <param name="nn_max_len"> The maximum sequence length for
        /// using the nearest neighbor model
        /// (as implemented in oligotm.  For
        /// sequences longer than this, seqtm
        /// uses the "GC%" formula implemented
        /// in long_seq_tm.
        /// </param>
        /// <param name="tm_method">Melting themperature method. See <see cref="p3_tm_method_type"/></param>
        /// <param name="salt_corrections">Melting themperature method. See <see cref="p3_salt_correction_type"/></param>
        /// <returns>Oligo's melting temperature.</returns>
        [DllImport("tmthal.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern double p3_seqtm(String seq,
            double dna_conc,
            double salt_conc,
            double divalent_conc,
            double dntp_conc,
            int nn_max_len,
            p3_tm_method_type tm_method,
            p3_salt_correction_type salt_corrections
        );

        /// <summary>
        /// Primer3's thermodynamic sequence alignment.
        /// </summary>
        /// <param name="oligo_f">Forward oligo.</param>
        /// <param name="oligo_r">Reverse oligo.</param>
        /// <param name="a">Thal arguments. See <see cref="p3_thal_args"/></param>
        /// <param name="o">Thal results. See <see cref="thal_results"/></param>
        [DllImport("tmthal.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void p3_thal(String oligo_f,
            String oligo_r,
            ref p3_thal_args a,
            ref thal_results o);


        /// <summary>
        /// Loads the primer3's thermodynamic parametes for the primer3_config folder.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
       // [DllImport("tmthal.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
       // public static extern thal_results p3_get_thermodynamic_values(String path=".\\primer3_config\\");

    }
}
