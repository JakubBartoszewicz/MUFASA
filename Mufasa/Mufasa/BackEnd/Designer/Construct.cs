using Bio;
using Bio.IO;
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
        private void Init(ObservableCollection<Fragment> fragList)
        {
            this.Overlaps = new Dictionary<string, string>();
            String seq5 = "";
            String seq3 = "";
            String overlap = "";
            foreach (Fragment f in fragList)
            {
                seq3 = f.GetString();
                int len5 = Math.Min(15, seq5.Length);
                int len3 = Math.Min(15, seq3.Length);
                overlap = seq5.Substring(seq5.Length - len5, len5) + seq3.Substring(0, len3);
                seq5 += seq3;
                Overlaps.Add(f.Name + "_fwd", overlap);
                Overlaps.Add(f.Name + "_rev", new Sequence(Alphabets.DNA, overlap).GetReverseComplementedSequence().ToString());
            }

            this.Sequence = new Sequence(Alphabets.DNA, seq5);


        }

        /// <value>
        /// Generated overlaps collection.
        /// </value>
        public Dictionary<String, String> Overlaps { get; set; }


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
