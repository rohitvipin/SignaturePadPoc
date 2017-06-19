namespace SignaturePadPoc.Common
{
    public static class Extensions
    {
        public static int? ToIntSafe(this string input)
        {
            int returnValue;
            if (int.TryParse(input, out returnValue))
            {
                return returnValue;
            }
            return null;
        }
    }
}