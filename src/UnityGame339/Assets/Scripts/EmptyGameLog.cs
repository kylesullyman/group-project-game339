using Game339.Shared.Diagnostics;

namespace Game339.Tests
{
    public class EmptyGameLog : IGameLog
    {
        public static readonly EmptyGameLog Instance = new EmptyGameLog();

        public void Info(string message) { }
        public void Warn(string message) { }
        public void Error(string message) { }
    }
}