using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mufasa.BackEnd.Designer
{
    class Designer
    {
        public Designer()
        {
            FragmentList = new ObservableCollection<Fragment>();
            FragmentNames = new ObservableCollection<String>();
        }

        /// <summary>
        /// List of fragments.
        /// </summary>
        public ObservableCollection<Fragment> FragmentList { get; set; }

        /// <summary>
        /// List of fragment simple names.
        /// </summary>
        public ObservableCollection<String> FragmentNames { get; set; }
    }
}


