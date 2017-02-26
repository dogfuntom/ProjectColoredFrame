namespace ProjectColoredFrame
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Media;

    public static class PaletteMapping
    {
        public static IReadOnlyDictionary<int, int> Map(IReadOnlyList<string> uniqueNames, int paletteSize)
        {
            var postponed = new List<Tuple<string, Random>>(uniqueNames.Count);
            var mapping = new Dictionary<int, int>();

            /* Randomly get color using project name as seed, for consistency.
            This approach is adopted in hope of more stable coloring on solution contents changes.
            Avoid conflicts while possible by posponing them.*/

            foreach (var name in uniqueNames)
            {
                var nameHash = name.GetHashCode();

                var random = new Random(nameHash);
                var index = random.Next(paletteSize);

                if (mapping.ContainsValue(index))
                    postponed.Add(new Tuple<string, Random>(name, random));
                else
                    mapping.Add(nameHash, index);
            }

            // Resolve postponed conflicting mappings.
            foreach (var postponedItem in postponed)
            {
                var nameHash = postponedItem.Item1.GetHashCode();
                var random = postponedItem.Item2;

                // Check for hash conflict (unlikely, but far from impossible).
                if (mapping.ContainsKey(nameHash))
                    continue;

                // Try a few times to find unused color, otherwise give up and use any.
                var index = -1;
                for (int i = 0; i < 64; i++)
                {
                    index = random.Next(paletteSize);
                    if (!mapping.ContainsValue(index))
                        break;
                }

                // Use index we have settled on.
                mapping.Add(nameHash, index);
            }

            return new ReadOnlyDictionary<int, int>(mapping);
        }
    }
}
