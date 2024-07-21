// dotnet publish -p:PublishSingleFile=true -r linux-x64 --self-contained false
// See https://aka.ms/new-console-template for more information
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

[Command(
    Name = "DemoApp"
    , Description = "My app is very cool 😎")]
[HelpOption(
    "-h"
    , LongName = "help"
    , Description = "Get info")]
[VersionOptionFromMember(
    "-v"
    , MemberName = nameof(GetVersion))]
class Program
{
    [Option(
        "-o"
        , "Some option"
        , CommandOptionType.SingleValue
        , LongName = "option")]
    [Range(1, 5)]
    public int Option { get; } = 1;

    public static Task<int> Main(string[] args) =>
        CommandLineApplication.ExecuteAsync<Program>(args);

    public async Task<int> OnExecuteAsync(CancellationToken cancellationToken)
    {
        var host = CreateHostBuilder();
        await host.RunConsoleAsync(cancellationToken);
        return Environment.ExitCode;
    }

    private IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
            })
            .UseSerilog((hostContext, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
            })
            .ConfigureAppConfiguration((hostContext, builder) =>
            {
                builder.AddEnvironmentVariables();
            })
            .ConfigureServices(services =>
            {
                services.AddHostedService<Worker>();

                services.Configure<MyOptions>(options =>
                {
                    options.Value = Option;
                });

                services.AddTransient<IMyService, MyService>();
            });
    }

    private static string? GetVersion()
    {
        return typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
    }
}