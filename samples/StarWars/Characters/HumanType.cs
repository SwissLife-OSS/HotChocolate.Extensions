using HotChocolate;
using HotChocolate.Extensions.Translation;
using HotChocolate.Types;

namespace StarWars.Characters
{
    public class HumanType: ObjectType<Human>
    {
        protected override void Configure(IObjectTypeDescriptor<Human> descriptor)
        {
            descriptor.Field(h => h.HairColor).Translate<HairColor>("HairColors", nullable: true);
        }
    }

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
