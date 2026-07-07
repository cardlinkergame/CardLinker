using UnityEngine;

namespace CardLinker.Simulation
{
    /// <summary>
    /// SimulationWorld ile Unity arasındaki tek köprü.
    /// Bilinçli olarak "ince" tutulmuştur - hiçbir üretim mantığı burada yok,
    /// sadece SimulationWorld.Tick() her frame çağrılıyor.
    ///
    /// DontDestroyOnLoad ile persistent tutulur. Boot sahnesinde bir kez
    /// yerleştirilmesi, GameplayScene ve TechTreeScene arasında hiç yok olmaması gerekir.
    /// </summary>
    public class SimulationManager : MonoBehaviour
    {
        public static SimulationManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            SimulationWorld.Instance.Tick(Time.deltaTime);
        }
    }
}
