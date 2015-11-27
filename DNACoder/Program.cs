using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNACoder
{
    class Program
    {
        private static List<byte> Genome = new List<byte>(); 

        static void Main(string[] args)
        {
            Console.Write("Load: ");
            var input = Console.ReadLine();

            if (File.Exists(input))
            {
                var lines = File.ReadAllLines(input);
                Genome.AddRange(lines.ToBytes());
            }

            while (true) {
                Console.Clear();
                RenderGenome();
                input = Console.ReadLine();

                if (input == "done")
                    break;

                if (input == "del")
                {
                    Genome.RemoveAt(Genome.Count - 1);
                }
                else if (!IsHex(input))
                {
                    Console.WriteLine("Invalid Hex-value");
                    Console.ReadLine();
                }
                else
                {
                    var marker = Convert.ToByte(input, 16);
                    Genome.Add(marker);
                }
            }

            Console.Write("Filename: ");
            var filename = Console.ReadLine();

            File.WriteAllLines(filename, Genome.Select(b => b.ToString("X2")));
        }

        private static bool DataIsValid(string[] inputData, int argumentBytes)
        {
            if (inputData.Length != argumentBytes)
                return false;

            return inputData.All(IsHex);
        }

        private static bool IsHex(string input)
        {
            if (input.Length != 2)
                return false;

            try
            {
                Convert.ToByte(input, 16);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static void RenderGenome()
        {
            Console.WriteLine("Genome: ");
            Console.WriteLine(String.Join(" ", Genome.Select(b => b.ToString("X2"))));
            Console.WriteLine();
        }
    }
}
