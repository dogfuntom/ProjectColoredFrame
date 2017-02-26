namespace ProjectColoredFrame.VisualTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static System.Math;
    using System.Collections.Immutable;
    using System.Windows.Media;

    class Program
    {
        private static readonly IReadOnlyList<ConsoleColor> PredefinedPalette = ImmutableArray.Create(
            ConsoleColor.Red,
            ConsoleColor.Green,
            ConsoleColor.Yellow,
            ConsoleColor.White, // instead of cobalt
            ConsoleColor.Magenta,
            ConsoleColor.Cyan
        );

        private static readonly Random Random = new Random();

        static void Main(string[] args)
        {
            var priorColor = Console.ForegroundColor;

            DoRandomnessTest();

            Console.ForegroundColor = priorColor;

            DoStabilityTest();

            Console.ForegroundColor = priorColor;
            Console.ReadKey();
        }

        private static void DoStabilityTest()
        {
            Console.WriteLine("Stability test\n");

            var names = GenerateMiniNames().ToList();
            MapAndPrint(names);

            for (int i = 0; i < 16; i++)
            {
                var mutateType = Random.Next(4);
                switch (mutateType)
                {
                    case 0:
                        // Add.
                        names.Add(GenerateMiniName());
                        break;
                    case 1:
                        // Remove.
                        if (names.Count > 1)
                            names.RemoveAt(Random.Next(names.Count));
                        break;
                    case 2:
                        // Rename.
                        names[Random.Next(names.Count)] = GenerateMiniName();
                        break;
                    case 3:
                        // Move.
                        var fromIndex = Random.Next(names.Count);
                        var toIndex = Random.Next(names.Count - 1);
                        names.Insert(toIndex, names[fromIndex]);
                        break;
                    default:
                        break;
                }

                MapAndPrint(names);
            }
            Console.WriteLine();
        }

        private static void DoRandomnessTest()
        {
            Console.WriteLine("Randomness and inter-stability test\n");
            for (int i = 0; i < 16; i++)
            {
                var names = GenerateMiniNames();
                MapAndPrint(names);
            }
            Console.WriteLine();
        }

        private static void MapAndPrint(ICollection<string> names)
        {
            var mapping = PaletteMapping.Map(names.ToImmutableArray(), PredefinedPalette.Count);

            foreach (var name in names)
            {
                var color = PredefinedPalette[mapping[name.GetHashCode()]];
                Console.ForegroundColor = color;
                Console.Write(name);
            }

            Console.WriteLine();
        }

        private static string[] GenerateMiniNames()
        {
            var count = Random.Next(2, 8);
            var result = new string[count];

            for (int i = 0; i < count; i++)
            {
                result[i] = GenerateMiniName();
            }

            return result;
        }

        private static string GenerateMiniName()
        {
            return ((char)Random.Next('a', 'z')).ToString();
        }
    }
}
