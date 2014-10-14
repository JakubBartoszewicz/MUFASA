using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Mufasa.BackEnd;
using Mufasa.BackEnd.Exceptions;
using FirstFloor.ModernUI.Windows.Controls;
using System.Net;
using System.Xml;
using System.Xml.XPath;

namespace Mufasa.Pages
{
    /// <summary>
    /// Interaction logic for Design.xaml
    /// </summary>
    public partial class Design : UserControl
    {
        public Design()
        {
            fragmentList = new ObservableCollection<Fragment>();
            fragmentNames = new ObservableCollection<String>();
            InitializeComponent();
        }

        /// <summary>
        /// Open fragment file dialog.
        /// </summary>
        private Microsoft.Win32.OpenFileDialog openFragmentFileDialog;

        /// <summary>
        /// List of fragments.
        /// </summary>
        private ObservableCollection<Fragment> fragmentList;

        /// <summary>
        /// List of fragment simple names.
        /// </summary>
        private ObservableCollection<String> fragmentNames;

        /// <summary>
        /// <paramref>OpenFragmentFileDialog</paramref> initialization.
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
        /// <paramref>bbLabel</paramref> initialozation.
        /// <paramref>bbInputTextBox</paramref> initialization.
        /// <paramref>bbAcceptButton</paramref> initialization.
        /// </summary>
        private void InitializeBbSearching()
        {
            bbLabel.Visibility = System.Windows.Visibility.Visible;
            bbInputTextBox.Visibility = System.Windows.Visibility.Visible;
            bbSearchButton.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// <paramref></paramref> initialozation.
        /// <paramref></paramref> initialization.
        /// <paramref></paramref> initialization.
        /// </summary>
        private void InitializeBbInformations()
        {

        } 
        
        /// <summary>
        /// <paramref>openFileButton</paramref> click event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openFileButton_Click(object sender, RoutedEventArgs e)
        {
            bbLabel.Visibility = System.Windows.Visibility.Hidden;
            bbInputTextBox.Visibility = System.Windows.Visibility.Hidden;
            bbSearchButton.Visibility = System.Windows.Visibility.Hidden;

            // Initialize
            InitializeOpenFragmentFileDialog();

            // Show open file dialog box
            Nullable<bool> openResult = openFragmentFileDialog.ShowDialog();

            // Process open file dialog box results 
            if (openResult == true)
            {

                List<String> invalidNames = new List<String>();
                foreach (String file in openFragmentFileDialog.FileNames)
                {
                    String name = System.IO.Path.GetFileNameWithoutExtension(file);
                    try
                    {
                        if (fragmentNames.Contains(name))
                        {
                            throw new FragmentNamingException(name);
                        }
                        fragmentNames.Add(name);
                        fragmentList.Add(new Fragment(file, name));
                    }
                    catch (FragmentNamingException fne)
                    {
                        invalidNames.Add(fne.Message);
                    }
                    
                }
                if (invalidNames.Count > 0)
                {
                    StringBuilder message = new StringBuilder();
                    message.AppendLine("Following fragment names already exist and will be ignored:" + Environment.NewLine);
                    foreach (String n in invalidNames)
                    {
                        message.AppendLine("\t" + n);
                    }
                    message.AppendLine(Environment.NewLine + "Please choose other names.");
                    ModernDialog.ShowMessage(message.ToString(), "warning", MessageBoxButton.OK);
                } 
                fragmentListBox.ItemsSource = fragmentNames;
                fragmentListBox.Items.Refresh();

            }
        }

        private void fragmentListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// <paramref>searchButton</paramref> click event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeBbSearching();
        }

        /// <summary>
        /// <paramref>bbSearchButton</paramref> click event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bbSearchButton_Click(object sender, RoutedEventArgs e)
        {
            //Initialize url.
            string url = "http://parts.igem.org/cgi/xml/part.cgi?part=" + bbInputTextBox.Text;
            string message;

            //Initialize parsing.
            try
            {
                XPathDocument oXPathDocument = new XPathDocument(url);
                XPathNavigator oXPathNavigator = oXPathDocument.CreateNavigator();

                //Checking for BioBrick.
                XPathNodeIterator oPartNodesIterator = oXPathNavigator.Select("/rsbpml/part_list/part");
                if (oPartNodesIterator.Count == 0)
                {
                    ModernDialog.ShowMessage("BioBrick " + bbInputTextBox.Text + " not found!\nCheck the name of BioBrick and try again.", "warning", MessageBoxButton.OK);
                }
                else
                {
                    foreach (XPathNavigator oCurrentPart in oPartNodesIterator)
                    {
                        message = "\nPart name: " + oCurrentPart.SelectSingleNode("part_name").Value + "\nPart type: " + oCurrentPart.SelectSingleNode("part_type").Value + "\nShort description: " + oCurrentPart.SelectSingleNode("part_short_desc").Value + "\n\nAdd this BioBrick to fragments list?";
                        var result = ModernDialog.ShowMessage("Please check informations about " + bbInputTextBox.Text + " and click Yes to add BioBrick to fragment list.\n" + message, bbInputTextBox.Text + " informations", MessageBoxButton.YesNo);
                        
                        if (MessageBoxResult.Yes == result)
                        {
                            try 
                            { 
                                if (fragmentNames.Contains(bbInputTextBox.Text))
                                {
                                    throw new FragmentNamingException(bbInputTextBox.Text);
                                }
                                else
                                {
                                    fragmentNames.Add(bbInputTextBox.Text);
                                    fragmentListBox.ItemsSource = fragmentNames;
                                    fragmentListBox.Items.Refresh();
                                }
                            }
                            catch (FragmentNamingException)
                            {
                                ModernDialog.ShowMessage("Following fragment names already exist and will be ignored: " + bbInputTextBox.Text, "Warning", MessageBoxButton.OK);
                            }
                            
                        }
                        if (MessageBoxResult.No == result) { bbInputTextBox.Clear(); }
                    }
                }
            }
            catch
            {
                ModernDialog.ShowMessage("Cannot open url. Check your Internet connection and try again.", "Warning", MessageBoxButton.OK);
            }
            
        }

    }
}
