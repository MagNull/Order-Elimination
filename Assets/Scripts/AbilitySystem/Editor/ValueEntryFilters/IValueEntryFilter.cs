using static OrderElimination.Infrastructure.ReflectionExtensions;

namespace OrderElimination.Editor
{
    public interface IValueEntryFilter
    {
        public bool IsAllowed(SerializedMember entry);
    }
}
