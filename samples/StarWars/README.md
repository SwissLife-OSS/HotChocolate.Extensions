This sample project aims to demonstrate translation features for a HotChocolate Api

## Getting Started

Start the Api and explore the Schema under `/graphql` with your favorite GraphQL Client. Banana Cakepop is available on the endpoint under `/graphql`.


Test the following Query against the Api: 
```
{
  characters{
    nodes{
      name
      ... on Human{
        hairColor{
          key
          label
        }
      }
    }
  }
}
```

Our default language is english. Now repeat the query but this time add the HTTP Header `"Accept-Language": "fr"`.


