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

namespace DashDashVersion
{
    /// <summary>
    /// This class represents branches of the 'develop' type.
    /// </summary>
    internal class DevelopBranchInfo : BranchInfo
    {
        internal DevelopBranchInfo(string name) : base(name)
        {
        }

        internal PreReleaseLabel DeterminePreReleaseLabel(uint mainOrdinal) =>
            new PreReleaseLabel(
                new PreReleaseLabelParticle(
                    Constants.DevelopPreReleaseLabel,
                    mainOrdinal));
    }
}