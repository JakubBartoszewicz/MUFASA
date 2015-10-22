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

namespace Mufasa.Pages.Settings
{
    /// <summary>
    /// Interaction logic for DesignerSettings.xaml
    /// </summary>
    public partial class ReactionSettings : UserControl
    {
        public ReactionSettings()
        {
            InitializeComponent();
            
            // create and assign the appearance view model
            this.DataContext = new ReactionSettingsViewModel();

            targetTmScrollBar.Value = Design.Designer.Settings.TargetTm;
            maxThScrollBar.Value = Design.Designer.Settings.MaxTh;
            maxTdScrollBar.Value = Design.Designer.Settings.MaxTd;
            //reactionTempScrollBar.Value = Design.Designer.Settings.TmThalSettings.ReactionTemperature;

            minlen_3ScrollBar.Value = Design.Designer.Settings.MinLen_3;
            maxlen_3ScrollBar.Value = Design.Designer.Settings.MaxLen_3;
            minlen_5ScrollBar.Value = Design.Designer.Settings.MinLen_5;
            maxlen_5ScrollBar.Value = Design.Designer.Settings.MaxLen_5;

            nnMaxLenScrollBar.Value = Design.Designer.Settings.TmThalSettings.NnMaxLen;
            maxLoopScrollBar.Value = Design.Designer.Settings.TmThalSettings.MaxLoop;

            mutationScrollBar.Value = Design.Designer.Settings.LeaSettings.MutationRate;
            crossoverScrollBar.Value = Design.Designer.Settings.LeaSettings.CrossoverRate;
            learningScrollBar.Value = Design.Designer.Settings.LeaSettings.LearningRate;

            popsizeScrollBar.Value = Design.Designer.Settings.LeaSettings.PopulationSize;
            tournamentScrollBar.Value = Design.Designer.Settings.LeaSettings.TournamentSize;
        }

        private void targetTmScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Design.Designer.Settings.TargetTm = targetTmScrollBar.Value;
           
        }

        private void maxThScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Design.Designer.Settings.MaxTh = maxThScrollBar.Value;
        }

        private void maxTdScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Design.Designer.Settings.MaxTd = maxTdScrollBar.Value;
        }


        private void minlen_3ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Design.Designer.Settings.MinLen_3 = (int)(minlen_3ScrollBar.Value + 0.5);
        }

        private void maxlen_3ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Design.Designer.Settings.MaxLen_3 = (int)(maxlen_3ScrollBar.Value + 0.5);
        }

        private void minlen_5ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Design.Designer.Settings.MinLen_5 = (int)(minlen_5ScrollBar.Value + 0.5);
        }

        private void maxlen_5ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Design.Designer.Settings.MaxLen_5 = (int)(maxlen_5ScrollBar.Value + 0.5);
        }

        private void nnMaxLenScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Design.Designer.Settings.TmThalSettings.NnMaxLen = (int)(nnMaxLenScrollBar.Value + 0.5);
        }

        private void maxLoopScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Design.Designer.Settings.TmThalSettings.MaxLoop = (int)(maxLoopScrollBar.Value + 0.5);
        }

        private void mutationScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Design.Designer.Settings.LeaSettings.MutationRate = mutationScrollBar.Value;
        }

        private void crossoverScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Design.Designer.Settings.LeaSettings.CrossoverRate = crossoverScrollBar.Value;
        }

        private void learningScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Design.Designer.Settings.LeaSettings.LearningRate = learningScrollBar.Value;
        }

        private void popsizeScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Design.Designer.Settings.LeaSettings.PopulationSize = (int)(popsizeScrollBar.Value + 0.5);
        }

        private void tournamentScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Design.Designer.Settings.LeaSettings.TournamentSize = (int)(tournamentScrollBar.Value + 0.5);
        }

       
    }
}
