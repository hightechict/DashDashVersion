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

using DashDashVersion;
using FluentAssertions;
using Xunit;

namespace DashDashVersionTests
{
    public class FeaturePreReleaseLabelTest
    {
        [Theory]
        [InlineData(0u, 0u, 65u)]
        [InlineData(1u, 1u, 130u)]
        [InlineData(0u, 500u, 126u)]
        [InlineData(uint.MaxValue - 1, uint.MaxValue - 1, ushort.MaxValue - 1)]
        public void FeaturePreReleaseLabelRevision(
            uint developOrdinal,
            uint featureOrdinal,
            ushort expectedOutcome)
        {
            var developLabel = new PreReleaseLabelParticle("dev", developOrdinal);
            var featureLabel = new PreReleaseLabelParticle("featureA", featureOrdinal);
            var label = new FeaturePreReleaseLabel(developLabel, featureLabel);
            label.CalculatedRevision.Should().Be(expectedOutcome);
        }
    }
}
