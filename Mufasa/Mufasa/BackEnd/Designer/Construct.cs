﻿using Bio;
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
            Overlaps.Add(new Overlap("",new Sequence(Alphabets.DNA,"")));
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
            ObservableCollection<Fragment > fragList = new ObservableCollection<Fragment>();
            for (int i = 0; i < nameList.Count; i++ )
            {
                Fragment f = fragDict[nameList[i]];
                if (i==0)
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
            for (int i = fragList.Count-1; i >= 0; i--)
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
            GreedyOptimizeOverlaps();
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
        /// Overlap temperature optimization.
        /// </summary>
        private void GreedyOptimizeOverlaps()
        {
            const byte end_3 = 255;
            const byte end_5 = 255;
            
            for (int i = 0; i < this.Overlaps.Count; i++)
            {
                byte item_3 = 0;
                byte item_5 = 0;
                bool done_3 = false;
                bool done_5 = false;

                bool tmTooHigh = (this.Overlaps[i].Temperature > this.Settings.TargetTm);
                do
                {
                    if ((item_5 != end_5) && tmTooHigh)
                    {
                        item_5 = this.Overlaps[i].Dequeue(Settings.MinLen_5);
                        tmTooHigh = (this.Overlaps[i].Temperature > this.Settings.TargetTm);
                    }
                    else
                    {
                        done_5 = true;
                    }

                    if ((item_3 != end_3) && tmTooHigh)
                    {
                        item_3 = this.Overlaps[i].Pop(Settings.MinLen_3);
                        tmTooHigh = (this.Overlaps[i].Temperature > this.Settings.TargetTm);
                    }
                    else
                    {
                        done_3 = true;
                    }

                } while (!done_5 || !done_3);

                //item = 0;
                //tmTooHigh = (this.Overlaps[i].Temperature > this.Settings.TargetTm);
                //while ((item != end) && tmTooHigh)
                //{
                //    // not vector primers
                //    item = this.Overlaps[i].Pop(Settings.MinLen_3);
                //    tmTooHigh = (this.Overlaps[i].Temperature > this.Settings.TargetTm);
                //}
            }
        }
    }
}
