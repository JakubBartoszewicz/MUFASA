﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mufasa.BackEnd.Designer;

//Copyright (C) 2015 Jakub Bartoszewicz (if not stated otherwise)
namespace Mufasa.BackEnd.Scores
{
    class ScoreMean : Score
    {

        /// <summary>
        /// ScoreMean constructor.
        /// </summary>
        public ScoreMean()
        {
            this.Label = "Sm";
            this.Description = "Deviation from mean Tm";
        }

        /// <summary>
        /// ScoreMean constructor.
        /// </summary>
        public ScoreMean(List<Overlap> overlaps) 
        {
            this.Label = "Sm";
            this.Description = "Deviation from mean Tm";
            Rescore(overlaps);
        }

        /// <summary>
        /// ScoreMean copying constructor.
        /// </summary>
        /// <param name="s">Score to copy.</param>
        public ScoreMean(ScoreMean s)
        {
            this.RawScore = s.RawScore;
            this.NormalizedScore = s.NormalizedScore;
        }

        /// <summary>
        /// Scoring function.
        /// </summary>
        /// <param name="overlaps">Overlap list.</param>
        override public void Rescore(List<Overlap> overlaps)
        {
            this.RawScore = 0.0;
            double sum = 0.0;

            foreach (Overlap o in overlaps)
            {
                sum += o.MeltingTemperature;
            }

            double mean = sum / overlaps.Count;

            foreach (Overlap o in overlaps)
            {
                RawScore += Math.Abs(o.MeltingTemperature - mean);
            }
            
            this.NormalizedScore = this.RawScore / overlaps.Count;
        }
    }
}
