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
        /// Overlap greedy temperature optimization.
        /// </summary>
        public ScoreTotal GreedyOptimizeOverlaps(ref Construct construct)
        {
            const byte end_3 = 255;
            const byte end_5 = 255;

            for (int i = 0; i < construct.Overlaps.Count; i++)
            {
                byte item_3 = 0;
                byte item_5 = 0;
                bool done_3 = false;
                bool done_5 = false;

                double _diff = construct.Overlaps[i].Temperature - construct.Settings.TargetTm;
                double diff = _diff;
                Overlap _previous;

                do
                {
                    if ((item_5 != end_5))
                    {
                        _previous = new Overlap(construct.Overlaps[i]);
                        item_5 = construct.Overlaps[i].Dequeue(construct.Settings.MinLen_5);
                        diff = construct.Overlaps[i].Temperature - construct.Settings.TargetTm;

                        //if previous was more optimal, do not accept the candidate solution and stop
                        if (Math.Abs(_diff) < Math.Abs(diff))
                        {
                            construct.Overlaps[i] = _previous;
                            diff = _diff;
                            done_5 = true;
                        }
                        else
                        {
                            _diff = diff;
                            done_5 = false;
                        }
                    }
                    else
                    {
                        done_5 = true;
                    }

                    if ((item_3 != end_3))
                    {
                        _previous = new Overlap(construct.Overlaps[i]);
                        item_3 = construct.Overlaps[i].Pop(construct.Settings.MinLen_3);
                        diff = construct.Overlaps[i].Temperature - construct.Settings.TargetTm;

                        //if previous was more optimal, do not accept the candidate solution and stop
                        if (Math.Abs(_diff) < Math.Abs(diff))
                        {
                            construct.Overlaps[i] = _previous;
                            diff = _diff;
                            done_3 = true;
                        }
                        else
                        {
                            _diff = diff;
                            done_3 = false;
                        }
                    }
                    else
                    {
                        done_3 = true;
                    }

                } while (!done_5 || !done_3);                
            }
            return new ScoreTotal(construct.Overlaps, construct.Settings.TargetTm);
        }
    }
}
