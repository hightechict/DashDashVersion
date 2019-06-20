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
using System.Reflection;
using DashDashVersion;
using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace GitFlowVersion
{
    internal class Program
    {
        // ReSharper disable once UnusedParameter.Local
        private static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "git-flow-version"
            };
            app.HelpOption("-?|-h|--help");

            var optionBranch = app.Option("-b|--branch", "Manualy tell what branch to use, this can be the full branch name or a git path like 'refs/heads/master'", CommandOptionType.SingleValue);
            var optionVersion = app.Option("--version", "Returns the currently installed version of git-flow-version", CommandOptionType.NoValue);
            app.OnExecute(() =>
            {
                if(optionVersion.Value() == "on")
                {
                    WriteGitFlowVersion();
                    return 0;
                }
                try
                {
                    Console.WriteLine(GenerateJson(VersionNumberGenerator.GenerateVersionNumber(Environment.CurrentDirectory, optionBranch.Value())));
                    return 0;
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                    return -1;
                }
            });

            app.Execute(args);
            return -1;
        }

        private static string GenerateJson(VersionNumber version)
        {
            var toReturn = new
            {
                version.FullSemVer,
                version.SemVer,
                version.AssemblyVersion
            };

            return JsonConvert.SerializeObject(toReturn, Formatting.Indented);
        }

        private static void WriteGitFlowVersion()
        {
            var version = Attribute
               .GetCustomAttribute(
                   Assembly.GetEntryAssembly(),
                   typeof(AssemblyInformationalVersionAttribute))
               as AssemblyInformationalVersionAttribute;
            Console.WriteLine(version?.InformationalVersion ?? "Unkown Version");
        }
    }
}