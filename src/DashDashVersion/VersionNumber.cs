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
    /// This class is used to represent a single version number.
    /// </summary>
    public class VersionNumber : IComparable<VersionNumber>
    {
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
            return new VersionNumber(
                major,
                minor,
                patch,
                preReleaseLabel,
                metadata);
        }

        internal VersionNumber(
            uint major,
            uint minor,
            uint patch,
            PreReleaseLabel? preReleaseTag = null,
            string metadata = "")
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            PreReleaseLabel = preReleaseTag;
            Metadata = metadata;
        }

        public uint Major { get; }

        public uint Minor { get; }

        public uint Patch { get; }

        public PreReleaseLabel? PreReleaseLabel { get; }

        public string Metadata { get; }

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

        public string AssemblyVersion => $"{Major}{Constants.ParticleDelimiter}{Minor}{Constants.ParticleDelimiter}{Patch}{Constants.ParticleDelimiter}{PreReleaseLabel?.CalculatedRevision ?? 0}";

        public override string ToString() => FullSemVer;

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

        public string SemVer
        {
            get
            {
                var toReturn = $"{Major}{Constants.ParticleDelimiter}{Minor}{Constants.ParticleDelimiter}{Patch}";
                if (PreReleaseLabel != null)
                    toReturn = $"{toReturn}{Constants.PreReleaseLabelDelimiter}{PreReleaseLabel}";
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