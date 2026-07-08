using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bir kartın çalışma zamanı (runtime) verisi. MonoBehaviour DEĞİL - JSON'a
/// serialize edilebilir, sahne kavramından bağımsız plain bir sınıf.
///
/// NOT (KAPSAM): Bu, referans projendeki CardJSONData'nın TRIM EDİLMİŞ bir
/// versiyonu. Şu anki test senaryomuz (Iron Mine -> Miner -> Container) için
/// gerekli alanlar burada. Henüz eklenmeyenler (referans projende vardı,
/// burada bilinçli olarak yok):
///   - assignedDataList, engineerRankEnum, engineerExperience (mühendis sistemi)
///   - currentDurability (dayanıklılık sistemi)
///   - stackedCardNumber (stacking sistemi)
///   - portFilterDataList (port filtreleme)
///   - isOnExploration, specialBarValue (keşif sistemi)
///   - hiddenIncomesTempStorage, hiddenOutcomesTempStorage, isAlertHidden,
///     isNotificationSignalActiveForDeckCard (UI/gizli envanter detayları)
/// Bu sistemleri ele aldığımızda ilgili alanları buraya geri ekleyeceğiz.
/// </summary>
[System.Serializable]
public class CardJSONData
{
    /// <summary>Bu kartın tanım verisi (sabit, tasarım zamanı). Henüz oluşturulmadı - sıradaki adım.</summary>
    public CardData cardData;

    [Header("Production State")]
    public bool isOnProduction;
    public float consumedProductionTime;
    public int activeRecipeIndex;
    public bool isAutoSelect;

    [Header("Grid Card Pos")]
    public Vector3 positionsOfGridCard;

    [Header("Display Inventory")]
    public List<RecipeData> incomesTempStorage;
    public List<RecipeData> outcomesTempStorage;

    [Header("Storage Demand List")]
    public List<RecipeComponentEnums> demandStorageList;

    public CardJSONData()
    {
        this.isOnProduction = false;
        this.consumedProductionTime = 0f;
        this.activeRecipeIndex = -1;
        this.isAutoSelect = false;
        this.positionsOfGridCard = Vector3.zero;
        this.incomesTempStorage = new List<RecipeData>();
        this.outcomesTempStorage = new List<RecipeData>();
        this.demandStorageList = new List<RecipeComponentEnums>();
    }

    /// <summary>
    /// Bir DeckCard'ın GridCard'a dönüşmesi gibi durumlarda, önceki karttan
    /// yeni bir instance türetmek için kullanılacak kopya constructor.
    /// </summary>
    public CardJSONData(CardJSONData previous)
    {
        this.cardData = previous.cardData;
        this.isOnProduction = previous.isOnProduction;
        this.consumedProductionTime = previous.consumedProductionTime;
        this.activeRecipeIndex = previous.activeRecipeIndex;
        this.isAutoSelect = previous.isAutoSelect;
        this.positionsOfGridCard = previous.positionsOfGridCard;
        this.incomesTempStorage = new List<RecipeData>(previous.incomesTempStorage);
        this.outcomesTempStorage = new List<RecipeData>(previous.outcomesTempStorage);
        this.demandStorageList = new List<RecipeComponentEnums>(previous.demandStorageList);
    }
}
