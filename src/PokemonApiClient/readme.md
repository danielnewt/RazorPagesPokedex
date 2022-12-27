This project contains a simple (and minimal) library to query the poke api.

This can be added to any project by calling the AddPokeApiClient() extension method on IServicesCollection.

The main interface is the IPokeApiClient which will handle calling the pokeapi and caching any successful responses.
Note: if no implementation of IDistribuedCache has been configured in the service provider the responses will instead be cached in a dictionary.