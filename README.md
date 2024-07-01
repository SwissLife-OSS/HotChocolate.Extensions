
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=SwissLife-OSS_HotChocolate_Extensions&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=SwissLife-OSS_HotChocolate_Extensions) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=SwissLife-OSS_HotChocolate_Extensions&metric=coverage)](https://sonarcloud.io/summary/new_code?id=SwissLife-OSS_HotChocolate_Extensions) [![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=SwissLife-OSS_HotChocolate_Extensions&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=SwissLife-OSS_HotChocolate_Extensions) [![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=SwissLife-OSS_HotChocolate_Extensions&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=SwissLife-OSS_HotChocolate_Extensions) 

This repository contains features that can be added to your HotChocolate Server to enhance your HotChocolate experience.

- [HotChocolate.Extensions.Translations](#hotchocolateextensionstranslations)
  * [Install](#install)
  * [Translating fields](#translating-fields)
    + [Translation on demand](#translation-on-demand)
    + [Translation to a { key label } type](#translation-to-a---key-label---type)
    + [Translate arrays to { key label } items](#translate-arrays-to---key-label---items)
  * [FAQ](#faq)
    + [What language is used by default by this extension?](#what-language-is-used-by-default-by-this-extension-)
    + [What is the performance impact of translations?](#what-is-the-performance-impact-of-translations-)
- [HotChocolate.Extensions.Tracking](#hotchocolateextensionstracking)
  * [Setting up the depencies and schema](#setting-up-the-depencies-and-schema)
  * [Basic tracking by Tags](#basic-tracking-by-tags)
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

The alternative way is to implement IStringLocalizer interfaces.
Additionally, we can use resource type marker classes.
This approach overrides all logic related to the usage of the IResourcesProvider interface.
```csharp
namespace Namespace.Namespace1.Namespace2;

[ResourceTypeAlias("SomePath")]
public class CustomResource
{
}

new ServiceCollection()
  .AddGraphQLServer()
  .SetSchema<MySchema>()
  .AddTranslation(    
      /* add all translatable types explicitely, except String, which is already added implicitely. */
      c => c.AddTranslatableType<Country>()
            .AddTranslatableType<MyEnum2>()
  )
  .AddStringLocalizer<CustomLocalizer>(ServiceLifetime.Singleton, typeof(CustomResource))
  .AddStringLocalizer<OtherCustomLocalizer>(ServiceLifetime.Scoped, [ typeof(OtherCustomResource) /* additional resources can share the same localizer logic */ ])
  .AddStringLocalizer(typeof(OpenGenericLocalizer<>), [ typeof(AnyResource), ......]);
```

If the resource type is not decorated with the ResourceTypeAlias attribute,
the default alias is generated from the namespace and name of the resource class.
For the case described above, it would generate the alias "Namespace::Namespace1::Namespace2::CustomResource".


With this we have registered the necessary objects to support translation on our fields. Now we can start adding translation support to our fields.

### Translating fields

You can translate field values in several ways, listed below.

#### Translation to a { key label } type

We can also rewrite our string/enum/other field to make it a `{ key label }` field.
This can be useful if we also use Array Translations, which use the same typing (see next chapter).

```csharp
[ResourceTypeAlias("Ref/Aex/Countries"))]
public class CountryResource
{
}

public class AddressType : ObjectType<Query>
{
    protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
    {
        descriptor
            .Field(c => c.Country) // The Country property is a Country enum
            .Translate("Ref/Aex/Countries");
       
       // OR
       // descriptor
       //     .Field(c => c.Country)
       //     .Translate(typeof(CountryResource));
    }
}
```

Alternatively it is possible to translate the field via an attribute directly on the property:
```csharp
[ResourceTypeAlias("Ref/Aex/Countries"))]
public class CountryResource
{
}

public class Address
{
    [Translate("Ref/Aex/Countries")]
    public Country Country { get; }
    
    
    // OR
    // [Translate(typeof(CountryResource)]
    // public Country Country { get; }
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

#### Translate arrays to { key label } items

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

This project enables tracking via middlewares on HotChocolate Query and mutation fields.
In order to save the tracking items, a custom repository needs to be injected. 
As for now, the only provided repository is the MassTransitRepository, which essentially sends the tracking item to an azure ServiceBus topic.

Note: In order to not slow down the resolver pipelines of Tracked HotChocolate Fields, the context data of the tracked Field is passed into a System.Threading.Channels.Channel and then persisted asynchonously.
As a consequence, some tracking items could be lost if the server were to shut down unexpectedly.

### Setting up the depencies and schema

Register the tracking pipeline in the GraphQL Schema.
```
  Schema schema = await services
            .AddGraphQLServer()
                .AddTrackingPipeline(builder => builder
                    .AddExporter<NotifyOnFirstEntryExporter>()
                        .AddSupportedType<MyTrace>()
                        .AddSupportedType<MyOtherTrace>());
```

### Basic tracking by Tags

The basic field invocation tracking produces a message with the following data:
- a tag
- a timestamp

You can add tracking on a field in the following way:
```csharp
fieldDescriptor.Field("myField").Track(tag:"myFieldWasCalled");
```

Alternatively, you can also add tracking on the field via an attribute on the field/resolver:
```csharp
public class Query
{
    [Track("FooInvoked")]
    public string Foo => "bar";
}
```


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

Or alternatively via an attribute:
```csharp
public class Query
{
    [Track<MyTrackingEntryFactory>]
    public string Foo => "bar";
}
```


## Community

This project has adopted the code of conduct defined by the [Contributor Covenant](https://contributor-covenant.org/)
to clarify expected behavior in our community. For more information, see the [Swiss Life OSS Code of Conduct](https://swisslife-oss.github.io/coc).
