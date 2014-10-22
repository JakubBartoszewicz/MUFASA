using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Mufasa.BackEnd.Designer;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Mufasa.Pages
{

    /// <summary>
    /// Interaction logic for Reaction.xaml
    public partial class Reaction : UserControl
    {
        public Reaction()
        {
            InitializeComponent();
            if (Design.Designer.ConstructionList.Count() != 0)
            {
                fragmentsListBoxInitialize();
            }
        }

        /// <summary>
        /// Fragment list.
        /// </summary>
        private ObservableCollection<Fragment> fragmentList;


        /// <summary>
        /// Reaction list box initialization.
        /// </summary>
        private void fragmentsListBoxInitialize()
        {
            fragmentList = new ObservableCollection<Fragment>();
            for (int i = 0; i < Design.Designer.ConstructionList.Count(); i++)
            {

                fragmentList.Add(Design.Designer.FragmentDict[Design.Designer.ConstructionList[i]]);
            }

            this.Items = new ObservableCollection<FragmentViewModel>(fragmentList.Select(m => new FragmentViewModel(m)));
            this.Fragments = (ListCollectionView)CollectionViewSource.GetDefaultView(this.Items);

            this.Items.CollectionChanged += new NotifyCollectionChangedEventHandler(Items_CollectionChanged);
            foreach (var m in this.Items)
                m.PropertyChanged += new PropertyChangedEventHandler(Item_PropertyChanged);

            concentrationsDataGrid.ItemsSource = Fragments;
        }

        //Date or Value were changed
        void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Update the collection view if refresh isn't possible
            if (this.Fragments.IsEditingItem)
                this.Fragments.CommitEdit();
            if (this.Fragments.IsAddingNew)
                this.Fragments.CommitNew();

            this.Fragments.Refresh();
        }

        //Items were added or removed
        void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Attach the observer for the properties
            if (e.NewItems != null)
                foreach (var vm in e.NewItems.OfType<FragmentViewModel>())
                    vm.PropertyChanged += Item_PropertyChanged;

            //Refresh when it is possible
            if (!this.Fragments.IsAddingNew && !this.Fragments.IsEditingItem)
                this.Fragments.Refresh();
        }

        private ObservableCollection<FragmentViewModel> Items { get; set; }

        public ListCollectionView Fragments { get; set; }

        //private void concentrationDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //   for (int i = 0; i < fragmentList.Count(); i++)
        //    {
        //        if (i == 0)
        //        {
        //            fragmentList[i].Volume = fragmentList[i].Length * 0.05 / fragmentList[i].Concentration;
        //        }
        //        else
        //        {
        //            fragmentList[i].Volume = fragmentList[i].Length * 0.1 / fragmentList[i].Concentration;                    
        //        }
        //        Console.WriteLine(fragmentList[i].Volume);
        //    }
        //   //concentrationsDataGrid.ItemsSource = fragmentList;
        //    concentrationsDataGrid.Items.
        //    concentrationsDataGrid.Items.Refresh();
        //}

        //private void concentrationDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        //{

        //}

        
    }
}
