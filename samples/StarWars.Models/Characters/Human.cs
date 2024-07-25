using System;
using System.Collections.Generic;

namespace StarWars.Characters
{
    /// <summary>
    /// A human character in the Star Wars universe.
    /// </summary>
    public class Human : ICharacter
    {
        public Human(
            int id,
            string name,
            IReadOnlyList<int> friends,
            IReadOnlyList<Episode> appearsIn,
            HairColor? hairColor,
            MaritalStatus maritalStatus,
            string? homePlanet = null,
            double height = 1.72d)
        {
            Id = id;
            Name = name;
            Friends = friends;
            AppearsIn = appearsIn;
            HairColor = hairColor;
            MaritalStatus = maritalStatus;
            HomePlanet = homePlanet;
            Height = height;
        }

        /// <inheritdoc />
        public int Id { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IReadOnlyList<int> Friends { get; }

        /// <inheritdoc />
        public IReadOnlyList<Episode> AppearsIn { get; }

        /// <summary>
        /// The planet the character is originally from.
        /// </summary>
        public string? HomePlanet { get; }

        /// <summary>
        /// Color of the Human's hair
        /// </summary>
        public HairColor? HairColor { get; }

        /// <summary>
        /// the Humans's marital status
        /// </summary>
        public MaritalStatus MaritalStatus { get; }

        /// <inheritdoc />
        [Obsolete(message: "invocations of this field will be tracked!")]
        public double Height { get; }
    }
}
