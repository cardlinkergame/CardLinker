namespace CardLinker.Data
{
    /// <summary>
    /// Sistemdeki malzeme/kaynak tiplerini tanımlar.
    /// CardInstanceData ve depo mantığı bu enum üzerinden çalışır (string yerine).
    /// Yeni malzeme eklenmek istendiğinde sadece buraya yeni bir değer eklenir.
    /// </summary>
    public enum RecipeComponentEnums
    {
        IronOre,
        IronIngot
    }
}
