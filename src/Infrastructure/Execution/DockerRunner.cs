using System.Diagnostics;
using System.Text;

namespace Infrastructure.Execution;

public static class DockerRunner
{
    public static async Task<(int ExitCode, string Output)> RunAsync(
        string workspace,
        string command,
        CancellationToken ct = default)
    {
        var output = new StringBuilder();

        var args =
            $"run --rm -v \"{workspace}:/workspace\" -w /workspace mcr.microsoft.com/dotnet/sdk:8.0 bash -c \"{command}\"";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null) output.AppendLine(e.Data);
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null) output.AppendLine(e.Data);
        };

        process.Start();

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync(ct);

        return (process.ExitCode, output.ToString());
    }
}