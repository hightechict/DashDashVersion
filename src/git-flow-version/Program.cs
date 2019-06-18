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
using System.Collections.Generic;
using DashDashVersion;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GitFlowVersion
{
    internal class Program
    {
        // ReSharper disable once UnusedParameter.Local
        private static int Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder();
                builder.AddCommandLine(
                    args, 
                    new Dictionary<string, string> {{"-b", "branch"}});
                var configuration = builder.Build();
                var toWrite = GenerateJson(VersionNumberGenerator.GenerateVersionNumber(Environment.CurrentDirectory, configuration["branch"]));
                Console.WriteLine(toWrite);
                return 0;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return -1;
            }
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
    }
}