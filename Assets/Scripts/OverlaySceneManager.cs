using UnityEngine;
using UnityEngine.SceneManagement;

namespace CardLinker.SceneFlow
{
    /// <summary>
    /// GameplayScene aktifken üzerine açılan overlay sahnelerini (TechTree, Discovery, ...)
    /// yönetir. Sabit tek bir sahne adına bağlı değildir - herhangi bir overlay sahnesi
    /// adı ile çağrılabilir.
    ///
    /// KRİTİK: Hiçbir sahne unload edilmez, sadece root objeler SetActive ile
    /// gizlenir/gösterilir. Bu sayede GameplayScene'deki SimulationManager tick'i
    /// (persistent, Splash sahnesinde yaşıyor) hangi overlay açık olursa olsun
    /// kesintisiz çalışmaya devam eder.
    ///
    /// Bu script'i Splash sahnesine, SimulationManager ile birlikte koyun
    /// (DontDestroyOnLoad ile persistent).
    /// </summary>
    public class OverlaySceneManager : MonoBehaviour
    {
        public static OverlaySceneManager Instance { get; private set; }

        [SerializeField] private string gameplaySceneName = "GameplayScene";

        private string _activeOverlaySceneName;

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

        /// <summary>
        /// GameplayScene'i gizler, verilen overlay sahnesini açar/gösterir.
        /// TechTree ve Discovery butonları bunu çağıracak: ShowOverlay("TechTreeScene"), ShowOverlay("DiscoveryScene").
        /// </summary>
        public void ShowOverlay(string overlaySceneName)
        {
            SetSceneRootObjectsActive(gameplaySceneName, false);

            if (!SceneManager.GetSceneByName(overlaySceneName).isLoaded)
            {
                SceneManager.LoadScene(overlaySceneName, LoadSceneMode.Additive);
            }
            else
            {
                SetSceneRootObjectsActive(overlaySceneName, true);
            }

            _activeOverlaySceneName = overlaySceneName;
        }

        /// <summary>
        /// Aktif overlay'i gizler, GameplayScene'i tekrar gösterir.
        /// TechTreeScene ve DiscoveryScene'deki "Geri Dön" butonu bunu çağıracak.
        /// </summary>
        public void HideOverlayAndReturnToGameplay()
        {
            if (!string.IsNullOrEmpty(_activeOverlaySceneName))
            {
                SetSceneRootObjectsActive(_activeOverlaySceneName, false);
                _activeOverlaySceneName = null;
            }

            SetSceneRootObjectsActive(gameplaySceneName, true);
        }

        private void SetSceneRootObjectsActive(string sceneName, bool active)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded)
                return;

            foreach (var rootObject in scene.GetRootGameObjects())
            {
                rootObject.SetActive(active);
            }
        }
    }
}
