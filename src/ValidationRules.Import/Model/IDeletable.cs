namespace NuClear.ValidationRules.Import.Model
{
    public interface IDeletable
    {
        /// <summary>
        /// Признак удалённой сущности.
        /// </summary>
        /// <remarks>
        /// Имеет непривычное для значение: записи IsDeleted = true физически удаляются из базы,
        /// несмотря на наличие колонки IsDeleted. Можно объявить голосования за более удачное название.
        /// </remarks>
        bool IsDeleted { get; }
    }
}
