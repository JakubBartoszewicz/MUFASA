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
            this.fragmentsVolume = 0.0;

            if (Design.Designer.ConstructionList.Count() != 0)
            {
                InitializeFragmentsListBox();
            }
        }

        private double dNTP;
        private double poly;
        private double water;
        private double buffer;
        private double reactionVolume;
        private double fragmentsVolume;


        /// <summary>
        /// Fragment list.
        /// </summary>
        private ObservableCollection<Fragment> fragmentList;


        /// <summary>
        /// Reaction list box initialization.
        /// </summary>
        private void InitializeFragmentsListBox()
        {            
            this.reactionVolume = Design.Designer.Settings.ReactionVolume;

            //slider calls calculateVolumes
            this.volumeSlider.Value = this.reactionVolume;

            

            fragmentList = new ObservableCollection<Fragment>();
            for (int i = 0; i < Design.Designer.ConstructionList.Count(); i++)
            {
                Fragment f = Design.Designer.FragmentDict[Design.Designer.ConstructionList[i]];
                
                if(i==0)
                {
                    f.IsVector = true;
                }
                else
                {
                    f.IsVector = false;
                }
                fragmentList.Add(f);
                fragmentList[i].ReactionVolume = this.reactionVolume;
            }

            this.Items = new ObservableCollection<FragmentViewModel>(fragmentList.Select(m => new FragmentViewModel(m)));
            this.Fragments = (ListCollectionView)CollectionViewSource.GetDefaultView(this.Items);

            this.Items.CollectionChanged += new NotifyCollectionChangedEventHandler(Items_CollectionChanged);
            foreach (var m in this.Items)
                m.PropertyChanged += new PropertyChangedEventHandler(Item_PropertyChanged);

            concentrationsDataGrid.ItemsSource = Fragments;
        }

        /// <summary>
        /// Calculate reagent volumes.
        /// </summary>
        /// <param name="reactionVolume">Total reaction volume.</param>
        void calculateVolumes(double reactionVolume)
        {
            dNTP = reactionVolume / 50.0; //Qian & Tian, 2014
            poly = reactionVolume / 50.0; //Qian & Tian, 2014
            buffer = reactionVolume / 5.0;
            dNTPTextBlock.Text = dNTP.ToString();
            polyTextBlock.Text = poly.ToString();
            bufferTextBlock.Text = buffer.ToString();
            water = reactionVolume - dNTP - poly - buffer - fragmentsVolume;
            waterTextBlock.Text = water.ToString();
            waterTextBlock.UpdateLayout();
        }

        //Concentrations were changed
        void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Update the collection view if refresh isn't possible
            if (this.Fragments.IsEditingItem)
                this.Fragments.CommitEdit();
            if (this.Fragments.IsAddingNew)
                this.Fragments.CommitNew();

            this.Fragments.Refresh();
            double volume = 0.0;
            for (int i = 0; i < Fragments.Count; i++ )
            {
                concentrationsDataGrid.UpdateLayout();
               // FragmentViewModel it = (FragmentViewModel)concentrationsDataGrid.Items[i];
                concentrationsDataGrid.ScrollIntoView(concentrationsDataGrid.Items[i]);
                DataGridRow row = (DataGridRow)concentrationsDataGrid.ItemContainerGenerator.ContainerFromIndex(i);
                DataGridCell RowColumn = concentrationsDataGrid.Columns[3].GetCellContent(row).Parent as DataGridCell;

                NumberStyles style = NumberStyles.AllowDecimalPoint;
                //CultureInfo culture = CultureInfo.CreateSpecificCulture("en-GB");
                CultureInfo culture = System.Globalization.CultureInfo.CurrentCulture;

                string cellValue = ((TextBlock)RowColumn.Content).Text;
               
                double vol = Double.Parse(cellValue, style, culture);

                if(!Double.IsInfinity(vol))
                    volume += vol;
            }

            fragmentsVolume = volume;
            calculateVolumes(this.reactionVolume);
            
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

        /// <summary>
        /// IsVisible click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.InitializeFragmentsListBox();
        }

        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.reactionVolume = volumeSlider.Value;
            Design.Designer.Settings.ReactionVolume = this.reactionVolume;

            if (this.Items != null)
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    this.Items[i].ReactionVolume = this.reactionVolume;
                }
            }

            calculateVolumes(this.reactionVolume);
        }           
    }
}
