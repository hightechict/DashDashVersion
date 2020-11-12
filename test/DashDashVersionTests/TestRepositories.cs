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

using DashDashVersion;
using DashDashVersion.RepositoryAbstraction;
using System.Collections.Generic;

namespace DashDashVersionTests
{
    public static class TestRepositories
    {
        internal static GitRepository MasterRepository()
        {
            var commits = new List<GitCommit>
            {
                new GitCommit("a")
            };

            var branches = new List<GitBranch>
            {
                new GitBranch(false,"",Constants.MasterBranchName,true,commits),
                new GitBranch(false,"",Constants.DevelopBranchName,false,commits)

            };
            var tags = new List<GitTag>
            {
                new GitTag("1.0.0", "a")
            };

            return new GitRepository(branches, commits, tags, false);
        }
        
        internal static GitRepository MainRepository()
        {
            var commits = new List<GitCommit>
            {
                new GitCommit("a")
            };

            var branches = new List<GitBranch>
            {
                new GitBranch(false,"",Constants.MainBranchName,true,commits),
                new GitBranch(false,"",Constants.DevelopBranchName,false,commits)

            };
            var tags = new List<GitTag>
            {
                new GitTag("1.0.0", "a")
            };

            return new GitRepository(branches, commits, tags, false);
        }

        internal static GitRepository TwoCommitsOnDevelopRepository()
        {
            var masterCommits = new List<GitCommit>
            {
                new GitCommit("a")
            };
            var developCommits = new List<GitCommit>
            {
                new GitCommit("b"),
                new GitCommit("a")
            };
            var branches = new List<GitBranch>
            {
                new GitBranch(false,"",Constants.MasterBranchName,false,masterCommits),
                new GitBranch(false,"",Constants.DevelopBranchName,true,developCommits)
            };
            var tags = new List<GitTag>
            {
                new GitTag("0.0.0", "a")
            };

            return new GitRepository(branches, developCommits, tags, false);
        }

        internal static GitRepository FeatureDebugMergedRepository()
        {
            var masterCommits = new List<GitCommit>
            {
                new GitCommit("1")
            };
            var developCommits = new List<GitCommit>
            {
                new GitCommit("8"),
                new GitCommit("7"),
                new GitCommit("3"),
                new GitCommit("2"),
                new GitCommit("1")
            };
            var featureCommits = new List<GitCommit>
            {
                new GitCommit("12"),
                new GitCommit("11"),
                new GitCommit("10"),
                new GitCommit("9"),
                new GitCommit("8"),
                new GitCommit("7"),
                new GitCommit("6"),
                new GitCommit("5"),
                new GitCommit("4"),
                new GitCommit("3"),
                new GitCommit("2"),
                new GitCommit("1")
            };
            var branches = new List<GitBranch>
            {
                new GitBranch(false,"",Constants.MasterBranchName,false,masterCommits),
                new GitBranch(false,"",Constants.DevelopBranchName,false,developCommits),
                new GitBranch(false,"",$"{Constants.FeatureBranchName}{Constants.BranchNameInfoDelimiter}test",true,featureCommits)
            };
            var tags = new List<GitTag>
            {
                new GitTag("0.0.0", "1")
            };

            return new GitRepository(branches, featureCommits, tags, false);
        }

        internal static GitRepository TwoCommitsOnDevelopWithoutCoreVersionRepository()
        {
            var masterCommits = new List<GitCommit>
            {
                new GitCommit("a")
            };
            var developCommits = new List<GitCommit>
            {
                new GitCommit("b"),
                new GitCommit("a")
            };
            var branches = new List<GitBranch>
            {
                new GitBranch(false,"",Constants.MasterBranchName,false,masterCommits),
                new GitBranch(false,"",Constants.DevelopBranchName,true,developCommits)
            };

            return new GitRepository(branches, developCommits, new List<GitTag>(), false);
        }

        internal static GitRepository ReleaseBranchRepositoryWithoutTag()
        {
            var masterCommits = new List<GitCommit>
            {
                new GitCommit("a")
            };
            var developCommits = new List<GitCommit>
            {
                new GitCommit("b"),
                new GitCommit("a")
            };
            var releaseCommits = new List<GitCommit>
            {
                new GitCommit("c"),
                new GitCommit("b"),
                new GitCommit("a")
            };
            var branches = new List<GitBranch>
            {
                new GitBranch(false,"",Constants.MasterBranchName,false,masterCommits),
                new GitBranch(false,"",Constants.DevelopBranchName,false,developCommits),
                new GitBranch(false,"",$"{Constants.ReleaseBranchName}{Constants.BranchNameInfoDelimiter}1.0.0",true,releaseCommits)
            };
            var tags = new List<GitTag>
            {
                new GitTag("0.0.0", "a")
            };

            return new GitRepository(branches, releaseCommits, tags, false);
        }

        internal static GitRepository ReleaseBranchRepositoryWithTaggedRc()
        {
            var masterCommits = new List<GitCommit>
            {
                new GitCommit("a")
            };
            var developCommits = new List<GitCommit>
            {
                new GitCommit("b"),
                new GitCommit("a")
            };
            var releaseCommits = new List<GitCommit>
            {
                new GitCommit("e"),
                new GitCommit("d"),
                new GitCommit("c"),
                new GitCommit("b"),
                new GitCommit("a")
            };
            var branches = new List<GitBranch>
            {
                new GitBranch(false,"",Constants.MasterBranchName,false,masterCommits),
                new GitBranch(false,"",Constants.DevelopBranchName,false,developCommits),
                new GitBranch(false,"",$"{Constants.ReleaseBranchName}{Constants.BranchNameInfoDelimiter}1.0.0",true,releaseCommits)
            };
            var tags = new List<GitTag>
            {

                new GitTag("1.0.0-rc.2", "d"),
                new GitTag("1.0.0-rc.1", "c"),
                new GitTag("0.0.0", "a")
            };

            return new GitRepository(branches, releaseCommits, tags, false);
        }

        internal static GitRepository FeatureBranchOnFeatureBranchRepository()
        {
            var masterCommits = new List<GitCommit>
            {
                new GitCommit("a")
            };
            var developCommits = new List<GitCommit>
            {
                new GitCommit("e"),
                new GitCommit("b"),
                new GitCommit("a")
            };
            var featureACommits = new List<GitCommit>
            {
                new GitCommit("c"),
                new GitCommit("b"),
                new GitCommit("a")
            };
            var featureBCommits = new List<GitCommit>
            {
                new GitCommit("d"),
                new GitCommit("c"),
                new GitCommit("b"),
                new GitCommit("a")
            };
            var branches = new List<GitBranch>
            {
                new GitBranch(false,"",Constants.MasterBranchName,false,masterCommits),
                new GitBranch(false,"",Constants.DevelopBranchName,false,developCommits),
                new GitBranch(false,"",$"{Constants.FeatureBranchName}{Constants.BranchNameInfoDelimiter}A",false,featureACommits),
                new GitBranch(false,"",$"{Constants.FeatureBranchName}{Constants.BranchNameInfoDelimiter}B",true,featureBCommits)
            };
            var tags = new List<GitTag>
            {
                new GitTag("0.0.0", "a")
            };

            return new GitRepository(branches, featureBCommits, tags, false);
        }

        internal static GitRepository MasterAheadOfDevelopRepository()
        {
            var masterCommits = new List<GitCommit>
            {
                new GitCommit("a"),
                new GitCommit("c")
            };
            var developCommits = new List<GitCommit>
            {
                new GitCommit("b"),
                new GitCommit("a")
            };
            var branches = new List<GitBranch>
            {
                new GitBranch(false,"",Constants.MasterBranchName,false,masterCommits),
                new GitBranch(false,"",Constants.DevelopBranchName,true,developCommits)
            };
            var tags = new List<GitTag>
            {
                new GitTag("0.0.0", "a"),
                new GitTag("0.1.0", "c")
            };

            return new GitRepository(branches, developCommits, tags, false);
        }

        internal static GitRepository RemoteDevelopRepository()
        {
            var masterCommits = new List<GitCommit>
            {
                new GitCommit("a")
            };
            var developCommits = new List<GitCommit>
            {
                new GitCommit("b"),
                new GitCommit("a")
            };
            var branches = new List<GitBranch>
            {
                new GitBranch(false,"",Constants.MasterBranchName,false,masterCommits),
                new GitBranch(true,"",$"{Constants.DefaultRemoteName}{Constants.BranchNameInfoDelimiter}{Constants.DevelopBranchName}",true,developCommits)
            };
            var tags = new List<GitTag>
            {
                new GitTag("0.0.0", "a")
            };

            return new GitRepository(branches, developCommits, tags, false);
        }

        internal static GitRepository DeteachedHeadRepo()
        {
            var commits = new List<GitCommit>
            {
                new GitCommit("b"),
                new GitCommit("a")
            };
            var branches = new List<GitBranch>
            {
                new GitBranch(false,"",Constants.MasterBranchName,false,commits),
                new GitBranch(false,"",Constants.DevelopBranchName,false,commits),
                new GitBranch(false,"",Constants.FeatureBranchName +Constants.BranchNameInfoDelimiter+"Feature-develop",false,commits),
                new GitBranch(true,"",Constants.DefaultRemoteName+Constants.BranchNameInfoDelimiter+Constants.FeatureBranchName +Constants.BranchNameInfoDelimiter+"FeatureA",false,commits)
            };
            var tags = new List<GitTag>
            {
                new GitTag("0.1.0","b"),
                new GitTag("0.0.0", "a")
            };

            return new GitRepository(branches, commits, tags, false);
        }

        internal static GitRepository PatchReleaseRepository()
        {
            var masterCommits = new List<GitCommit>
            {
                new GitCommit("d"),
                new GitCommit("c"),
                new GitCommit("b"),
                new GitCommit("a")
            };
            var developCommits = new List<GitCommit>
            {
                new GitCommit("e"),
                new GitCommit("d"),
                new GitCommit("c"),
                new GitCommit("b"),
                new GitCommit("a")
            };
            var branches = new List<GitBranch>
            {
                new GitBranch(false,"",Constants.MasterBranchName,false,masterCommits),
                new GitBranch(true,"",$"{Constants.DefaultRemoteName}{Constants.BranchNameInfoDelimiter}{Constants.DevelopBranchName}",true,developCommits)
            };
            var tags = new List<GitTag>
            {
                new GitTag("0.0.0", "a"),
                new GitTag("0.1.0", "b"),
                new GitTag("0.1.1", "d"),
            };

            return new GitRepository(branches, developCommits, tags, false);
        }

        internal static GitRepository ServiceRepository()
        {
            var masterCommits = new List<GitCommit>
            {
                new GitCommit("d"),
                new GitCommit("c"),
                new GitCommit("b"),
                new GitCommit("a")
            };
            var supportCommits = new List<GitCommit>
            {
                new GitCommit("e"),
                new GitCommit("c"),
                new GitCommit("b"),
                new GitCommit("a")
            };
            var branches = new List<GitBranch>
            {
                new GitBranch(false,"",Constants.MasterBranchName,false,masterCommits),
                new GitBranch(false,"",Constants.DevelopBranchName,false,masterCommits),
                new GitBranch(true,"",$"{Constants.DefaultRemoteName}{Constants.BranchNameInfoDelimiter}{Constants.SupportBranchName}{Constants.BranchNameInfoDelimiter}1.1.0",true,supportCommits)
            };
            var tags = new List<GitTag>
            {
                new GitTag("0.0.0", "a"),
                new GitTag("1.0.0", "b"),
                new GitTag("2.0.0", "d"),
                new GitTag("1.5.0", "e")
            };

            return new GitRepository(branches, supportCommits, tags, false);
        }

        internal static GitRepository BugfixRepository()
        {
            var masterCommits = new List<GitCommit>
            {
                new GitCommit("a")
            };
            var developCommits = new List<GitCommit>
            {
                new GitCommit("b"),
                new GitCommit("a")
            };
            var bugfixCommits = new List<GitCommit>
            {
                new GitCommit("d"),
                new GitCommit("b"),
                new GitCommit("a")
            };
            var branches = new List<GitBranch>
            {
                new GitBranch(false,"",Constants.MasterBranchName,false,masterCommits),
                new GitBranch(false,"",Constants.DevelopBranchName,false,developCommits),
                new GitBranch(false,"",$"{Constants.BugFixBranchName}{Constants.BranchNameInfoDelimiter}test",true,bugfixCommits)
            };
            var tags = new List<GitTag>
            {
                new GitTag("0.0.0", "a")
            };

            return new GitRepository(branches, bugfixCommits, tags, false);
        }
        
        internal static GitRepository RemoteBugfixRepository()
        {
            var masterCommits = new List<GitCommit>
            {
                new GitCommit("a")
            };
            var developCommits = new List<GitCommit>
            {
                new GitCommit("b"),
                new GitCommit("a")
            };
            var bugfixCommits = new List<GitCommit>
            {
                new GitCommit("d"),
                new GitCommit("b"),
                new GitCommit("a")
            };
            var branches = new List<GitBranch>
            {
                new GitBranch(false,"",Constants.MasterBranchName,false,masterCommits),
                new GitBranch(false,"",Constants.DevelopBranchName,false,developCommits),
                new GitBranch(true,"",$"{Constants.DefaultRemoteName}{Constants.BranchNameInfoDelimiter}{Constants.BugFixBranchName}{Constants.BranchNameInfoDelimiter}test",false,bugfixCommits)
            };
            var tags = new List<GitTag>
            {
                new GitTag("0.0.0", "a")
            };

            return new GitRepository(branches, bugfixCommits, tags, false);
        }
        internal static GitRepository MasterOutOfSyncRepository()
        {
            var masterCommits = new List<GitCommit>
            {
                new GitCommit("b"),
                new GitCommit("a")
            };
            var originMaster = new List<GitCommit>
            {
                new GitCommit("a")
            };
            var branches = new List<GitBranch>
            {
                new GitBranch(false,"",Constants.MasterBranchName,false,masterCommits),
                new GitBranch(true,"",Constants.OriginMaster,false,originMaster),
            };
            var tags = new List<GitTag>
            {
                new GitTag("0.0.0", "a")
            };

            return new GitRepository(branches, masterCommits, tags, false);
        }
        internal static GitRepository DevelopOutOfSyncRepository()
        {
            var masterCommits = new List<GitCommit>
            {
                new GitCommit("a")
            };
            var develop = new List<GitCommit>
            {
                new GitCommit("b"),
                new GitCommit("a")
            };
            var branches = new List<GitBranch>
            {
                new GitBranch(false,"",Constants.MasterBranchName,false,masterCommits),
                new GitBranch(true,"",Constants.OriginDevelop,false,masterCommits),
                new GitBranch(false,"",Constants.OriginMaster,false,develop),
            };
            var tags = new List<GitTag>
            {
                new GitTag("0.0.0", "a")
            };

            return new GitRepository(branches, masterCommits, tags, false);
        }
    }
}
