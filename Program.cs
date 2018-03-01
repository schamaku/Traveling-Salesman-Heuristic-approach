using System;
using System.Collections.Generic;

/**
 * 
 * @author BinduK This program tries to emulate the implementation provided in the paper
 *         "A Traveling-Salesman-Based Approach to Aircraft Scheduling in the Terminal Area"
 *         by Robert A. Luenberger
 *
 */

namespace CSharp
{
    class Program
    {
        static List<int> inputlist = new List<int>();
        static int[] inputArray; 
        static int nofAirCrafts, initCost = 0, iDiff, init; 
        static int[] IACTP = { 0, 1, 1, 2, 5, 7, 1, 2, 5, 7, 6, 1, 1, 2, 5, 7, 1, 2, 5, 7, 6, 6, 1, 1, 2, 5, 7, 1, 2, 5, 7,
            6, 6, 1, 1, 2, 6, 6, 1, 1, 2 };
        static int[] IFFX = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25,
            26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40 };
        static int[,] iCost = { { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 104, 125, 0, 0, 114, 135, 230 },
            { 0, 78, 78, 0, 0, 88, 88, 170 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 114, 135, 0, 0, 124, 145, 240 }, { 0, 88, 88, 0, 0, 98, 98, 180 }, { 0, 88, 88, 0, 0, 98, 98, 130 } };
        static int[] iCoord ;
        static double[] rnd;
        static int[] iWeight = new int[IACTP.Length];
        static int[] iBestOrd;
        static int[] iRoute = new int[IFFX.Length];
        static int[] iEver;
        static int mps, imark;
        static List<int> localOptimumList = new List<int>();
        static Random randomG = new Random(2);
        static List<string> WriteToFile = new List<string>();

        static void Main(string[] args)
        {
            int iTime = 0, ifc = 0, iBever = 0, iFOIS = 0, zHap = 0;
            var readInput = System.IO.File.ReadAllLines(@"TSPInput.txt");
            // Read input data and initialize arrays
            foreach (var line in readInput)
            {
                int x = int.Parse(line);
                inputlist.Add(x);
            }
            
            inputArray = inputlist.ToArray();
            nofAirCrafts = inputArray.Length;
            iCoord = new int[nofAirCrafts + 1];
            rnd = new double[nofAirCrafts + 1];
            iBestOrd = new int[nofAirCrafts + 1];
            iEver = new int[nofAirCrafts + 1];

            // Initially all aircrafts are in FCFS-RW order
            // Input mps from user
            Console.WriteLine("Enter MPS Value: ");
            mps = Convert.ToInt32(Console.ReadLine());

            WriteToFile.Add ( "MPS value: " + mps.ToString());

            // If mps is 0, simply return the input fcfs order
            if (mps == 0)
            {
                for (int x = 0; x < nofAirCrafts; x++)
                {
                    iEver[x] = inputArray[x];
                    return;
                }
            }

            // Instantiating so that iWeight contains weight class and iRoute
            // contains approach path information
            for (int i = 0; i < nofAirCrafts + 1; i++)
            {
                iWeight[i] = IACTP[i];
                iRoute[i] = IFFX[i];
            }
            init = 0;

            // Calculating initial cost
            for (int i = 1; i < nofAirCrafts; i++)
            {
                init += iCost[iWeight[i], iWeight[i + 1]];
            }
            
            // Call for finding optimum method
            while (iTime < 500)
            {
                for (int i = 1; i <= nofAirCrafts; i++)
                {
                    iCoord[i] = i;
                    iBestOrd[i] = i;
                }
                iTime += 1;

                if (iTime == 1)
                {
                    findLocalOptimum();
                }
                else
                {
                    randomGen(iTime);
                    findLocalOptimum();
                }
                int flag1 = 0;
                if (imark != 0)
                {

                    /*
                     * Here we check if current schedule has been already evaluated
                     * in earlier run; If yes, we do not waste time on further
                     * evaluation
                     */
                    ifc = 0;
                    iBestOrd[1] = 1;
                    for (int k = 1; k < nofAirCrafts; k++)
                    {
                        ifc = ifc + iCost[iWeight[iBestOrd[k]], iWeight[iBestOrd[k + 1]]];
                    }
                    flag1 = 0;
                    if (localOptimumList.Contains(init - ifc))
                    {
                        flag1 = 1;
                    }

                    /*
                     * If a move was made during the last pass then we will go
                     * through the schedule again, from the beginning
                     */
                    findLocalOptimum();
                }
                if (flag1 == 0)
                {
                    // Here we compute the final cost for this solution
                    ifc = 0;
                    iBestOrd[1] = 1;
                    for (int k = 1; k < nofAirCrafts; k++)
                    {
                        ifc = ifc + iCost[iWeight[iBestOrd[k]], iWeight[iBestOrd[k + 1]]];
                    }
                    Console.WriteLine("ITIME: " + iTime + " IFC: " + ifc);
                    WriteToFile.Add("ITIME: " + iTime + " IFC: " + ifc);

                    localOptimumList.Add(init - ifc);

                    // If this is the best solution to date we store it
                    if ((ifc < iBever) || (iTime == 1))
                    {
                        iBever = ifc;
                        iFOIS = iTime;
                        Console.WriteLine("iFOIS: " + iFOIS);
                        WriteToFile.Add("iFOIS: " + iFOIS);
                        zHap = 0;
                        for (int k = 1; k <= nofAirCrafts; k++)
                        {
                            iEver[k] = iBestOrd[k];
                        }
                    }
                }

                // Keep track of how many times this solution has occurred
                if (ifc == iBever)
                {
                    zHap = zHap + 1;
                }

                // Print out the results
                if (iTime == 1)
                {
                    Console.WriteLine("One Run");
                    WriteToFile.Add("One Run");
                    Console.WriteLine("Initial Cost " + init);
                    WriteToFile.Add("Initial Cost " + init);
                    Console.WriteLine("Final Cost: " + iBever);
                    WriteToFile.Add("Final Cost: " + iBever);
                    Console.WriteLine("% Savings: " + 100 * ((double)(init - iBever) / iBever));
                    WriteToFile.Add("% Savings: " + 100 * ((double)(init - iBever) / iBever));
                }
                //
            }
            Console.WriteLine("Best Order: ");
            WriteToFile.Add("Best Order: ");
          
            iEver[1] = 1;
            for (int k = 1; k <= nofAirCrafts; k++)
            {
                Console.WriteLine(iEver[k] + " ");
                WriteToFile.Add(iEver[k] + " ");
            }
            Console.WriteLine("Weights: ");
            WriteToFile.Add("Weights: ");

            for (int k = 1; k <= nofAirCrafts; k++)
            {
                Console.WriteLine(iWeight[iEver[k]] + " ");
                WriteToFile.Add( iWeight[iEver[k]] + " ");
            }

            Console.WriteLine("Best of 500 runs");
            WriteToFile.Add("Best of 500 runs");
            Console.WriteLine("Initial Cost " + init);
            WriteToFile.Add("Initial Cost " + init);
            Console.WriteLine("Final Cost " + iBever);
            WriteToFile.Add("Final Cost " + iBever);

            /**
             * The formula below is for 'savings' is for airport throughput savings
             */
            Console.WriteLine("% Savings: " + 100 * ((double)(init - iBever) / iBever));
            WriteToFile.Add("% Savings: " + 100 * ((double)(init - iBever) / iBever));
            Console.WriteLine("It took: " + iFOIS);
            WriteToFile.Add("It took: " + iFOIS);
            Console.WriteLine("% of Time it occured: " + zHap / 5);
            Console.WriteLine("");
            WriteToFile.Add("% of Time it occured: " + zHap / 5);

            System.IO.File.WriteAllLines(@"TPSOutput.txt", WriteToFile.ToArray());
            Console.ReadLine();
        }

        public static void randomGen(int iTime)
        {

            int temp;
            for (int j = 1; j <= mps; j++)
            {
                for (int l = 1; l < nofAirCrafts; l++)
                {
                    rnd[l] = randomG.NextDouble();
                }
                int k = 3;
                if ((iTime + j) % 2 == 0)
                    k = 2;
                for (int i = k; i < nofAirCrafts - 1; i++)
                {
                    if (iRoute[iCoord[i]] != iRoute[iCoord[i + 1]])
                    {
                        if (iWeight[iCoord[i]] != iWeight[iCoord[i + 1]])
                        {
                            if (rnd[i] < 0.5)
                            {
                                temp = iCoord[i];
                                iCoord[i] = iCoord[i + 1];
                                iBestOrd[i] = iCoord[i + 1];
                                iCoord[i + 1] = temp;
                                iBestOrd[i + 1] = temp;
                            }
                        }
                    }
                }
            }
        }

        private static void findLocalOptimum()
        {
            int ib = 2, iBack, iStart, iStep, iStore, iCST, legal;
            imark = 0;
            /*
             * System.out.println("Initially icord is "); for (int k = 0; k <
             * iCoord.length; k++) { System.out.print(iCoord[k]); }
             */

            for (int i = ib; i < (nofAirCrafts - 1); i++)
            {
                iBack = iCoord[i] - i;

                // Finding the initial cost of the block
                iStart = 0;
                for (int k = (i - 1 - mps + iBack); k <= (i + 1 + mps + iBack); k++)
                {
                    if (k > 0 && k < nofAirCrafts)
                    {
                        iStart = iStart + iCost[iWeight[iCoord[k]], iWeight[iCoord[k + 1]]];
                    }
                }

                // iStep will cover the range of possible locations for the
                // current(ith) aircraft
                for (iStep = (iBack - mps); iStep <= (iBack + mps); iStep++)
                {
                    if ((i + iStep) > nofAirCrafts || (i + iStep) < 2)
                    {
                        continue;
                    }

                    // Forward moves
                    if (iStep > 0)
                    {
                        iStore = iCoord[i];
                        for (int k = i; k <= (i + iStep - 1); k++)
                        {
                            iCoord[k] = iCoord[k + 1];
                        }
                        iCoord[i + iStep] = iStore;
                    }

                    // Backward moves
                    if (iStep < 0)
                    {
                        iStore = iCoord[i];
                        for (int k = i; k >= (i + iStep + 1); k--)
                        {
                            iCoord[k] = iCoord[k - 1];
                        }
                        iCoord[i + iStep] = iStore;
                    }

                    // Evaluate this arrangement
                    iCST = 0;
                    for (int k = (i - 1 - mps + iBack); k <= (i + 1 + mps + iBack); k++)
                    {
                        if (k > 0 && k < nofAirCrafts)
                        {
                            iCST = iCST + iCost[iWeight[iCoord[k]], iWeight[iCoord[k + 1]]];
                        }
                    }
                    iDiff = iCST - iStart;
                    if (iDiff > 0)
                    {
                        undoMove();
                        continue;
                    }

                    // check the legality of the arrangement - MPS Constraint
                    legal = 1;
                    for (int k = (i - 1 - mps + iBack); k <= (i + mps + 1 + iBack); k++)
                    {
                        if (k > 0 && k <= nofAirCrafts)
                        {
                            if (Math.Abs(k - iCoord[k]) > mps)
                            {
                                legal = 0;
                                undoMove();
                                break;
                            }
                        }
                    }
                    if (legal == 0)
                        continue;

                    // check the legality of the arrangement - Runway Constraint and
                    // Fairness to aircraft of same type
                    for (int j = (i - 1 - mps + iBack); j <= (i + mps + iBack); j++)
                    {
                        for (int k = j + 1; k <= (j + mps + 2); k++)
                        {
                            if (k < iCoord.Length)
                            {
                                if (j > 0 && k > 0)
                                {
                                    if (iRoute[iCoord[j]] == iRoute[iCoord[k]])
                                    {
                                        if (iCoord[j] > iCoord[k])
                                        {
                                            legal = 0;
                                            undoMove();
                                            break;
                                        }
                                    }
                                    if (iWeight[iCoord[j]] == iWeight[iCoord[k]])
                                    {
                                        if (iCoord[j] > iCoord[k])
                                        {
                                            legal = 0;
                                            undoMove();
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if (legal == 0)
                            break;
                    }
                    if (legal == 0)
                        continue;

                    // If the move improves the schedule and is legal -- do it
                    if ((iDiff < 0) && (legal == 1))
                    {
                        // initCost = 0;
                        if (iStep > 0)
                        {
                            iStore = iBestOrd[i];
                            for (int k = i; k <= (i + iStep - 1); k++)
                            {
                                iBestOrd[k] = iBestOrd[k + 1];
                            }
                            iBestOrd[i + iStep] = iStore;
                        }
                        if (iStep < 0)
                        {
                            iStore = iBestOrd[i];
                            for (int k = i; k >= (i + iStep + 1); k--)
                            {
                                iBestOrd[k] = iBestOrd[k - 1];
                            }
                            iBestOrd[i + iStep] = iStore;
                        }

                        // Store the current best order
                        iBestOrd[1] = 1;
                        for (int k = 0; k < iBestOrd.Length; k++)
                        {
                            if (k > 0)
                            {
                                iCoord[k] = iBestOrd[k];
                            }
                        }
                        for (int k = 1; k < iBestOrd.Length - 1; k++)
                        {
                            initCost += iCost[iWeight[iBestOrd[k]] , iWeight[iBestOrd[k + 1]]];
                        }
                        imark = 1;
                        // i--;
                        ib = i;
                        break;
                    }
                    undoMove();
                }
            }

        }

        /*
         * This method is called when a move was evaluated and decided as not needed
         * and hence we need to undo it
         */
        private static void undoMove()
        {
            for (int k = 0; k < iBestOrd.Length; k++)
            {
                if (k > 0)
                {
                    iCoord[k] = iBestOrd[k];
                }
            }
        }
    }

}