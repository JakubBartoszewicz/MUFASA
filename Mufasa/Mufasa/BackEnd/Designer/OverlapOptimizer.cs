using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Mufasa.BackEnd.Lea;
using Mufasa.BackEnd.Scores;
using Mufasa.BackEnd.Exceptions;

namespace Mufasa.BackEnd.Designer
{
    /// <remarks>
    /// Overlap optimizer class.
    /// </remarks>
    class OverlapOptimizer
    {
        /// <summary>
        /// Overlap optimizer constructor.
        /// </summary>
        public OverlapOptimizer()
        { }

        /// <summary>
        /// Overlap optimizer constructor.
        /// </summary>
        /// <param name="construct">A construct to assemble.</param>
        public OverlapOptimizer(Construct construct, DesignerSettings settings)
        {
            this.Construct = construct;
            this.Settings = settings;
        }

        /// <value>
        /// A construct to assemble.
        /// </value>
        public Construct Construct { get; set; }

        /// <value>
        /// Designer settings.
        /// </value>
        public DesignerSettings Settings { get; set; }

        /// <value>
        /// BackGroudWorker for optimization.
        /// </value>
        private BackgroundWorker b;


        /// <summary>
        /// Lamarckian evolutionary algorithm for overlap optimization.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        public void LeaOptimizeOverlaps(object o, DoWorkEventArgs args)
        {
            this.b = o as BackgroundWorker;

            List<Chromosome> population = Populate();
        }

        /// <summary>
        /// Generate starting population.
        /// </summary>
        /// <returns></returns>
        private List<Chromosome> Populate()
        {
            List<Chromosome> population = new List<Chromosome>();
            for (int c = 0; c < this.Settings.LeaSettings.PopulationSize; c++)
            {
                List<int> len_3 = new List<int>();
                List<int> len_5 = new List<int>();
                Random rand = new Random();

                for (int i = 0; i < this.Construct.Overlaps.Count; i++)
                {
                    len_3.Add(rand.Next(this.Settings.MinLen_3, this.Settings.MaxLen_3 + 1));
                    len_5.Add(rand.Next(this.Settings.MinLen_5, this.Settings.MaxLen_5 + 1));
                }

                population.Add(new Chromosome(len_3, len_5));
            }

            return population;
        }

        private void Crossover() { }

        private void LocalSearch() { }

        private void Mutate() { }

        private void Select() { }

        private bool Stop()
        {
            return false;
        }






        /// <summary>
        /// Overlap naive-greedy temperature optimization.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        public void SemiNaiveOptimizeOverlaps(object o, DoWorkEventArgs args)
        {
            this.b = o as BackgroundWorker;

            const byte end_3 = 255;
            const byte end_5 = 255;
            int progress = 0;

            for (int i = 0; i < this.Construct.Overlaps.Count; i++)
            {
                byte item_3 = 0;
                byte item_5 = 0;
                bool done_3 = false;
                bool done_5 = false;
                bool tmTooHigh = true;

                double diff = this.Construct.Overlaps[i].MeltingTemperature - this.Construct.Settings.TargetTm;
                double _bestDiff = diff;
                Overlap _best = new Overlap(this.Construct.Overlaps[i]);

                do
                {
                    if ((item_5 != end_5))
                    {
                        item_5 = this.Construct.Overlaps[i].Dequeue(this.Construct.Settings.MinLen_5);

                        diff = this.Construct.Overlaps[i].MeltingTemperature - this.Construct.Settings.TargetTm;
                        tmTooHigh = (this.Construct.Overlaps[i].MeltingTemperature > this.Construct.Settings.TargetTm);

                        if (Math.Abs(_bestDiff) > Math.Abs(diff))
                        {
                            //check if hairpin is acceptable

                            if (this.Construct.Overlaps[i].IsAcceptable(this.Construct.Settings.MaxTh, this.Construct.Settings.MaxTd, false))
                            {
                                //if found a better solution, copy it, and do not stop
                                _bestDiff = diff;
                                _best = new Overlap(this.Construct.Overlaps[i]);
                                done_5 = false;
                            }
                        }
                        else
                        {
                            //if passed the threshold, done
                            if (!tmTooHigh)
                            {
                                done_5 = true;
                            }
                        }

                    }
                    else
                    {
                        done_5 = true;
                    }

                    if ((item_3 != end_3))
                    {
                        item_3 = this.Construct.Overlaps[i].Pop(this.Construct.Settings.MinLen_3);
                        diff = this.Construct.Overlaps[i].MeltingTemperature - this.Construct.Settings.TargetTm;

                        tmTooHigh = (this.Construct.Overlaps[i].MeltingTemperature > this.Construct.Settings.TargetTm);

                        if (Math.Abs(_bestDiff) > Math.Abs(diff))
                        {
                            //check if hairpin is acceptable

                            if (this.Construct.Overlaps[i].IsAcceptable(this.Construct.Settings.MaxTh, this.Construct.Settings.MaxTd, false))
                            {
                                //if found a better solution, copy it, and do not stop
                                _bestDiff = diff;
                                _best = new Overlap(this.Construct.Overlaps[i]);
                                done_3 = false;
                            }
                        }
                        else
                        {
                            //if passed the threshold, done
                            if (!tmTooHigh)
                            {
                                done_3 = true;
                            }
                        }
                    }
                    else
                    {
                        done_3 = true;
                    }
                    
                    if (item_3 == end_3 && item_5 == end_5 && !this.Construct.Overlaps[i].IsAcceptable(this.Construct.Settings.MaxTh, this.Construct.Settings.MaxTd, false))
                    {

                        /*
                         * If you are running under the Visual Studio debugger, the debugger will break
                         * at the point in the DoWork event handler where the unhandled exception was raised.
                         * - Mark Cranness, Mar 9 '11 @ StackOverflow
                         * 
                         * Solution: run without debugging. I want that exception here.
                         */
                        throw new AssemblyException(this.Construct.Overlaps[i].ToString());
                    }

                } while (!done_5 || !done_3);

                this.Construct.Overlaps[i] = _best;

                progress = (int)(100.0 * (double)i / (double)(this.Construct.Overlaps.Count - 1));
                b.ReportProgress(progress);
            }

            for (int i = 0; i < this.Construct.Overlaps.Count; i++)
            {
                //Duplex melting temperatures
                this.Construct.Overlaps[i].HeterodimerMeltingTemperature = this.Construct.Overlaps[i].GetDuplexTemperature(this.Construct.Overlaps[this.Construct.Overlaps[i].PairIndex]);
            }
            this.Construct.Evaluate();
        }
    }
}
