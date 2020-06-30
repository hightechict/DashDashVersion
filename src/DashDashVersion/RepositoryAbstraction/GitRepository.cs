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
using System.Linq;
using LibGit2Sharp;

namespace DashDashVersion.RepositoryAbstraction
{
    /// <summary>
    /// This class is a stand-in for the LibGit2Sharp `Repository` type, used to confine the LibGit2Sharp dependency.
    /// </summary>
    internal sealed class GitRepository : IGitRepository
    {
        public static GitRepository FromRepository(IRepository repository) =>
          new GitRepository(
              repository.Branches.Select(
                  branch => new GitBranch(
                      branch.IsRemote,
                      branch.RemoteName,
                      branch.FriendlyName,
                      branch.IsCurrentRepositoryHead,
                      branch.Commits.Select(
                          commit => new GitCommit(commit.Sha)).ToList())).ToList(),
              repository.Commits.OrderTopological().Select(
                  commit => new GitCommit(commit.Sha)).ToList(),
              repository.Tags.Select(
                  tag => new GitTag(
                      tag.FriendlyName,
                      tag.PeeledTarget.Sha)).ToList());

        public GitRepository(
            IReadOnlyCollection<GitBranch> branches,
            IReadOnlyCollection<GitCommit> commits,
            IReadOnlyCollection<GitTag> tags)
        {
            Branches = branches;
            Master = OriginMasterOrMasterCommits(branches);
            Develop = OriginDevelopOrDevelopCommits(branches);
            CurrentBranch = new ListOfCommits(commits);
            Tags = tags;
        }

        public IReadOnlyCollection<GitBranch> Branches { get; }

        public GitBranch Master { get; }

        public GitBranch Develop { get; }

        public ListOfCommits CurrentBranch { get; }

        public IReadOnlyCollection<GitTag> Tags { get; }

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
            return develop;
        }

        private static GitBranch OriginMasterOrMasterCommits(IEnumerable<GitBranch> branches)
        {
            var develop = branches.FirstOrDefault(IsOriginMaster);
            if (develop == null)
            {
                // ReSharper disable once PossibleMultipleEnumeration
                develop = branches.FirstOrDefault(IsMaster);
                if (develop == null)
                {
                    throw new InvalidOperationException(
                        $"Git repository does not contain a branch named '{Constants.MasterBranchName}' or '{Constants.OriginMaster}'.");
                }
            }
            return develop;
        }

        private static bool IsMaster(GitBranch branch) =>
            (branch.FriendlyName.Equals(Constants.MasterBranchName) ||
            branch.FriendlyName.Equals(Constants.MainBranchName));

        private static bool IsOriginMaster(GitBranch branch) =>
            branch.IsRemote &&
            (branch.FriendlyName.Equals(Constants.OriginMaster) ||
            branch.FriendlyName.Equals(Constants.OriginMain));

        private static bool IsDevelop(GitBranch branch) =>
            branch.FriendlyName.Equals(Constants.DevelopBranchName);

        private static bool IsOriginDevelop(GitBranch branch) =>
            branch.IsRemote &&
            branch.FriendlyName.Equals(Constants.OriginDevelop);
    }
}
