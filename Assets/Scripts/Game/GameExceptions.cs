using System;

namespace Game
{
    public class GameLogicException : Exception
    {
        public GameLogicException(string message) : base(message)
        {
        }
    }

    public class TableKeyNotFoundException : Exception
    {
        public TableKeyNotFoundException(Type tableType, int key) : base($"table {tableType.FullName} no key: {key}")
        {
        }
    }
}