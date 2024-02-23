namespace OrderElimination.Battle
{
    public interface IBattleFieldLayout
    {
        public string Name { get; }
        public int Width { get; }
        public int Height { get; }

        public BattleFieldSpawnData[] GetSpawns();//index = priority
        public BattleFieldStructureData[] GetStructures();
        public BattleFieldCharacterData[] GetCharacters();
    }
}
