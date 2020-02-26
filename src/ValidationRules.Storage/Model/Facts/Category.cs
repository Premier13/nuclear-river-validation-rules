namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class Category
    {
        // Псевдо-рубрика "Товары"
        // не участвует в проверке на принадлежность фирме
        // участвует в проверке на сопутствие-запрещение
        public const long CategoryIdProducts = 112454;
        
        public long Id { get; set; }

        public long? L1Id { get; set; }
        public long? L2Id { get; set; }
        public long? L3Id { get; set; }

        public bool IsActiveNotDeleted { get; set; }
    }
}