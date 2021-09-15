namespace Chat.Server.Extensions
{
    public static class StringExtensions
    {
        public static bool IsConnectionIdEmpty(this string connectionId) 
        {
            return string.IsNullOrWhiteSpace(connectionId)
                && connectionId == string.Empty;
        }
    }
}
