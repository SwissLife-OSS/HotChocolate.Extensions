using System.Collections.Generic;
using System.Linq;
using HotChocolate;
using HotChocolate.Types;
using StarWars.Repositories;

namespace StarWars.Characters
{
    [ExtendObjectType<Human>]
    public class HumanResolvers
    {
        [UsePaging(typeof(InterfaceType<ICharacter>))]
        [BindMember(nameof(Human.Friends))]
        public IEnumerable<ICharacter> GetFriends(
            [Parent] Human character,
            [Service] ICharacterRepository repository) =>
            repository.GetCharacters(character.Friends.ToArray());
    }

    [ExtendObjectType<Droid>]
    public class DroidResolvers
    {
        [UsePaging(typeof(InterfaceType<ICharacter>))]
        [BindMember(nameof(Droid.Friends))]
        public IEnumerable<ICharacter> GetFriends(
            [Parent] Droid character,
            [Service] ICharacterRepository repository) =>
            repository.GetCharacters(character.Friends.ToArray());
    }
}
