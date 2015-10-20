using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mufasa.BackEnd.Scores;

namespace Mufasa.BackEnd.Designer
{
    class OverlapOptimizer
    {
        /// <summary>
        /// Overlap optimizer constructor.
        /// </summary>
        public OverlapOptimizer()
        { }

        /// <summary>
        /// Overlap naive-greedy temperature optimization.
        /// </summary>
        public ScoreTotal SemiNaiveOptimizeOverlaps(ref Construct construct)
        {
            const byte end_3 = 255;
            const byte end_5 = 255;

            for (int i = 0; i < construct.Overlaps.Count; i++)
            {
                byte item_3 = 0;
                byte item_5 = 0;
                bool done_3 = false;
                bool done_5 = false;
                bool tmTooHigh = true;
                
                double diff = construct.Overlaps[i].MeltingTemperature - construct.Settings.TargetTm;
                double _bestDiff = diff;
                               Overlap _best = construct.Overlaps[i];



                do
                {
                    if ((item_5 != end_5))
                    {
                        item_5 = construct.Overlaps[i].Dequeue(construct.Settings.MinLen_5);
                                                
                        diff = construct.Overlaps[i].MeltingTemperature - construct.Settings.TargetTm;
                        tmTooHigh = (construct.Overlaps[i].MeltingTemperature > construct.Settings.TargetTm);

                        if (Math.Abs(_bestDiff) > Math.Abs(diff))
                        {
                            //if found a better solution, copy it, and do not stop
                            _bestDiff = diff;
                            _best = construct.Overlaps[i];
                            done_5 = false;
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
                        item_3 = construct.Overlaps[i].Pop(construct.Settings.MinLen_3);
                        diff = construct.Overlaps[i].MeltingTemperature - construct.Settings.TargetTm;

                        tmTooHigh = (construct.Overlaps[i].MeltingTemperature > construct.Settings.TargetTm);

                        if (Math.Abs(_bestDiff) > Math.Abs(diff))
                        {
                            //if found a better solution, copy it, and do not stop
                            _bestDiff = diff;
                            _best = construct.Overlaps[i];
                            done_3 = false;
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

                    //Duplex melting temperatures
                    construct.Overlaps[i].DuplexMeltingTemperature = construct.Overlaps[i].GetDuplexTemperature(construct.Overlaps[construct.Overlaps[i].PairIndex]);
                    construct.Overlaps[construct.Overlaps[i].PairIndex].DuplexMeltingTemperature = construct.Overlaps[i].DuplexMeltingTemperature;

                } while (!done_5 || !done_3);

                construct.Overlaps[i] = _best;
            }
            return new ScoreTotal(construct.Overlaps, construct.Settings.TargetTm);
        }
    }
}
