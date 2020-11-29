This repository contains features that can be added to your HotChocolate Server to enhance your HotChocolate experience.

## Translations

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
        descriptor.Field("country")
            .Resolve(c => c.Parent<Query>().CountryCodeAlpha2)
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

#### Translation to a { key label } item

Alternatively, we can rewrite our string/enum/other field to make it a { key label } field.
This can be useful if we also use Array Translations, which use the same typing (see next chapter).

```csharp
public class AddressType : ObjectType<Query>
{
    protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
    {
        descriptor.Field("country")
            .Resolve(c => c.Parent<Query>().CountryCodeAlpha2) //CountryCodeAlpha2 is a Country enum
            .Translate("Ref/Aex/Countries");
  
    }
}
```

This will generate the following Schema:
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

We can translate string/enum/other arrays to { key label } arrays.

```csharp
public class AddressType : ObjectType<Query>
{
    protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
    {
        descriptor.Field("countries")
            .Resolve(c => new []{ Country.Fr, Country.Ch } )
            .TranslateArray("Ref/Aex/Countries");
    }
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


## Community

This project has adopted the code of conduct defined by the [Contributor Covenant](https://contributor-covenant.org/)
to clarify expected behavior in our community. For more information, see the [Swiss Life OSS Code of Conduct](https://swisslife-oss.github.io/coc).
