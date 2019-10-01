# DotNetEnvironmentExtensions
Extension library to provide some Environment functionality, for example for using with Docker-apps.

## Build status
|Branch|Status|
|--|--|
|`master`|[![Build Status](https://travis-ci.org/bluewalk/DotNetEnvironmentExtensions.svg?branch=master)](https://travis-ci.org/bluewalk/DotNetEnvironmentExtensions)|
|`develop`|[![Build Status](https://travis-ci.org/bluewalk/DotNetEnvironmentExtensions.svg?branch=develop)](https://travis-ci.org/bluewalk/DotNetEnvironmentExtensions)|

## NuGet Package
This appender is available as a [NuGet package](https://www.nuget.org/packages/Net.Bluewalk.DotNetEnvironmentExtensions/).

Run `Install-Package Net.Bluewalk.DotNetEnvironmentExtensions` in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console) or search for "DotNetEnvironmentExtensions" in your IDE's package management plug-in.

## Usage
The following functionalities are available:

|Method|Parameters|Result|Extends|
|--|--|--|--|
|`GetEnvironmentVariable<T>`|`name`, _optional_ `defaultValue`| `T` |
|`GetEnvironmentVariable`|`type`, `name`, _optional_ `defaultValue`| `object` |
|`FromEnvironment`| `autoCreateInstances`|`void`|`object`|
|`FromEnvironment`| `autoCreateInstances`|`void`|`Type`|

Use the following attribute to decorate properties with environment variables: `EnvironmentVariable`

## Examples

### Read USE_THIS as boolean from the environment
```csharp
var useThis = EnvironmentExtensions.GetEnvironmentVariable<bool>("USE_THIS", false);
```
OR
```csharp
var useThis = EnvironmentExtensions.GetEnvironmentVariable(typeof(bool), "USE_THIS", false);
```

### Fill object from environment
Parameter `autoCreateInstances` can be used to define if properties that are non-simple types need to me instantiated automatically.
__Note: these classes need to have a parameter-less constructor__

Example class
```csharp
public class MySettings
{
  [EnvironmentVariable(Name = "SOME_STRING", Default = "ABCdef")]
  public string SomeString { get; set; }
  [EnvironmentVariable(Name = "ENABLE_SMTHNG", Default = true)]
  public bool EnableSomething { get; set; }  
  [EnvironmentVariable(Name = "MY_INT_VAL", Default = 1234)]
  public int MyIntValue { get; set; }
  public SubSettings SubSettings { get; set; }
}

public class SubSettings
{
  [EnvironmentVariable(Name = "ANOTHER_STRING")]
  public string AnotherString { get; set; }
  [EnvironmentVariable(Name = "ANOTHER_INT_VAL", Default = -1)]
  public int AnotherIntValue { get; set; }
}
```
Read from environment
```csharp
var mySettings = typeof(MySettings).FromEnvironment();
```
OR
```csharp
var mySettings = new MySettings();
mySettings.FromEnvironment();
```
