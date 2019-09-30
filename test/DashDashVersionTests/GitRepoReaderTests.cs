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

using DashDashVersion.RepositoryAbstraction;
using FluentAssertions;
using Xunit;

namespace DashDashVersionTests
{
    public class GitRepoReaderTests
    {
        [Fact]
        public void MasterRepoTest()
        {
            var repoReader = new GitRepoReader(TestRepositories.MasterRepository(), string.Empty);
            repoReader.CurrentBranch.Name.Should().Be(DashDashVersion.Constants.MasterBranchName);
            repoReader.CommitCountDevelopSinceLastMinorCoreVersion.Should().Be(0);
            repoReader.HeadCommitHash.Should().Be("a");
            repoReader.CurrentCoreVersion.SemVer.Should().Be("1.0.0");
        }

        [Fact]
        public void MasterPastDevelopTest()
        {
            var repoReader = new GitRepoReader(TestRepositories.MasterAheadOfDevelopRepository(), string.Empty);
            repoReader.CurrentBranch.Name.Should().Be(DashDashVersion.Constants.DevelopBranchName);
            repoReader.CommitCountDevelopSinceLastMinorCoreVersion.Should().Be(1);
            repoReader.HeadCommitHash.Should().Be("b");
            repoReader.CurrentCoreVersion.SemVer.Should().Be("0.0.0");
        }

        [Fact]
        public void DevelopRepoTest()
        {
            var repoReader = new GitRepoReader(TestRepositories.TwoCommitsOnDevelopRepository(), string.Empty);
            repoReader.CommitCountUniqueToFeature.Should().Be(0);
            repoReader.CommitCountDevelopSinceLastMinorCoreVersion.Should().Be(1);
            repoReader.CurrentBranch.Name.Should().Be(DashDashVersion.Constants.DevelopBranchName);
            repoReader.CurrentCoreVersion.SemVer.Should().Be("0.0.0");
            repoReader.HeadCommitHash.Should().Be("b");
        }

        [Fact]
        public void RemoteDevelopRepoTest()
        {
            var repoReader = new GitRepoReader(TestRepositories.RemoteDevelopRepository(), string.Empty);
            repoReader.CommitCountUniqueToFeature.Should().Be(0);
            repoReader.CommitCountDevelopSinceLastMinorCoreVersion.Should().Be(1);
            repoReader.CurrentBranch.Name.Should().Be(DashDashVersion.Constants.DevelopBranchName);
            repoReader.CurrentCoreVersion.SemVer.Should().Be("0.0.0");
            repoReader.HeadCommitHash.Should().Be("b");
        }

        [Fact]
        public void ReleaseRepoWithoutTagTest()
        {
            var repoReader = new GitRepoReader(TestRepositories.ReleaseBranchRepositoryWithoutTag(), string.Empty);
            repoReader.CommitCountUniqueToFeature.Should().Be(1);
            repoReader.CommitCountDevelopSinceLastMinorCoreVersion.Should().Be(1);
            repoReader.HeadCommitHash.Should().Be("c");
            repoReader.CurrentCoreVersion.SemVer.Should().Be("0.0.0");
            repoReader.CurrentBranch.Name.Should().Be($"{DashDashVersion.Constants.ReleaseBranchName}/1.0.0");
            repoReader.HighestMatchingTagForReleaseCandidate.Should().BeNull();
        }

        [Fact]
        public void ReleaseRepoWithTagTest()
        {
            var repoReader = new GitRepoReader(TestRepositories.ReleaseBranchRepositoryWithTaggedRc(), string.Empty);
            repoReader.CommitCountUniqueToFeature.Should().Be(3);
            repoReader.CommitCountDevelopSinceLastMinorCoreVersion.Should().Be(1);
            repoReader.HeadCommitHash.Should().Be("e");
            repoReader.CurrentCoreVersion.SemVer.Should().Be("0.0.0");
            repoReader.CurrentBranch.Name.Should().Be($"{DashDashVersion.Constants.ReleaseBranchName}/1.0.0");
            repoReader.HighestMatchingTagForReleaseCandidate?.ToString().Should().Be("1.0.0-rc.2");
        }

        [Fact]
        public void FeatureRepoTest()
        {
            var repoReader = new GitRepoReader(TestRepositories.FeatureBranchOnFeatureBranchRepository(), string.Empty);
            repoReader.CommitCountUniqueToFeature.Should().Be(2);
            repoReader.CommitCountDevelopSinceLastMinorCoreVersion.Should().Be(1);
            repoReader.HeadCommitHash.Should().Be("d");
            repoReader.CurrentCoreVersion.SemVer.Should().Be("0.0.0");
            repoReader.CurrentBranch.Name.Should().Be($"{DashDashVersion.Constants.FeatureBranchName}/B");
        }

        [Fact]
        public void DevelopRepoWithoutCoreVersion()
        {
            var repoReader = new GitRepoReader(TestRepositories.TwoCommitsOnDevelopWithoutCoreVersionRepository(), string.Empty);
            repoReader.CommitCountUniqueToFeature.Should().Be(0);
            repoReader.CommitCountDevelopSinceLastMinorCoreVersion.Should().Be(1);
            repoReader.CurrentBranch.Name.Should().Be(DashDashVersion.Constants.DevelopBranchName);
            repoReader.CurrentCoreVersion.SemVer.Should().Be("0.0.0");
            repoReader.HeadCommitHash.Should().Be("b");

        }

        [Fact]
        public void FeatureDebugMergedVersion()
        {
            var repoReader = new GitRepoReader(TestRepositories.FeatureDebugMergedRepository(), string.Empty);
            repoReader.CommitCountDevelopSinceLastMinorCoreVersion.Should().Be(4);
            repoReader.CommitCountUniqueToFeature.Should().Be(7);
        }
    }
}
