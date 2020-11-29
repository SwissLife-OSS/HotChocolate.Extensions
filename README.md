**Bewit is an authentication scheme alternative to cookies and bearer tokens.**

Bewit enables you to provide authentication in use cases where cookies and authentication headers can not be used. With support for both stateful and stateless authentication, Bewit is a practical solution for many scenarii, including file downloads and temporary or single-use links.

## Getting Started

We've prepared a simple example of how to use Bewit to provide stateless secure file downloads for a HotChocolate Server.
In this scenario, a HotChocolate server will be used to generate secure links. These links can then be used to download content from an Asp.Net MVC Server without cookies or authentication headers.

### Install

You will need the following package for your HotChocolate Server:

```bash
dotnet add package Bewit.Extensions.HotChocolate
```

On your MVC Server, add the following package:

```bash
dotnet add package Bewit.Extensions.Mvc
```

### HotChocolate Server

First create a simple HotChocolate API with the following Schema:

```csharp
const string mvcApiUrl = "http://localhost:5000"; //your mvc api url here

ISchema schema = SchemaBuilder.New()
    .SetOptions(new SchemaOptions
    {
        StrictValidation = false //because we don't have a QueryType in this example
    })
    .AddMutationType(
        new ObjectType(
            d =>
            {
                d.Name("Mutation");
                d.Field("RequestAccessUrl")
                    .Type<NonNullType<StringType>>()
                    .Resolver(ctx => $"{mvcApiUrl}/api/file/123")
                    .UseBewitUrlProtection();
            }))
    .Create();
```

You'll also need to register some things in the service container:

```csharp
services.AddBewitGeneration<string>(
    new BewitOptions
    {
        Secret = "my encryption key",
        TokenDuration = TimeSpan.FromMinutes(1) //lifespan of the generated url
    },
    builder => builder.UseHmacSha256Encryption()
);
```

### MVC Server

Create a simple Asp.Net MVC Server with the following Controller:

```csharp
    [Route("api/[controller]")]
    [ApiController]
    public class FileController: Controller
    {
        [HttpGet("{id}")]
        [BewitUrlAuthorization]
        public FileResult GetFile(string id)
        {
            return File(/* your file here*/);
        }
    }
```

You'll also need to register some things in the service container:

```csharp
services.AddBewitUrlAuthorizationFilter(
    new BewitOptions
    {
        Secret = "my encryption key"
    },
    builder => builder
        .UseHmacSha256Encryption()
    );
```

### Use

You can now generate secure download urls by calling your mutation:

```graphql
mutation foo {
  requestAccessUrl
}
```

## Features

### Generating secured Links

- [x] Vanilla (no overhead)
- [x] HotChocolate integration
- [ ] graphql-dotnet integration

### Authentication

- [x] Asp.Net MVC

### Persistance (for Stateful authentication)

- [x] MongoDb
- [ ] Sql Server
- [ ] Azure Blob Storage
- [ ] PostgresSQL

## Community

This project has adopted the code of conduct defined by the [Contributor Covenant](https://contributor-covenant.org/)
to clarify expected behavior in our community. For more information, see the [Swiss Life OSS Code of Conduct](https://swisslife-oss.github.io/coc).
