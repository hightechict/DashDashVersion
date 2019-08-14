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

namespace DashDashVersion.RepositoryAbstraction
{
    /// <summary>
    /// This interface is a stand-in for the LibGit2Sharp `Repository` type, used to confine the LibGit2Sharp dependency.
    /// </summary>
    internal interface IGitRepository
    {
        IReadOnlyCollection<GitBranch> Branches { get; }

        GitBranch Master { get; }

        GitBranch Develop { get; }

        ListOfCommits CurrentBranch { get; }

        IReadOnlyCollection<GitTag> Tags { get; }
    }
}
