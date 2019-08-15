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

using LibGit2Sharp;

namespace DashDashVersion.RepositoryAbstraction
{
    /// <summary>
    /// This class adds implements a sorting option so the current branch can be sorted by the commits child parent relationship.
    /// This is to prevent unpredictable ordering in the commit list which can happen if you order by time.
    /// </summary>
    internal static class QueryableCommitLogExtensions
    {
        public static ICommitLog OrderTopological(this IQueryableCommitLog commits) =>
            commits.QueryBy(
                new CommitFilter
                {
                    SortBy = CommitSortStrategies.Topological
                });
    }
}
