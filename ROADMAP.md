# SqlProfiler - Roadmap

- Provide a new `SimpleFactoryProfiler` wrapper for `DbProviderFactory`, whose `CreateCommand` method will create commands wrapped in the existing `SimpleCommandProfiler`. (For now, it would be possible to write that yourself, using what is already in this library.) This will make it possible to profile any provider-agnostic code which accepts a `DbProviderFactory` instance as the way to specify which provider to use.

- Provide wrappers for `DbDataReader` and `DbConnection`.