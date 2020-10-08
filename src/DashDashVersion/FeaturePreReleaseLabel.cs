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
    ///  This class represents a pre-release label for feature branches, composed of two pre-release label particles.
    /// </summary>
    internal class FeaturePreReleaseLabel : PreReleaseLabel
    {
        internal const int NumberOfBitsAllocatedForFeatureRevision = 6;
        private const ushort FeatureRevisionMaxValue = 0b0000_0000_00_11_1110;

        internal FeaturePreReleaseLabel(PreReleaseLabelParticle branchLabel, PreReleaseLabelParticle featureBranchLabel) : base(branchLabel)
        {
            FeatureBranchLabel = featureBranchLabel;
        }

        public PreReleaseLabelParticle FeatureBranchLabel { get; }

        internal override ushort CalculatedRevision
        {
            get
            {
                var featureRevision = Math.Min(FeatureBranchLabel.Ordinal + 1, FeatureRevisionMaxValue);
                return (ushort)(base.CalculatedRevision + featureRevision);
            }
        }

        public override string ToString() => base.ToString() + $"{Constants.ParticleDelimiter}{FeatureBranchLabel}";
    }
}
