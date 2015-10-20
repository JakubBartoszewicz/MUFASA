using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Bio;
using Bio.IO;
using Mufasa.BackEnd.TmThal;

namespace Mufasa.BackEnd.Designer
{
    /// <remarks>
    /// Overlap class.
    /// </remarks>
    class Overlap : Fragment
    {
        /// <summary>
        /// Overlap constructor.
        /// </summary>
        /// <param name="name">Overlap name.</param>
        /// <param name="overhang_5">Overhang sequence.</param>
        /// <param name="geneSpecific_3">Gene specific sequence.</param>
        public Overlap(String name, ISequence overhang_5, ISequence geneSpecific_3, TmThalSettings settings, int pairIndex = -1)
        {
            this.PairIndex = pairIndex;
            this.Settings = settings;
            this.Name = name;
            this.TemplateSeq_3 = new Sequence(Alphabets.AmbiguousDNA, geneSpecific_3.ToString().ToUpper());
            this.TemplateSeq_5 = new Sequence(Alphabets.AmbiguousDNA, overhang_5.ToString().ToLower());
            this.Seq_3 = this.TemplateSeq_3;
            this.Seq_5 = this.TemplateSeq_5;
            this.Sequence = new Sequence(Alphabets.AmbiguousDNA, this.Seq_5.ToString() + this.Seq_3.ToString());
            this.TempInit();
            this.meltingTemperature = GetMeltingTemperature();
            this.hairpinMeltingTemperature = GetHairpinTemperature();
        }

        /// <summary>
        /// Overlap constructor.
        /// </summary>
        /// <param name="name">Overlap name.</param>
        /// <param name="primer">Primer sequence.</param>
        public Overlap(String name, ISequence primer, TmThalSettings settings, int pairIndex = -1)
            : this(name, new Sequence(Alphabets.AmbiguousDNA, ""), primer, settings, pairIndex) { }

        /// <summary>
        /// Overlap copying constructor.
        /// </summary>
        /// <param name="overlap">Overlap.</param>
        public Overlap(Overlap overlap)
            : this(overlap.Name, overlap.Seq_5, overlap.Seq_3, overlap.Settings, overlap.PairIndex)
        { }

        /// <summary>
        /// Simple temperature computation initialization.
        /// </summary>
        private void TempInit()
        {
            this.SimpleT = new Dictionary<byte, double>();
            this.SimpleT.Add(Alphabets.AmbiguousDNA.A, 2.0);
            this.SimpleT.Add(Alphabets.AmbiguousDNA.T, 2.0);
            this.SimpleT.Add(Alphabets.AmbiguousDNA.G, 4.0);
            this.SimpleT.Add(Alphabets.AmbiguousDNA.C, 4.0);
            this.SimpleT.Add(Alphabets.AmbiguousDNA.Gap, 0.0);
        }

        /// <summary>
        /// Paired overlap index.
        /// </summary>
        public int PairIndex { get; set; }

        /// <value>
        /// Nucleotide temperature dictionary.
        /// </value>
        private Dictionary<byte, double> SimpleT;

        /// <value>
        /// Overlap melting temperature.
        /// </value>
        public double MeltingTemperature { get { return meltingTemperature; } }

        /// <value>
        /// Overlap melting temperature.
        /// </value>
        private double meltingTemperature;

        /// <summary>
        /// Overlap's hairpin melting temperature.
        /// </summary>
        public double HairpinMeltingTemperature { get { return hairpinMeltingTemperature; } }

        /// <summary>
        /// Overlap's hairpin melting temperature.
        /// </summary>
        private double hairpinMeltingTemperature;

        /// <summary>
        /// Settings for thermodynamic evaluation.
        /// </summary>
        public TmThalSettings Settings { get; set; }

        /// <value>
        /// 3' ("gene-specific") subsequence template.
        /// </value>
        public ISequence TemplateSeq_3 { get; set; }

        /// <value>
        /// 5' ("overhang") subsequence template.
        /// </value>
        public ISequence TemplateSeq_5 { get; set; }

        /// <value>
        /// 3' ("gene-specific") subsequence.
        /// </value>
        public ISequence Seq_3 { get; set; }

        /// <value>
        /// 5' ("overhang") subsequence.
        /// </value>
        public ISequence Seq_5 { get; set; }

        /// <summary>
        /// Prints the overlap in a human-readable format.
        /// </summary>
        /// <returns>String represanting the overlap.</returns>
        public override string ToString()
        {
            String sep = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            String result = this.Name + sep + this.MeltingTemperature + sep + this.HairpinMeltingTemperature + sep + this.Sequence.Count + sep + this.Sequence;
            return result;
        }


        /// <value>
        /// Sequence string.
        /// </value>
        public String SequenceString
        {
            get { return this.Sequence.ToString(); }
        }


        /// <summary>
        /// Compute overlap's simple-style melting temperature.
        /// </summary>
        /// <returns>Overlap's Tm.</returns>
        public double GetSimpleMeltingTemperature()
        {
            double T = 0.0;
            Sequence upper = null;
            upper = new Sequence(Alphabets.AmbiguousDNA, this.Sequence.ToString().ToUpper());

            for (long index = 0; index < upper.Count; index++)
            {
                T += SimpleT[upper[index]];
            }
            return T;
        }

        /// <summary>
        /// Compute overlap's NN-model melting temperature.
        /// </summary>
        /// <returns>Overlap's Tm.</returns>
        private double GetMeltingTemperature()
        {
            double T = 0.0;
            Sequence upper = null;
            upper = new Sequence(Alphabets.AmbiguousDNA, this.Sequence.ToString().ToUpper());


            T = Thermodynamics.p3_seqtm(upper.ToString(), this.Settings.DnaConcentration,
                this.Settings.MonovalentConcentration,
                this.Settings.DivalentConcentration,
                this.Settings.DntpConcentration,
                this.Settings.NnMaxLen,
                this.Settings.TmMethod,
                this.Settings.SaltCorrectionMethod);


            return T;
        }


        /// <summary>
        /// Compute overlap's hairpin melting temperature.
        /// </summary>
        /// <returns>Hairpin melting temperature.</returns>
        private double GetHairpinTemperature()
        {
            double T = 0.0;
            Sequence upper = null;
            upper = new Sequence(Alphabets.AmbiguousDNA, this.Sequence.ToString().ToUpper());


            Thermodynamics.thal_results results = new Thermodynamics.thal_results();
            Thermodynamics.p3_thal_args args = this.Settings.ThalHairpinSettings;

            Thermodynamics.p3_thal(upper.ToString(), upper.ToString(), ref args, ref results);

            T = results.temp;


            return T;
        }

        /// <summary>
        /// Cut the first nucleotide off.
        /// </summary>
        /// <param name="minLen">Minimum overhang length.</param>
        /// <returns>First nucleotide or 255 if oligo too short to dequeue.</returns>
        public byte Dequeue(int minLen, int length = 1)
        {
            if (IsDequeueAllowed(minLen, length))
            {
                long index = length;
                byte item = this.Seq_5[length];
                this.Seq_5 = this.Seq_5.GetSubSequence(index, this.Seq_5.Count - index);
                this.Sequence = new Sequence(Alphabets.AmbiguousDNA, Seq_5.ToString() + Seq_3.ToString());
                this.meltingTemperature = GetMeltingTemperature();
                this.hairpinMeltingTemperature = GetHairpinTemperature();
                return item;
            }
            else
            {
                return 255;
            }
        }

        /// <summary>
        /// Cut the last nucleotide of.
        /// </summary>
        /// <param name="minLen">Minimum primer length.</param>
        /// <returns>Last nucleotide or 255 if oligo too short to pop.</returns>
        public byte Pop(int minLen, int length = 1)
        {
            if (IsPopAllowed(minLen, length))
            {
                long index = this.Seq_3.Count - length;
                byte item = this.Seq_3[index];
                this.Seq_3 = this.Seq_3.GetSubSequence(0, index);
                this.Sequence = new Sequence(Alphabets.AmbiguousDNA, Seq_5.ToString() + Seq_3.ToString());
                this.meltingTemperature = GetMeltingTemperature();
                this.hairpinMeltingTemperature = GetHairpinTemperature();
                return item;
            }
            else
            {
                return 255;
            }
        }

        /// <summary>
        /// Add a nucleotide to the oligo's 3' end.
        /// </summary>
        /// <param name="maxLen">Maximum primer length.</param>
        /// <returns>New nucleotide or 255 if oligo too long to push.</returns>
        public byte Push(int maxLen, int length = 1)
        {
            if (IsPushAllowed(maxLen, length))
            {
                long index = this.Seq_3.Count + length - 1;
                byte item = this.TemplateSeq_3[index];
                this.Seq_3 = new Sequence(Alphabets.AmbiguousDNA, Seq_3.ToString() + this.TemplateSeq_3.GetSubSequence(this.Seq_3.Count, length));
                this.Sequence = new Sequence(Alphabets.AmbiguousDNA, Seq_5.ToString() + Seq_3.ToString());
                this.meltingTemperature = GetMeltingTemperature();
                this.hairpinMeltingTemperature = GetHairpinTemperature();
                return item;
            }
            else
            {
                return 255;
            }
        }

        /// <summary>
        /// Add a nucleotide to the oligo's 5' end.
        /// </summary>
        /// <param name="maxLen">Maximum overhang length.</param>
        /// <returns>New nucleotide or 255 if oligo too long to enqueue.</returns>
        public byte Enqueue(int maxLen, int length = 1)
        {
            if (IsEnqueueAllowed(maxLen, length))
            {
                long index = this.TemplateSeq_5.Count - this.Seq_5.Count - length;
                byte item = this.TemplateSeq_5[index];
                this.Seq_5 = new Sequence(Alphabets.AmbiguousDNA, this.TemplateSeq_5.GetSubSequence(index, length) + Seq_5.ToString());
                this.Sequence = new Sequence(Alphabets.AmbiguousDNA, Seq_5.ToString() + Seq_3.ToString());
                this.meltingTemperature = GetMeltingTemperature();
                this.hairpinMeltingTemperature = GetHairpinTemperature();
                return item;
            }
            else
            {
                return 255;
            }
        }

        /// <summary>
        /// Check if pop is allowed.
        /// </summary>
        /// <returns>True if conditions satisfied.</returns>
        private bool IsPopAllowed(int minLen, int length = 1)
        {
            //To modify.
            if (this.Seq_3.Count - length > minLen)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Check if dequeue is allowed.
        /// </summary>
        /// <returns>True if conditions satisfied.</returns>
        private bool IsDequeueAllowed(int minLen, int length = 1)
        {
            //To modify.
            if (this.Seq_5.Count - length > minLen)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if enqueue is allowed.
        /// </summary>
        /// <returns>True if conditions satisfied.</returns>
        private bool IsEnqueueAllowed(int maxLen, int length = 1)
        {
            //To modify.
            if (this.Seq_5.Count + length - 1 < maxLen && this.Seq_5.Count + length - 1 < this.TemplateSeq_5.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if push is allowed.
        /// </summary>
        /// <returns>True if conditions satisfied.</returns>
        private bool IsPushAllowed(int maxLen, int length = 1)
        {
            //To modify.
            if (this.Seq_3.Count + length - 1 < maxLen && this.Seq_3.Count + length - 1 < this.TemplateSeq_3.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
