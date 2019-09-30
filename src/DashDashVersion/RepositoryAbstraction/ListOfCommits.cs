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

namespace DashDashVersion.RepositoryAbstraction
{
    internal class ListOfCommits
    {
        private readonly HashSet<string> _hashset;

        public ListOfCommits(IEnumerable<GitCommit> source)
        {
            Commits = source.ToList();
            _hashset = new HashSet<string>(Commits.Select(gitCommit => gitCommit.Sha));
        }

        public IReadOnlyCollection<GitCommit> Commits { get; }

        public GitCommit Head => Commits.First();

        public GitCommit First => Commits.Last();

        public bool Contains(string sha) => _hashset.Contains(sha);

        public bool Overlaps(ListOfCommits commits) => _hashset.Overlaps(commits._hashset);

        public IEnumerable<string> Except(ListOfCommits commits) => _hashset.Except(commits._hashset);

        public IEnumerable<GitCommit> IntersectWith(ListOfCommits commits)
        {
            var result = new HashSet<string>(_hashset);
            result.IntersectWith(commits._hashset);

            return Commits.Where(commit => commits.Contains(commit.Sha));
        }
    }
}
