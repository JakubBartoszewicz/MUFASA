using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mufasa.BackEnd.Scores;
using System.ComponentModel;

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
        public OverlapOptimizer(Construct construct)
        {
            this.Construct = construct;
        }

        /// <value>
        /// A construct to assemble.
        /// </value>
        public Construct Construct { get; set; }

        /// <value>
        /// BackGroudWorker for optimization.
        /// </value>
        private BackgroundWorker b;

        /// <summary>
        /// Overlap naive-greedy temperature optimization.
        /// </summary>
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
                Overlap _best = this.Construct.Overlaps[i];

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

                            if (this.Construct.Overlaps[i].HairpinMeltingTemperature <= this.Construct.Settings.MaxTh)
                            {
                                //if found a better solution, copy it, and do not stop
                                _bestDiff = diff;
                                _best = this.Construct.Overlaps[i];
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

                            if (this.Construct.Overlaps[i].HairpinMeltingTemperature <= this.Construct.Settings.MaxTh)
                            {
                                //if found a better solution, copy it, and do not stop
                                _bestDiff = diff;
                                _best = this.Construct.Overlaps[i];
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
