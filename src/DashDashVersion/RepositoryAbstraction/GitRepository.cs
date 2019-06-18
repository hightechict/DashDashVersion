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
                    b => new GitBranch(
                        b.IsRemote,
                        b.RemoteName,
                        b.FriendlyName,
                        b.IsCurrentRepositoryHead,
                        b.Commits.Select(
                            c => new GitCommit(c.Sha)))),
                repository.Commits.Select(
                    c => new GitCommit(c.Sha)),
                repository.Tags.Select(
                    t => new GitTag(
                        t.FriendlyName,
                        t.PeeledTarget.Sha)));

        public GitRepository(
            IEnumerable<GitBranch> branches,
            IEnumerable<GitCommit> commits,
            IEnumerable<GitTag> tags)
        {
            Branches = branches;
            Commits = commits;
            Tags = tags;
        }

        public IEnumerable<GitBranch> Branches { get; }

        public IEnumerable<GitCommit> Commits { get; }

        public IEnumerable<GitTag> Tags { get; }

    }
}