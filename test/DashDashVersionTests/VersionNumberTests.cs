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

// See issue https://github.com/dotnet/roslyn/issues/33199
#nullable disable 
using FluentAssertions;
using System;
using System.Collections.Generic;
using DashDashVersion;
using Xunit;

namespace DashDashVersionTests
{
    public class VersionNumberTests
    {
        [Theory]
        [InlineData("0.1.0", 0, 1, 0)]
        [InlineData("1.1.0", 1, 1, 0)]
        public void VersionNumberBase(
            string versionTag,
            uint expectedMajor,
            uint expectedMinor,
            uint expectedPatch)
        {
            var versionNumber = VersionNumber.Parse(versionTag);
            versionNumber.Major.Should().Be(expectedMajor);
            versionNumber.Minor.Should().Be(expectedMinor);
            versionNumber.Patch.Should().Be(expectedPatch);
            versionNumber.PreReleaseLabel.Should().BeNull();
            versionNumber.Metadata.Should().BeEmpty();

        }

        [Theory]
        [InlineData("0.1.0-dev.1", 0, 1, 0, "dev", 1)]
        [InlineData("0.1.0-rc.1", 0, 1, 0, "rc", 1)]
        [InlineData("0.1.0-a.1", 0, 1, 0, "a", 1)]
        [InlineData("0.1.0-23.1", 0, 1, 0, "23", 1)]
        public void VersionNumberPreReleaseTag(
            string versionTag,
            uint expectedMajor,
            uint expectedMinor,
            uint expectedPatch,
            string expectedBranchLabel,
            uint expectedBranchLabelOrdinal)
        {
            var versionNumber = VersionNumber.Parse(versionTag);
            versionNumber.Major.Should().Be(expectedMajor);
            versionNumber.Minor.Should().Be(expectedMinor);
            versionNumber.Patch.Should().Be(expectedPatch);
            versionNumber.Metadata.Should().BeEmpty();
            versionNumber.PreReleaseLabel.Should().NotBeNull();
            versionNumber.PreReleaseLabel?.BranchLabel.Label.Should().Be(expectedBranchLabel);
            versionNumber.PreReleaseLabel?.BranchLabel.Ordinal.Should().Be(expectedBranchLabelOrdinal);
            versionNumber.PreReleaseLabel?.BranchLabel.Should().NotBeOfType(typeof(FeaturePreReleaseLabel));
        }

        [Theory]
        [InlineData("0.1.0-dev.2.a.2", 0, 1, 0, "dev", 2, "a", 2, "")]
        [InlineData("0.1.0-dev.2.a.2+sdfdsdff232", 0, 1, 0, "dev", 2, "a", 2, "sdfdsdff232")]
        public void VersionNumberFeaturePreRelease(
            string versionTag,
            uint expectedMajor,
            uint expectedMinor,
            uint expectedPatch,
            string expectedBranchLabel,
            uint expectedBranchLabelOrdinal,
            string expectedFeatureBranchLabel,
            uint expectedFeatureBranchLabelOrdinal,
            string expectedMetadata)
        {
            var versionNumber = VersionNumber.Parse(versionTag);
            versionNumber.Major.Should().Be(expectedMajor);
            versionNumber.Minor.Should().Be(expectedMinor);
            versionNumber.Patch.Should().Be(expectedPatch);
            versionNumber.Metadata.Should().Be(expectedMetadata);
            versionNumber.PreReleaseLabel.Should().NotBeNull();
            var actual = versionNumber.PreReleaseLabel.Should().BeAssignableTo<FeaturePreReleaseLabel>().Subject;
            actual.BranchLabel.Label.Should().Be(expectedBranchLabel);
            actual.BranchLabel.Ordinal.Should().Be(expectedBranchLabelOrdinal);
            actual.FeatureBranchLabel.Label.Should().Be(expectedFeatureBranchLabel);
            actual.FeatureBranchLabel.Ordinal.Should().Be(expectedFeatureBranchLabelOrdinal);
        }

        [Theory]
        [InlineData("0.1.0-dev.2.a.2.debug", 0, 1, 0, "dev", 2, "a", 2, "", true)]
        [InlineData("0.1.0-dev.2.a.2.debug+sdfdsdff232", 0, 1, 0, "dev", 2, "a", 2, "sdfdsdff232", true)]
        [InlineData("0.1.0-dev.2.a.2", 0, 1, 0, "dev", 2, "a", 2, "", false)]
        [InlineData("0.1.0-dev.2.a.2+sdfdsdff232", 0, 1, 0, "dev", 2, "a", 2, "sdfdsdff232", false)]
        public void DebugVersionNumberFeature(
            string versionTag,
            uint expectedMajor,
            uint expectedMinor,
            uint expectedPatch,
            string expectedBranchLabel,
            uint expectedBranchLabelOrdinal,
            string expectedFeatureBranchLabel,
            uint expectedFeatureBranchLabelOrdinal,
            string expectedMetadata,
            bool debug)
        {
            var versionNumber = VersionNumber.Parse(versionTag);
            versionNumber.Major.Should().Be(expectedMajor);
            versionNumber.Minor.Should().Be(expectedMinor);
            versionNumber.Patch.Should().Be(expectedPatch);
            versionNumber.Metadata.Should().Be(expectedMetadata);
            versionNumber.PreReleaseLabel.Should().NotBeNull();
            versionNumber.DebugVersion.Should().Be(debug);
            var actual = versionNumber.PreReleaseLabel.Should().BeAssignableTo<FeaturePreReleaseLabel>().Subject;
            actual.BranchLabel.Label.Should().Be(expectedBranchLabel);
            actual.BranchLabel.Ordinal.Should().Be(expectedBranchLabelOrdinal);
            actual.FeatureBranchLabel.Label.Should().Be(expectedFeatureBranchLabel);
            actual.FeatureBranchLabel.Ordinal.Should().Be(expectedFeatureBranchLabelOrdinal);
        }

        [Theory]
        [InlineData("0.1.0-dev.2", 0, 1, 0, "dev", 2, "", false)]
        [InlineData("0.1.0-dev.2+sdfdsdff232", 0, 1, 0, "dev", 2, "sdfdsdff232", false)]
        [InlineData("0.1.0-dev.2.debug", 0, 1, 0, "dev", 2, "", true)]
        [InlineData("0.1.0-dev.2.debug+sdfdsdff232", 0, 1, 0, "dev", 2, "sdfdsdff232", true)]
        public void DebugVersionNumberDevelop(
            string versionTag,
            uint expectedMajor,
            uint expectedMinor,
            uint expectedPatch,
            string expectedBranchLabel,
            uint expectedBranchLabelOrdinal,
            string expectedMetadata,
            bool debug)
        {
            var versionNumber = VersionNumber.Parse(versionTag);
            versionNumber.Major.Should().Be(expectedMajor);
            versionNumber.Minor.Should().Be(expectedMinor);
            versionNumber.Patch.Should().Be(expectedPatch);
            versionNumber.Metadata.Should().Be(expectedMetadata);
            versionNumber.PreReleaseLabel.Should().NotBeNull();
            versionNumber.DebugVersion.Should().Be(debug);
            var actual = versionNumber.PreReleaseLabel;
            actual.BranchLabel.Label.Should().Be(expectedBranchLabel);
            actual.BranchLabel.Ordinal.Should().Be(expectedBranchLabelOrdinal);
        }

        [Theory]
        [InlineData("0.1.0", 0, 1, 0, "", false)]
        [InlineData("0.1.0+sdfdsdff232", 0, 1, 0, "sdfdsdff232", false)]
        [InlineData("0.1.1-debug.0.1.0", 0, 1, 0, "", true)]
        [InlineData("0.1.1-debug.0.1.0+sdfdsdff232", 0, 1, 0, "sdfdsdff232", true)]
        public void CoreDebugVersionNumber(
            string versionTag,
            uint expectedMajor,
            uint expectedMinor,
            uint expectedPatch,
            string expectedMetadata,
            bool debug)
        {
            var versionNumber = VersionNumber.Parse(versionTag);
            versionNumber.Major.Should().Be(expectedMajor);
            versionNumber.Minor.Should().Be(expectedMinor);
            versionNumber.Patch.Should().Be(expectedPatch);
            versionNumber.Metadata.Should().Be(expectedMetadata);
            versionNumber.DebugVersion.Should().Be(debug);
        }

        [Theory]
        [InlineData("s.2.1")]
        [InlineData("1s.2.1")]
        [InlineData("1.2.1-dev.s")]
        [InlineData("1.2.1-dev.1.feat.2.f.2")]
        [InlineData("1.2.1-dev.1.feat.2+23434dsa23@##")]
        [InlineData("1.2.1-dev.1.feat.2+23434dsa2+sdfdsdf")]
        [InlineData("1.2.1-dev.1.feat.2%")]
        public void VersionNumberInvalidCreation(string versionLabel)
        {
            Action action = () =>
            {
                // ReSharper disable once AssignmentIsFullyDiscarded
                _ = VersionNumber.Parse(versionLabel);
            };
            action.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("0.1.0-dev.1")]
        [InlineData("0.1.0-rc.1")]
        [InlineData("0.1.0-a.1")]
        [InlineData("0.1.0-23.1")]
        [InlineData("0.1.0")]
        [InlineData("0.1.0-dev.2.a.2")]
        [InlineData("0.1.0-dev.2.a.2+sdfdsdf")]
        [InlineData("0.1.1-debug.0.1.0+sdfdsdf")]
        [InlineData("0.1.0-dev.1.debug+sdfdsdf")]
        [InlineData("0.1.0-dev.1.feature.1.debug+sdfdsdf")]
        [InlineData("0.1.1-debug.0.1.0")]
        [InlineData("0.1.0-dev.1.debug")]
        [InlineData("0.1.0-dev.1.feature.1.debug")]
        public void VersionNumberParseToStringIsIdempotent(string versionLabel)
        {
            var version = VersionNumber.Parse(versionLabel);
            version.ToString().Should().Be(versionLabel);
        }

        [Theory]
        [MemberData(nameof(VersionNumberCompareLowerHigherTestCases))]
        public void VersionNumberCompareToHigherTest(VersionNumber lowVersion, VersionNumber highVersion)
        {
            lowVersion.Should().BeLessThan(highVersion);
            highVersion.Should().BeGreaterThan(lowVersion);
        }

        [Theory]
        [MemberData(nameof(VersionNumberCompareEqualsTestCases))]
        public void VersionNumberCompareToEqualsTest(VersionNumber firstVersion, VersionNumber secondVersion)
        {
            firstVersion.Should().BeGreaterOrEqualTo(secondVersion);
            firstVersion.Should().BeLessOrEqualTo(secondVersion);
            secondVersion.Should().BeGreaterOrEqualTo(firstVersion);
            secondVersion.Should().BeLessOrEqualTo(firstVersion);
        }

        [Theory]
        [InlineData(1, 0, 0, "1.0.0.0")]
        [InlineData(0, 0, 0, "0.0.0.0")]
        [InlineData(1, 1, 0, "1.1.0.0")]
        [InlineData(1, 0, 1, "1.0.1.0")]
        public void AssemblyVersionGeneration(uint major, uint minor, uint patch, string expected)
        {
            var version = new VersionNumber(major, minor, patch);
            version.AssemblyVersion.Should()
                .Be(expected);
        }

        public static IEnumerable<object[]> VersionNumberCompareEqualsTestCases
        {
            get
            {
                yield return new object[] { new VersionNumber(0, 1, 0), new VersionNumber(0, 1, 0) };
                yield return new object[] { new VersionNumber(1, 0, 0), new VersionNumber(1, 0, 0) };
                yield return new object[] { new VersionNumber(0, 0, 1), new VersionNumber(0, 0, 1) };
                yield return new object[] { new VersionNumber(0, 0, 1, new PreReleaseLabel(new PreReleaseLabelParticle("a", 1))), new VersionNumber(0, 0, 1, new PreReleaseLabel(new PreReleaseLabelParticle("a", 1))) };
                yield return new object[] { new VersionNumber(0, 0, 1, new FeaturePreReleaseLabel(new PreReleaseLabelParticle("a", 1), new PreReleaseLabelParticle("b", 2))), new VersionNumber(0, 0, 1, new FeaturePreReleaseLabel(new PreReleaseLabelParticle("a", 1), new PreReleaseLabelParticle("b", 2))) };
            }
        }

        public static IEnumerable<object[]> VersionNumberCompareLowerHigherTestCases
        {
            get
            {
                yield return new object[] { new VersionNumber(0, 1, 0), new VersionNumber(0, 2, 0) };
                yield return new object[] { new VersionNumber(1, 0, 0), new VersionNumber(3, 0, 0) };
                yield return new object[] { new VersionNumber(0, 0, 1), new VersionNumber(0, 0, 2) };
                yield return new object[] { new VersionNumber(0, 0, 1, new PreReleaseLabel(new PreReleaseLabelParticle("a", 1))), new VersionNumber(0, 0, 1, new PreReleaseLabel(new PreReleaseLabelParticle("a", 2))) };
                yield return new object[] { new VersionNumber(0, 0, 1, new FeaturePreReleaseLabel(new PreReleaseLabelParticle("a", 1), new PreReleaseLabelParticle("b", 3))), new VersionNumber(0, 0, 1, new FeaturePreReleaseLabel(new PreReleaseLabelParticle("a", 1), new PreReleaseLabelParticle("b", 5))) };
                yield return new object[] { new VersionNumber(0, 0, 1, new PreReleaseLabel(new PreReleaseLabelParticle("a", 1))), new VersionNumber(0, 0, 1, new FeaturePreReleaseLabel(new PreReleaseLabelParticle("a", 1), new PreReleaseLabelParticle("b", 2))) };
            }
        }
    }
}
