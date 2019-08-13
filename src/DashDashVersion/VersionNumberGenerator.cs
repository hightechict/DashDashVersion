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
using DashDashVersion.RepositoryAbstraction;

namespace DashDashVersion
{
    /// <summary>
    /// This class generates a version number for a git repository.
    /// </summary>
    public static class VersionNumberGenerator
    {
        /// <summary>
        /// Generate a VersionNumber for the git repository.
        /// </summary>
        /// <param name="path">The path to the git repository.</param>
        /// <param name="branch">The name of the branch to consider when generating the version number.</param>
        /// <returns></returns>
        public static VersionNumber GenerateVersionNumber(
            string path, 
            string branch)
        {
            var repo = GitRepoReader.Load(path, branch);
            return GenerateVersionNumber(repo);
        }

        internal static VersionNumber GenerateVersionNumber(IGitRepoReader repo)
        {
            var currentBranch = repo.CurrentBranch;
            var headCommitHash = repo.HeadCommitHash.Length > Constants.BuildMetadataHashLength
                ? repo.HeadCommitHash.Substring(0, Constants.BuildMetadataHashLength)
                : repo.HeadCommitHash;
            var tagOnHead = repo.TagOnHead;
            return currentBranch switch
            {
                FeatureBranchInfo feature => GenerateFeatureVersionNumber(repo, feature, headCommitHash),
                ReleaseCandidateBranchInfo releaseCandidate => GenerateReleaseVersionNumber(repo, releaseCandidate, headCommitHash),
                DevelopBranchInfo develop => GenerateDevelopVersionNumber(repo, develop, headCommitHash),
                MasterBranchInfo _ when TagOnHeadIsMajorMinorPatch(tagOnHead) => VersionNumber.Parse(tagOnHead.FriendlyName),
                _ => throw new ArgumentOutOfRangeException(
                        $"'{currentBranch.Name}' is not a branch that is supported for automated version generation, please tag the commit manualy.",
                        nameof(currentBranch.Name))
            };
        }

        private static VersionNumber GenerateDevelopVersionNumber(
            IGitRepoReader repo, 
            DevelopBranchInfo develop,
            string headCommitHash) =>
            new VersionNumber(
                repo.CurrentCoreVersion.Major, 
                repo.CurrentCoreVersion.Minor + 1,
                0,
                develop.DeterminePreReleaseLabel(repo.CommitCountSinceLastMinorVersion),
                headCommitHash);

        private static VersionNumber GenerateReleaseVersionNumber(
            IGitRepoReader repo,
            ReleaseCandidateBranchInfo releaseCandidate, 
            string headCommitHash)
        {
            var ordinal = repo.HighestMatchingTagForReleaseCandidate?.PreReleaseLabel?.BranchLabel
                              .Ordinal + 1;
            var preReleaseLabel = releaseCandidate.DeterminePreReleaseLabel(ordinal ?? 1);
            var version = releaseCandidate.VersionFromName;
            return new VersionNumber(
                version.Major,
                version.Minor,
                version.Patch,
                preReleaseLabel,
                headCommitHash);
        }

        private static VersionNumber GenerateFeatureVersionNumber(
            IGitRepoReader repo, 
            FeatureBranchInfo feature,
            string headCommitHash)
        {
            var developOrdinal = repo.CommitCountSinceLastMinorVersion - repo.CommitCountUniqueToFeature;
            if((int)developOrdinal < 0)
            {
                throw new InvalidOperationException("While calculating the develop version a negative number was found, ether the feature has no root in the develop branch or a incorect core-version tag was placed like '0.1.0'");
            }
            var preReleaseLabel = feature.DeterminePreReleaseLabel(
                developOrdinal,
                repo.CommitCountUniqueToFeature);
            return new VersionNumber(
                repo.CurrentCoreVersion.Major, 
                repo.CurrentCoreVersion.Minor + 1,
                0,
                preReleaseLabel,
                headCommitHash);
        }

        private static bool TagOnHeadIsMajorMinorPatch(GitTag tagOnHead) => 
            tagOnHead != null && 
            Patterns.IsCoreVersionTag.IsMatch(tagOnHead.FriendlyName);
    }
}
