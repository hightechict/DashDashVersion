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
using DashDashVersion;
using FluentAssertions;
using Xunit;

namespace DashDashVersionTests
{
    public class PreReleaseLabelTests
    {
        [Theory]
        [InlineData("dev", 2)]
        [InlineData("rec", 3)]
        [InlineData("dfdf2dfsdfSDF233-DDD-FFF", 0)]
        public void PreReleaseParticleCreation(string preReleaseLabel, uint preReleaseOrdinal)
        {
            var particle = new PreReleaseLabelParticle(preReleaseLabel, preReleaseOrdinal);
            particle.Label.Should().Be(preReleaseLabel);
            particle.Ordinal.Should().Be(preReleaseOrdinal);
        }

        [Theory]
        [InlineData("DdD3-df.sddf2.2323", 0)]
        [InlineData("dev.1", 2)]
        [InlineData("#435@@#5325", 2)]
        public void PreReleaseParticleInvalidCreation(string preReleaseLabel, uint preReleaseOrdinal)
        {
            Action action = () =>
            {
                // ReSharper disable once AssignmentIsFullyDiscarded
                _ = new PreReleaseLabelParticle(preReleaseLabel, preReleaseOrdinal);
            };
            action.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(0u, 64u)]
        [InlineData(1u, 128u)]
        [InlineData(uint.MaxValue - 1, ushort.MaxValue - 63)]

        public void PreReleaseLabelRevision(uint preReleaseOrdinal, ushort expectedOutcome)
        {
            var label = new PreReleaseLabel(new PreReleaseLabelParticle("dev", preReleaseOrdinal));
            label.CalculatedRevision.Should().Be(expectedOutcome);
        }
    }
}
