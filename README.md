This repository contains features that can be added to your HotChocolate Server to enhance your HotChocolate experience.

- [HotChocolate.Extensions.Translations](#hotchocolateextensionstranslations)
  * [Install](#install)
  * [Translating fields](#translating-fields)
    + [Translation on demand](#translation-on-demand)
    + [Translation to a { key label } type](#translation-to-a---key-label---type)
    + [Array Translation to { key label } items](#array-translation-to---key-label---items)
  * [FAQ](#faq)
    + [What language is used by default by this extension?](#what-language-is-used-by-default-by-this-extension-)
    + [What is the performance impact of translations?](#what-is-the-performance-impact-of-translations-)
- [HotChocolate.Extensions.Tracking](#hotchocolateextensionstracking)
  * [Setting up the depencies and schema](#setting-up-the-depencies-and-schema)
  * [Default tracking](#default-tracking)
  * [Custom tracking](#custom-tracking)
- [Community](#community)


## HotChocolate.Extensions.Translations

This package uses HotChocolate directives and middlewares to add translations to your HotChocolate server. 
Easily extend your fields to transform their code-value (for instance "CH") into a more presentable value for your consumer ("Switzerland"/"Schweiz"/"Suisse"...). 

The resource strings containing the translations can be provided from any source you like: external microservices, from memory, resx files...

### Install

You will need to include the following package on your HotChocolate Server:

```bash
dotnet add package HotChocolate.Extensions.Translation
```

The next step is to register some directives to your GraphQL Schema:
```csharp
new ServiceCollection()
  .AddGraphQLServer()
  .SetSchema<MySchema>()
  .AddTranslation(
    /* add all translatable types explicitely, except String, which is already added implicitely. */
    c => c.AddTranslatableType<Country>()
          .AddTranslatableType<MyEnum2>()
  );
```

The last step is to implement and register an IResourcesProvider. This provider will be used to retrieve the string resources in the target language. 
```csharp
services.AddSingleton<IResourcesProvider, MyResourcesProvider>();
```

With this we have registered the necessary objects to support translation on our fields. Now we can start adding translation support to our fields.

### Translating fields

You can translate field values in several ways, listed below.

#### Translation on demand

A field can be made translatable. In this scenario, the field has two possible states: translated or not translated.
The consumer of the application decides whether he wants the translated state of the field or the non-translated state. 
He does so by adding, or not adding, the field directive ```@translate``` to the field in his query. 

```csharp
public class AddressType : ObjectType<Query>
{
    protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
    {
        descriptor
            .Field(c => c.Country) // the Country property is a Country enum
            .Translatable("Ref/Aex/Countries")
            .Type<NonNullType<StringType>>();
    }
}
```

Querying this field will produce the following results:
```graphql
{ 
  country // -> "CH"
  countryLabel: country @translate // -> "Switzerland" if your Thread language is english
  country_fr: country @translate(lang:"fr") // -> "Suisse"
}
```

#### Translation to a { key label } type

We can also rewrite our string/enum/other field to make it a `{ key label }` field.
This can be useful if we also use Array Translations, which use the same typing (see next chapter).

```csharp
public class AddressType : ObjectType<Query>
{
    protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
    {
        descriptor
            .Field(c => c.Country) // The Country property is a Country enum
            .Translate("Ref/Aex/Countries");
    }
}
```

Alternatively it is possible to translate the field via an attribute directly on the property:
```csharp
public class Address
{
    [Translate("Ref/Aex/Countries")]
    public Country Country { get; }
}
```

In both cases, this will generate the following Schema:
```graphql
type Query {
  country: TranslatedResourceOfCountry!
}

type TranslatedResourceOfCountry {
  key: Country!
  label: String!
}
```

Querying this field will produce the following results:
```graphql
{
  country {
    key // -> "CH",
    label // -> "Switzerland" if your Thread language is english
  }
}
```

#### Array Translation to { key label } items

We can translate string/enum/other arrays to `{ key label }` arrays.

```csharp
public class AddressType : ObjectType<Query>
{
    protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
    {
        descriptor.Field(c => c.Countries)
            .TranslateArray<Country>("Ref/Aex/Countries");
    }
}
```
Alternatively it is possible to translate the field via an attribute directly on the property:
```csharp
public class Address
{
    [TranslateArray<Country>("Ref/Aex/Countries")]
    public Country[] Countries { get; }
}
```

This will generate the following Schema:
```graphql
type Query {
  countries: [TranslatedResourceOfCountry!]!
}

type TranslatedResourceOfCountry {
  key: Country!
  label: String!
}
```

Querying this field will produce the following results:
```graphql
{
  countries {
    key
    label 
  }
}
```

```json
{
  countries: [
    {
      key: "FR",
      label: "France"
    },
    {
      key: "CH",
      label: "Switzerland"
    }
}
```


### FAQ

#### What language is used by default by this extension?

The language of the thread, which you can for instance initialize via the [ASP.NET Core Localization Middleware](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-5.0#localization-middleware-2).

#### What is the performance impact of translations?

This will depend in large part on your implementation of ```IResourcesProvider```. This provider is registered in the service container and can therefore use any registered service of your applicaton. 

In our samples, we usually build our resource strings into our assemblies so that we can retrieve them very quickly from the memory.

If you are retrieving your resource strings from another microservice, you could consider injecting [Greendonut DataLoaders](https://github.com/ChilliCream/greendonut) or [IMemoryCache](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-5.0) to reduce calls to the external microservice. 


## HotChocolate.Extensions.Tracking
***

This project enables tracking via features on HotChocolate Query and mutation fields.
In order to save the tracking items, a repository needs to be specified. 
As for now, the only provided repository is the MassTransitRepository, which essentially sends the tracking item to an azure ServiceBus topic.

Notes on the usage of MassTransit: In order to not slow down the resolver pipelines of Tracked HotChocolate Fields, the publishing into MassTransit is done asynchronously in another thread, potentially after the user's request is already ended.
As a consequence, some tracking items could be lost if the server were to crash. Although this is an edge case under normal circumstances. 

### Setting up the depencies and schema

Register the tracking pipeline in the service collection.
```
  var services = new ServiceCollection();
  services
      .AddHttpContextAccessor()
      .AddTrackingPipeline(
          builder => builder.UseMassTransitRepository(
              new MassTransitOptions(
                  new ServiceBusOptions("ServiceBusConnectionString"))));
```

Register the HotChocolate directives in your HotChocolate Schema:
```
ISchema schema = SchemaBuilder.New()
                .RegisterTracking()
                ...
                .AddServices(serviceProvider)
                .Create();
```

### Default tracking

The default field tracking saves the following data:
- email: extracted from HttpContext.Claims["email"]
- a tag
- a timestamp

You can add systematic Tracking on fields :
```
fieldDescriptor.Field("myField").Track(tag:"myFieldWasCalled");
```

- Or you can make the field trackable, in which case the consumer has to tell you if he wants you to track the call or not:
```
fieldDescriptor.Field("myField").Trackable(tag:"myFieldWasCalled");
```
This feature can be usefull in a scenario where a consumer invokes a field repeatedly, but only wants to track the call once.
The consumer can trigger the tracking by using the field directive @track in his query:
```query foo { myField @track(if:true) }``` or ```query foo { myField @track }```


### Custom tracking

If you want to save custom data fields, you have the possibility to write your own TrackingEntryFactory. 

```
  public sealed class MyTrackingEntryFactory : ITrackingEntryFactory
  {
    public ITrackingEntry CreateTrackingEntry(
        IHttpContextAccessor httpContextAccessor,
        IResolverContext context)
    {
      // Get data from httpContextAccessor or the resolver context and create and return a custom ITrackingEntry from it.
    }
```

You can then add custom tracking to your fields:
```fieldDescriptor.Field("myField").Track(new MyTrackingEntryFactory());```

```fieldDescriptor.Field("myField").Trackable(new MyTrackingEntryFactory());```


## Community

This project has adopted the code of conduct defined by the [Contributor Covenant](https://contributor-covenant.org/)
to clarify expected behavior in our community. For more information, see the [Swiss Life OSS Code of Conduct](https://swisslife-oss.github.io/coc).
