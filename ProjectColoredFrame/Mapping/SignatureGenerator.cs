using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectColoredFrame.Mapping
{
    /// <summary>
    /// Generates hash for given file based on its unique name
    /// and, if configured so, other properties.
    /// </summary>
    internal sealed class SignatureGenerator
    {
        public SignatureGenerator(bool isSolutionPathFactoredIn) => IsSolutionPathFactoredIn = isSolutionPathFactoredIn;

        public bool IsSolutionPathFactoredIn { get; }

        public int GetSignature(string uniqueName, string solutionPath)
        {
            unchecked
            {
                var hash = 469_612_973;
				hash = hash * 4_830_799 + uniqueName.GetHashCode();

                if (IsSolutionPathFactoredIn)
                    hash = hash * 4_830_799 + solutionPath.GetHashCode();

                return hash;
            }
        }
    }
}
