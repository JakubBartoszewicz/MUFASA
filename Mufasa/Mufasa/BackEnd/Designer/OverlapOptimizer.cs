using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mufasa.BackEnd.Designer
{
    class OverlapOptimizer
    {
        /// <summary>
        /// Overlap optimizer constructor.
        /// </summary>
        public OverlapOptimizer(DesignerSettings settings)
        {
            this.Settings = settings;
        }

        /// <summary>
        /// Desinger settings used for overlap optimization.
        /// </summary>
        public DesignerSettings Settings { get; set; }


        /// <summary>
        /// Overlap greedy temperature optimization.
        /// </summary>
        public List<Overlap> GreedyOptimizeOverlaps(List<Overlap> overlaps)
        {
            const byte end_3 = 255;
            const byte end_5 = 255;

            for (int i = 0; i < overlaps.Count; i++)
            {
                byte item_3 = 0;
                byte item_5 = 0;
                bool done_3 = false;
                bool done_5 = false;

                double _diff = overlaps[i].Temperature - this.Settings.TargetTm;
                double diff = _diff;
                Overlap _previous;

                do
                {
                    if ((item_5 != end_5))
                    {
                        _previous = new Overlap(overlaps[i]);
                        item_5 = overlaps[i].Dequeue(Settings.MinLen_5);
                        diff = overlaps[i].Temperature - this.Settings.TargetTm;

                        //if previous was more optimal, do not accept the candidate solution and stop
                        if (Math.Abs(_diff) < Math.Abs(diff))
                        {
                            overlaps[i] = _previous;
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
                        _previous = new Overlap(overlaps[i]);
                        item_3 = overlaps[i].Pop(Settings.MinLen_3);
                        diff = overlaps[i].Temperature - this.Settings.TargetTm;

                        //if previous was more optimal, do not accept the candidate solution and stop
                        if (Math.Abs(_diff) < Math.Abs(diff))
                        {
                            overlaps[i] = _previous;
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

            return overlaps;
        }
    }
}
