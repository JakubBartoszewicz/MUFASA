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
using System.IO;
using FirstFloor.ModernUI.Presentation;

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
            fragmentCurveEndPoints = new List<Point>();

            InitializeComponent();

            designer = new Designer();

            
            Style itemContainerStyle = new Style(typeof(ListBoxItem), fragmentListBox.ItemContainerStyle);
            itemContainerStyle.Setters.Add(new Setter(ListBoxItem.AllowDropProperty, true));

            itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.DragOverEvent, new DragEventHandler(s_DragOver)));
            itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.PreviewMouseMoveEvent, new MouseEventHandler(s_PreviewMouseMove)));
            itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.DropEvent, new DragEventHandler(constructionListBox_Drop)));
            
            black.Color = Color.FromRgb(0x0a, 0x0a, 0x0a);
            white.Color = Color.FromRgb(0xe5, 0xe5, 0xe5);
            grey.Color = Color.FromRgb(0x3e, 0x3e, 0x3e);
            bkgd.Color = Color.FromRgb(0x25, 0x25, 0x25);
            accent.Color = Color.FromRgb(AppearanceManager.Current.AccentColor.R, AppearanceManager.Current.AccentColor.G, AppearanceManager.Current.AccentColor.B);
            itemContainerStyle.Resources.Add(SystemColors.HighlightBrushKey, accent);
            itemContainerStyle.Resources.Add(SystemColors.InactiveSelectionHighlightBrushKey, accent);

            constructionListBox.Foreground = white;
            constructionListBox.ItemContainerStyle = itemContainerStyle;
        }

        SolidColorBrush black = new SolidColorBrush();
        SolidColorBrush white = new SolidColorBrush();
        SolidColorBrush grey = new SolidColorBrush();
        SolidColorBrush bkgd = new SolidColorBrush();
        SolidColorBrush accent = new SolidColorBrush();

        /// <summary>
        /// List of fragment visualisation curves end points.
        /// </summary>
        private List<Point> fragmentCurveEndPoints;

        /// <summary>
        /// Open fragment file dialog.
        /// </summary>
        private Microsoft.Win32.OpenFileDialog openFragmentFileDialog;

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
        /// Shows bb information.
        /// </summary>
        private void ShowBbInformation()
        {
            if (bbInputTextBox.Text.Length > 0)
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
                        ModernDialog.ShowMessage("BioBrick " + bbInputTextBox.Text + " not found!\nCheck the name of BioBrick and try again.", "warning: " + bbInputTextBox.Text, MessageBoxButton.OK);
                    }
                    else
                    {
                        foreach (XPathNavigator oCurrentPart in oPartNodesIterator)
                        {
                            message = "\nPart name: " + oCurrentPart.SelectSingleNode("part_name").Value + "\nPart type: " + oCurrentPart.SelectSingleNode("part_type").Value + "\nShort description: " + oCurrentPart.SelectSingleNode("part_short_desc").Value + "\n\nAdd this BioBrick to fragments list?";
                            var result = ModernDialog.ShowMessage("Please check information about " + bbInputTextBox.Text + " and click Yes to add BioBrick to fragment list.\n" + message, bbInputTextBox.Text + " information", MessageBoxButton.YesNo);
                            if (MessageBoxResult.Yes == result)
                            {
                                try
                                {
                                    designer.AddBrickFromRegistry(url, oCurrentPart.SelectSingleNode("sequences").Value, bbInputTextBox.Text);
                                }
                                catch (FragmentNamingException)
                                {
                                    ModernDialog.ShowMessage("Following fragment names already exist and will be ignored: " + bbInputTextBox.Text, "Warning", MessageBoxButton.OK);
                                }

                            }
                            if (MessageBoxResult.No == result) { bbInputTextBox.Clear(); }
                            fragmentListBox.ItemsSource = designer.FragmentDict.Keys;
                            fragmentListBox.Items.Refresh();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModernDialog.ShowMessage(ex.Message, "Warning: " + bbInputTextBox.Text, MessageBoxButton.OK);
                }
                bbInputTextBox.Focus();
                bbInputTextBox.Select(4, bbInputTextBox.Text.Length);
            }
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
                        designer.AddFragment(file, name);
                    }
                    catch (FragmentNamingException fne)
                    {
                        invalidNames.Add(fne.Message);
                    }
                    catch (Exception ex)
                    {
                        ModernDialog.ShowMessage(ex.Message, "warning: " + name, MessageBoxButton.OK);
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
                fragmentListBox.ItemsSource = designer.FragmentDict.Keys;
                fragmentListBox.Items.Refresh();

            }
        }


        /// <summary>
        /// Prints fragment sequence in the fragmentListBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fragmentListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(fragmentListBox.SelectedItem!=null)
            { 
                Fragment sel = designer.FragmentDict[fragmentListBox.SelectedItem.ToString()];
                fragmentSequenceTextBox.Text = sel.GetString();
            }
            else
            {
                fragmentSequenceTextBox.Text = "";
            }
            
        }

        /// <summary>
        /// Deletes selected fragments.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteFragmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (fragmentListBox.SelectedItems.Count != 0)
            {

                StringBuilder message = new StringBuilder();
                message.AppendLine("Do you really want to delete selected fragments?:" + Environment.NewLine);
                foreach (String n in fragmentListBox.SelectedItems)
                {
                    message.AppendLine("\t" + n);
                }
                MessageBoxResult result = ModernDialog.ShowMessage(message.ToString(), "confirm", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (String name in fragmentListBox.SelectedItems)
                    {
                        designer.FragmentDict.Remove(name);
                    }
                    fragmentListBox.ItemsSource = designer.FragmentDict.Keys;
                    fragmentListBox.Items.Refresh();
                }
            }

        }

        /// <summary>
        /// Search event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bbInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {            
            if (e.Key == Key.Return)
            {
                ShowBbInformation();
            }
        }

       
        /// <summary>
        /// Delete construction fragment event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteConstructionFragmentButton_Click(object sender, RoutedEventArgs e)
        {
            List<String> listItems = new List<String>();
            foreach (String str in constructionListBox.SelectedItems)
            {
                listItems.Add(str);
            } 

            foreach (String str in listItems )
            {
                designer.ConstructionList.Remove(str);
            }
            constructionListBox.ItemsSource = designer.ConstructionList;
            constructionListBox.Items.Refresh();
        }

        /// <summary>
        /// Save button click event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Add construction fragment event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addConstructionFragmentButton_Click(object sender, RoutedEventArgs e)
        {
            
            List<String> invalidNames = new List<String>();
            foreach (String name in fragmentListBox.SelectedItems)
            {                
                try
                {
                    designer.AddConstructionFragment(name);
                }
                catch (FragmentNamingException fne)
                {
                    invalidNames.Add(fne.Message);
                }
                catch (Exception ex)
                {
                    ModernDialog.ShowMessage(ex.Message, "warning", MessageBoxButton.OK);
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
                message.AppendLine(Environment.NewLine + "Please choose other fragments.");
                ModernDialog.ShowMessage(message.ToString(), "warning", MessageBoxButton.OK);
            }
            constructionListBox.ItemsSource = designer.ConstructionList;
            constructionListBox.Items.Refresh();
        }

        void s_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is ListBoxItem)
            {
                ListBoxItem draggedItem = sender as ListBoxItem;
                if (e.GetPosition(sender as ListBoxItem).X > 0 && e.GetPosition(sender as ListBoxItem).X <= 177 && e.GetPosition(sender as ListBoxItem).Y > 0 && e.GetPosition(sender as ListBoxItem).Y <= 12)
                {
                    
                    draggedItem.Background = grey;
                }
                else
                {
                    draggedItem.Background = bkgd;
                }

                if ( e.LeftButton == MouseButtonState.Pressed)
                {
                    DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                    draggedItem.IsSelected = true;
                }
            }
        }

        void s_DragOver(object sender, DragEventArgs e)
        {
            if (sender is ListBoxItem)
            {
                ListBoxItem draggedItem = sender as ListBoxItem;
                if (e.GetPosition(sender as ListBoxItem).X > 0 && e.GetPosition(sender as ListBoxItem).X <= 177 && e.GetPosition(sender as ListBoxItem).Y > 0 && e.GetPosition(sender as ListBoxItem).Y <= 12)
                {

                    draggedItem.Background = grey;
                }
                else
                {
                    draggedItem.Background = bkgd;
                }

            }
        }

        void constructionListBox_Drop(object sender, DragEventArgs e)
        {
            String droppedData = e.Data.GetData(typeof(String)) as String;
            String target = ((ListBoxItem)(sender)).DataContext as String;

            int removedIdx = constructionListBox.Items.IndexOf(droppedData);
            int targetIdx = constructionListBox.Items.IndexOf(target);

            if (removedIdx < targetIdx)
            {
                designer.ConstructionList.Insert(targetIdx + 1, droppedData);
                designer.ConstructionList.RemoveAt(removedIdx);
            }
            else
            {
                int remIdx = removedIdx + 1;
                if (designer.ConstructionList.Count + 1 > remIdx)
                {
                    designer.ConstructionList.Insert(targetIdx, droppedData);
                    designer.ConstructionList.RemoveAt(remIdx);
                }
            }
        }
    }
}
