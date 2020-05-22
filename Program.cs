using System;
using System.IO;
using System.Linq;
using System.Text;
using static System.Console;
using static System.Convert;
using static System.Environment;

namespace HadesStats
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder builder = new StringBuilder();
            var appDataPath = GetFolderPath(SpecialFolder.ApplicationData);
            var statsFolder = Path.Combine(appDataPath, "HadesStats");
            var dataFile = Path.Combine(statsFolder, "data");
            byte[] data;
            if (!Directory.Exists(statsFolder))
            {
                Directory.CreateDirectory(statsFolder);
            }

            if (!File.Exists(dataFile))
            {
                using var stream = new StreamWriter(dataFile);
                stream.WriteLine(true);
                stream.WriteLine("0,0,0,0,0,0,0");
            }

            StreamReader reader = new StreamReader(dataFile);
            bool printWelcome = ToBoolean(reader.ReadLine());
            var stats = reader.ReadLine().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => ToInt32(x)).ToArray();
            reader.Dispose();
            using var writer = File.Open(dataFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            if (printWelcome)
            {
                WriteLine($"Welcome to Hades Stats! A program to help you track your personal stats to the game \"Hades\", made by Supergiant Games.{NewLine}To begin writing your stats, use the following input commands:{NewLine}F or 1 - You did not finish Tartarus.{NewLine}T or 2 - You finished Tartarus.{NewLine}A or 3 - You finished Asphodel.{NewLine}E or 4 - You finished Elysium.{NewLine}S or 5 - You finished the Temple of Styx.{NewLine}GG or 6 - You won the run.{NewLine}{NewLine}{NewLine}Write exactly \"OK\" to hide this forever and get to recording your special stats!{NewLine}");
                while (ReadLine() != "OK") ;
                printWelcome = false;
                WriteCurrentDataToFile();
                Clear();
            }

        printMe:

            WriteLine($"Did not finish Tartarus: {Calculate(0):0.##}% of runs.{NewLine}Finished Tartarus: {Calculate(1):0.##}% of runs.{NewLine}Finished Asphodel: {Calculate(2):0.##}% of runs.{NewLine}Finished Elysium: {Calculate(3):0.##}% of runs.{NewLine}Finished Temple of Styx: {Calculate(4):0.##}% of runs.{NewLine}Won the game: {Calculate(5):0.##}% of runs.{NewLine}{NewLine}Total recorded runs: {stats[^1]}.");
            while (ReadLine() switch
            {
                string a when a is "1" || a is "F" => SaveStats(0),
                string b when b is "2" || b is "T" => SaveStats(1),
                string c when c is "3" || c is "A" => SaveStats(2),
                string d when d is "4" || d is "E" => SaveStats(3),
                string e when e is "5" || e is "S" => SaveStats(4),
                string f when f is "6" || f is "GG" => SaveStats(5),
                _ => true
            }) ;

            goto printMe;

 // -~-~-~- ======================   Line of the Dead ====================== 

            decimal Calculate(int index) => stats[^1] == 0 ? stats[^1] : (decimal)stats[index] / stats[^1] * 100;

            bool SaveStats(int statIndex)
            {
                if (statIndex == 0)
                    stats[0]++;
                else for (int i = 1; i <= statIndex; i++)
                    stats[i]++;
                stats[^1]++;
                WriteCurrentDataToFile();
                return false;
            }

            void WriteCurrentDataToFile()
            {
                data = Encoding.ASCII.GetBytes(printWelcome + NewLine);
                writer.Write(data, 0, data.Length);
                foreach (var item in stats)
                {
                    builder.Append(item).Append(',');
                }

                builder.AppendLine();
                writer.Write(Encoding.ASCII.GetBytes(builder.ToString()));
                builder.Clear();
                writer.Flush();
                writer.Position = 0;
            }
        }
    }
}
