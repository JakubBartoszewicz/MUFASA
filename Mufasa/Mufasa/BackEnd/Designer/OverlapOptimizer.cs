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
        public ScoreTotal NaiveOptimizeOverlaps(ref Construct construct)
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
                
                //double _diff = construct.Overlaps[i].Temperature - construct.Settings.TargetTm;
                double diff = construct.Overlaps[i].Temperature - construct.Settings.TargetTm;
                double _bestDiff = diff;
                //Overlap _previous;
                Overlap _best = construct.Overlaps[i];



                do
                {
                    if ((item_5 != end_5))
                    {
                        //_previous = new Overlap(construct.Overlaps[i]);
                        item_5 = construct.Overlaps[i].Dequeue(construct.Settings.MinLen_5);
                        diff = construct.Overlaps[i].Temperature - construct.Settings.TargetTm;
                        tmTooHigh = (construct.Overlaps[i].Temperature > construct.Settings.TargetTm);

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

                        

                        ////if after the tmTooHigh treshold and previous was more optimal, do not accept the candidate solution and stop
                        //if (!tmTooHigh && (Math.Abs(_diff) < Math.Abs(diff)))
                        //{
                        //    construct.Overlaps[i] = _previous;
                        //    diff = _diff;
                        //    done_5 = true;
                        //}
                        //else
                        //{
                        //    _diff = diff;
                        //    done_5 = false;
                        //}
                    }
                    else
                    {
                        done_5 = true;
                    }

                    if ((item_3 != end_3))
                    {
                        //_previous = new Overlap(construct.Overlaps[i]);
                        item_3 = construct.Overlaps[i].Pop(construct.Settings.MinLen_3);
                        diff = construct.Overlaps[i].Temperature - construct.Settings.TargetTm;

                        tmTooHigh = (construct.Overlaps[i].Temperature > construct.Settings.TargetTm);

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


                        //if (!tmTooHigh)
                        //{
                        //    tmTooHigh = (construct.Overlaps[i].Temperature > construct.Settings.TargetTm);
                        //}

                        ////if after the tmTooHigh treshold previous was more optimal, do not accept the candidate solution and stop
                        //if (!tmTooHigh && (Math.Abs(_diff) < Math.Abs(diff)))
                        //{
                        //    construct.Overlaps[i] = _previous;
                        //    diff = _diff;
                        //    done_3 = true;
                        //}
                        //else
                        //{
                        //    _diff = diff;
                        //    done_3 = false;
                        //}
                    }
                    else
                    {
                        done_3 = true;
                    }

                } while (!done_5 || !done_3);

                construct.Overlaps[i] = _best;
            }
            return new ScoreTotal(construct.Overlaps, construct.Settings.TargetTm);
        }
    }
}
