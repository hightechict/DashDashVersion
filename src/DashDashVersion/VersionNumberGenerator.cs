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
    /// This class generates a version number for the current head of a git repository.
    /// </summary>
    public class VersionNumberGenerator
    {
        private readonly IGitRepoReader _repoReader;

        public static VersionNumber GenerateVersionNumber(string path)
        {
            var repo = GitRepoReader.Load(path);
            var versionNumberGenerator = new VersionNumberGenerator(repo);
            return versionNumberGenerator.VersionNumber;
        }

        internal VersionNumberGenerator(IGitRepoReader repoReader) =>
            _repoReader = repoReader;

        internal VersionNumber VersionNumber
        {
            get
            {
                PreReleaseLabel preReleaseLabel;
                var currentBranch = _repoReader.CurrentBranch;
                var headCommitHash = _repoReader.HeadCommitHash.Length > Constants.BuildMetadataHashLength ?
                    _repoReader.HeadCommitHash.Substring(0, Constants.BuildMetadataHashLength) :
                    _repoReader.HeadCommitHash;
                switch (currentBranch)
                {
                    case FeatureBranchInfo feature:
                        preReleaseLabel = feature.DeterminePreReleaseLabel(
                            _repoReader.CommitCountSinceLastReleaseVersion - _repoReader.CommitCountSinceBranchOffFromDevelop,
                            _repoReader.CommitCountSinceBranchOffFromDevelop);
                        return new VersionNumber(
                            _repoReader.CurrentReleaseVersion.Major,
                            _repoReader.CurrentReleaseVersion.Minor + 1,
                            0,
                            preReleaseLabel,
                            headCommitHash);
                    case ReleaseCandidateBranchInfo releaseCandidate:
                        var ordinal = _repoReader.HighestMatchingTagForReleaseCandidate?.PreReleaseLabel?.BranchLabel.Ordinal + 1;
                        preReleaseLabel = releaseCandidate.DeterminePreReleaseLabel(ordinal ?? 1);
                        var version = releaseCandidate.VersionFromName;
                        return new VersionNumber(
                            version.Major,
                            version.Minor,
                            version.Patch,
                            preReleaseLabel,
                            headCommitHash);
                    case DevelopBranchInfo develop:
                        return new VersionNumber(
                                _repoReader.CurrentReleaseVersion.Major,
                                _repoReader.CurrentReleaseVersion.Minor + 1,
                                0,
                                develop.DeterminePreReleaseLabel(_repoReader.CommitCountSinceLastReleaseVersion),
                                headCommitHash);
                }

                if (_repoReader.CommitCountSinceLastReleaseVersion == 0)
                {
                    return _repoReader.CurrentReleaseVersion;
                }

                throw new ArgumentOutOfRangeException($"{currentBranch.Name} is not a branch that is supported for automated version generation, please tag the commit manualy.",
                    nameof(currentBranch.Name));
            }
        }
    }
}