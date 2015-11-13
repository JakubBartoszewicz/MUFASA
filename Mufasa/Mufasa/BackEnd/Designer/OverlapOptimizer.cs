using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Mufasa.BackEnd.Lea;
using Mufasa.BackEnd.Scores;
using Mufasa.BackEnd.Exceptions;

//Copyright (C) 2014, 2015 Jakub Bartoszewicz (if not stated otherwise)
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
            this.Templates = new List<Overlap>();
            foreach (Overlap o in this.Construct.Overlaps)
            {
                this.Templates.Add(new Overlap(o));
            }
            this.Settings = settings;
            this.leaBestAcrossGenerations = new List<double>();
            this.IgnorePreoptimizationExceptions = true;
        }

        /// <value>
        /// A construct to assemble.
        /// </value>
        public Construct Construct { get; set; }

        /// <value>
        /// True to ignore inacceptable solutions during the preoptimization stage.
        /// </value>
        public bool IgnorePreoptimizationExceptions { get; set; }

        /// <value>
        /// Overlap templates.
        /// </value>
        public List<Overlap> Templates { get; set; }

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
        private List<double> leaBestAcrossGenerations;

        /// <value>
        /// Best solution.
        /// </value>
        private Chromosome leaBest;

        /// <value>
        /// True to stop optimization.
        /// </value>
        private bool stop;

        /// <summary>
        /// Lamarckian evolutionary algorithm for overlap optimization.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        public void LeaOptimizeOverlaps(object o, DoWorkEventArgs args)
        {
            stop = false;

            this.b = o as BackgroundWorker;

            Random rand = new Random();
            List<Chromosome> population;

            List<Chromosome> nextPopulation = new List<Chromosome>();
            int progress = 0;
            b.ReportProgress(progress);
            List<Chromosome> tournament;
            Chromosome mom, dad, child, best;
            Tuple<Chromosome, Chromosome> children;
            double variance;
            double maxVariance = 0.0;
            int i = 0;

            population = Populate(rand, this.Construct.Overlaps);

            //Local Search evaluates population
            EvaluatePopulation(population, this.Settings.LeaSettings.IgnoreHeterodimers);
            if (!stop)
                population = LocalSearch(population, rand, this.Settings.LeaSettings.IgnoreHeterodimers);
            best = new Chromosome(Tournament(population));
            leaBest = new Chromosome(best);
            List<Chromosome> bestes = new List<Chromosome>();
            leaBestAcrossGenerations.Add(best.Score.NormalizedScore);
            bestes.Add(new Chromosome(best));

            do
            {
                //Selection
                do
                {
                    //Crossover
                    if (rand.NextDouble() <= this.Settings.LeaSettings.CrossoverRate)
                    {
                        tournament = SelectForTournament(this.Settings.LeaSettings.TournamentSize, population, rand);
                        mom = Tournament(tournament);
                        tournament = SelectForTournament(this.Settings.LeaSettings.TournamentSize, population, rand);
                        dad = Tournament(tournament);
                        children = Crossover(mom, dad, rand);
                        nextPopulation.Add(children.Item1);
                        nextPopulation.Add(children.Item2);
                    }
                    else
                    {
                        //Add two children without crossing over
                        tournament = SelectForTournament(this.Settings.LeaSettings.TournamentSize, population, rand);
                        child = new Chromosome(Tournament(tournament));
                        nextPopulation.Add(child);
                        tournament = SelectForTournament(this.Settings.LeaSettings.TournamentSize, population, rand);
                        child = new Chromosome(Tournament(tournament));
                        nextPopulation.Add(child);
                    }
                } while (nextPopulation.Count < population.Count);

                //Mutation
                nextPopulation = MutatePopulation(nextPopulation, rand);

                //Local Search evaluates population
                if (!stop)
                    EvaluatePopulation(nextPopulation, this.Settings.LeaSettings.IgnoreHeterodimers);
                if (!stop)
                    nextPopulation = LocalSearch(nextPopulation, rand, this.Settings.LeaSettings.IgnoreHeterodimers);
                if (!stop)
                {
                    best = Tournament(nextPopulation);

                    bestes.Add(new Chromosome(best));
                    leaBestAcrossGenerations.Add(best.Score.NormalizedScore);
                }
                if (!stop && best.Score.NormalizedScore < leaBest.Score.NormalizedScore)
                {
                    //copy the best solution so far
                    leaBest = new Chromosome(best);
                }

                population.Clear();
                population.AddRange(nextPopulation);
                nextPopulation.Clear();


                // assess variance only if i > MinIterations
                variance = Variance(leaBestAcrossGenerations);

                if (variance > maxVariance)
                {
                    maxVariance = variance;
                }


                //progress = 100 if epsilon == variance
                //if variance descending
                if (maxVariance > variance && i > this.Settings.LeaSettings.MinIterations)
                {
                    //don't confuse the user
                    progress = Math.Max((int)(100.0 * (double)i / (double)this.Settings.LeaSettings.MaxIterations), (int)((100.0 / maxVariance) * (maxVariance + this.Settings.LeaSettings.Epsilon - variance) + 0.5));
                }
                else
                {
                    progress = (int)Math.Max((100.0 * (double)i / (double)this.Settings.LeaSettings.MaxIterations), (double)progress);
                }

                if (progress > 100)
                {
                    //when variance lower than epsilon
                    progress = 100;
                }
                b.ReportProgress(progress);
                i++;
            } while (!stop && (progress < 100) && (i < this.Settings.LeaSettings.MaxIterations));

            progress = 100;
            b.ReportProgress(progress);
            stop = false;

            if (leaBest == null || leaBest.Score.Equals(ScoreTotal.Inacceptable))
            {
                throw new AssemblyException();
            }
            else
            {
                this.Construct.Overlaps = leaBest.ToOverlaps(this.Templates);
                this.Construct.Evaluate();
            }
        }

        /// <summary>
        /// Generate starting population.
        /// </summary>
        /// <param name="rand">Randomizer.</param>
        /// <param name="preoptimized">preoptimized solution.</param>
        /// <returns>Starting population.</returns>
        private List<Chromosome> Populate(Random rand, List<Overlap> preoptimized = null)
        {
            List<Chromosome> population = new List<Chromosome>();
            if (preoptimized != null)
            {
                population.Add(new Chromosome(preoptimized, this.Settings.TargetTm));
                population.Add(new Chromosome(preoptimized, this.Settings.TargetTm));
            }
            for (int c = population.Count; c < this.Settings.LeaSettings.PopulationSize; c++)
            {
                List<int> len_3 = new List<int>();
                List<int> len_5 = new List<int>();

                for (int i = 0; i < this.Templates.Count; i++)
                {
                    len_3.Add(rand.Next(this.Settings.MinLen_3, this.Settings.MaxLen_3 + 1));
                    len_5.Add(rand.Next(this.Settings.MinLen_5, this.Settings.MaxLen_5 + 1));
                }

                population.Add(new Chromosome(len_3, len_5, this.Settings.TargetTm));
            }

            return population;
        }

        /// <summary>
        /// Evaluate population
        /// </summary>
        /// <param name="population">Population to evaluate.</param>
        private void EvaluatePopulation(List<Chromosome> population, bool ignoreHeterodimers)
        {
            foreach (Chromosome c in population)
            {
                c.Evaluate(this.Templates, this.Settings, ignoreHeterodimers);
            }
        }

        /// <summary>
        /// Search for local improvement.
        /// </summary>
        /// <param name="population">Population to improve.</param>
        /// <param name="rand">Randomizer.</param>
        /// <param name="ignoreHeterodimers">True to ignore heterodimer melting temperatures.</param>
        /// <returns>Refined population.</returns>
        private List<Chromosome> LocalSearch(List<Chromosome> population, Random rand, bool ignoreHeterodimers = false)
        {
            List<Chromosome> pool = new List<Chromosome>();
            int index;
            //Ascending order - score minimization
            population = population.OrderBy(o => o.Score.NormalizedScore).ToList();

            for (int i = 0; i < population.Count; i++)
            {
                if ((double)i < this.Settings.LeaSettings.LearningRate * (double)population.Count)
                {
                    index = rand.Next(this.Templates.Count);
                    pool.Clear();
                    pool.Add(new Chromosome(population[i]));
                    pool.Add(new Chromosome(population[i]));
                    pool.Add(new Chromosome(population[i]));
                    pool.Add(new Chromosome(population[i]));

                    if (population[i].Lengths_3[index] < this.Settings.MaxLen_3)
                        pool[0].Lengths_3[index]++;
                    if (population[i].Lengths_3[index] > this.Settings.MinLen_3)
                        pool[1].Lengths_3[index]--;
                    if (population[i].Lengths_5[index] < this.Settings.MaxLen_5)
                        pool[2].Lengths_5[index]++;
                    if (population[i].Lengths_5[index] > this.Settings.MinLen_5)
                        pool[3].Lengths_5[index]--;

                    EvaluatePopulation(pool, ignoreHeterodimers);
                    pool.Add(population[i]);
                    population[i] = Tournament(pool);

                }
                else
                {
                    break;
                }
            }

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
            Chromosome child1 = new Chromosome(len_3_1, len_5_1, this.Settings.TargetTm);
            Chromosome child2 = new Chromosome(len_3_2, len_5_2, this.Settings.TargetTm);

            Tuple<Chromosome, Chromosome> children = new Tuple<Chromosome, Chromosome>(child1, child2);
            return children;
        }

        /// <summary>
        /// Mutate one chromosome.
        /// </summary>
        /// <param name="solution">Solution to mutate.</param>
        /// <param name="rand">Randomizer.</param>
        /// <returns>Mutated chromosome.</returns>
        private Chromosome Mutate(Chromosome solution, Random rand)
        {
            int index;
            int value;
            if (rand.NextDouble() < 0.5D)
            {
                //Mutate 3'
                index = rand.Next(solution.Lengths_3.Count);
                value = rand.Next(this.Settings.MinLen_3, this.Settings.MaxLen_3 + 1);
                solution.Lengths_3[index] = value;
            }
            else
            {
                //Mutate 5'
                index = rand.Next(solution.Lengths_5.Count);
                value = rand.Next(this.Settings.MinLen_5, this.Settings.MaxLen_5 + 1);
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
        /// Stop the calcualations.
        /// </summary>
        public void Stop()
        {
            this.stop = true;
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
                //Do not count invalid solutions
                if (!Double.IsPositiveInfinity(value))
                {
                    double tmpM = M;
                    M += (value - tmpM) / k;
                    S += (value - tmpM) * (value - M);
                    k++;
                }
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
            stop = false;

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

                    if (!this.IgnorePreoptimizationExceptions && item_3 == end_3 && item_5 == end_5 && !this.Construct.Overlaps[i].IsAcceptable(this.Construct.Settings.MaxTh, this.Construct.Settings.MaxTd, true))
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

                } while (!stop && (!done_5 || !done_3));

                this.Construct.Overlaps[i] = _best;

                progress = (int)(100.0 * (double)i / (double)(this.Construct.Overlaps.Count - 1));
                b.ReportProgress(progress);
            }

            stop = false;
            this.Construct.Evaluate();

            if (!this.IgnorePreoptimizationExceptions && Double.IsPositiveInfinity(this.Construct.Score.NormalizedScore))
            {
                //if failed to achieve an acceptable construct
                throw new AssemblyException("Heteroduplex melting temperature too high. ");
            }

        }

    }
}
