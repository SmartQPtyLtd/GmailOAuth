namespace GmailDaemon;

public static class LoopInfrastructure
{
    private static readonly System.Diagnostics.Stopwatch Stopwatch = new();
    private static bool _running;

    private delegate bool HandlerRoutine(CtrlTypes ctrlType);

    private enum CtrlTypes
    {
        CTRL_C_EVENT = 0
        /*,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT
        */
    }

    public static long GetElapsedMilliseconds() => Stopwatch.ElapsedMilliseconds;

    public static void Initialize() => _running = SetConsoleCtrlHandler(new(ConsoleCtrlCheck), true);

    public static bool IsRunning() => _running;

    public static void Restart() => Stopwatch.Restart();

    public static void StopRunning() => _running = false;

    public static void StopTheClock() => Stopwatch.Stop();

    private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
    {
        if (ctrlType != CtrlTypes.CTRL_C_EVENT)
            return true;

        System.Console.WriteLine("CTRL+C Received!");
        return false;
    }

    [System.Runtime.InteropServices.DllImport("Kernel32")]
    private static extern bool SetConsoleCtrlHandler(HandlerRoutine handler, bool add);
}