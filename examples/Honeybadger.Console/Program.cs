// See https://aka.ms/new-console-template for more information

using Honeybadger;

Console.WriteLine("Hello, World!");

var client = HoneybadgerSdk.Init(new HoneybadgerOptions("80ee8156")
{
    AppEnvironment = "dotnet-environment"
});

client.Notify("console app");

Thread.Sleep(1000);