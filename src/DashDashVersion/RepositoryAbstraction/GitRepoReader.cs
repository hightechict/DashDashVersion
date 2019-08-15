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
        private readonly Lazy<uint> _commitCountUniqueToFeature;
        private readonly Lazy<uint> _commitCountSinceLastMinorVersion;
        private readonly Lazy<VersionNumber> _currentCoreVersion;
        private readonly Lazy<List<(GitTag tag, VersionNumber versionNumber)>> _highestCoreVersionListHighToLow;
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

            using var repository = new Repository(repoPath);
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

            _visibleTags = new Lazy<List<GitTag>>(() => VisibleTags);
            _commitCountUniqueToFeature = new Lazy<uint>(CalculateCommitCountUniqueToFeature);
            _highestCoreVersionListHighToLow = new Lazy<List<(GitTag tag, VersionNumber versionNumber)>>(HighestCoreVersionsMajorMinor);
            _currentCoreVersion = new Lazy<VersionNumber>(CalculateCurrentCoreVersion);
            _commitCountSinceLastMinorVersion = new Lazy<uint>(CalculateCommitCountSinceLastMinorVersion);
            _tagOnHead = new Lazy<GitTag>(() => CalculateTagOnHead);
        }

        public string HeadCommitHash { get; }

        public GitTag TagOnHead => _tagOnHead.Value;

        public VersionNumber CurrentCoreVersion => _currentCoreVersion.Value;

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

        public uint CommitCountUniqueToFeature => _commitCountUniqueToFeature.Value;

        private VersionNumber CalculateCurrentCoreVersion() => _highestCoreVersionListHighToLow.Value.First().versionNumber;

        private uint CalculateCommitCountUniqueToFeature()
        {
            var toReturn = (uint)_repository.CurrentBranch.Except(_repository.Develop.Commits).Count();
            if (!_repository.CurrentBranch.Overlaps(_repository.Develop.Commits))
                throw new InvalidOperationException($"Git repository does not contain a common ancestor between '{Constants.DevelopBranchName}' and the current HEAD.");
            return toReturn;
        }

        private string? HighestMatchingTag(string toMatch) =>
               _repository.Tags
                    .Where(tag => tag.FriendlyName.StartsWith(toMatch))
                    .Where(tag => Patterns.ValidVersionNumber.IsMatch(tag.FriendlyName))
                    .OrderByDescending(t => VersionNumber.Parse(t.FriendlyName))
                    .FirstOrDefault()?.FriendlyName;


        private List<(GitTag tag, VersionNumber versionNumber)> HighestCoreVersionsMajorMinor()
        {
            var coreVersionTagsAndVersions = _visibleTags.Value
                .Where(tag => Patterns.IsCoreVersionTag.IsMatch(tag.FriendlyName))
                .Where(tag => _repository.Master.Commits.Contains(tag.Sha))
                .Select(tag => (Tag: tag, VersionNumber: VersionNumber.Parse(tag.FriendlyName)))
                .OrderByDescending(pair => pair.VersionNumber)
                .ToList();

            var CoreVersionTagNotOnMaster = coreVersionTagsAndVersions.Select(pair => pair.Tag).FirstOrDefault(tag => !_repository.Master.Commits.Contains(tag.Sha));
            if (!coreVersionTagsAndVersions.Any())
            {
                var sha = _repository.CurrentBranch.First.Sha;
                coreVersionTagsAndVersions.Add((new GitTag("0.0.0+assumption", sha), new VersionNumber(0, 0, 0, null, "assumption")));
                Console.Error.WriteLine(
@$"Warning you currently have no core version tag on master like '0.0.0'.
You could use 'git tag 0.0.0 {sha}' to place a tag.");
            }
            else if (CoreVersionTagNotOnMaster != null)
            {
                throw new InvalidOperationException($"The tag: {CoreVersionTagNotOnMaster.FriendlyName} is not on master, if this is on a build server maybe the remote refs have not been fetched? all core tags must be on master for this program to function");
            }
            var highestVersion = coreVersionTagsAndVersions.First().VersionNumber;

            return coreVersionTagsAndVersions
                .Where(
                    pair => pair.VersionNumber.Major == highestVersion.Major &&
                    pair.VersionNumber.Minor == highestVersion.Minor).ToList();
        }

        private uint CalculateCommitCountSinceLastMinorVersion()
        {
            var versionTagSha = _highestCoreVersionListHighToLow.Value
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
                BranchForRepositoryHead :
                FindBranch(branchName);

        private List<GitTag> VisibleTags =>
            _repository.Tags.Where(
                tag => _repository.CurrentBranch.Contains(tag.Sha))
                .ToList();

        private GitBranch BranchForRepositoryHead =>
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

        private GitTag CalculateTagOnHead =>
            _visibleTags.Value.FirstOrDefault(t => t.Sha.Equals(HeadCommitHash));
    }
}
