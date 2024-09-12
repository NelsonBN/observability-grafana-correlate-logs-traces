using System.Diagnostics;
using System.Reflection;

namespace Demo.Api.Infrastructure;

public static class Telemetry
{
    private static readonly string _name = Assembly.GetEntryAssembly()!.GetName().Name!;

    public static readonly ActivitySource Source = new(_name);
}
