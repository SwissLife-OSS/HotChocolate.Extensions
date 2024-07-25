using System.Collections.Generic;
using HotChocolate.Extensions.Translation;
using HotChocolate.Types;

namespace StarWars.Characters
{
    /// <summary>
    /// A character in the Star Wars universe.
    /// </summary>
    [InterfaceType("Character")]
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
        /// The episodes the character appears in.
        /// </summary>
        [TranslateArray<Episode>("Episodes")]
        IReadOnlyList<Episode> AppearsIn { get; }

        /// <summary>
        /// The height of the character.
        /// </summary>
        double Height { get; }
    }
}
