using System.Collections.Generic;

namespace CardLinker.Data
{
    /// <summary>
    /// Tüm kart veri türlerinin temel sınıfı.
    /// DİKKAT: Bu sınıf ve alt sınıfları MonoBehaviour DEĞİLDİR.
    /// Unity sahne yaşam döngüsünü (scene load/unload) hiç bilmezler.
    /// Sahne unload olsa bile bu nesneler SimulationWorld içinde yaşamaya devam eder.
    /// </summary>
    public abstract class CardInstanceData
    {
        public string Id;

        /// <summary>
        /// İleride "iç içe grid" (nested grid) özelliği için:
        /// Bu kart tıklandığında açılacak bağımsız bir grid varsa, o grid'in id'si burada tutulur.
        /// Şimdilik hep null - implementasyonu yapılmıyor, sadece mimari buna kapalı olmasın diye var.
        /// </summary>
        public string ChildGridContextId;
    }

    /// <summary>
    /// ResourcesGridCard karşılığı. Sonsuz kaynak - gerçek üretim/tüketim yapmaz.
    /// Sadece bağlı olduğu GatheringGridCard'a hangi malzemeyi beslediğini bildirir.
    /// </summary>
    public class ResourceCardData : CardInstanceData
    {
        public RecipeComponentEnums ResourceType;
    }

    /// <summary>
    /// GatheringGridCard karşılığı. Bağlı olduğu ResourceCardData'dan (varsayılan/hardcoded
    /// bağlantıyla, bu ilk testte port sistemi yok) üretim yapar.
    /// </summary>
    public class GatheringCardData : CardInstanceData
    {
        /// <summary>Bağlı olduğu ResourceCardData'nın Id'si. Port sistemi henüz yok, doğrudan referans.</summary>
        public string SourceCardId;

        /// <summary>Ürettiği malzeme.</summary>
        public RecipeComponentEnums OutputMaterial;

        /// <summary>Bir birim üretmek için gereken süre (saniye).</summary>
        public float ProductionInterval = 2f;

        /// <summary>Tick biriktirici. Model'in kendi state'i - sahne değişse de kaybolmaz.</summary>
        public float ProgressTimer;

        /// <summary>Üretilen malzemenin aktarılacağı StorageCardData'nın Id'si. Yine hardcoded referans.</summary>
        public string OutputTargetId;
    }

    /// <summary>
    /// StorageGridCard karşılığı. Malzeme depolar.
    /// </summary>
    public class StorageCardData : CardInstanceData
    {
        /// <summary>Kabul edilen malzeme listesi. Boşsa (bu test için) her şeyi kabul eder.</summary>
        public List<RecipeComponentEnums> AcceptedMaterials = new List<RecipeComponentEnums>();

        /// <summary>Mevcut depo içeriği: malzeme türü -> miktar.</summary>
        public Dictionary<RecipeComponentEnums, int> Stock = new Dictionary<RecipeComponentEnums, int>();

        public bool CanAccept(RecipeComponentEnums material)
        {
            return AcceptedMaterials.Count == 0 || AcceptedMaterials.Contains(material);
        }

        public void AddStock(RecipeComponentEnums material, int amount)
        {
            if (!Stock.ContainsKey(material))
                Stock[material] = 0;
            Stock[material] += amount;
        }
    }
}
