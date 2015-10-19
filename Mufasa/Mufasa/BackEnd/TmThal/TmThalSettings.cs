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
        /// <param name="dnaConc">DNA concentration. Assume primer3's default.</param>
        /// <param name="dntpConc">dNTP-s concentration. (Qian &amp; Tian, 2014): 0.8 mM.</param>
        /// <param name="mvConc">Monovalent cations concentration. Assume phusion buffer.</param>
        /// <param name="dvConc">Divovalent cations concentration. Assume phusion buffer.</param>
        /// <param name="nnMaxLen"></param>
        /// <param name="tmMethod">Melting temperature method. See <see cref="TmThalSettings.TmMethod"/></param>
        /// <param name="saltMethod">Salt correction method. See <see cref="TmThalSettings.SaltCorrectionMethod"/></param>
        /// <param name="temperature">Reaction temperature. Annealing temperature for CPEC reaction (Qian &amp; Tian, 2014), as it is constant and lower than suggested annealing temperature for the PCR step (Tm + 3).</param>
        /// <param name="thalType">Thermodynamic alignment type. See <see cref="TmThalSettings.ThermoAlignmentType"/>. ANY by default.</param>
        /// <param name="maxLoop">Max hairpin loop size to consider.</param>
        public TmThalSettings(
            double dnaConc = 50.0,
            double dntpConc = 0.8,
            double mvConc = 50.0,
            double dvConc = 1.5,
            int nnMaxLen = 60,
            Thermodynamics.p3_tm_method_type tmMethod = Thermodynamics.p3_tm_method_type.p3_santalucia_auto,
            Thermodynamics.p3_salt_correction_type saltMethod = Thermodynamics.p3_salt_correction_type.p3_santalucia, 
            double temperature = 55,
            int maxLoop = 30)
        {

            Thermodynamics.p3_thal_args thalArgs = new Thermodynamics.p3_thal_args();
            Thermodynamics.p3_thal_args thalHairpinArgs = new Thermodynamics.p3_thal_args();

            unsafe
            {
                this.thalSettings = &thalArgs;
                this.thalHairpinSettings = &thalHairpinArgs;
            }


            this.DnaConcentration = dnaConc; 
            this.DntpConcentration = dntpConc; 
            this.MonovalentConcentration = mvConc; 
            this.DivalentConcentration = dvConc;

            this.NnMaxLen = nnMaxLen;
            this.TmMethod = tmMethod;
            this.SaltCorrectionMethod = saltMethod;

            this.ReactionTemperature = temperature; 
            this.MaxLoop = maxLoop;

            unsafe
            {
                this.thalSettings->debug = 0;
                this.thalSettings->temponly = 1;
                this.thalHairpinSettings->debug = 0;
                this.thalHairpinSettings->temponly = 1;

                //this.thalSettings->type = Thermodynamics.p3_thal_alignment_type.thal_any;
                //this.thalHairpinSettings->type = Thermodynamics.p3_thal_alignment_type.thal_hairpin;
            }


        }


        /// <summary>
        /// TmThal.p3_seqtm settings
        /// </summary>
        private Thermodynamics.p3_tm_args tmSettings;

        /// <summary>
        /// TmThal.p3_thal settings for duplex calculation
        /// </summary>
        public Thermodynamics.p3_tm_args TmSettings
        {
            get { return tmSettings; }
        }

        /// <summary>
        /// TmThal.p3_thal settings for duplex calculation
        /// </summary>
        private unsafe Thermodynamics.p3_thal_args *thalSettings;

        /// <summary>
        /// TmThal.p3_thal settings for duplex calculation
        /// </summary>
        public unsafe Thermodynamics.p3_thal_args *ThalSettings
        {
            get { return thalSettings; }
        }
        
        /// <summary>
        /// TmThal.p3_thal settings for hairpin calculation
        /// </summary>
        private unsafe Thermodynamics.p3_thal_args *thalHairpinSettings;

        /// <summary>
        /// TmThal.p3_thal settings for duplex calculation
        /// </summary>
        public unsafe Thermodynamics.p3_thal_args *ThalHairpinSettings
        {
            get { return thalHairpinSettings; }
        }

        /// <summary>
        /// Salt correction method. See <see cref="Thermodynamics.p3_salt_correction_type"/>
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
        public Thermodynamics.p3_salt_correction_type SaltCorrectionMethod
        {
            get
            {
                return this.tmSettings.salt_corrections;
            }
            set
            {
                this.tmSettings.salt_corrections = value;
            }
        }

        /// <summary>
        /// Melting temperature computation method. See <see cref="Thermodynamics.p3_tm_method_type"/>
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
        public Thermodynamics.p3_tm_method_type TmMethod
        {
            get
            {
                return this.tmSettings.tm_method;
            }
            set
            {
                this.tmSettings.tm_method = value;
            }
        }

        /// <summary>
        /// Maximum size of loop to consider; longer than 30 bp are not allowed.
        /// </summary>
        public int MaxLoop
        {
            get
            {
                unsafe
                {
                    return this.thalSettings->maxLoop;
                }
            }
            set
            {
                unsafe
                {
                    this.thalSettings->maxLoop = value;
                    this.thalHairpinSettings->maxLoop = value;
                }
            }
        }

        /// <summary>
        /// Max oligo length for Nearest Neighbor-model computations.
        /// </summary>
        /// <remarks>
        /// Default = 60. 
        /// The rationale behind this value (60) is that this is the maxium
        /// reasonable length for nearest neighbor models. It is the maxium
        /// length at which we can restrict our model to only two states of
        /// melting: fully intact duplex or completely dissociated single
        /// strands. 
        /// 
        /// (But: defined as MAX_PRIMER_LENGTH = 36 in primer3's libprimer.c for melting temperature computations)
        ///
        /// Both functions return the melting temperature of the given oligo
        /// calculated as specified by user, NN-model _should_ only be used on
        /// DNA sequences of length &lt;= NnMaxLen.
        /// seqtm uses NN-model for sequences of length &lt;= NnMaxLen,
        /// and a different, G+C% based formula for longer
        /// sequences.  For NN-model, no error is generated on sequences
        /// longer than NnMaxLen, but the formula becomes less
        /// accurate as the sequence grows longer.  Caveat emptor.
        /// 
        /// If oligo length &gt; NnMaxLen,
        /// calculate the melting temperature of substr(seq, start, length) using the
        /// formula from Bolton and McCarthy, PNAS 84:1390 (1962) as presented in
        /// Sambrook, Fritsch and Maniatis, Molecular Cloning, p 11.46 (1989, CSHL
        /// Press).
        /// 
        /// Tm = 81.5 + 16.6(log10([Na+])) + .41*(%GC) - 600/length
        /// 
        /// Where [Na+] is the molar sodium concentration, (%GC) is the percent of Gs
        /// and Cs in the sequence, and length is the length of the sequence.
        /// </remarks>  
        public int NnMaxLen
        {
            get
            {
                return this.tmSettings.nn_max_len;
            }
            set
            {
                this.tmSettings.nn_max_len = value;
            }
        }

        /// <summary>
        /// Temperature from which hairpin structures will be calculated.
        /// </summary>
        public double ReactionTemperature
        {
            get
            {
                unsafe
                {
                    return this.thalSettings->temp;
                }
            }
            set
            {
                unsafe
                {
                    this.thalSettings->temp = value;
                    this.thalHairpinSettings->temp = value;
                }
            }
        }


        /// <summary>
        /// DNA concentration.
        /// </summary>
        public double DnaConcentration 
        {
            get
            {
                return this.tmSettings.dna_conc;
            }
            set
            {
                this.tmSettings.dna_conc = value;
                unsafe
                {
                    this.thalSettings->dna_conc = value;
                    this.thalHairpinSettings->dna_conc = value;
                }
            }
        }
    
        /// <summary>
        /// dNTP concentration.
        /// </summary>
        public double DntpConcentration
        {
            get
            {
                return this.tmSettings.dntp_conc;
            }
            set
            {
                this.tmSettings.dntp_conc = value;
                unsafe
                {
                    this.thalSettings->dntp = value;
                    this.thalHairpinSettings->dntp = value;
                }
            }
        }
        
        /// <summary>
        /// Divalent [Mg2+] cations concentration.
        /// </summary>
        public double DivalentConcentration
        {
            get
            {
                return this.tmSettings.divalent_conc;
            }
            set
            {
                this.tmSettings.divalent_conc = value;
                unsafe
                {
                    this.thalSettings->dv = value;
                    this.thalHairpinSettings->dv = value;
                }
            }
        }

        /// <summary>
        /// Monovalent [Na+/K+] cations concentration.
        /// </summary>
        public double MonovalentConcentration
        {
            get
            {
                return this.tmSettings.salt_conc;
            }
            set
            {
                this.tmSettings.salt_conc = value;
                unsafe
                {
                    this.thalSettings->mv = value;
                    this.thalHairpinSettings->mv = value;
                }
            }
        }
    }
}
