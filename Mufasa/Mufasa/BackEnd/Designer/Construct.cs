﻿using Bio;
using Bio.IO;
using Bio.IO.GenBank;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mufasa.BackEnd.TmThal;
using Mufasa.BackEnd.Exceptions;
using Mufasa.BackEnd.Scores;

//Copyright (C) 2014, 2015 Jakub Bartoszewicz (if not stated otherwise)
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
        /// Pre-optimized Construct constructor.
        /// </summary>
        /// <param name="fragList">Fragment list.</param>
        public Construct(List<Overlap> overlaps, ISequence sequence, DesignerSettings settings)
            : base()
        {
            this.Overlaps = overlaps;
            this.Sequence = sequence;
            this.Settings = settings;
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
        /// <param name="constructionList">Fragment names. Dictionary keys.</param>
        public Construct(ObservableCollection<String> constructionList, Dictionary<String, Fragment> fragDict, DesignerSettings settings)
            : base()
        {
            ObservableCollection<String> nameList = new ObservableCollection<String>(constructionList);

            for (int i = 0; i < nameList.Count; i++)
            {
                nameList[i] = nameList[i].Replace(Designer.VectorLabel, "");
            }

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
        /// Total construct score.
        /// </value>
        public ScoreTotal Score { get; set; }


        /// <value>
        /// Designer settings.
        /// </value>
        public DesignerSettings Settings { get; set; }


        /// <value>
        /// Generated overlaps collection.
        /// </value>
        public List<Overlap> Overlaps { get; set; }

        /// <summary>
        /// Construct initialization.
        /// </summary>
        /// <param name="fragList">Fragment list.</param>
        /// <param name="maxOverlapLen">Minimum overlap length.</param>
        private void Init(ObservableCollection<Fragment> fragList, DesignerSettings settings)
        {
            this.Overlaps = new List<Overlap>();
            this.Settings = settings;

            Thermodynamics.thal_results results = new Thermodynamics.thal_results();
            Thermodynamics.p3_get_thermodynamic_values(Settings.TmThalParamPath, ref results);
            String message = new String(results.msg);
            message = message.Trim('\0');

            if (!String.IsNullOrEmpty(message))
            {
                throw new TmThalParamException(message);
            }

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
                    Overlaps.Add(new Overlap(Designer.VectorLabel + fragList[i].Name + "-fwd", new Sequence(Alphabets.AmbiguousDNA, geneSpecific_3), settings.TmThalSettings, pairIndex));
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
                    Overlaps.Add(new Overlap(Designer.VectorLabel + fragList[i].Name + "-rev", new Sequence(Alphabets.AmbiguousDNA, geneSpecific_3), settings.TmThalSettings, pairIndex));
                }
                else
                {
                    pairIndex = i + 1;
                    Overlaps.Add(new Overlap(fragList[i].Name + "-rev", new Sequence(Alphabets.AmbiguousDNA, overhang_5), new Sequence(Alphabets.AmbiguousDNA, geneSpecific_3), settings.TmThalSettings, pairIndex));
                }
            }

            for (int i = 0; i < fragList.Count; i++)
            {
                //Duplex melting temperatures
                Overlaps[i].HeterodimerMeltingTemperature = Overlaps[i].GetDuplexTemperature(Overlaps[Overlaps[i].PairIndex]);
            }
        }

        /// <summary>
        /// Checks if the construct is empty.
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
        /// Compute construct score.
        /// </summary>
        /// <returns>Total construct score.</returns>
        public ScoreTotal Evaluate(bool ignoreHeterodimers = false)
        {
            if (this.IsAcceptable(ignoreHeterodimers))
                this.Score = new ScoreTotal(this.Overlaps, this.Settings.TargetTm);
            else
                this.Score = ScoreTotal.Inacceptable;

            return this.Score;
        }

        /// <summary>
        /// Check if this is an acceptable solution.
        /// </summary>
        /// <param name="overlaps"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        private bool IsAcceptable(bool ignoreHeterodimers = false)
        {
            bool accept = true;
            foreach (Overlap o in this.Overlaps)
            {
                if (!o.IsAcceptable(this.Settings.MaxTh, this.Settings.MaxTd, ignoreHeterodimers))
                {
                    accept = false;
                    break;
                }
            }
            return accept;
        }
    }
}
