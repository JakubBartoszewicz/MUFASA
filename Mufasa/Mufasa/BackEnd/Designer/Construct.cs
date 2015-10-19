using Bio;
using Bio.IO;
using Bio.IO.GenBank;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mufasa.BackEnd.TmThal;

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
            Overlaps.Add(new Overlap("", new Sequence(Alphabets.AmbiguousDNA, ""), new TmThalSettings()));
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

            int pairIndex;
            int len_5;
            int len_3;

            for (int i = 0; i < fragList.Count; i++)
            {
                name += fragList[i].Name;
                seq_3 = fragList[i].GetString();
                len_5 = Math.Min(settings.MaxLen_5, seq_5.Length);
                len_3 = Math.Min(settings.MaxLen_3, seq_3.Length);
                String overhang_5 = seq_5.Substring(seq_5.Length - len_5, len_5);
                String geneSpecific_3 = seq_3.Substring(0, len_3);
                String loc = (seq_5.Length + 1).ToString() + ".." + (seq_5.Length + seq_3.Length).ToString();
                MiscFeature gene = new MiscFeature(loc);
                gene.StandardName = fragList[i].Name;
                featList.Add(gene);
                seq_5 += seq_3;


                if (i == 0)
                {
                    pairIndex = fragList.Count;
                    Overlaps.Add(new Overlap(fragList[i].Name + "-fwd", new Sequence(Alphabets.AmbiguousDNA, geneSpecific_3), settings.TmThalSettings, pairIndex));
                }
                else
                {
                    pairIndex = 2 * fragList.Count - i;
                    Overlaps.Add(new Overlap(fragList[i].Name + "-fwd", new Sequence(Alphabets.AmbiguousDNA, overhang_5), new Sequence(Alphabets.AmbiguousDNA, geneSpecific_3), settings.TmThalSettings, pairIndex));
                }
            }

            this.Sequence = new Sequence(Alphabets.AmbiguousDNA, seq_5);
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
                len_5 = Math.Min(settings.MaxLen_5, seq_3.Length);
                len_3 = Math.Min(settings.MaxLen_3, seq_5.Length);
                String overhang_5 = seq_3.Substring(seq_3.Length - len_5, len_5);
                String geneSpecific_3 = seq_5.Substring(0, len_3);
                seq_3 += seq_5;

                if (i == fragList.Count - 1)
                {
                    pairIndex = 0;
                    Overlaps.Add(new Overlap(fragList[i].Name + "-rev", new Sequence(Alphabets.AmbiguousDNA, geneSpecific_3), settings.TmThalSettings, pairIndex));
                }
                else
                {
                    pairIndex = i + 1;
                    Overlaps.Add(new Overlap(fragList[i].Name + "-rev", new Sequence(Alphabets.AmbiguousDNA, overhang_5), new Sequence(Alphabets.AmbiguousDNA, geneSpecific_3), settings.TmThalSettings, pairIndex));
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

    }
}
