using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PosponedMapping = System.Tuple<int, System.Random>;

namespace ProjectColoredFrame
{
	public static class PaletteDistribution
	{
		public static IReadOnlyDictionary<int, int> Map(IReadOnlyList<int> signatures, int paletteSize)
		{
			var postponed = new List<PosponedMapping>(signatures.Count);
			var mapping = new Dictionary<int, int>();

			/* Randomly get color using project name as seed, for consistency.
            This approach is adopted in hope of more stable coloring on solution contents changes.
            Avoid conflicts while possible by posponing them.*/

			foreach (var signature in signatures)
			{
				var random = new Random(signature);
				var index = random.Next(paletteSize);

				if (mapping.ContainsValue(index))
					postponed.Add(new PosponedMapping(signature, random));
				else
					mapping.Add(signature, index);
			}

			// Resolve postponed conflicting mappings.
			foreach (var item in postponed)
			{
				var signature = item.Item1;
				Random seeded = item.Item2;

				// Check for hash conflict (unlikely, but not impossible).
				if (mapping.ContainsKey(signature))
					continue;

				// Try a few times to find unused color, otherwise give up and use any.
				var index = -1;
				for (var i = 0; i < 64; i++)
				{
					index = seeded.Next(paletteSize);
					if (!mapping.ContainsValue(index))
						break;
				}

				// Use index we have settled on.
				mapping.Add(signature, index);
			}

			return new ReadOnlyDictionary<int, int>(mapping);
		}
	}
}
