# HybridLazyCache

A lightweight, Native AOT-friendly, thread-safe in-memory cache for .NET.

HybridLazyCache provides an API similar to `IMemoryCache.GetOrCreateAsync`, while preventing cache stampedes by ensuring only a single thread executes the factory for a given key.

It is designed to work with:

* .NET Standard 2.0+
* .NET 6+
* .NET 7+
* .NET 8 Native AOT
* .NET 9
* Future .NET releases

## Why HybridLazyCache?

When multiple requests attempt to populate the same cache entry simultaneously, many caching libraries will execute the factory multiple times. This is commonly known as a **cache stampede**.

HybridLazyCache guarantees:

* One factory execution per cache key.
* Other callers await the same result.
* Familiar `IMemoryCache`-style API.
* Native AOT compatibility.
* No reflection.
* No dynamic code generation.
* No expression compilation.
* Minimal allocations.

## Installation

```bash
dotnet add package HybridLazyCache
```

## Registration

```csharp
using HybridLazyCacheLib.Cache.Extensions;

builder.Services.AddHybridLazyCache();
```

## Basic Usage

```csharp
var user = await cache.GetOrCreateAsync(
    $"user:{id}",
    async (entry, ct) =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

        return await repository.GetUser(id, ct);
    });
```

Only one request will execute `GetUser()` if multiple threads request the same key simultaneously.

---

## Dynamic Expiration

One of the primary goals of HybridLazyCache is allowing cache lifetime to be determined **after** the value has been retrieved.

For example, an OAuth token typically contains its own expiration time.

```csharp
var token = await cache.GetOrCreateAsync(
    "oauth-token",
    async (entry, ct) =>
    {
        var token = await authClient.GetTokenAsync(ct);

        entry.AbsoluteExpirationRelativeToNow =
            TimeSpan.FromSeconds(token.ExpiresIn - 300);

        return token;
    });
```

This allows the cache lifetime to be based on the returned value.

---

## Sliding Expiration

```csharp
await cache.GetOrCreateAsync(
    "products",
    async (entry, ct) =>
    {
        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

        return await repository.GetProducts(ct);
    });
```

---

## Skip Caching

Sometimes you want to return a value without storing it.

```csharp
await cache.GetOrCreateAsync(
    key,
    async (entry, ct) =>
    {
        var result = await api.Call(ct);

        if (!result.Success)
            entry.Cache = false;

        return result;
    });
```

---

## API

```csharp
ValueTask<T> GetOrCreateAsync<T>(
    object key,
    Func<IHybridCacheEntry, CancellationToken, ValueTask<T>> factory,
    CancellationToken cancellationToken = default);

ValueTask<T?> GetAsync<T>(object key);

void Remove(object key);

void Clear();
```

---

## Features

* Thread-safe
* Prevents cache stampedes
* Async-first API
* Familiar `IMemoryCache` programming model
* Supports:

  * Absolute expiration
  * Sliding expiration
  * Dynamic expiration
  * Skip caching
  * Remove entries
  * Clear cache
* Native AOT compatible
* Trimming friendly
* No runtime reflection

---

## Native AOT

HybridLazyCache has been designed with Native AOT in mind.

It does not rely on:

* Reflection
* Runtime code generation
* Expression compilation
* Dynamic proxies

This makes it suitable for applications targeting Native AOT without additional configuration.

---

## Roadmap

Planned features include:

* Keyed async locking with reduced allocations
* Entry eviction callbacks
* Cache statistics
* Optional logging integration
* Distributed cache abstraction
* Memory pressure optimizations
* Source generators (where beneficial)

Suggestions and contributions are welcome.

---

## Contributing

Issues and pull requests are welcome.

If you discover a bug, have a feature request, or want to improve performance, please open an issue first so we can discuss the proposed change.

---

## License

MIT License.
