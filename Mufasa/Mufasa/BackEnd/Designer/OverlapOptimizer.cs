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
            this.LeaBestAcrossGenerations = new List<double>();
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

        /// <value>
        /// List of best solutions of each generation.
        /// </value>
        private List<double> LeaBestAcrossGenerations;

        /// <value>
        /// Best solution.
        /// </value>
        private Chromosome LeaBest;

        /// <summary>
        /// Lamarckian evolutionary algorithm for overlap optimization.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        public void LeaOptimizeOverlaps(object o, DoWorkEventArgs args)
        {
            this.b = o as BackgroundWorker;

            List<Chromosome> population = Populate();
            List<Chromosome> nextPopulation;
            Random rand = new Random();
            int progress;

            do
            {
                //Local Search
                nextPopulation = LocalSearch(population);

                do
                {
                    EvaluatePopulation(nextPopulation);
                    List<Chromosome> tournament = SelectForTournament(this.Settings.LeaSettings.TournamentSize, population, rand);

                    //Selection
                    if (rand.NextDouble() <= this.Settings.LeaSettings.CrossoverRate)
                    {
                        //Crossover
                        Chromosome mom = Tournament(tournament);
                        Chromosome dad = Tournament(tournament);
                        Tuple<Chromosome, Chromosome> children = Crossover(mom, dad, rand);
                        nextPopulation.Add(children.Item1);
                        nextPopulation.Add(children.Item2);
                    }
                    else
                    {
                        Chromosome child = Tournament(tournament);
                        nextPopulation.Add(child);
                    }
                } while (nextPopulation.Count < population.Count);

                //Mutation
                nextPopulation = MutatePopulation(nextPopulation, rand);

                Chromosome best = Tournament(nextPopulation);
                LeaBestAcrossGenerations.Add(best.Score.NormalizedScore);

                if (best.Score.NormalizedScore < LeaBest.Score.NormalizedScore)
                {
                    //copy the best solution so far
                    LeaBest = new Chromosome(best);
                }

                population = nextPopulation;

                double worstScore = LeaBestAcrossGenerations.Max();

                //progress = 100 if epsilon == variance
                progress = (int)((100.0 / worstScore) * (worstScore + this.Settings.LeaSettings.Epsilon - Variance(LeaBestAcrossGenerations)) + 0.5);
                if (progress > 100)
                {
                    //when variance lower than epsilon
                    progress = 100;
                }
                b.ReportProgress(progress);

            } while (progress != 100);
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

        /// <summary>
        /// Evaluate population
        /// </summary>
        /// <param name="population">Population to evaluate.</param>
        private void EvaluatePopulation (List<Chromosome> population)
        {
            foreach(Chromosome c in population)
            {
                c.Evaluate(this.Construct.Overlaps, this.Settings);
            }
        }

        private List<Chromosome> LocalSearch(List<Chromosome> population)
        {
            //TODO
            return population;
        }

        /// <summary>
        /// Selects random chromosomes for tournament.
        /// </summary>
        /// <param name="tournamentSize">Tournament size.</param>
        /// <param name="population">Whole population to select from.</param>
        /// <param name="rand">Randomizer.</param>
        /// <returns>List of contestors.</returns>
        private List<Chromosome> SelectForTournament(int tournamentSize, List<Chromosome> population, Random rand)
        {
            List<Chromosome> tournament = new List<Chromosome>();

            for (int i = 0; i < tournamentSize; i++)
            {
                tournament.Add(population[rand.Next(population.Count)]);
            }

            return tournament;
        }

        /// <summary>
        /// Tournament with one winner.
        /// </summary>
        /// <param name="contestors">Tournament contestors.</param>
        /// <returns>Best chromosome in the tournament.</returns>
        private Chromosome Tournament(List<Chromosome> contestors)
        {
            //Ascending order - score minimization
            contestors = contestors.OrderBy(o => o.Score.NormalizedScore).ToList();
            return contestors[0];
        }

        /// <summary>
        /// Perform uniform crossover.
        /// </summary>
        /// <param name="mom">Parent 1.</param>
        /// <param name="dad">Parent 2.</param>
        /// <param name="rand">Randomizer.</param>
        /// <returns>Tuple of children.</returns>
        private Tuple<Chromosome, Chromosome> Crossover(Chromosome mom, Chromosome dad, Random rand)
        {
            List<int> len_3_1 = new List<int>();
            List<int> len_5_1 = new List<int>();
            List<int> len_3_2 = new List<int>();
            List<int> len_5_2 = new List<int>();

            for (int i = 0; i < mom.Lengths_3.Count; i++)
            {
                if (rand.NextDouble() < 0.5)
                {
                    len_3_1.Add(mom.Lengths_3[i]);
                    len_5_1.Add(mom.Lengths_5[i]);
                    len_3_2.Add(dad.Lengths_3[i]);
                    len_5_2.Add(dad.Lengths_5[i]);
                }
                else
                {
                    len_3_1.Add(dad.Lengths_3[i]);
                    len_5_1.Add(dad.Lengths_5[i]);
                    len_3_2.Add(mom.Lengths_3[i]);
                    len_5_2.Add(mom.Lengths_5[i]);
                }

            }
            Chromosome child1 = new Chromosome(len_3_1, len_5_1);
            Chromosome child2 = new Chromosome(len_3_2, len_5_2);

            Tuple<Chromosome, Chromosome> children = new Tuple<Chromosome, Chromosome>(child1, child2);
            return children;
        }

        private Chromosome Mutate(Chromosome solution, Random rand)
        {
            if (rand.NextDouble() < 0.5D)
            {
                //Mutate 3'
                int index = rand.Next(solution.Lengths_3.Count);
                int value = rand.Next(this.Settings.MinLen_3, this.Settings.MaxLen_3 + 1);
                solution.Lengths_3[index] = value;
            }
            else
            {
                //Mutate 5'
                int index = rand.Next(solution.Lengths_5.Count);
                int value = rand.Next(this.Settings.MinLen_5, this.Settings.MaxLen_5 + 1);
                solution.Lengths_5[index] = value;
            }
            return solution;
        }

        /// <summary>
        /// Mutate population of solutions.
        /// </summary>
        /// <param name="population">Population.</param>
        /// <param name="rand">Randomizer.</param>
        /// <returns>Mutated population.</returns>
        private List<Chromosome> MutatePopulation(List<Chromosome> population, Random rand)
        {
            for (int i = 0; i < population.Count; i++)
            {
                if (rand.NextDouble() <= this.Settings.LeaSettings.MutationRate)
                {
                    //Solution chosen to be mutated
                    population[i] = Mutate(population[i], rand);
                }
            }
            return population;
        }
     
        /// <summary>
        /// Check if stopping criterion satisfied.
        /// </summary>
        /// <returns>True if variance lower than epsilon.</returns>
        private bool Stop()
        {
            double variance = Variance(this.LeaBestAcrossGenerations);

            return (variance < this.Settings.LeaSettings.Epsilon);
        }

        /// <summary>
        /// Calculate variance using Welford's method.
        /// </summary>
        /// <param name="valueList">List of values.</param>
        /// <returns></returns>
        private double Variance(List<double> valueList)
        {
            //Jaime, Jason S, Alexandre C; May 22 '09 @ StackOverflow
            //John D. Cook's blog of statistical computing.

            double M = 0.0;
            double S = 0.0;
            int k = 1;
            foreach (double value in valueList)
            {
                double tmpM = M;
                M += (value - tmpM) / k;
                S += (value - tmpM) * (value - M);
                k++;
            }

            return S / (k - 1); //whole population variance. 
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
                         * Solution: run without debugging. I want that exception here, for now.
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
