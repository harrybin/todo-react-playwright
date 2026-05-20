using System.Diagnostics;

public static class LocalServer
{
    static Process? localServerProcess;

    public static void StartLocalServer()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "sh",
            Arguments = "-c \"cd ../../../../../src && npm i && npm run dev &\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false
        };

        localServerProcess = new Process { StartInfo = startInfo };  
        localServerProcess.Start();
    }

    public static void StopLocalServer()
    {
        if (localServerProcess != null && !localServerProcess.HasExited)
        {
            localServerProcess.Kill();
        }
    }
}