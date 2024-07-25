using System.Collections.Generic;

namespace StarWars.Characters
{
    /// <summary>
    /// A character in the Star Wars universe.
    /// </summary>
    public interface ICharacter : ISearchResult
    {
        /// <summary>
        /// The unique identifier for the character.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of the character.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The height of the character.
        /// </summary>
        double Height { get; }
    }
}
