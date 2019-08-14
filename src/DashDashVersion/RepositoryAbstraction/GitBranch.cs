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

namespace DashDashVersion.RepositoryAbstraction
{
    /// <summary>
    /// This class is a stand-in for the LibGit2Sharp `Branch` type, used to confine the LibGit2Sharp dependency.
    /// </summary>
    internal sealed class GitBranch
    {
        private readonly Lazy<ListOfCommits> _commits;

        public GitBranch(
            bool isRemote,
            string remoteName,
            string friendlyName,
            bool isCurrentRepositoryHead,
            List<GitCommit> commits)
        {
            if (commits.Count == 0)
            {
                throw new InvalidOperationException($"Branch {friendlyName}, does not contain any commits.");
            }
            IsRemote = isRemote;
            RemoteName = remoteName;
            FriendlyName = friendlyName;
            IsCurrentRepositoryHead = isCurrentRepositoryHead;
            _commits = new Lazy<ListOfCommits>(() => new ListOfCommits(commits));
            Head = commits[0];
        }

        public bool IsRemote { get; }

        public string RemoteName { get; }

        public string FriendlyName { get; }

        public bool IsCurrentRepositoryHead { get; }

        public GitCommit Head { get; }

        public ListOfCommits Commits => _commits.Value;

        public override string ToString() => $"branch: {FriendlyName}";
    }
}
