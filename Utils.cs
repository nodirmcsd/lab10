using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ConsoleApplication
{
    public class Utils
    {
        public static void Converter(string srcPath)
        {
            var path = Path.GetDirectoryName(srcPath);
            var fileName = Path.GetFileName(srcPath);
            var destPath = Path.Combine(path, "conv_" + fileName);

            File.Delete(destPath);

            Console.WriteLine("Start {0}", destPath);
            
            using (var sr = new StreamReader(srcPath))
            using (var sw = new StreamWriter(destPath))
            {
                var i = 0;
                while (true)
                {
                    i++;
                    if (i%5 == 0)
                    {
                        sw.Flush();
                        Console.Write("*");
                    }

                    var line = sr.ReadLine();

                    if (string.IsNullOrEmpty(line)) break;
                    var part = line.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                    if (part.Length > 2)
                    {
                        try
                        {
                            sw.WriteLine("{0}\t{1}\t{2}", int.Parse(part[0]), int.Parse(part[1]), double.Parse(part[2], CultureInfo.InvariantCulture));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }

            Console.WriteLine("End {0}", srcPath);
        }

        public static void Split(string src, int part1, int part2)
        {
            var lines = File.ReadAllLines(src);

            var part1Count = Convert.ToInt32(Math.Floor(lines.Length*part1/100.0D));
            var part2Count = Convert.ToInt32(Math.Floor(lines.Length*part2/100.0D));

            var part1Lines = lines.Skip(0).Take(part1Count);
            var part2Lines = lines.Skip(part1Count).Take(part2Count);

            var path = Path.GetDirectoryName(src);
            var fileName = Path.GetFileName(src);

            File.WriteAllLines(Path.Combine(path, "part_0_" + fileName), part1Lines);
            File.WriteAllLines(Path.Combine(path, "part_1_" + fileName), part2Lines);
        }
    }
}