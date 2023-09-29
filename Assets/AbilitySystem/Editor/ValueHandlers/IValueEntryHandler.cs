using static OrderElimination.Infrastructure.ReflectionExtensions;

namespace OrderElimination.Editor
{
    public interface IValueEntryHandler
    {
        public void HandleValueEntry(SerializedMember entry);
    }
}
