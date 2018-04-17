// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace ProjectColoredFrame.VisualTest
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    /// <summary>
    /// This utility allows to check visually if palette mapping works as intented.
    /// </summary>
    internal static class Program
    {
        private static readonly IReadOnlyList<ConsoleColor> s_predefinedPalette = ImmutableArray.Create(
            ConsoleColor.Red,
            ConsoleColor.Green,
            ConsoleColor.Yellow,
            ConsoleColor.White, // instead of cobalt
            ConsoleColor.Magenta,
            ConsoleColor.Cyan
        );

        private static readonly Random s_random = new Random();

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

        private static void DoStabilityTest()
        {
            Console.WriteLine("Stability test\n");

            var names = GenerateMiniNames().ToList();
            MapAndPrint(names);

            for (int i = 0; i < 16; i++)
            {
                var mutateType = s_random.Next(4);
                switch (mutateType)
                {
                    case 0:
                        // Add.
                        names.Add(GenerateMiniName());
                        break;
                    case 1:
                        // Remove.
                        if (names.Count > 1)
                            names.RemoveAt(s_random.Next(names.Count));
                        break;
                    case 2:
                        // Rename.
                        names[s_random.Next(names.Count)] = GenerateMiniName();
                        break;
                    case 3:
                        // Move.
                        var fromIndex = s_random.Next(names.Count);
                        var toIndex = s_random.Next(names.Count - 1);
                        names.Insert(toIndex, names[fromIndex]);
                        break;
                    default:
                        break;
                }

                MapAndPrint(names);
            }
            Console.WriteLine();
        }

        private static string GenerateMiniName()
        {
            return ((char)s_random.Next('a', 'z')).ToString();
        }

        private static string[] GenerateMiniNames()
        {
            var count = s_random.Next(2, 8);
            var result = new string[count];

            for (int i = 0; i < count; i++)
            {
                result[i] = GenerateMiniName();
            }

            return result;
        }

        static void Main(string[] args)
        {
            var priorColor = Console.ForegroundColor;

            DoRandomnessTest();

            Console.ForegroundColor = priorColor;

            DoStabilityTest();

            Console.ForegroundColor = priorColor;
            Console.ReadKey();
        }

        private static void MapAndPrint(ICollection<string> names)
        {
            var mapping = PaletteDistribution.Map(names.Select(name => name.GetHashCode()).ToImmutableArray(), s_predefinedPalette.Count);

            foreach (var name in names)
            {
                var color = s_predefinedPalette[mapping[name.GetHashCode()]];
                Console.ForegroundColor = color;
                Console.Write(name);
            }

            Console.WriteLine();
        }
    }
}
