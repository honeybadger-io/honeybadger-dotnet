// See https://aka.ms/new-console-template for more information

using Honeybadger;

Console.WriteLine("Hello, World!");

var client = HoneybadgerSdk.Init(new HoneybadgerOptions("test"));

client.Notify("console app");