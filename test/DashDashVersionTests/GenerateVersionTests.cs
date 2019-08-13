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

using Xunit;
using FluentAssertions;
using DashDashVersion;
using DashDashVersion.RepositoryAbstraction;
using Moq;
using System;

namespace DashDashVersionTests
{
    public class GenerateVersionTests
    {
        private readonly Mock<IGitRepoReader> _mock;

        public GenerateVersionTests()
        {
            _mock = new Mock<IGitRepoReader>();
        }

        [Theory]
        [InlineData("0.1.0", 4, "asd3bds", "0.2.0-dev.4+asd3bds")]
        [InlineData("0.1.1", 4, "asd3bds", "0.2.0-dev.4+asd3bds")]
        [InlineData("1.1.1", 4, "asd3bds", "1.2.0-dev.4+asd3bds")]

        [InlineData("1.1.1", 1, "asd3bds", "1.2.0-dev.1+asd3bds")]
        [InlineData("1.1.1", 33, "asd3bds", "1.2.0-dev.33+asd3bds")]
        [InlineData("1.1.1", 1123200, "asd3bds", "1.2.0-dev.1123200+asd3bds")]

        [InlineData("1.1.1", 1, "cdesddss", "1.2.0-dev.1+cdesdds")]
        [InlineData("1.1.1", 33, "3454544s", "1.2.0-dev.33+3454544")]
        [InlineData("1.1.1", 1123200, "As2FdDs", "1.2.0-dev.1123200+As2FdDs")]
        [InlineData("1.1.1", 1123200, "skldfjeierjkmdsmdkfewjmvkxcgklskjlsvjeoajgflkvdfiklgjiag", "1.2.0-dev.1123200+skldfje")]
        public void VersionNumberDevelopVersion(
            string highestAnnotatedTag,
            uint commitsSinceAnnotatedTag,
            string hash,
            string expectedVersion)
        {
            _mock.Setup(f => f.CurrentBranch).Returns(BranchInfoFactory.CreateBranchInfo(Constants.DevelopBranchName));
            _mock.Setup(f => f.CurrentCoreVersion).Returns(VersionNumber.Parse(highestAnnotatedTag));
            _mock.Setup(f => f.CommitCountSinceLastMinorVersion).Returns(commitsSinceAnnotatedTag);
            _mock.Setup(f => f.HeadCommitHash).Returns(hash);

            var version = VersionNumberGenerator.GenerateVersionNumber(_mock.Object);

            version.FullSemVer.Should().Be(expectedVersion);
        }

        [Theory]
        [InlineData("1.0.0", "feature/a", 6, 1, "aa34232", "1.1.0-dev.5.a.1+aa34232")]
        [InlineData("1.0.0", "feature/Bert", 6, 1, "aa34232", "1.1.0-dev.5.Bert.1+aa34232")]
        [InlineData("1.0.0", "feature/b", 6, 1, "aa34232", "1.1.0-dev.5.b.1+aa34232")]
        public void VersionNumberFeatureVersion(
            string highestAnnotatedTag,
            string branchName,
            uint commitsSinceAnnotatedTag,
            uint commitsSinceBranchOff,
            string hash,
            string expectedVersion)
        {
            _mock.Setup(f => f.CurrentBranch).Returns(BranchInfoFactory.CreateBranchInfo(branchName));
            _mock.Setup(f => f.CurrentCoreVersion).Returns(VersionNumber.Parse(highestAnnotatedTag));
            _mock.Setup(f => f.CommitCountSinceLastMinorVersion).Returns(commitsSinceAnnotatedTag);
            _mock.Setup(f => f.CommitCountUniqueToFeature).Returns(commitsSinceBranchOff);
            _mock.Setup(f => f.HeadCommitHash).Returns(hash);

            var version = VersionNumberGenerator.GenerateVersionNumber(_mock.Object);

            version.ToString().Should().Be(expectedVersion);
        }

        [Theory]
        [InlineData(6, 7)]
        [InlineData(3, 4)]
        public void VersionNumberReleaseBranchLabelOrdinalIncrements(
            uint ordinal,
            uint expectedOrdinal)
        {
            _mock.Setup(f => f.CurrentBranch).Returns(new ReleaseCandidateBranchInfo("release/1.0.0", "1.0.0"));
            _mock.Setup(f => f.HeadCommitHash).Returns("a");
            _mock.Setup(f => f.HighestMatchingTagForReleaseCandidate).Returns(new VersionNumber(1, 0, 0, new PreReleaseLabel(new PreReleaseLabelParticle("rc", ordinal))));

            var version = VersionNumberGenerator.GenerateVersionNumber(_mock.Object);
            version.PreReleaseLabel.Should().NotBeNull();
            version.PreReleaseLabel?.BranchLabel.Ordinal.Equals(expectedOrdinal);
        }

        [Fact]
        public void VersionNumberReleaseBranchLabelOrdinalStartsAtOne()
        {
            _mock.Setup(f => f.CurrentBranch).Returns(new ReleaseCandidateBranchInfo("release/1.0.0", "1.0.0"));
            _mock.Setup(f => f.HeadCommitHash).Returns("a");

            var version = VersionNumberGenerator.GenerateVersionNumber(_mock.Object);
            version.PreReleaseLabel.Should().NotBeNull();
            version.PreReleaseLabel?.BranchLabel.Ordinal.Equals(1);
        }

        [Fact]
        public void MasterRepoVersionTest()
        {
            var version = GenerateVersionNumber(TestRepositories.MasterOnlyRepository());
            version.SemVer.Should().Be("1.0.0");
        }

        [Fact]
        public void DevelopRepoVersionTest()
        {
            var version = GenerateVersionNumber(TestRepositories.TwoCommitsOnDevelopRepository());
            version.SemVer.Should().Be("0.1.0-dev.1");
        }

        [Fact]
        public void FeatureRepoVersionTest()
        {
            var version = GenerateVersionNumber(TestRepositories.FeatureBranchOnFeatureBranchRepository());
            version.SemVer.Should().Be("0.1.0-dev.1.B.2");
        }

        [Fact]
        public void ReleaseRepoWithoutTagVersionTest()
        {
            var version = GenerateVersionNumber(TestRepositories.ReleaseBranchRepositoryWithoutTag());
            version.SemVer.Should().Be("1.0.0-rc.1");
        }

        [Fact]
        public void ReleaseRepoWithTagVersionTest()
        {
            var version = GenerateVersionNumber(TestRepositories.ReleaseBranchRepositoryWithTaggedRc());
            version.SemVer.Should().Be("1.0.0-rc.3");
        }

        [Fact]
        public void RepoWithoutHeadDevelopTest()
        {
            var version = GenerateVersionNumber(TestRepositories.DeteachedHeadRepo(), "develop");
            version.SemVer.Should().Be("0.2.0-dev.0");
        }

        [Fact]
        public void RepoWithoutHeadMasterTest()
        {
            var version = GenerateVersionNumber(TestRepositories.DeteachedHeadRepo(), "master");
            version.SemVer.Should().Be("0.1.0");
        }

        [Fact]
        public void RepoWithoutHeadFullPathTest()
        {
            var version = GenerateVersionNumber(TestRepositories.DeteachedHeadRepo(), "Feature-develop");
            version.SemVer.Should().Be("0.2.0-dev.0.Feature-develop.0");
        }

        [Fact]
        public void RepoWithoutHeadFullRemotePathTest()
        {
            var version = GenerateVersionNumber(TestRepositories.DeteachedHeadRepo(), "A");
            version.SemVer.Should().Be("0.2.0-dev.0.FeatureA.0");
        }

        [Fact]
        public void RepoWithoutHeadIncorrectPartialBranchNameTest()
        {
            Action action = () =>
            {
                _ = GenerateVersionNumber(TestRepositories.DeteachedHeadRepo(), "evelop");
            };
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void RepoWithoutHeadIncorrectBranchTest()
        {
            Action action = () =>
            {
                _ = GenerateVersionNumber(TestRepositories.DeteachedHeadRepo(), "feature/a");
            };
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void RepoWithoutHeadNoBranchTest()
        {
            Action action = () =>
            {
                _ = GenerateVersionNumber(TestRepositories.DeteachedHeadRepo());
            };
            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void PatchReleaseTest()
        {

            var version = GenerateVersionNumber(TestRepositories.PatchReleaseRepository());
            version.SemVer.Should().Be("0.2.0-dev.3");
        }

        private static VersionNumber GenerateVersionNumber(IGitRepository gitRepository, string branchName = "") =>
            VersionNumberGenerator.GenerateVersionNumber(
                new GitRepoReader(
                    gitRepository,
                    branchName));
    }
}
