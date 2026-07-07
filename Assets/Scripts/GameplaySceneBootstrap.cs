using CardLinker.Data;
using UnityEngine;

namespace CardLinker.Simulation
{
    /// <summary>
    /// Test senaryosunu (1 Iron Mine, 1 Miner, 1 Container) SimulationWorld'e
    /// bir kez kaydeder. GameplayScene her yüklendiğinde (additive geri dönüşte
    /// dahil) çalışır ama veri zaten SimulationWorld'de duruyorsa tekrar oluşturmaz -
    /// bu sayede sahneye her dönüşte üretim state'i sıfırlanmaz, kaldığı yerden devam eder.
    ///
    /// Bu script'i GameplayScene içinde boş bir GameObject'e ekleyin.
    /// </summary>
    public class GameplaySceneBootstrap : MonoBehaviour
    {
        private const string GridId = "MainGrid";

        private void Awake()
        {
            var world = SimulationWorld.Instance;

            if (world.GridContexts.ContainsKey(GridId))
            {
                Debug.Log("[Bootstrap] MainGrid zaten kurulu, mevcut simülasyon state'i korunuyor.");
                return;
            }

            var grid = new GridContext { Id = GridId };
            world.GridContexts[GridId] = grid;

            var ironMine = new ResourceCardData
            {
                Id = "IronMine",
                ResourceType = RecipeComponentEnums.IronOre
            };

            var miner = new GatheringCardData
            {
                Id = "Miner",
                SourceCardId = ironMine.Id,
                OutputMaterial = RecipeComponentEnums.IronOre,
                ProductionInterval = 2f,
                OutputTargetId = "Container"
            };

            var container = new StorageCardData
            {
                Id = "Container"
                // AcceptedMaterials boş bırakıldı -> her malzemeyi kabul eder (bu test için yeterli)
            };

            grid.Cards[ironMine.Id] = ironMine;
            grid.Cards[miner.Id] = miner;
            grid.Cards[container.Id] = container;

            Debug.Log("[Bootstrap] MainGrid kuruldu: IronMine -> Miner -> Container");
        }
    }
}
