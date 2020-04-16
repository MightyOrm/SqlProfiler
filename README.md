# ![logo](https://raw.githubusercontent.com/MikeBeaton/SqlProfiler/master/yellow_magnify_32x32.png) SqlProfiler

[![NuGet](https://img.shields.io/nuget/v/SqlProfiler.svg)](https://nuget.org/packages/SqlProfiler)

A SQL profiler for `System.Data.Common` objects, with some clever .NET `dynamic` tricks so that it can work as a profiler even if your code needs to set ADO.NET provider-specific properties.

## Pre-Requisites

This library will work for any `System.Data.Common` items which you wish to wrap and profile (at the moment only `DbCommand` and `DbProviderFactory` are supported, but the intention is to add support for `DbConnection` and `DbDataReader` as well), as long as you:

a) Either do not access any ADO.NET provider-specific properties, or b) if you do access any ADO.NET provider-specific properties, always use this dynamic trick:

```c#
public void SetOracleThings(DbCommand command)
{
    // These are properties of Oracle.DataAccess.Client.OracleCommand, not of System.Data.Common.DbCommand,
    // but this code works fine, and without requiring any explicit dependency on the Oracle ADO.NET library
    ((dynamic)command).BindByName = true;
    ((dynamic)command).InitialLONGFetchSize = -1;
}
```

What you can't do, if you want to profile `DbCommand` calls with this library, is take an *explicit* dependency on a provider specific library. So you can't use code like this:

```c#
((OracleCommand)command).BindByName = true;
```

We cannot intercept that.

As long as the sections of code you want to profile obey the above rules (which, as noted, includes all code which doesn't access ADO.NET provider specifc properties at all), and as long as there is a place at which you can pass in a wrapped `DbCommand` then you can profile your - or even someone else's - SQL code with this library.

## Simple usage

At some point before using it, wrap your `DbCommand` as follows:

```c#
DbCommand wrapped = new SqlProfiler.Simple.SimpleCommandProfiler(
    command,
    // provide a callback action for SqlProfiler's SimpleCommandProfiler
    (method, command, behavior) =>
    {
        // or whatever you want...
        Debug.WriteLine(command.CommandText);
    });
```

Now, just before every call to `ExecuteDbDataReader`, `ExecuteNonQuery` or `ExecuteScalar` on the wrapped command you will get a callback to your callback action, and you can examine, log, or whatever the `DbCommand` that is just about to be executed.

## Advanced Usage

To be able to have much more visibility into how `DbCommand` is being used, wrap all of your commands in your own sub-class of `SqlProfiler.CommandWrapper`

```c#
// Define the wrapper
public class MyCommandWrapper : CommandWrapper
{
    // Or any other more complex constructor as long as it calls base(wrapped)
    public MyCommandWrapper(DbCommand wrapped) : base (wrapped)
    {
    }
    
    // Your CommandWrapper method overrides go here
    ...
}
...

// Wrap the command
DbCommand wrapped = new MyCommandWrapper(command);
```

and then use the command exactly as normal (including accessing any ADO.NET provider-specific properties, if you need to, as long as they are accessed in the way described in 'Pre-Requisites' above), and you will get callbacks to hooks on your sub-class of `CommandWrapper` for any of the following virtual methods which you choose to override:

```c#
protected virtual object PreCancel(DbCommand command) { return null; }
protected virtual void PostCancel(DbCommand command, object profilingObject) {}
protected virtual object PreExecuteNonQuery(DbCommand command) { return null; }
protected virtual void PostExecuteNonQuery(DbCommand command, object profilingObject) {}
protected virtual object PreExecuteScalar(DbCommand command) { return null; }
protected virtual void PostExecuteScalar(DbCommand command, object profilingObject) {}
protected virtual object PrePrepare(DbCommand command) { return null; }
protected virtual void PostPrepare(DbCommand command, object profilingObject) {}
protected virtual object PreCreateDbParameter(DbCommand command) { return null; }
protected virtual void PostCreateDbParameter(DbCommand command, object profilingObject) {}
protected virtual object PreExecuteDbDataReader(DbCommand command, CommandBehavior behavior) { return null; }
protected virtual void PostExecuteDbDataReader(DbCommand command, CommandBehavior behavior, object profilingObject) {}
```

The object (if any) returned from each `Pre...()` hook is the `profilingObject` passed to the corresponding `Post...()` hook. This could be used, for instance, to start and then pass a `System.Diagnostics.Stopwatch`, in order to time the delay between `Pre...()` and `Post...()`.
