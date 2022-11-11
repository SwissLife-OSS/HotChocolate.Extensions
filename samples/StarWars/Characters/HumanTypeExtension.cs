using HotChocolate.Extensions.Translation;
using HotChocolate.Types;
using HotChocolate;

namespace StarWars.Characters
{
    [ExtendObjectType(typeof(Human))]
    public class HumanTypeExtension
    {
        [Translate<HairColor>("HairColors", nullable: true)]
        public HairColor? GetHairColor2([Parent] Human human)
        {
            return human.HairColor;
        }
    }
}
