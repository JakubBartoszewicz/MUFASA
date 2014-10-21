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
        /// Construct constructor.
        /// </summary>
        /// <param name="source">Filename or URL.</param>
        /// <param name="name">Construct name.</param>
        public Construct(String source, String name, ISequence sequence) : base(source,name,sequence)
        {
            
        }

        /// <summary>
        /// Construct constructor.
        /// </summary>
        public Construct()
            : base()
        {

        }

        /// <summary>
        /// Construct constructor.
        /// </summary>
        /// <param name="fragList">Fragment list.</param>
        public Construct(ObservableCollection<Fragment> fragList)
            : base()
        {
            Init(fragList);
        }

        /// <summary>
        /// Construct constructor.
        /// </summary>
        /// <param name="fragDict">Fragment Dictionary.</param>
        /// <param name="nameList">Fragment names. Dictionary keys.</param>
        public Construct(ObservableCollection<String> nameList, Dictionary<String, Fragment> fragDict)
            : base()
        {
            ObservableCollection<Fragment > fragList = new ObservableCollection<Fragment>();
            foreach (String name in nameList)
            {
                fragList.Add(fragDict[name]);
            }
            Init(fragList);
        }


        /// <summary>
        /// Construct initialization.
        /// </summary>
        /// <param name="fragList">Fragment list.</param>
        /// <param name="minLen">Minimum overlap length.</param>
        private void Init(ObservableCollection<Fragment> fragList, int minLen = 20)
        {
            this.Overlaps = new List<Overlap>();
            
            //forward
            String seq5 = "";
            String seq3 = "";
            String overlap = "";
            String name = "";
            List<MiscFeature> featList = new List<MiscFeature>();
            for (int i = 0; i < fragList.Count; i++)
            {
                name += fragList[i].Name;
                seq3 = fragList[i].GetString();
                int len5 = Math.Min(minLen, seq5.Length);
                int len3 = Math.Min(minLen, seq3.Length);
                overlap = seq5.Substring(seq5.Length - len5, len5) + seq3.Substring(0, len3);
                String loc = (seq5.Length + 1).ToString() + ".." + (seq5.Length + seq3.Length).ToString();
                MiscFeature gene = new MiscFeature(loc);
                gene.StandardName = fragList[i].Name;
                featList.Add(gene);
                seq5 += seq3;
                Overlaps.Add(new Overlap(fragList[i].Name + "_fwd", new Sequence(Alphabets.DNA, overlap)));                     
            }

            this.Sequence = new Sequence(Alphabets.DNA, seq5);
            //meta
            GenBankMetadata meta = new GenBankMetadata();
            meta.Locus = new GenBankLocusInfo();
            meta.Locus.MoleculeType = MoleculeType.DNA;
            meta.Locus.Name = name;
            meta.Locus.Date = System.DateTime.Now;
            meta.Locus.SequenceLength = seq5.Length;
            meta.Source = new SequenceSource();
            meta.Source.Organism = new OrganismInfo();
            meta.Source.Organism.Species = "synthetic";
            meta.Comments.Add("designed with mufasa");
            meta.Definition = "synthetic construct";
            meta.Features = new SequenceFeatures();
            meta.Features.All.AddRange(featList);
            this.Sequence.Metadata.Add("GenBank", meta);

            //reverse
            fragList.Add(new Fragment(fragList[0]));
            fragList.RemoveAt(0);
            seq5 = "";
            seq3 = "";
            overlap = "";
            for (int i = fragList.Count-1; i >= 0; i--)
            {
                seq5 = fragList[i].GetReverseComplementString();
                int len3 = Math.Min(minLen, seq3.Length);
                int len5 = Math.Min(minLen, seq5.Length);
                overlap = seq3.Substring(seq3.Length - len3, len3) + seq5.Substring(0, len5);
                seq3 += seq5;
                Overlaps.Add(new Overlap(fragList[i].Name + "_rev", new Sequence(Alphabets.DNA, overlap)));
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
