using OrderElimination.Battle;
using Sirenix.OdinInspector;

namespace OrderElimination.GameContent
{
    public abstract class BattleFieldLayout : SerializedScriptableObject, IBattleFieldLayout
    {
        public abstract string Name { get; }

        public abstract int Width { get; }

        public abstract int Height { get; }

        public abstract BattleFieldCharacterData[] GetCharacters();

        public abstract BattleFieldSpawnData[] GetSpawns();

        public abstract BattleFieldStructureData[] GetStructures();
    }
}
