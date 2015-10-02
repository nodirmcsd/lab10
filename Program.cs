using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MyMediaLite.Eval;
using MyMediaLite.IO;
using MyMediaLite.RatingPrediction;

namespace RecSys
{
    class Program
    {
        const string TRAIN_DATA_FILE_NAME = @"D:\__testdata\lab10\conv_train.csv";
        //const string EVAL_DATA_FILE_NAME = @"D:\projects\lab10\part_1_conv_train.csv";
        const string TEST_FILE_NAME = @"D:\__testdata\lab10\test.csv";
        const string RESULTS_FILE_NAME = @"D:\__testdata\lab10\result_bmf.csv";
        
        static void Main(string[] args)
        {
            var t = new Stopwatch();
            
            //Utils.Converter(@"D:\__testdata\lab10\train.csv");
            //return;

            t.Restart();
            Console.WriteLine("RatingData.Read Begin");
            var trainingData = RatingData.Read(TRAIN_DATA_FILE_NAME);
            Console.WriteLine("RatingData.Read End {0}", t.Elapsed.TotalSeconds);

            //t.Restart();
            //Console.WriteLine("RatingData.Read Begin");
            //var evalData = RatingData.Read(EVAL_DATA_FILE_NAME);
            //Console.WriteLine("RatingData.Read End {0}", t.Elapsed.TotalSeconds);
            
            //http://www.mymedialite.net/examples/datasets.html
            // num_factors=120 
            // bias_reg=0.001 
            // regularization=0.055 
            // learn_rate=0.07 
            // num_iter=100 
            // bold_driver=true
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

            //t.Restart();
            //Console.WriteLine("Evaluate begin");
            //var resultsb = recommender.Evaluate(evalData);
            //Console.WriteLine("Evaluate end {0}", t.Elapsed.TotalSeconds);

            Console.WriteLine(recommender.DoCrossValidation());

            //Console.WriteLine("RMSE={0} MAE={1}", resultsb["RMSE"], resultsb["MAE"]);


            t.Restart();
            Console.WriteLine("Result Begin");
            
            var users = recommender.Ratings.AllUsers.ToArray();
            var items = recommender.Ratings.AllItems.ToArray();
            var ratingAvg = recommender.Ratings.Average;

            using(var testIn = new StreamReader(TEST_FILE_NAME))
            using (var resultOut = new StreamWriter(RESULTS_FILE_NAME))
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

                        var rating = !users.Contains(userId) || !items.Contains(itemId)
                            ? ratingAvg
                            : recommender.Predict(userId, itemId);
                        
                        resultOut.WriteLine("{0}, {1}, {2}", userId, itemId, rating);
                    }

                    if (count%10000 == 0)
                    {
                        resultOut.Flush();
                        Console.Write(".");
                    }
                    count++;
                }

                resultOut.Close();
                testIn.Close();
            }

            Console.WriteLine();
            Console.WriteLine("Result End {0}", t.Elapsed.TotalSeconds);

            t.Stop();

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
