﻿using DashDashVersion;
using DashDashVersion.RepositoryAbstraction;
using System.Collections.Generic;

namespace DashDashVersionTests
{
    public static class TestRepositories
    {
        internal static GitRepository MasterOnlyRepository()
        {
            var commits = new List<GitCommit>
            {
                new GitCommit("a")
            };

            var branches = new List<GitBranch>
            {
                new GitBranch(false,"",Constants.MasterBranchName,true,commits)
            };
            var tags = new List<GitTag>
            {
                new GitTag("1.0.0", "a")
            };

            return new GitRepository(branches, commits, tags);
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

            return new GitRepository(branches, developCommits, tags);
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

            return new GitRepository(branches, releaseCommits, tags);
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

            return new GitRepository(branches, releaseCommits, tags);
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

            return new GitRepository(branches, featureBCommits, tags);
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

            return new GitRepository(branches, developCommits, tags);
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
                new GitBranch(true,"","origin/"+Constants.DevelopBranchName,true,developCommits)
            };
            var tags = new List<GitTag>
            {
                new GitTag("0.0.0", "a")
            };

            return new GitRepository(branches, developCommits, tags);
        }
    }
}