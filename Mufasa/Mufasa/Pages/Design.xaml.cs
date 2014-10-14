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
using Mufasa.BackEnd.Designer;
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
            designer = new Designer();
            InitializeComponent();
        }

        /// <summary>
        /// Open fragment file dialog.
        /// </summary>
        private Microsoft.Win32.OpenFileDialog openFragmentFileDialog;

        /// <summary>
        /// BioBrick label.
        /// </summary>
        private Label bbLabel; 

        /// <summary>
        /// BioBrick name input.
        /// </summary>
        private TextBox bbInputTextBox; 

        /// <summary>
        /// BioBrick accept button.
        /// </summary>
        private Button bbSearchButton;

        /// <summary>
        /// Mufasa reaction designer object.
        /// </summary>
        private Designer designer;

        /// <summary>
        /// OpenFragmentFileDialog initialization.
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
        /// Bb-related components initialization.
        /// </summary>
        private void InitializeBbSearching()
        {
            this.bbLabel = new Label();
            bbLabel.Content = "Please enter full Biobrick name:";
            bbLabel.Width = 300;
            bbLabel.Height = 30;
            searchCanvas.Children.Add(bbLabel);
            this.bbInputTextBox = new TextBox();
            bbInputTextBox.Name = "bbInputTextBox";
            bbInputTextBox.Width = 200;
            bbInputTextBox.Height = 30;
            searchCanvas.Children.Add(bbInputTextBox);
            this.bbSearchButton = new Button();
            bbSearchButton.Name = "bbSearchButton";
            bbSearchButton.Content = "Search";
            bbSearchButton.Width = 100;
            bbSearchButton.Height = 30;
            bbSearchButton.Click += bbSearchButton_Click;
            searchCanvas.Children.Add(bbSearchButton);
            Canvas.SetTop(bbLabel, 0);
            Canvas.SetLeft(bbLabel, 10);
            Canvas.SetTop(bbInputTextBox, 30);
            Canvas.SetLeft(bbInputTextBox, 10);
            Canvas.SetTop(bbSearchButton, 30);
            Canvas.SetLeft(bbSearchButton, 250);
        }

        /// <summary>
        /// bb information initialization.
        /// </summary>
        private void InitializeBbInformation()
        {

        } 
        
        /// <summary>
        /// openFileButton click event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openFileButton_Click(object sender, RoutedEventArgs e)
        {
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
                        CheckFragment(file, name);
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
                fragmentListBox.ItemsSource = designer.FragmentNames;
                fragmentListBox.Items.Refresh();

            }
        }

        /// <summary>
        /// Check if Fragment <paramref name="name"/> is valid.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="name"></param>
        private void CheckFragment(String file, String name)
        {
            if (designer.FragmentNames.Contains(name))
            {
                throw new FragmentNamingException(name);
            }
            designer.FragmentNames.Add(name);
            designer.FragmentList.Add(new Fragment(file, name));
        }

        private void fragmentListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// searchButton click event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeBbSearching();
        }

        /// <summary>
        /// bbSearchButton click event handler.
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
                        ModernDialog.ShowMessage("Please check informations about " + bbInputTextBox.Text + " and click Yes to add BioBrick to fragment list.\n" + message, bbInputTextBox.Text + " informations", MessageBoxButton.YesNo);
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
