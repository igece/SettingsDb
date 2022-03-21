[![nuget](https://img.shields.io/nuget/v/SettingsDb.svg)](https://www.nuget.org/packages/SettingsDb)
[![nuget](https://img.shields.io/nuget/dt/SettingsDb.svg)](https://www.nuget.org/packages/SettingsDb)
[![Build](https://github.com/igece/SettingsDb/actions/workflows/build.yml/badge.svg)](https://github.com/igece/SettingsDb/actions/workflows/build.yml)
[![Tests](https://github.com/igece/SettingsDb/actions/workflows/tests.yml/badge.svg)](https://github.com/igece/SettingsDb/actions/workflows/tests.yml)

# SettingsDb
SettingsDb is a .NET Standard library that allows to manage persistence of application settings in a simple and fast way. Settings values are serialized as JSON objects and stored in a [SQLite](http://sqlite.org) database located in the same directory as the application. This offers a great level of flexibility in terms of what type of data can be stored.


## Installation

Using NuGet package manager console:

```
Install-Package SettingsDb
```

Using .NET CLI:

```
dotnet add package SettingsDb
```


## Usage

When a new instance of the `Settings` class is created, it checks for existence of the database file and creates it if necessary. By default, the assembly name of the application will be used as the database name, but you can specify any other name in the constructor.

### Storing a Value

To store a value, just call the `Store` method (or its async version `StoreAsync`) passing the name to be used for the setting and its value:

``` C#
public void Store<T>(string settingName, T value);
public async Task StoreAsync<T>(string settingName, T value);
```
``` C#
var settings = new Settings();

settings.Store("UserName", "John Doe");
settings.Store("ID", 12345);
await settings.StoreAsync("ShowToolbar", true);
```

Remember that specified values are serialized to JSON strings prior to be stored into the database, so you can even pass in simple objects:

``` C#
class WindowPosition
{
    public int X { get; set; }
    public int Y { get; set; }
}

var settings = new Settings();

settings.Store("WindowPosition", new WindowPosition { X = 250; Y = 100});
```

### Reading a Value

To read a value from the database, you use any of the `Read` or `ReadAsync` generic methods, passing the name of the setting, the expected type to be returned
and, optionally, a default value to be used if the specified setting is not found in the database.

``` C#
public T Read<T>(string settingName, T defaultValue = default);
public async Task<T> ReadAsync<T>(string settingName, T defaultValue = default);
```
``` C#
var settings = new Settings();

var showToolbar = settings.Read<bool>("ShowToolbar", true);
var windowPosition = await settings.ReadAsync<WindowPosition>("WindowPosition");
```

If no default value is specified and the setting name is not found, SettingsDb will return the default value for the requested data type.

### Other Operations

```  C#
public void Clear(string settingName);
public Task ClearAsync(string settingName);
```
```  C#
public void ClearAll();
public Task ClearAllAsync();
```
