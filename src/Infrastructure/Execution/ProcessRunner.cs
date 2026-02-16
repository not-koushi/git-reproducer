using System.Diagnostics;
using System.Text;

namespace Infrastructure.Execution;

public static class ProcessRunner
{
    public static async Task<(int ExitCode, string Output)> RunAsync(
        string file,
        string args,
        string workingDir,
        CancellationToken ct = default)
    {
        var output = new StringBuilder();

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = file,
                Arguments = args,
                WorkingDirectory = workingDir,
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