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
        private readonly VersionNumberGenerator _versionGenerator;
        private readonly Mock<IGitRepoReader> _mock;

        public GenerateVersionTests()
        {
            _mock = new Mock<IGitRepoReader>();
            _versionGenerator = new VersionNumberGenerator(_mock.Object);
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
            _mock.Setup(f => f.CurrentReleaseVersion).Returns(VersionNumber.Parse(highestAnnotatedTag));
            _mock.Setup(f => f.CommitCountSinceLastMinorReleaseVersion).Returns(commitsSinceAnnotatedTag);
            _mock.Setup(f => f.HeadCommitHash).Returns(hash);

            var version = _versionGenerator.VersionNumber;

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
            _mock.Setup(f => f.CurrentReleaseVersion).Returns(VersionNumber.Parse(highestAnnotatedTag));
            _mock.Setup(f => f.CommitCountSinceLastMinorReleaseVersion).Returns(commitsSinceAnnotatedTag);
            _mock.Setup(f => f.CommitCountSinceBranchOffFromDevelop).Returns(commitsSinceBranchOff);
            _mock.Setup(f => f.HeadCommitHash).Returns(hash);

            var version = _versionGenerator.VersionNumber;

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

            var version = _versionGenerator.VersionNumber;
            version.PreReleaseLabel.Should().NotBeNull();
            version.PreReleaseLabel?.BranchLabel.Ordinal.Equals(expectedOrdinal);
        }

        [Fact]
        public void VersionNumberReleaseBranchLabelOrdinalStartsAtOne()
        {
            _mock.Setup(f => f.CurrentBranch).Returns(new ReleaseCandidateBranchInfo("release/1.0.0", "1.0.0"));
            _mock.Setup(f => f.HeadCommitHash).Returns("a");

            var version = _versionGenerator.VersionNumber;
            version.PreReleaseLabel.Should().NotBeNull();
            version.PreReleaseLabel?.BranchLabel.Ordinal.Equals(1);
        }

        [Fact]
        public void MasterRepoVersionTest()
        {
            var versionNumberGenerator = CreateVersionNumberGenerator(TestRepositories.MasterOnlyRepository());
            versionNumberGenerator.VersionNumber.SemVer.Should().Be("1.0.0");
        }

        [Fact]
        public void DevelopRepoVersionTest()
        {
            var versionNumberGenerator = CreateVersionNumberGenerator(TestRepositories.TwoCommitsOnDevelopRepository());
            versionNumberGenerator.VersionNumber.SemVer.Should().Be("0.1.0-dev.1");
        }

        [Fact]
        public void FeatureRepoVersionTest()
        {
            var versionNumberGenerator = CreateVersionNumberGenerator(TestRepositories.FeatureBranchOnFeatureBranchRepository());
            versionNumberGenerator.VersionNumber.SemVer.Should().Be("0.1.0-dev.1.B.2");
        }

        [Fact]
        public void ReleaseRepoWithoutTagVersionTest()
        {
            var versionNumberGenerator = CreateVersionNumberGenerator(TestRepositories.ReleaseBranchRepositoryWithoutTag());
            versionNumberGenerator.VersionNumber.SemVer.Should().Be("1.0.0-rc.1");
        }

        [Fact]
        public void ReleaseRepoWithTagVersionTest()
        {
            var versionNumberGenerator = CreateVersionNumberGenerator(TestRepositories.ReleaseBranchRepositoryWithTaggedRc());
            versionNumberGenerator.VersionNumber.SemVer.Should().Be("1.0.0-rc.3");
        }

        [Fact]
        public void RepoWithoutHeadDevelopTest()
        {
            var versionNumberGenerator = CreateVersionNumberGenerator(TestRepositories.DeteachedHeadRepo(), "develop");
            versionNumberGenerator.VersionNumber.SemVer.Should().Be("0.2.0-dev.0");
        }

        [Fact]
        public void RepoWithoutHeadMasterTest()
        {
            var versionNumberGenerator = CreateVersionNumberGenerator(TestRepositories.DeteachedHeadRepo(), "master");
            versionNumberGenerator.VersionNumber.SemVer.Should().Be("0.1.0");
        }

        [Fact]
        public void RepoWithoutHeadFullPathTest()
        {
            var versionNumberGenerator = CreateVersionNumberGenerator(TestRepositories.DeteachedHeadRepo(), "Feature-develop");
            versionNumberGenerator.VersionNumber.SemVer.Should().Be("0.2.0-dev.0.Feature-develop.0");
        }

        [Fact]
        public void RepoWithoutHeadFullRemotePathTest()
        {
            var versionNumberGenerator = CreateVersionNumberGenerator(TestRepositories.DeteachedHeadRepo(), "A");
            versionNumberGenerator.VersionNumber.SemVer.Should().Be("0.2.0-dev.0.FeatureA.0");
        }

        [Fact]
        public void RepoWithoutHeadIncorrectPartialBranchNameTest()
        {
            Action action = () =>
            {
                _ = CreateVersionNumberGenerator(TestRepositories.DeteachedHeadRepo(), "evelop");
            };
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void RepoWithoutHeadIncorrectBranchTest()
        {
            Action action = () =>
            {
                _ = CreateVersionNumberGenerator(TestRepositories.DeteachedHeadRepo(), "feature/a");
            };
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void RepoWithoutHeadNoBranchTest()
        {
            Action action = () =>
            {
                _ = CreateVersionNumberGenerator(TestRepositories.DeteachedHeadRepo());
            };
            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void PatchReleaseTest()
        {

            var versionNumberGenerator = CreateVersionNumberGenerator(TestRepositories.PatchReleaseRepository());
            versionNumberGenerator.VersionNumber.SemVer.Should().Be("0.2.0-dev.3");
        }

        private static VersionNumberGenerator CreateVersionNumberGenerator(IGitRepository gitRepository, string branchName = "") =>
            new VersionNumberGenerator(
                new GitRepoReader(
                    gitRepository,
                    branchName));
    }
}