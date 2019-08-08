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
        private static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "git-flow-version"
            };
            app.HelpOption("-?|-h|--help");

            var optionBranch = app.Option("-b|--branch", "Manually set the branch to use, for determining the branch 'type' and pre-release label. This can be a full or partial name.", CommandOptionType.SingleValue);
            var optionVersion = app.Option("-v|--version", "Returns the currently installed version of git-flow-version.", CommandOptionType.NoValue);
            var optionDebug = app.Option("-d|--debug", "Adds a debug prerelease lable to the version number.", CommandOptionType.NoValue);

            app.OnExecute(() =>
            {
                if (optionVersion.Value() == "on")
                {
                    WriteGitFlowVersion();
                    return 0;
                }
                try
                {
                    OutputJsonToConsole(VersionNumberGenerator.GenerateVersionNumber(Environment.CurrentDirectory, optionBranch.Value()),optionDebug.Value() == "on");
                    return 0;
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                    return -1;
                }
            });
            return app.Execute(args);
        }

        private static void OutputJsonToConsole(VersionNumber version, bool debugVersion)
        {
            version.debugVersion = debugVersion;
            var writer = new JsonTextWriter(Console.Out) { Formatting = Formatting.Indented };

            writer.WriteStartObject();

            writer.WritePropertyName(nameof(version.AssemblyVersion));
            writer.WriteValue(version.AssemblyVersion);

            writer.WritePropertyName(nameof(version.FullSemVer));
            writer.WriteValue(version.FullSemVer);

            writer.WritePropertyName(nameof(version.SemVer));
            writer.WriteValue(version.SemVer);

            writer.WriteEndObject();
        }

        private static void WriteGitFlowVersion()
        {
            var version = Attribute
               .GetCustomAttribute(
                   Assembly.GetEntryAssembly(),
                   typeof(AssemblyInformationalVersionAttribute))
               as AssemblyInformationalVersionAttribute;
            Console.WriteLine(version?.InformationalVersion ?? "Unknown Version");
        }
    }
}
