using System;

namespace OrderElimination
{
    public class Player
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public static int SaveIndex { get; set; }

        public Player(string id, string name)
        {
            Id = id;
            Name = String.Empty;
        }
    }
}