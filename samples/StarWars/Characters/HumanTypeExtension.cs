using HotChocolate.Extensions.Translation;
using HotChocolate.Types;
using HotChocolate;
using System.Collections.Generic;
using StarWars.Repositories;
using System.Linq;

namespace StarWars.Characters;

[ExtendObjectType(typeof(Human))]
public class HumanTypeExtension
{
    [UsePaging(typeof(InterfaceType<ICharacter>))]
    [BindMember(nameof(Human.Friends))]
    public IEnumerable<ICharacter> GetFriends(
        [Parent] Human character,
        [Service] ICharacterRepository repository) =>
        repository.GetCharacters(character.Friends.ToArray());

    [Translate<HairColor>("HairColors", nullable: true)]
    public HairColor? GetHairColor2([Parent] Human human)
    {
        return human.HairColor;
    }

    [TranslateArray<Episode>(resourceKeyPrefix: "Episodes")]
    public IReadOnlyList<Episode> GetAppearsIn(
        [Parent] Human character) => character.AppearsIn;

    [TranslateArray<MaritalStatus>(resourceKeyPrefix: "MaritalStatus")]
    public MaritalStatus GetMaritalStatus(
    [Parent] Human character) => character.MaritalStatus;
}
