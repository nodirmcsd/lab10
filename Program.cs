using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using MyMediaLite.Eval;
using MyMediaLite.IO;
using MyMediaLite.RatingPrediction;

namespace RecSys
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = new Stopwatch();

            //Console.WriteLine("Begin");
            //Utils.Split(@"D:\projects\lab10\conv_train.csv", 70, 20);
            //return;
            
            const string trainDataFileName = @"D:\projects\lab10\part_0_conv_train.csv";
            const string evalDataFileName = @"D:\projects\lab10\part_1_conv_train.csv";
            const string testInFileName = @"D:\projects\lab10\test.csv";
            const string resultOutFileName = @"D:\projects\lab10\result_bmf.csv";

            /*
            const string trainDataFileName = @"c:\Users\nand2tetris\lab10\o_train_0.csv";
            const string evalDataFileName = @"c:\Users\nand2tetris\lab10\o_train_1.csv";
            const string testInFileName = @"c:\Users\nand2tetris\lab10\o_test.csv";
            const string resultOutFileName = @"c:\Users\nand2tetris\lab10\o_result_bmf.csv";
            */

            t.Restart();
            Console.WriteLine("RatingData.Read Begin");
            var trainingData = RatingData.Read(trainDataFileName);
            Console.WriteLine("RatingData.Read End {0}", t.Elapsed.TotalSeconds);

            t.Restart();
            Console.WriteLine("RatingData.Read Begin");
            var evalData = RatingData.Read(evalDataFileName);
            Console.WriteLine("RatingData.Read End {0}", t.Elapsed.TotalSeconds);


            var recommender = new BiasedMatrixFactorization
            {
                Ratings = trainingData,
                NumIter = 10,
                MaxRating = 5,
                MinRating = 1,
            };
            
            

            t.Restart();
            Console.WriteLine("Training begin");
            recommender.Train();
            Console.WriteLine("Training end {0}", t.Elapsed.TotalSeconds);

            t.Restart();
            Console.WriteLine("Evaluate begin");
            var resultsb = recommender.Evaluate(evalData);
            Console.WriteLine("Evaluate end {0}", t.Elapsed.TotalSeconds);

            //Console.WriteLine(recommender.DoCrossValidation());

            Console.WriteLine("RMSE={0} MAE={1}", resultsb["RMSE"], resultsb["MAE"]);


            t.Restart();
            Console.WriteLine("Result Begin");

            using(var testIn = new StreamReader(testInFileName))
            using (var resultOut = new StreamWriter(resultOutFileName))
            {

                var count = 0;
                string line;

                while ((line = testIn.ReadLine()) != null)
                {
                    if (count > 0)
                    {
                        var parts = line.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                        var userId = int.Parse(parts[0]);
                        var itemId = int.Parse(parts[1]);
                        var rating = recommender.Predict(userId, itemId);
                        resultOut.WriteLine("{0}, {1}, {2}", userId, itemId,
                            Math.Round(rating, 1).ToString(CultureInfo.InvariantCulture));
                    }

                    if (count%10000 == 0) Console.Write(".");
                    count++;
                }

                resultOut.Close();
                testIn.Close();
            }


            Console.WriteLine("Result End {0}", t.Elapsed.TotalSeconds);

            t.Stop();

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
