using System.Diagnostics;
using System.Text;

namespace Infrastructure.Execution;

public static class ProcessRunner
{
    public static async Task<(int ExitCode, string Output, bool TimedOut)> RunAsync(
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

        var timeout = TimeSpan.FromMinutes(2);

        var waitTask = process.WaitForExitAsync(ct);

        var completedTask = await Task.WhenAny(
            waitTask,
            Task.Delay(timeout, ct)
        );

        if (completedTask != waitTask)
        {
            try
            {
                process.Kill(true);
            }
            catch { }

            return (-1, output.ToString(), true);
        }

        return (process.ExitCode, output.ToString(), false);
    }
}