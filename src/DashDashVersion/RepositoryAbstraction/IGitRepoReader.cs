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

namespace DashDashVersion.RepositoryAbstraction
{
    /// <summary>
    /// This interface provides the minimum information needed from a git repository in order to generate a version number.
    /// </summary>
    internal interface IGitRepoReader
    {
        BranchInfo CurrentBranch { get; }

        string HeadCommitHash { get; }

        VersionNumber CurrentVersionCore { get; }
        
        GitTag TagOnHead { get; }

        VersionNumber? HighestMatchingTagForReleaseCandidate { get; }

        uint CommitCountSinceLastMinorVersion { get; }

        uint CommitCountSinceBranchOffFromDevelop { get; }
    }
}
