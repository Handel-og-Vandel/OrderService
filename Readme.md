# HaaV OrderService

Løsningsforslag til microservicen OrderService fra opgave M7.02.

## Tip: Opret ny controller

``` bash
 $ dotnet new apicontroller -n OrderController -o Controllers -ac
```

## Tip: Deserialisering af JSON til Enums

- [How to read JSON as .NET objects (deserialize)](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/deserialization)
- [JsonConverterAttribute](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/customize-properties?source=recommendations#jsonconverterattribute):
  
  ``` csharp
    public class WeatherForecastWithPrecipEnum
    {
        public DateTimeOffset Date { get; set; }
        public int TemperatureCelsius { get; set; }
        public Precipitation? Precipitation { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter<Precipitation>))]
    public enum Precipitation
    {
        Drizzle, Rain, Sleet, Hail, Snow
    }
  ```
