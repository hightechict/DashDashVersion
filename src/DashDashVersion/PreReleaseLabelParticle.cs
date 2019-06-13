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
    /// This is the base class for a pre-release label that checks if the label is SemVer 2.0.0 compliant and requires a number to indicate the specific pre-release version.  
    /// </summary>
    public class PreReleaseLabelParticle
    {
        public PreReleaseLabelParticle(string label, uint ordinal)
        {
            if (!IsSemverValid(label))
            {
                throw new ArgumentException($"Label: {label} is not valid, {Patterns.SemverAllowedPreReleaseLabelCharacters}", nameof(label));
            }
            Label = label;
            Ordinal = ordinal;
        }

        public uint Ordinal { get; }

        public string Label { get; }

        private static bool IsSemverValid(string label) =>
            Patterns.SemverAllowedPreReleaseLabelCharacters.IsMatch(label);

        public override string ToString() =>
            $"{Label}{Constants.ParticleDelimiter}{Ordinal}";
    }
}