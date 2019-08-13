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
using System.Text.RegularExpressions;

namespace DashDashVersion
{
    /// <summary>
    /// This class is used to represent a single [Semantic Versioning 2.0.0](https://semver.org) version number.
    /// </summary>
    public class VersionNumber : IComparable<VersionNumber>
    {
        /// <summary>
        /// Parse a semver 2.0.0 compliant string to a VersionNumber instance.
        /// </summary>
        public static VersionNumber Parse(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentException("A null or white-space only version string can not be parsed.", nameof(version));
            }

            var matches = Patterns.ValidVersionNumber.Match(version);
            if (!matches.Success)
            {
                throw new ArgumentException($"{version} does not match our expectations: {Patterns.ValidVersionNumber}", nameof(version));
            }
            var major = uint.Parse(matches.Groups["Major"].Captures[0].Value);
            var minor = uint.Parse(matches.Groups["Minor"].Captures[0].Value);
            var patch = uint.Parse(matches.Groups["Patch"].Captures[0].Value);
            var preReleaseLabel = SplitPreReleaseLabel(
                matches.Groups["PreReleaseLabelBase"].Captures,
                matches.Groups["PreReleaseLabelFeature"].Captures);
            var buildMetadata = matches.Groups["BuildMetadata"].Captures;
            var metadata = buildMetadata.Count > 0 ? buildMetadata[0].Value : string.Empty;
            var debug = matches.Groups["CoreDebugLabel"].Success || matches.Groups["PreDebugLabel"].Success;
            
            if(matches.Groups["CoreDebugLabel"].Success)
            {
                major = uint.Parse(matches.Groups["Major2"].Captures[0].Value);
                minor = uint.Parse(matches.Groups["Minor2"].Captures[0].Value);
                patch = uint.Parse(matches.Groups["Patch2"].Captures[0].Value);
            }
    
            return new VersionNumber(
                major,
                minor,
                patch,
                preReleaseLabel,
                metadata,
                debug);
        }

        public VersionNumber(
            uint major,
            uint minor,
            uint patch,
            PreReleaseLabel? preReleaseTag = null,
            string metadata = "",
            bool debugVersion = false)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            PreReleaseLabel = preReleaseTag;
            Metadata = metadata;
            DebugVersion = debugVersion;
        }

        /// <summary>
        /// The major version number.
        /// </summary>
        public uint Major { get; }

        /// <summary>
        /// The minor version number.
        /// </summary>
        public uint Minor { get; }

        /// <summary>
        /// The patch number.
        /// </summary>
        public uint Patch { get; }

        /// <summary>
        /// The (optional) pre-release label.
        /// </summary>
        public PreReleaseLabel? PreReleaseLabel { get; }

        /// <summary>
        /// This property contains the possibly empty build metadata.
        /// </summary>
        public string Metadata { get; }

        /// <summary>
        /// This method determines the ordering of VersionNumber instances.
        /// </summary>
        /// <see cref="IComparable{T}.CompareTo"/>
        public int CompareTo(VersionNumber other)
        {
            var compare = Major.CompareTo(other.Major);
            if (compare != 0) return compare;

            compare = Minor.CompareTo(other.Minor);
            if (compare != 0) return compare;

            compare = Patch.CompareTo(other.Patch);
            if (compare != 0) return compare;

            if (PreReleaseLabel == null && other.PreReleaseLabel == null)
            {
                return 0;
            }

            if (PreReleaseLabel != null && other.PreReleaseLabel == null)
            {
                return -1;
            }

            if (PreReleaseLabel == null && other.PreReleaseLabel != null)
            {
                return 1;
            }

            if (PreReleaseLabel != null && other.PreReleaseLabel != null)
            {
                return PreReleaseLabel.CompareTo(other.PreReleaseLabel);
            }

            return 0;
        }

        /// <summary>
        /// A string representation of this VersionNumber that can be used as the constructor parameter of the [AssemblyVersionAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.assemblyversionattribute?view=netstandard-2.0).
        /// </summary>
        /// <see cref="System.Reflection.AssemblyVersionAttribute"/>
        public string AssemblyVersion => $"{Major}{Constants.ParticleDelimiter}{Minor}{Constants.ParticleDelimiter}{Patch}{Constants.ParticleDelimiter}{PreReleaseLabel?.CalculatedRevision ?? 0}";

        /// <returns>The full semantic version string of the version number including a pre-release label (if present) and the build meta data.</returns>
        public override string ToString() => FullSemVer;

        /// <summary>
        /// This property conveys whether the debug flag should be added to the version number.
        /// </summary>
        public bool DebugVersion{ get; }

        /// <summary>
        /// The full semantic version string of the version number including a pre-release label (if present) and the build meta data.
        /// </summary>
        public string FullSemVer
        {
            get
            {
                var toReturn = SemVer;
                if (!string.IsNullOrEmpty(Metadata))
                    toReturn = $"{toReturn}{Constants.BuildMetadataDelimiter}{Metadata}";
                return toReturn;
            }
        }

        /// <summary>
        /// The semantic version string of the version number including a pre-release label (if present), but without the build meta data.
        /// </summary>
        public string SemVer
        {
            get
            {
                var toReturn = $"{Major}{Constants.ParticleDelimiter}{Minor}{Constants.ParticleDelimiter}";
                if (PreReleaseLabel != null)
                {
                    toReturn = $"{toReturn}{Patch}{Constants.PreReleaseLabelDelimiter}{PreReleaseLabel}";
                    if(DebugVersion)
                    {
                        toReturn = $"{toReturn}{Constants.ParticleDelimiter}{Constants.DebugPreReleaseLabel}";
                    }
                }
                else if(DebugVersion)
                {
                    toReturn = $"{toReturn}{Patch+1}{Constants.PreReleaseLabelDelimiter}{Constants.DebugPreReleaseLabel}{Constants.ParticleDelimiter}{toReturn}{Patch}";
                }
                else
                {
                    toReturn = $"{toReturn}{Patch}";
                }
                return toReturn;
            }
        }

        private static PreReleaseLabel? SplitPreReleaseLabel(
            CaptureCollection baseLabel,
            CaptureCollection featureLabel)
        {
            if (baseLabel.Count == 0)
            {
                return null;
            }
            var baseParticle = SplitLabel(baseLabel[0].Value);
            return featureLabel.Count > 0 ?
                new FeaturePreReleaseLabel(baseParticle, SplitLabel(featureLabel[0].Value)) :
                new PreReleaseLabel(baseParticle);
        }

        private static PreReleaseLabelParticle SplitLabel(string label)
        {
            var splitString = label.Split(Constants.ParticleDelimiter);
            var preReleaseIndicator = splitString[0];
            var ordinal = uint.Parse(splitString[1]);
            return new PreReleaseLabelParticle(preReleaseIndicator, ordinal);
        }
    }
}
