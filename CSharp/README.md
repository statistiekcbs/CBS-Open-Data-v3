# C#
Het c# voorbeeld is geschreven voor [.NET 5.0](https://dotnet.microsoft.com/download)
Dit betekent dat de code compatible is met Windows, MacOS en Linux.

Start de code vanaf de command prompt/terminal met :
```
dotnet restore
dotnet run
```

In program.cs kan de url gewijzigd worden:
```csharp
string url =  "https://opendata.cbs.nl/ODataApi/odata/37296ned/UntypedDataSet?$select=Perioden,+TotaleBevolking_1,+Mannen_2,+Vrouwen_3";
```
De ODataComposer haalt de benodigde informatie op en vult een object (SeriesTableViewModel) met data. De data wordt gevuld in een standaard DataTable en vervolgens afgedrukt.