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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace DashDashVersion.RepositoryAbstraction
{
    /// <summary>
    /// This class uses LibGet2Sharp to read the information from the git repository needed to determine a version number.
    /// </summary>
    internal sealed class GitRepoReader : IGitRepoReader
    {
        private readonly IGitRepository _repository;
        private readonly Lazy<uint> _commitCountSinceBranchOffFromDevelop;
        private readonly Lazy<uint> _commitCountSinceLastMinorVersion;
        private readonly Lazy<VersionNumber> _currentVersionCore;
        private readonly Lazy<List<(GitTag tag, VersionNumber versionNumber)>> _highestVersionCoreListHighToLow;
        private readonly Lazy<List<GitTag>> _visibleTags;
        private readonly Lazy<GitTag> _tagOnHead;

        internal static GitRepoReader Load(
            string path,
            string currentBranch)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("The path should not be null or empty.", nameof(path));
            }
            if (!Directory.Exists(path) && !File.Exists(path))
            {
                throw new ArgumentException($"The path '{path}' does not exist.", nameof(path));
            }

            var repoPath = Repository.Discover(path);
            if (string.IsNullOrEmpty(repoPath))
            {
                throw new ArgumentException($"The path: '{path}' is not the root of or in a git repository.", nameof(path));
            }

            var repository = new Repository(repoPath);
            var gitRepo = GitRepository.FromRepository(repository);
            return new GitRepoReader(gitRepo, currentBranch);
        }

        internal GitRepoReader(
            IGitRepository repo,
            string currentBranchName)
        {
            _repository = repo;
            CurrentBranch = FindCurrentBranch(currentBranchName);

            var hash = _repository.CurrentBranch.Head;
            HeadCommitHash = hash?.Sha ?? throw new InvalidOperationException("Git repositories without commits are not supported.");

            _visibleTags = new Lazy<List<GitTag>>(VisibleTags);
            _commitCountSinceBranchOffFromDevelop = new Lazy<uint>(CalculateCommitCountSinceBranchOff);
            _highestVersionCoreListHighToLow = new Lazy<List<(GitTag tag, VersionNumber versionNumber)>>(HighestVersionCoresMajorMinor);
            _currentVersionCore = new Lazy<VersionNumber>(CalculateCurrentVersionCore);
            _commitCountSinceLastMinorVersion = new Lazy<uint>(CalculateCommitCountSinceLastMinorVersion);
            _tagOnHead = new Lazy<GitTag>(CalculateTagOnHead);
        }


        public string HeadCommitHash { get; }

        public GitTag TagOnHead => _tagOnHead.Value;

        public VersionNumber CurrentVersionCore => _currentVersionCore.Value;

        public BranchInfo CurrentBranch { get; }

        public uint CommitCountSinceLastMinorVersion => _commitCountSinceLastMinorVersion.Value;

        public VersionNumber? HighestMatchingTagForReleaseCandidate
        {
            get
            {
                if (!(CurrentBranch is ReleaseCandidateBranchInfo currentRc))
                {
                    throw new InvalidOperationException(
                        $"Determining the highest matching tag for release candidates is only possible if the current branch is a 'release' branch, the current branch is'{CurrentBranch}'.");
                }
                var toCheckFor =
                    $"{currentRc.VersionFromName}{Constants.PreReleaseLabelDelimiter}{Constants.ReleasePreReleaseLabel}";
                var returnTag = HighestMatchingTag(toCheckFor);
                return returnTag == null ? null : VersionNumber.Parse(returnTag);
            }
        }

        public uint CommitCountSinceBranchOffFromDevelop => _commitCountSinceBranchOffFromDevelop.Value;

        private VersionNumber CalculateCurrentVersionCore() => _highestVersionCoreListHighToLow.Value.First().versionNumber;

        private uint CalculateCommitCountSinceBranchOff()
        {
            var developBranch = OriginDevelopOrDevelopCommits(_repository.Branches);
            uint toReturn = 0;
            foreach (var commit in _repository.CurrentBranch.Commits)
            {
                if (developBranch.CommitCollection.Contains(commit))
                {
                    return toReturn;
                }

                toReturn++;
            }
            throw new InvalidOperationException(
                $"Git repository does not contain a common ancestor between '{Constants.DevelopBranchName}' and the current HEAD.");
        }

        private string? HighestMatchingTag(string toMatch) =>
               _repository.Tags
                    .Where(tag => tag.FriendlyName.StartsWith(toMatch))
                    .Where(tag => Patterns.ValidVersionNumber.IsMatch(tag.FriendlyName))
                    .OrderByDescending(t => VersionNumber.Parse(t.FriendlyName))
                    .FirstOrDefault()?.FriendlyName;

        private static GitBranch OriginDevelopOrDevelopCommits(IEnumerable<GitBranch> branches)
        {
            var develop = branches.FirstOrDefault(IsOriginDevelop);
            if (develop == null)
            {
                // ReSharper disable once PossibleMultipleEnumeration
                develop = branches.FirstOrDefault(IsDevelop);
                if (develop == null)
                {
                    throw new InvalidOperationException(
                        $"Git repository does not contain a branch named '{Constants.DevelopBranchName}' or '{Constants.OriginDevelop}'.");
                }
            }

            var developCommits = develop.CommitCollection.Commits;
            if (!developCommits.Any())
            {
                throw new InvalidOperationException(
                    $"Git repository does not contain any commits on '{Constants.DevelopBranchName}' or '{Constants.OriginDevelop}'.");
            }

            return develop;
        }

        private List<(GitTag tag, VersionNumber versionNumber)> HighestVersionCoresMajorMinor()
        {
            var releaseTagsAndVersions = _visibleTags.Value
                .Where(tag => Patterns.IsVersionCoreTag.IsMatch(tag.FriendlyName))
                .Select(tag => (Tag: tag, VersionNumber: VersionNumber.Parse(tag.FriendlyName)))
                .OrderByDescending(pair => pair.VersionNumber)
                .ToList();

            if (!releaseTagsAndVersions.Any())
                throw new InvalidOperationException($"There is no tag with in the '<major>.<minor>.<patch>' format in this repository looking from the HEAD down: {Patterns.IsVersionCoreTag}.");

            var highestVersion = releaseTagsAndVersions.First().VersionNumber;

            return releaseTagsAndVersions
                .Where(
                    pair => pair.VersionNumber.Major == highestVersion.Major &&
                    pair.VersionNumber.Minor == highestVersion.Minor).ToList();
        }

        private uint CalculateCommitCountSinceLastMinorVersion()
        {
            var versionTagSha = _highestVersionCoreListHighToLow.Value
                        .Last()
                        .tag
                        .Sha;

            return FindAncestor(
                 versionTagSha,
                 _repository.CurrentBranch.Commits);
        }

        private static uint FindAncestor(
            string sha,
            IEnumerable<GitCommit> commits)
        {
            var matches = commits.Select(
                    (commit, index) => (commit, index))
                .Where(tuple => tuple.commit.Sha == sha).ToList();
            if (matches.Count > 0)
            {
                return (uint)matches.First().index;
            }
            throw new ArgumentException($"No commit found with sha: '{sha}'.", nameof(sha));
        }

        private static string TrimRemoteName(GitBranch branch)
        {
            if (!branch.IsRemote)
            {
                return branch.FriendlyName;
            }
            var splitLocation = branch.FriendlyName.IndexOf(Constants.BranchNameInfoDelimiter) + 1;
            return branch.FriendlyName.Substring(splitLocation);
        }

        private GitBranch FindBranch(string branchName)
        {
            var perfectMatch = _repository.Branches.FirstOrDefault(b => b.FriendlyName.Equals(branchName));
            if (perfectMatch != null)
            {
                return perfectMatch;
            }

            var branches = _repository.Branches.Where(b => b.FriendlyName.EndsWith(branchName) && Patterns.DetermineBranchType.IsMatch(b.FriendlyName));
            if (!branches.Any())
            {
                throw new ArgumentException($"The branch '{branchName}' could not be found in the repository, or it was not of any of the supported types.", nameof(branchName));
            }

            var distinctBranchTypes = branches.Select(b => Patterns.DetermineBranchType.Match(b.FriendlyName).Groups["branchType"].Captures[0]?.Value).Distinct();
            if (distinctBranchTypes.Count() > 1)
            {
                throw new ArgumentException($"This partial branch name: '{branchName}' is not unique in the repository.", nameof(branchName));
            }

            return branches.First();
        }

        private GitBranch FindCurrentGitBranch(string branchName) =>
            string.IsNullOrWhiteSpace(branchName) ?
                BranchForRepositoryHead() :
                FindBranch(branchName);

        private List<GitTag> VisibleTags() => 
            _repository.Tags.Where(
                tag => _repository.CurrentBranch.Commits.Any(commit => commit.Sha == tag.Sha))
                .ToList();


        private static bool IsDevelop(GitBranch branch) =>
            branch.FriendlyName.Equals(Constants.DevelopBranchName);

        private static bool IsOriginDevelop(GitBranch branch) =>
            branch.IsRemote &&
            branch.FriendlyName.Equals(Constants.OriginDevelop);

        private GitBranch BranchForRepositoryHead() =>
            _repository.Branches
                .Where(f => f.IsCurrentRepositoryHead)
                .Select(f => f)
                .FirstOrDefault() ??
            throw new InvalidOperationException(
                "The repository is on a detached HEAD, please specify the name of the branch for which the version should be calculated using the --branch command-line argument.");

        private BranchInfo FindCurrentBranch(string branchName) =>
            BranchInfoFactory.CreateBranchInfo(
                TrimRemoteName(
                    FindCurrentGitBranch(branchName)));

        private GitTag CalculateTagOnHead() => 
            _visibleTags.Value.FirstOrDefault(t => t.Sha.Equals(HeadCommitHash));


    }
}
