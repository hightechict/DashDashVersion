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

namespace DashDashVersion
{
    /// <summary>
    /// This class determines what type of `BranchInfo` needs to be created and if its a valid git flow branch.
    /// If it is not a valid git flow branch it will throw an exception.
    /// </summary>
    internal static class BranchInfoFactory
    {
        public static BranchInfo CreateBranchInfo(string name)
        {
            if (name == Constants.DevelopBranchName)
            {
                return new DevelopBranchInfo(name);
            }

            if (name.Equals(Constants.MasterBranchName) || name.StartsWith(Constants.SupportBranchName))
            {
                return new MasterBranchInfo(name);
            }

            if (IsSingleSuffixBranch(name))
            {
                var versionNumber = Patterns.ContainsVersionNumber.Match(name);
                if (versionNumber.Success)
                {
                    if (name.StartsWith(Constants.ReleaseBranchName) || name.StartsWith(Constants.HotfixBranchName))
                    {
                        return new ReleaseCandidateBranchInfo(name, versionNumber.Groups["BaseVersion"].Value);
                    }
                    throw new ArgumentException($"This branch : {name} is not one of the supported release candidate branches, only '{Constants.ReleaseBranchName}' and '{Constants.HotfixBranchName}' are supported.", nameof(name));
                }
                if (name.StartsWith(Constants.FeatureBranchName))
                {
                    return new FeatureBranchInfo(name, name.Split(Constants.BranchNameInfoDelimiter)[1]);
                }
                throw new ArgumentException($"This branch : {name} is a 'feature' branch, only feature branches of the format '{Constants.FeatureBranchName}/<name>' are supported.", nameof(name));
            }


            throw new ArgumentException($"This branch : {name} is not any of the supported branch types, only [{string.Join(", ", Constants.GitFlowBranchTypes)}] branches are supported.", nameof(name));
        }

        private static bool IsSingleSuffixBranch(string name) =>
            name.Split(
                    new[] { Constants.BranchNameInfoDelimiter },
                    StringSplitOptions.RemoveEmptyEntries)
                .Length == 2;
    }
}

