using System.Collections.Generic;
using CardLinker.Data;
using UnityEngine;

namespace CardLinker.Simulation
{
    /// <summary>
    /// Üretim simülasyonunun tek gerçek kaynağı (single source of truth).
    /// BİLİNÇLİ OLARAK MonoBehaviour DEĞİLDİR - Unity sahne yaşam döngüsünü
    /// hiç bilmez. Tick() metodunu kim çağırırsa çağırsın (SimulationManager),
    /// bu sınıf hangi sahnenin yüklü/aktif olduğundan habersizdir.
    ///
    /// Tüm GridContext'ler burada tutulur (bugün tek grid, yarın nested grid -
    /// kod değişmeden çalışır çünkü zaten koleksiyon üzerinde dolaşıyoruz).
    /// </summary>
    public class SimulationWorld
    {
        private static SimulationWorld _instance;
        public static SimulationWorld Instance => _instance ??= new SimulationWorld();

        public Dictionary<string, GridContext> GridContexts = new Dictionary<string, GridContext>();

        public void Tick(float deltaTime)
        {
            foreach (var grid in GridContexts.Values)
            {
                TickGrid(grid, deltaTime);
            }
        }

        private void TickGrid(GridContext grid, float deltaTime)
        {
            foreach (var card in grid.Cards.Values)
            {
                if (card is GatheringCardData gathering)
                {
                    TickGathering(grid, gathering, deltaTime);
                }
                // İleride FacilityGridCard ve EnergyGridCard tick mantığı da
                // buraya, aynı desende (case per type) eklenecek.
            }
        }

        private void TickGathering(GridContext grid, GatheringCardData gathering, float deltaTime)
        {
            // Bağlantı kontrolü (hardcoded referans - port sistemi yok).
            bool sourceConnected = !string.IsNullOrEmpty(gathering.SourceCardId)
                                    && grid.Cards.ContainsKey(gathering.SourceCardId)
                                    && grid.Cards[gathering.SourceCardId] is ResourceCardData;

            if (!sourceConnected)
                return;

            gathering.ProgressTimer += deltaTime;

            if (gathering.ProgressTimer < gathering.ProductionInterval)
                return;

            gathering.ProgressTimer -= gathering.ProductionInterval;

            Debug.Log($"[Miner:{gathering.Id}] {gathering.OutputMaterial} üretildi.");

            if (string.IsNullOrEmpty(gathering.OutputTargetId))
                return;

            if (!grid.Cards.TryGetValue(gathering.OutputTargetId, out var targetCard))
                return;

            if (targetCard is StorageCardData storage && storage.CanAccept(gathering.OutputMaterial))
            {
                storage.AddStock(gathering.OutputMaterial, 1);
                Debug.Log($"[Container:{storage.Id}] {gathering.OutputMaterial} deposuna alındı. Toplam: {storage.Stock[gathering.OutputMaterial]}");
            }
        }
    }
}
