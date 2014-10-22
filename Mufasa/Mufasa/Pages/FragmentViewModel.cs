using Bio;
using Mufasa.BackEnd.Designer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mufasa.Pages
{
    /// <summary>
    /// Wraps Fragment class and provide notification of changes
    /// </summary>
    public class FragmentViewModel
    {
        public FragmentViewModel()
        {
            this.Model = new Fragment();
        }

        public FragmentViewModel(Fragment m)
        {
            this.Model = m;
        }

        public Fragment Model { get; private set; }

        public double Concentration
        {
            get { return this.Model.Concentration; }
            set
            {
                this.Model.Concentration = value;
                OnPropertyChanged("Concentration");
            }
        }

        public double Volume
        {
            get { return this.Model.Volume; }
            set
            {
                this.Model.Volume = value;
                OnPropertyChanged("Volume");
            }
        }

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
        /// <summary>
        /// Name of the fragment.
        /// </summary>
        public String Name
        {
            get { return this.Model.Name; }
            set
            {
                this.Model.Name = value;
                OnPropertyChanged("Name");
            }
        }
        /// <summary>
        /// Fragment sequence.
        /// </summary>
        public ISequence Sequence
        {
            get { return this.Model.Sequence; }
            set
            {
                this.Model.Sequence = value;
                OnPropertyChanged("Sequence");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
