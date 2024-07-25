using System.Collections.Generic;
using System.Linq;
using HotChocolate;
using HotChocolate.Extensions.Translation;
using HotChocolate.Types;
using StarWars.Repositories;

namespace StarWars.Characters;

[ExtendObjectType<Droid>]
public class DroidTypeExtension
{
    [UsePaging(typeof(InterfaceType<ICharacter>))]
    [BindMember(nameof(Droid.Friends))]
    public IEnumerable<ICharacter> GetFriends(
        [Parent] Droid character,
        [Service] ICharacterRepository repository) =>
        repository.GetCharacters(character.Friends.ToArray());

    [TranslateArray<Episode>("Episodes")]
    public IReadOnlyList<Episode> GetAppearsIn(
        [Parent] Droid character) => character.AppearsIn;
}
