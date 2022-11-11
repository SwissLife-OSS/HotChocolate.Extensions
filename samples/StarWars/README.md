This sample project aims to demonstrate translation features for a HotChocolate Api

## Getting Started

Start the Api and explore the Schema under `/graphql` with your favorite GraphQL Client. Banana Cakepop is available on the endpoint under `/graphql`.


Test the following Query against the Api: 
```
{
  characters {
    nodes {
      __typename
      name
      appearsIn { key label }
      ... on Human {
        hairColor { key label }
        maritalStatus { key label }
      }
    }
  }
}
```

Our default language is english, therefore the `label` fields contains the english translations of the hair color and marital status. 
Now repeat the query but this time add the HTTP Header `"Accept-Language": "fr"`. The `label` fields will now contain the french translations of the hair color and marital status.


