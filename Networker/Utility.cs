namespace networker
{
    public static class Utility
    {
        public static long UTCTimeAsLong { get { return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); } }
        public static string timeNowAsString { get { return DateTime.Now.ToString(); } }
        public static void log(string text)
        {
            Console.WriteLine($"{timeNowAsString}: {text}");
        }
    }
    
    
}
