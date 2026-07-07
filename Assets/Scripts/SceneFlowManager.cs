using UnityEngine;
using UnityEngine.SceneManagement;

namespace CardLinker.SceneFlow
{
    /// <summary>
    /// GameplayScene ve TechTreeScene arasında additive geçiş yapar.
    /// KRİTİK: GameplayScene TechTree'ye geçerken UNLOAD EDİLMEZ, sadece
    /// içindeki root objeler SetActive(false) yapılır. Bu sayede
    /// SimulationManager (persistent, boot sahnesinde) Update() çağırmaya
    /// devam eder ve SimulationWorld.Tick() kesintisiz çalışır - GameplayScene
    /// view objeleri görünmese bile.
    ///
    /// Bu script'i, SimulationManager ile birlikte bir "Boot" sahnesine koyun
    /// (DontDestroyOnLoad ile persistent). Boot sahnesi ilk açılışta
    /// GameplayScene'i additive yükler.
    /// </summary>
    public class SceneFlowManager : MonoBehaviour
    {
        public static SceneFlowManager Instance { get; private set; }

        [SerializeField] private string gameplaySceneName = "GameplayScene";
        [SerializeField] private string techTreeSceneName = "TechTreeScene";

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

        private void Start()
        {
            // Boot sahnesi açılınca GameplayScene'i additive yükle ve göster.
            if (!SceneManager.GetSceneByName(gameplaySceneName).isLoaded)
            {
                SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Additive);
            }
        }

        /// <summary>UI'daki "TechTree" butonuna bağlanacak.</summary>
        public void GoToTechTree()
        {
            SetSceneRootObjectsActive(gameplaySceneName, false);

            if (!SceneManager.GetSceneByName(techTreeSceneName).isLoaded)
            {
                SceneManager.LoadScene(techTreeSceneName, LoadSceneMode.Additive);
            }
            else
            {
                SetSceneRootObjectsActive(techTreeSceneName, true);
            }
        }

        /// <summary>TechTreeScene'deki "GameplayScene'e Dön" butonuna bağlanacak.</summary>
        public void GoToGameplay()
        {
            SetSceneRootObjectsActive(techTreeSceneName, false);
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
