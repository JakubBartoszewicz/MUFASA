using Bio;
using Bio.IO;
using Bio.IO.GenBank;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mufasa.BackEnd.Designer
{
    /// <remarks>
    /// Genetic construct class.
    /// </remarks>
    class Construct : Fragment
    {
        /// <summary>
        /// Empty Construct constructor.
        /// </summary>
        /// <param name="fragList">Fragment list.</param>
        public Construct()
            : base()
        {
            this.Overlaps = new List<Overlap>();
            Overlaps.Add(new Overlap("", new Sequence(Alphabets.DNA, "")));
        }

        /// <summary>
        /// Construct constructor.
        /// </summary>
        /// <param name="fragList">Fragment list.</param>
        public Construct(ObservableCollection<Fragment> fragList, DesignerSettings settings)
            : base()
        {
            Init(fragList, settings);
        }

        /// <summary>
        /// Construct constructor.
        /// </summary>
        /// <param name="fragDict">Fragment Dictionary.</param>
        /// <param name="nameList">Fragment names. Dictionary keys.</param>
        public Construct(ObservableCollection<String> nameList, Dictionary<String, Fragment> fragDict, DesignerSettings settings)
            : base()
        {
            ObservableCollection<Fragment> fragList = new ObservableCollection<Fragment>();
            for (int i = 0; i < nameList.Count; i++)
            {
                Fragment f = fragDict[nameList[i]];
                if (i == 0)
                {
                    f.IsVector = true;
                }
                fragList.Add(f);
            }
            Init(fragList, settings);
        }

        /// <value>
        /// Designer settings.
        /// </value>
        public DesignerSettings Settings { get; set; }

        /// <summary>
        /// Construct initialization.
        /// </summary>
        /// <param name="fragList">Fragment list.</param>
        /// <param name="maxOverlapLen">Minimum overlap length.</param>
        private void Init(ObservableCollection<Fragment> fragList, DesignerSettings settings)
        {
            this.Overlaps = new List<Overlap>();
            this.Settings = settings;
            //forward
            String seq_5 = "";
            String seq_3 = "";
            String name = "";
            List<MiscFeature> featList = new List<MiscFeature>();
            for (int i = 0; i < fragList.Count; i++)
            {
                name += fragList[i].Name;
                seq_3 = fragList[i].GetString();
                int len5 = Math.Min(settings.MaxLen_5, seq_5.Length);
                int len3 = Math.Min(settings.MaxLen_3, seq_3.Length);
                String overhang_5 = seq_5.Substring(seq_5.Length - len5, len5);
                String geneSpecific_3 = seq_3.Substring(0, len3);
                String loc = (seq_5.Length + 1).ToString() + ".." + (seq_5.Length + seq_3.Length).ToString();
                MiscFeature gene = new MiscFeature(loc);
                gene.StandardName = fragList[i].Name;
                featList.Add(gene);
                seq_5 += seq_3;
                if (i == 0)
                {
                    Overlaps.Add(new Overlap(fragList[i].Name + "-fwd", new Sequence(Alphabets.DNA, geneSpecific_3)));
                }
                else
                {

                    Overlaps.Add(new Overlap(fragList[i].Name + "-fwd", new Sequence(Alphabets.DNA, overhang_5), new Sequence(Alphabets.DNA, geneSpecific_3)));
                }
            }

            this.Sequence = new Sequence(Alphabets.DNA, seq_5);
            //meta
            GenBankMetadata meta = new GenBankMetadata();
            meta.Locus = new GenBankLocusInfo();
            meta.Locus.MoleculeType = MoleculeType.DNA;
            meta.Locus.Name = name;
            meta.Locus.Date = System.DateTime.Now;
            meta.Locus.SequenceLength = seq_5.Length;
            meta.Comments.Add("designed with mufasa");
            meta.Definition = "synthetic construct";
            meta.Features = new SequenceFeatures();
            meta.Features.All.AddRange(featList);
            this.Sequence.Metadata.Add("GenBank", meta);

            //reverse
            fragList.Add(new Fragment(fragList[0]));
            fragList.RemoveAt(0);
            seq_5 = "";
            seq_3 = "";
            for (int i = fragList.Count - 1; i >= 0; i--)
            {
                seq_5 = fragList[i].GetReverseComplementString();
                int len_5 = Math.Min(settings.MaxLen_5, seq_3.Length);
                int len_3 = Math.Min(settings.MaxLen_3, seq_5.Length);
                String overhang_5 = seq_3.Substring(seq_3.Length - len_5, len_5);
                String geneSpecific_3 = seq_5.Substring(0, len_3);
                seq_3 += seq_5;
                if (i == fragList.Count - 1)
                {
                    Overlaps.Add(new Overlap(fragList[i].Name + "-rev", new Sequence(Alphabets.DNA, geneSpecific_3)));
                }
                else
                {

                    Overlaps.Add(new Overlap(fragList[i].Name + "-rev", new Sequence(Alphabets.DNA, overhang_5), new Sequence(Alphabets.DNA, geneSpecific_3)));
                }
            }
        }

        /// <summary>
        /// Cheks if the construct is empty.
        /// <summary>
        public bool IsEmpty()
        {
            if (this.Sequence == null)
            {
                return true;
            }
            else
            {
                if (this.Sequence.Count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <value>
        /// Generated overlaps collection.
        /// </value>
        public List<Overlap> Overlaps { get; set; }


        /// <summary>
        /// Save in one of .NET Bio supported formats like fasta or GenBank.
        /// </summary>
        /// <param name="path">Filename.</param>
        public void SaveAsBio(String path)
        {
            ISequenceFormatter formatter = SequenceFormatters.FindFormatterByFileName(path);
            formatter.Write(this.Sequence);
            formatter.Close();
        }

        /// <summary>
        /// Overlap Greedy temperature optimization.
        /// </summary>
        /// <summary>
        /// Overlap greedy temperature optimization.
        /// </summary>
        public void GreedyOptimizeOverlaps()
        {
            const byte end_3 = 255;
            const byte end_5 = 255;

            for (int i = 0; i < this.Overlaps.Count; i++)
            {
                byte item_3 = 0;
                byte item_5 = 0;
                bool done_3 = false;
                bool done_5 = false;

                double _diff = this.Overlaps[i].Temperature - this.Settings.TargetTm;
                double diff = _diff;
                Overlap _previous;

                do
                {
                    if ((item_5 != end_5))
                    {
                        _previous = new Overlap(this.Overlaps[i]);
                        item_5 = this.Overlaps[i].Dequeue(Settings.MinLen_5);
                        diff = this.Overlaps[i].Temperature - this.Settings.TargetTm;

                        //if previous was more optimal, do not accept the candidate solution and stop
                        if (Math.Abs(_diff) < Math.Abs(diff))
                        {
                            this.Overlaps[i] = _previous;
                            diff = _diff;
                            done_5 = true;
                        }
                        else
                        {
                            _diff = diff;
                            done_5 = false;
                        }
                    }
                    else
                    {
                        done_5 = true;
                    }

                    if ((item_3 != end_3))
                    {
                        _previous = new Overlap(this.Overlaps[i]);
                        item_3 = this.Overlaps[i].Pop(Settings.MinLen_3);
                        diff = this.Overlaps[i].Temperature - this.Settings.TargetTm;

                        //if previous was more optimal, do not accept the candidate solution and stop
                        if (Math.Abs(_diff) < Math.Abs(diff))
                        {
                            this.Overlaps[i] = _previous;
                            diff = _diff;
                            done_3 = true;
                        }
                        else
                        {
                            _diff = diff;
                            done_3 = false;
                        }
                    }
                    else
                    {
                        done_3 = true;
                    }

                } while (!done_5 || !done_3);
            }
        }
    }
}
