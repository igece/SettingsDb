[![nuget](https://img.shields.io/nuget/v/SettingsDb.svg)](https://www.nuget.org/packages/SettingsDb)
[![nuget](https://img.shields.io/nuget/dt/SettingsDb.svg)](https://www.nuget.org/packages/SettingsDb)
[![Build](https://github.com/igece/SettingsDb/actions/workflows/build.yml/badge.svg)](https://github.com/igece/SettingsDb/actions/workflows/build.yml)
[![Tests](https://github.com/igece/SettingsDb/actions/workflows/tests.yml/badge.svg)](https://github.com/igece/SettingsDb/actions/workflows/tests.yml)

## SettingsDb
SettingsDb is a .NET Standard library that allows to manage persistence of application settings in a simple, fast and flexible way. The settings are stored in a [SQLite](http://sqlite.org) database located in the same directory as the application.

### How It Works

Settings are stored into a single table in the database, using a pair of "Setting Name" and "Value" values. Internally, all the values are serialized to a JSON string and then inserted into the table, this offers a great level of flexibility in terms of what type of data can be stored.

#### Initial Setup

When a new instance of the `Settings` class is created, it checks for existence of the database file and creates it if necessary. You can pass the name to be used for the database file in the constructor, but if no name is specified (parameterless contructor) the assembly name of the application will be used as the database name.

#### Storing a Value

To store a value, just call the `Store` method (or its async version `StoreAsync`) passing the name to be used for the setting and its value:

``` C#
void Store<T>(string settingName, T value)
```
``` C#
var settings = new Settings();

settings.Store("UserName", "John Doe");
settings.Store("ID", 12345);
settings.Store("ShowToolbar", true);
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

#### Reading a Value

To read a value from the database, you use any of the `Read` or `ReadAsync` generic methods, indicating the name of the setting, the expected type to be returned
and, optionally, a default value to be used if the specified setting is not found in the database.

``` C#
public T Read<T>(string settingName, T defaultValue = default)
```
``` C#
var settings = new Settings();

var showToolbar = settings.Read<bool>("ShowToolbar", true);
var windowPosition = settings.Read<WindowPosition>("WindowPosition");
```

If no default value is specified, SettingsDb will use the default value for the requested data type.

#### Other Operations

```  C#
void Clear(string settingName)
```
```  C#
void ClearAll()
```
