using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bio;
using Bio.IO;

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
        public Overlap(String name, ISequence overhang_5, ISequence geneSpecific_3)
        {
            this.Name = name;
            this.TemplateSeq_3 = new Sequence(Alphabets.DNA, geneSpecific_3.ToString().ToUpper());
            this.TemplateSeq_5 = new Sequence(Alphabets.DNA, overhang_5.ToString().ToLower());
            this.Seq_3 = this.TemplateSeq_3;
            this.Seq_5 = this.TemplateSeq_5;
            this.Sequence = new Sequence(Alphabets.DNA, this.Seq_5.ToString() + this.Seq_3.ToString());
            this.TempInit();
            this.Temperature = GetMeltingTemperature(this.Sequence);
        }

        /// <summary>
        /// Overlap constructor.
        /// </summary>
        /// <param name="name">Overlap name.</param>
        /// <param name="primer">Primer sequence.</param>
        public Overlap(String name, ISequence primer)
            : this(name, new Sequence(Alphabets.DNA, ""), primer) { }


        /// <summary>
        /// Simple temperature computation initialization.
        /// </summary>
        private void TempInit()
        {
            this.SimpleT = new Dictionary<byte, double>();
            this.SimpleT.Add(Alphabets.DNA.A, 2.0);
            this.SimpleT.Add(Alphabets.DNA.T, 2.0);
            this.SimpleT.Add(Alphabets.DNA.G, 4.0);
            this.SimpleT.Add(Alphabets.DNA.C, 4.0);
            this.SimpleT.Add(Alphabets.DNA.Gap, 0.0);
        }


        /// <value>
        /// Nucleotide temperature dictionary.
        /// </value>
        private Dictionary<byte, double> SimpleT;

        /// <value>
        /// Overlap melting temperature.
        /// </value>
        public double Temperature { get; set; }

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
            String result = this.Name + sep + this.Sequence + sep + this.Temperature;
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
        public double GetSimpleMeltingTemperature(ISequence sequence)
        {
            double T = 0.0;
            Sequence upper = null;
            upper = new Sequence(Alphabets.DNA, sequence.ToString().ToUpper());

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
        public double GetMeltingTemperature(ISequence sequence)
        {
            double T = 0.0;
            Sequence upper = null;
            upper = new Sequence(Alphabets.DNA, sequence.ToString().ToUpper());
            unsafe
            {
                char* seq = (char*)System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(upper.ToString());
                T = Tm_thal.p3_seqtm(seq, 50.0, 50.0, 0.0, 0.0, 36, Tm_thal.p3_tm_method_type.p3_breslauer_auto, Tm_thal.p3_salt_correction_type.p3_schildkraut);
            }
            T = Math.Round(T, 2);
            if (T < -273.15)
            {
                T = 0.0;
            }
            return T;
        }

        /// <summary>
        /// Cut the first nucleotide off.
        /// </summary>
        /// <param name="minLen">Minimum overlap length.</param>
        /// <returns>First nucleotide or 255 if oligo too short to dequeue.</returns>
        public byte Dequeue(int minLen)
        {
            if (IsDequeueAllowed(minLen))
            {
                byte item = this.Seq_5[this.Seq_5.Count - 1];
                this.Seq_5 = this.Seq_5.GetSubSequence(1, this.Seq_5.Count - 1);
                this.Sequence = new Sequence(Alphabets.DNA, Seq_5.ToString() + Seq_3.ToString());
                this.Temperature = GetMeltingTemperature(Sequence);
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
        public byte Pop(int minLen)
        {
            if (IsPopAllowed(minLen))
            {
                byte item = this.Seq_3[0];
                this.Seq_3 = this.Seq_3.GetSubSequence(0, this.Seq_3.Count - 1);
                this.Sequence = new Sequence(Alphabets.DNA, Seq_5.ToString() + Seq_3.ToString());
                this.Temperature = GetMeltingTemperature(Sequence);
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
        private bool IsPopAllowed(int minLen)
        {
            //To modify.
            if (this.Seq_3.Count > minLen)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Check if deque is allowed.
        /// </summary>
        /// <returns>True if conditions satisfied.</returns>
        private bool IsDequeueAllowed(int minLen)
        {
            //To modify.
            if (this.Seq_5.Count > minLen)
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
