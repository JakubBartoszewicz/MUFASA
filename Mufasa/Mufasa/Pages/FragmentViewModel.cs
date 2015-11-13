using Bio;
using FirstFloor.ModernUI.Presentation;
using Mufasa.BackEnd.Designer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Copyright (C) 2014 Jakub Bartoszewicz (if not stated otherwise)
namespace Mufasa.Pages
{
    /// <summary>
    /// Wraps Fragment class and provides notification of changes
    /// </summary>
    public class FragmentViewModel : NotifyPropertyChanged
    {
        /// <summary>
        /// FragmentViewModel constructor.
        /// </summary>
        public FragmentViewModel()
        {
            this.Model = new Fragment();
        }

        /// <summary>
        /// FragmentViewModel constructor.
        /// </summary>
        /// <param name="m">Fragment model.</param>
        public FragmentViewModel(Fragment m)
        {
            this.Model = m;
        }

        /// <value>
        /// Fragment model.
        /// </value>
        public Fragment Model { get; private set; }

        /// <value>
        /// Concentration.
        /// </value>
        public double Concentration
        {
            get { return this.Model.Concentration; }
            set
            {
                this.Model.Concentration = value;
                OnPropertyChanged("Concentration");
            }
        }

        /// <value>
        /// Sample volume.
        /// </value>
        public double Volume
        {
            get { return this.Model.Volume; }
            set
            {
                this.Model.Volume = value;
                OnPropertyChanged("Volume");
            }
        }

        /// <value>
        /// Reaction volume.
        /// </value>
        public double ReactionVolume
        {
            get { return this.Model.ReactionVolume; }
            set
            {
                this.Model.ReactionVolume = value;
                OnPropertyChanged("ReactionVolume");
            }
        }

        /// <value>
        /// True if a vector fragment.
        /// </value>
        public bool IsVector
        {
            get { return this.Model.IsVector; }
            set
            {
                this.Model.IsVector = value;
                OnPropertyChanged("IsVector");
            }
        }

        /// <value>
        /// Fragment length.
        /// </value>
        public long Length
        {
            get { return this.Model.Length; }
            set
            {
                this.Model.Length = value;
                OnPropertyChanged("Length");
            }
        }

        /// <value>
        /// Path to the file or url containing the fragment.
        /// </value>
        public String Source
        {
            get { return this.Model.Source; }
            set
            {
                this.Model.Source = value;
                OnPropertyChanged("Source");
            }
        }
        /// <value>
        /// Name of the fragment.
        /// </value>
        public String Name
        {
            get
            {
                if (this.Model.IsVector)
                {
                    return Designer.VectorLabel + this.Model.Name;
                }
                else
                {
                    return this.Model.Name;
                } 
            }
            set
            {
                this.Model.Name = value;
                OnPropertyChanged("Name");
            }
        }
        /// <value>
        /// Fragment sequence.
        /// </value>
        public ISequence Sequence
        {
            get { return this.Model.Sequence; }
            set
            {
                this.Model.Sequence = value;
                OnPropertyChanged("Sequence");
            }
        }
    }
}
