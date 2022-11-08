namespace Game
{
    public static class UniqueIdGenerator
    {
        private static int id;

        public static int GetUniqueId()
        {
            return id++;
        }
    }
}