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
    /// All constants that are regularly used in the program.
    /// </summary>
    internal static class Constants
    {
        internal const int BuildMetadataHashLength = 7;

        internal const char ParticleDelimiter = '.';
        internal const char PreReleaseLabelDelimiter = '-';
        internal const char BuildMetadataDelimiter = '+';
        internal const char BranchNameInfoDelimiter = '/';

        internal const string DevelopPreReleaseLabel = "dev";
        internal const string ReleasePreReleaseLabel = "rc";
        internal const string DebugPreReleaseLabel = "debug";

        internal const string DevelopBranchName = "develop";
        internal const string ReleaseBranchName = "release";
        internal const string HotfixBranchName = "hotfix";
        internal const string BugFixBranchName = "bugfix";
        internal const string FeatureBranchName = "feature";
        internal const string MasterBranchName = "master";
        internal const string MainBranchName = "main";
        internal const string SupportBranchName = "support";

        internal const string DefaultRemoteName = "origin";

        internal static string OriginDevelop => $"{DefaultRemoteName}{BranchNameInfoDelimiter}{DevelopBranchName}";

        internal static string OriginMaster => $"{DefaultRemoteName}{BranchNameInfoDelimiter}{MasterBranchName}";
        internal static string OriginMain => $"{DefaultRemoteName}{BranchNameInfoDelimiter}{MainBranchName}";

        internal static readonly string[] GitFlowBranchTypes =
        {
            DevelopBranchName,
            ReleaseBranchName,
            HotfixBranchName,
            FeatureBranchName,
            MasterBranchName,
            SupportBranchName,
            BugFixBranchName
        };
    }
}
