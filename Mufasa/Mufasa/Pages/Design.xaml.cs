﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using Mufasa.BackEnd.Scores;
using FirstFloor.ModernUI.Windows.Controls;
using System.Net;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using FirstFloor.ModernUI.Presentation;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

//Copyright (C) 2014, 2015 Jakub Bartoszewicz (if not stated otherwise)
namespace Mufasa.Pages
{
    /// <summary>
    /// Interaction logic for Design.xaml
    /// </summary>
    public partial class Design : UserControl
    {
        public Design()
        {

            Designer = new Designer();

            InitializeComponent();


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



        /// <value>
        /// Open fragment file dialog.
        /// </value>
        private Microsoft.Win32.OpenFileDialog openFragmentFileDialog;

        /// <value>
        /// Open fragment file dialog.
        /// </value>
        private Microsoft.Win32.OpenFileDialog openProjectFileDialog;


        /// <value>
        /// Save fragment file dialog.
        /// </value>
        private Microsoft.Win32.SaveFileDialog saveConstructFileDialog;

        /// <value>
        /// Save overlap file dialog.
        /// </value>
        private Microsoft.Win32.SaveFileDialog saveOverlapsDialog;

        /// <value>
        /// Mufasa reaction designer object.
        /// </value>
        internal static Designer Designer { get; set; }

        /// <summary>
        /// Mufasa construct object.
        /// </summary>
        private Construct construct;

        /// <value>
        /// Mufasa OverlapOptimizer object.
        /// </value>
        private OverlapOptimizer overlapOptimizer;

        /// <summary>
        /// OpenFragmentFileDialog initialization.
        /// </summary>
        private void InitializeOpenFragmentFileDialog()
        {
            this.openFragmentFileDialog = new Microsoft.Win32.OpenFileDialog();
            this.openFragmentFileDialog.FileName = ""; // Default file name
            this.openFragmentFileDialog.DefaultExt = ".fa"; // Default file extension
            this.openFragmentFileDialog.Filter = "Fasta files|*.fa;*.fas;*.fasta|GenBank files|*.gb;*.gbk|All files|*.*"; // Filter files by extension
            this.openFragmentFileDialog.Multiselect = true;
            this.openFragmentFileDialog.Title = "Open fragment file...";

        }

        /// <summary>
        /// SaveFragmentFileDialog initialization.
        /// </summary>
        private void InitializeSaveFragmentFileDialog()
        {
            this.saveConstructFileDialog = new Microsoft.Win32.SaveFileDialog();
            this.saveConstructFileDialog.FileName = ""; // Default file name
            this.saveConstructFileDialog.DefaultExt = ".gb"; // Default file extension
            this.saveConstructFileDialog.Filter = "GenBank files| *.gb;*.gbk|Fasta files|*.fa;*.fas;*.fasta|All files|*.*"; // Filter files by extension

            this.saveConstructFileDialog.Title = "Save construct...";

        }

        /// <summary>
        /// SaveOverlapsDialog initialization.
        /// </summary>
        private void InitializeSaveOverlapsDialog()
        {
            this.saveOverlapsDialog = new Microsoft.Win32.SaveFileDialog();
            this.saveOverlapsDialog.FileName = ""; // Default file name
            this.saveOverlapsDialog.DefaultExt = ".csv"; // Default file extension
            this.saveOverlapsDialog.Filter = "CSV| *.csv|Text files| *.txt|All files|*.*"; // Filter files by extension

            this.saveOverlapsDialog.Title = "Save overlaps...";

        }

        //Copyright (C) 2014 Melania Nowicka
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
                                    Designer.AddBrickFromRegistry(url, oCurrentPart.SelectSingleNode("sequences").Value, bbInputTextBox.Text);
                                }
                                catch (FragmentNamingException)
                                {
                                    ModernDialog.ShowMessage("Following fragment names already exist and will be ignored: " + bbInputTextBox.Text, "Warning", MessageBoxButton.OK);
                                }

                            }
                            if (MessageBoxResult.No == result) { bbInputTextBox.Clear(); }
                            fragmentListBox.ItemsSource = Designer.FragmentDict.Keys;
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
                        Designer.AddFragment(file, name);
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
                fragmentListBox.ItemsSource = Designer.FragmentDict.Keys;
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
            if (fragmentListBox.SelectedItem != null)
            {
                Fragment sel = Designer.FragmentDict[fragmentListBox.SelectedItem.ToString()];
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
                        Designer.FragmentDict.Remove(name);
                    }
                    fragmentListBox.ItemsSource = Designer.FragmentDict.Keys;
                    fragmentListBox.Items.Refresh();
                }
            }

        }

        //Copyright (C) 2014 Melania Nowicka
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

            foreach (String str in listItems)
            {
                Designer.ConstructionList.Remove(str);
            }

            LabelVector();

            constructionListBox.ItemsSource = Designer.ConstructionList;
            constructionListBox.Items.Refresh();
        }

        /// <summary>
        /// Save button click event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (construct != null)
            {
                InitializeSaveFragmentFileDialog();

                // Show save file dialog box
                Nullable<bool> openResult = saveConstructFileDialog.ShowDialog();

                // Process open file dialog box results 
                if (openResult == true)
                {
                    try
                    {
                        construct.SaveAsBio(saveConstructFileDialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBoxResult result = ModernDialog.ShowMessage(ex.Message, "Exception", MessageBoxButton.OK);
                    }
                }
            }

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
                    Designer.AddConstructionFragment(name);
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

            LabelVector();

            constructionListBox.ItemsSource = Designer.ConstructionList;
            constructionListBox.Items.Refresh();
        }

        /// <summary>
        /// PreviewMouseMove event handler for construction list drag&drop.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                    draggedItem.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// DragOver event handler for construction list drag&drop.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Drop event handler for construction list drag&drop.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void constructionListBox_Drop(object sender, DragEventArgs e)
        {
            String droppedData = e.Data.GetData(typeof(String)) as String;
            String target = ((ListBoxItem)(sender)).DataContext as String;

            int removedIdx = constructionListBox.Items.IndexOf(droppedData);
            int targetIdx = constructionListBox.Items.IndexOf(target);

            if (removedIdx < targetIdx)
            {
                Designer.ConstructionList.Insert(targetIdx + 1, droppedData);
                Designer.ConstructionList.RemoveAt(removedIdx);
            }
            else
            {
                int remIdx = removedIdx + 1;
                if (Designer.ConstructionList.Count + 1 > remIdx)
                {
                    Designer.ConstructionList.Insert(targetIdx, droppedData);
                    Designer.ConstructionList.RemoveAt(remIdx);
                }
            }

            LabelVector();

        }

        /// <summary>
        /// Labelling the vector fragment.
        /// </summary>
        private void LabelVector()
        {
            for (int i = 0; i < Designer.ConstructionList.Count; i++)
            {
                Designer.ConstructionList[i] = Designer.ConstructionList[i].Replace(Designer.VectorLabel, "");
            }
            if (Designer.ConstructionList.Count > 0)
                Designer.ConstructionList[0] = Designer.VectorLabel + Designer.ConstructionList[0];
        }

        /// <summary>
        /// Assemble button click event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void assembleButton_Click(object sender, RoutedEventArgs e)
        {
            if (assembleButton.Content.ToString() == "Stop")
            {
                overlapOptimizer.Stop();
                assembleButton.IsEnabled = false;
            }

            if (Designer.ConstructionList != null && Designer.ConstructionList.Count > 0 && assembleButton.Content.ToString() != "Stop")
            {
                testButton.IsEnabled = false;
                workingBar.Visibility = Visibility.Visible;
                progressBar.Value = 0;

                try
                {
                    construct = new Construct(Designer.ConstructionList, Designer.FragmentDict, Designer.Settings);
                }
                catch(TmThalParamException tex)
                {
                    ModernDialog.ShowMessage("Unable to assemble.\n(" + tex.Message + ")", "Warning: ", MessageBoxButton.OK);
                    workingBar.Visibility = Visibility.Hidden;
                    assembleButton.Content = "Assemble";
                    assembleButton.IsEnabled = true;
                    testButton.IsEnabled = true;
                    return;
                }

                overlapOptimizer = new OverlapOptimizer(construct, Designer.Settings);
                assembleButton.Content = "Stop";

                BackgroundWorker bw = new BackgroundWorker();
                // this allows our worker to report progress during work
                bw.WorkerReportsProgress = true;
                // what to do in the background thread

                if (Designer.Settings.UseNaive)
                {
                    overlapOptimizer.IgnorePreoptimizationExceptions = false;
                    bw.DoWork += new DoWorkEventHandler(overlapOptimizer.SemiNaiveOptimizeOverlaps);
                }
                else
                {
                    overlapOptimizer.IgnorePreoptimizationExceptions = true;
                    bw.DoWork += (s, args) =>
                    {
                        //preoptimize
                        overlapOptimizer.SemiNaiveOptimizeOverlaps(s, args);
                        overlapOptimizer.LeaOptimizeOverlaps(s, args);
                    };
                }


                // what to do when progress changed (update the progress bar)
                bw.ProgressChanged += new ProgressChangedEventHandler(
                delegate(object o, ProgressChangedEventArgs args)
                {
                    progressBar.Value = args.ProgressPercentage;
                });
                // what to do when worker completes its task (notify the user)
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                delegate(object o, RunWorkerCompletedEventArgs args)
                {
                    if (args.Error == null)
                    {
                        construct = overlapOptimizer.Construct;
                        ScoreTotal score = construct.Score;
                        overlapDataGrid.ItemsSource = construct.Overlaps;
                        overlapDataGrid.Items.Refresh();
                        List<Score> scoreList = new List<Score>();
                        scoreList.Add(score.Sm);
                        scoreList.Add(score.So);
                        scoreList.Add(score);
                        scoreDataGrid.ItemsSource = scoreList;
                        scoreDataGrid.Items.Refresh();
                    }
                    else
                    {
                        Exception ex = args.Error as Exception;
                        ModernDialog.ShowMessage("Unable to assemble.\n(" + ex.Message + ")", "Warning: ", MessageBoxButton.OK);
                    }

                    workingBar.Visibility = Visibility.Hidden;
                    assembleButton.Content = "Assemble";
                    assembleButton.IsEnabled = true;
                    testButton.IsEnabled = true;
                });

                bw.RunWorkerAsync();
            }

        }

        /// <summary>
        /// Save overlaps button click event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void overButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeSaveOverlapsDialog();
            if (construct != null)
            {
                InitializeSaveFragmentFileDialog();

                // Show save file dialog box
                Nullable<bool> openResult = saveOverlapsDialog.ShowDialog();

                // Process open file dialog box results 
                if (openResult == true)
                {
                    String sep = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(saveOverlapsDialog.FileName))
                        {
                            sw.WriteLine("Scores:" + sep + "Raw score" + sep + "Normalized score");
                            foreach (Score item in scoreDataGrid.Items)
                            {
                                sw.WriteLine(item.ToCsv());

                            }
                            sw.WriteLine();
                            sw.WriteLine("Name" + sep + "Tm" + sep + "Length" + sep + "Sequence");
                            foreach (Overlap item in overlapDataGrid.Items)
                            {
                                sw.WriteLine(item.ToCsv());

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ModernDialog.ShowMessage(ex.Message, "Warning: ", MessageBoxButton.OK);
                    }
                }
            }
        }


        /// <summary>
        /// OpenFragmentProjectDialog initialization.
        /// </summary>
        private void InitializeOpenProjectFileDialog()
        {
            this.openProjectFileDialog = new Microsoft.Win32.OpenFileDialog();
            this.openProjectFileDialog.FileName = ""; // Default file name
            this.openProjectFileDialog.DefaultExt = ".gb"; // Default file extension
            this.openProjectFileDialog.Filter = "GenBank files|*.gb;*.gbk|All files|*.*"; // Filter files by extension
            this.openProjectFileDialog.Multiselect = false;
            this.openProjectFileDialog.Title = "Open project file...";

        }

        /// <summary>
        /// openProjectButton event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openProjectButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeOpenProjectFileDialog();

            // Show open file dialog box
            Nullable<bool> openResult = openProjectFileDialog.ShowDialog();

            // Process open file dialog box results 
            if (openResult == true)
            {

                try
                {
                    Designer.openProject(openProjectFileDialog.FileName);

                }
                catch (Exception ex)
                {
                    ModernDialog.ShowMessage(ex.Message, "Warning: ", MessageBoxButton.OK);
                }
                fragmentListBox.ItemsSource = Designer.FragmentDict.Keys;
                fragmentListBox.Items.Refresh();

            }
        }
       
        /// <summary>
        /// Test button click event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void testButton_Click(object sender, RoutedEventArgs e)
        {
            if (testButton.Content.ToString() == "Stop")
            {
                overlapOptimizer.Stop();
                testButton.IsEnabled = false;
            }

            if (Designer.ConstructionList != null && Designer.ConstructionList.Count > 0 && testButton.Content.ToString() != "Stop")
            {
                test();
            }

        }

        /// <summary>
        /// Test mufasa performance.
        /// </summary>
        private void test()
        {
            workingBar.Visibility = Visibility.Visible;
            progressBar.Value = 0;
            
            try
            {
                construct = new Construct(Designer.ConstructionList, Designer.FragmentDict, Designer.Settings);
            }
            catch (TmThalParamException tex)
            {
                ModernDialog.ShowMessage("Unable to assemble.\n(" + tex.Message + ")", "Warning: ", MessageBoxButton.OK);
                workingBar.Visibility = Visibility.Hidden;
                assembleButton.Content = "Assemble";
                assembleButton.IsEnabled = true;
                testButton.IsEnabled = true;
                return;
            }

            overlapOptimizer = new OverlapOptimizer(construct, Designer.Settings);
            testButton.Content = "Stop";
            BackgroundWorker bw = new BackgroundWorker();
            Stopwatch w = new Stopwatch();
            // this allows our worker to report progress during work
            bw.WorkerReportsProgress = true;
            // what to do in the background thread

            if (Designer.Settings.UseNaive)
            {
                overlapOptimizer.IgnorePreoptimizationExceptions = false;
                w.Start();
                bw.DoWork += new DoWorkEventHandler(overlapOptimizer.SemiNaiveOptimizeOverlaps);
            }
            else
            {
                overlapOptimizer.IgnorePreoptimizationExceptions = true;
                bw.DoWork += (s, args) =>
                {
                    w.Start();
                    //preoptimize
                    overlapOptimizer.SemiNaiveOptimizeOverlaps(s, args);
                    overlapOptimizer.LeaOptimizeOverlaps(s, args);
                };
            }


            // what to do when progress changed (update the progress bar)
            bw.ProgressChanged += new ProgressChangedEventHandler(
            delegate(object o, ProgressChangedEventArgs args)
            {
                progressBar.Value = args.ProgressPercentage;
            });
            // what to do when worker completes its task (notify the user)
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
            delegate(object o, RunWorkerCompletedEventArgs args)
            {
                w.Stop();
                if (args.Error == null)
                {
                    construct = overlapOptimizer.Construct;
                    ScoreTotal score = construct.Score;
                    overlapDataGrid.ItemsSource = construct.Overlaps;
                    overlapDataGrid.Items.Refresh();
                    List<Score> scoreList = new List<Score>();
                    scoreList.Add(score.Sm);
                    scoreList.Add(score.So);
                    scoreList.Add(score);
                    scoreDataGrid.ItemsSource = scoreList;
                    scoreDataGrid.Items.Refresh();


                    String sep = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                    try
                    {
                        using (StreamWriter sw = new StreamWriter("testlog.csv",true))
                        {
                            sw.WriteLine(score.NormalizedScore.ToString() + sep + score.So.NormalizedScore.ToString() + sep + score.Sm.NormalizedScore.ToString() + sep + (double)w.ElapsedMilliseconds / 1000);
                        }
                    }
                    catch (Exception exc)
                    {
                        ModernDialog.ShowMessage(exc.Message, "Warning: ", MessageBoxButton.OK);
                    }
                }
                else
                {
                    Exception ex = args.Error as Exception;
                    ModernDialog.ShowMessage("Unable to assemble.\n(" + ex.Message + ")", "Warning: ", MessageBoxButton.OK);

                    String sep = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(saveOverlapsDialog.FileName))
                        {
                            sw.WriteLine(ScoreTotal.Inacceptable.NormalizedScore.ToString() + sep + ScoreTotal.Inacceptable.NormalizedScore.ToString() + sep + ScoreTotal.Inacceptable.NormalizedScore.ToString() + sep + (double)w.ElapsedMilliseconds / 1000);
                        }
                    }
                    catch (Exception exc)
                    {
                        ModernDialog.ShowMessage(exc.Message, "Warning: ", MessageBoxButton.OK);
                    }

                }

                workingBar.Visibility = Visibility.Hidden;
                testButton.Content = "Test";
                testButton.IsEnabled = true;
            });

            bw.RunWorkerAsync();
        }
    }
}
