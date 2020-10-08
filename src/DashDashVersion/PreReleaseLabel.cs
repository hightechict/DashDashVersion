// Copyright 2019 Hightech ICT and authors

// This file is part of DashDashVersion.

// DashDashVersion is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// DashDashVersion is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with DashDashVersion. If not, see<https://www.gnu.org/licenses/>.

using System;

namespace DashDashVersion
{
    /// <summary>
    ///  This class contains the pre-release label. 
    ///  It can also calculate a revision number for the assembly version.
    /// </summary>
    public class PreReleaseLabel : IComparable<PreReleaseLabel>
    {
        private const ushort DevelopRevisionMaxValue = 0b1111_1111_1100_0000;

        internal PreReleaseLabel(PreReleaseLabelParticle branchLabel)
        {
            BranchLabel = branchLabel;
        }

        public int CompareTo(PreReleaseLabel other)
        {
            var compareTo = BranchLabel.Ordinal.CompareTo(other.BranchLabel.Ordinal);
            if (compareTo != 0) return compareTo;
            if (this is FeaturePreReleaseLabel thisFeature)
            {
                if (other is FeaturePreReleaseLabel feature)
                {
                    return thisFeature.FeatureBranchLabel.Ordinal.CompareTo(
                        feature.FeatureBranchLabel.Ordinal);
                }
                return 1;
            }
            if (other is FeaturePreReleaseLabel)
            {
                return -1;
            }
            return 0;
        }

        public override string ToString() => BranchLabel.ToString();

        internal PreReleaseLabelParticle BranchLabel { get; }

        internal virtual ushort CalculatedRevision
        {
            get
            {
                var devRevision = (BranchLabel.Ordinal + 1) <<
                                  FeaturePreReleaseLabel.NumberOfBitsAllocatedForFeatureRevision;
                devRevision = Math.Min(DevelopRevisionMaxValue, devRevision);
                return (ushort)devRevision;
            }
        }
    }
}
