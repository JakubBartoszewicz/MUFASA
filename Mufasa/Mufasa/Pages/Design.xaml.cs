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

namespace Mufasa.Pages
{
    /// <summary>
    /// Interaction logic for Design.xaml
    /// </summary>
    public partial class Design : UserControl
    {
        public Design()
        {
            fragmentFiles = new List<String>();
            InitializeComponent();
        }

        /// <summary>
        /// Open fragment file dialog
        /// </summary>
        private Microsoft.Win32.OpenFileDialog openFragmentFileDialog;

        /// <summary>
        /// List of fragment filenames
        /// </summary>
        private List<String> fragmentFiles;

        /// <summary>
        /// <paramref>OpenFragmentFileDialog</paramref> initialization
        /// </summary>
        private void InitializeOpenFragmentFileDialog()
        {
            this.openFragmentFileDialog = new Microsoft.Win32.OpenFileDialog();
            this.openFragmentFileDialog.FileName = ""; // Default file name
            this.openFragmentFileDialog.DefaultExt = ".fa"; // Default file extension
            this.openFragmentFileDialog.Filter = "Fasta files (.fa, .fas, .fasta)|*.fa;*.fas;*.fasta|GenBank files (.gb)|*.gb|All files|*.*"; // Filter files by extension
            this.openFragmentFileDialog.Multiselect = true;
            this.openFragmentFileDialog.Title = "Open fragment file...";
        }


        /// <summary>
        /// <paramref>openFileButton</paramref> click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Initialize
            InitializeOpenFragmentFileDialog();

            // Show open file dialog box
            Nullable<bool> result = openFragmentFileDialog.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                fragmentFiles.AddRange(openFragmentFileDialog.FileNames);
            }
        }


    }
}
