namespace GmailDaemon;

public static class LoopInfrastructure
{
    private static readonly System.Diagnostics.Stopwatch _stopwatch = new();
    private static bool _running;

    private delegate bool HandlerRoutine(CtrlTypes CtrlType);

    private enum CtrlTypes
    {
        CTRL_C_EVENT = 0
        /*,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT
        */
    }

    public static long GetElapsedMilliseconds() => _stopwatch.ElapsedMilliseconds;

    public static void Initialize() => _running = SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);

    public static bool IsRunning() => _running;

    public static void Restart() => _stopwatch.Restart();

    public static void StopRunning() => _running = false;

    public static void StopTheClock() => _stopwatch.Stop();

    private static bool ConsoleCtrlCheck(CtrlTypes CtrlType)
    {
        if (CtrlType == CtrlTypes.CTRL_C_EVENT)
        {
            System.Console.WriteLine("CTRL+C Received!");

            return false;
        }

        return true;
    }

    [System.Runtime.InteropServices.DllImport("Kernel32")]
    private static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);
}