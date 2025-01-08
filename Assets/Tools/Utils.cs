namespace Tools
{
    public static class Utils
    {
        public static float Sign(this float value)
        {
            return value > 0 ? 1.0f : value < 0 ? -1.0f : 0.0f;
        }
    }
}