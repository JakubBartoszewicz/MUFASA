using Mufasa.BackEnd.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Bio.IO;
using Bio;
using FirstFloor.ModernUI.Windows.Controls;
using System.Windows;

namespace Mufasa.BackEnd.Designer
{
    class Designer
    {
        /// <summary>
        /// Designer constructor.
        /// </summary>
        public Designer()
        {
            FragmentDict = new Dictionary<String, Fragment>();
        }

        /// <summary>
        /// Fragment file parser
        /// </summary>
        private ISequenceParser parser;

        /// <summary>
        /// Dictionary of fragments.
        /// </summary>
        public Dictionary<String,Fragment> FragmentDict { get; set; }

        /// <summary>
        /// Add Fragment <paramref name="name"/> if valid.
        /// </summary>
        /// <param name="file">Fragment filename</param>
        /// <param name="name">Fragment name</param>
        public void AddFragment(String file, String name)
        {
            if (this.FragmentDict.Keys.Contains(name))
            {
                throw new FragmentNamingException(name);
            }
            ISequence sequence = null;
            try
            {
                sequence = ParseFragment(file);
                this.FragmentDict.Add(name, new Fragment(file, name, sequence));
            }
            catch(SequenceLengthException sle)
            {
                MessageBoxResult result = ModernDialog.ShowMessage(sle.Message + Environment.NewLine + "Do you really want to use it?", "warning", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    this.FragmentDict.Add(name, new Fragment(file, name, sle.Sequence));
                }
                else
                {
                    return;
                }                
            }
            
            
        }

        private ISequence ParseFragment(String file)
        {
            parser = SequenceParsers.FindParserByFileName(file);
            if (parser == null)
            {
                throw new FileFormatException("Parser for " + file + " not found.");
            }
            List<ISequence> sequences;
            using(parser)
            {
                sequences = parser.Parse().ToList();
            }
            if (sequences.Count != 1)
            {
                throw new SequenceCountException(file + " contains " + sequences.Count + " well-formatted sequences. It should contain exactly one.");
            }
            if (sequences.First().Count < 150)
            {
                throw new SequenceLengthException("Sequence in " + file + " is shorter than 150nt. It should not be used as a fragment.", sequences.First());
            }
            return sequences.First();
        }


    }
}


