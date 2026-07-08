using System.Collections;
using CardLinker.Simulation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CardLinker.SceneFlow
{
    /// <summary>
    /// Oturum sınırı geçişlerini yönetir: Splash -> MainMenu -> Gameplay -> MainMenu.
    /// OverlaySceneManager'dan farklı olarak burada sahneler GERÇEKTEN unload edilir
    /// (LoadSceneMode.Single) - çünkü bunlar birbirinden bağımsız oturum durumlarını
    /// temsil ediyor, aynı anda ikisi de aktif olmamalı.
    ///
    /// LoadingScene, geçiş sırasında additive olarak açılıp kapanan geçici bir
    /// arayüz katmanı. Hedef sahne Single modda yüklendiğinde önceki sahne ve
    /// LoadingScene otomatik olarak unload edilir (Unity'nin Single-load davranışı).
    ///
    /// Bu script'i Splash sahnesine, SimulationManager ve OverlaySceneManager ile
    /// birlikte koyun (DontDestroyOnLoad ile persistent).
    /// </summary>
    public class SessionFlowManager : MonoBehaviour
    {
        public static SessionFlowManager Instance { get; private set; }

        [SerializeField] private string mainMenuSceneName = "MainMenuScene";
        [SerializeField] private string gameplaySceneName = "GameplayScene";
        [SerializeField] private string loadingSceneName = "LoadingScene";

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

        /// <summary>Splash sahnesindeki açılış akışının sonunda çağrılır.</summary>
        public void GoToMainMenu()
        {
            StartCoroutine(TransitionRoutine(mainMenuSceneName));
        }

        /// <summary>MainMenu'deki "New Game" butonuna bağlanacak.</summary>
        public void StartNewGame()
        {
            SimulationWorld.Instance.Clear();
            StartCoroutine(TransitionRoutine(gameplaySceneName));
        }

        /// <summary>MainMenu'deki "Load Game" butonuna bağlanacak. Henüz implemente edilmedi.</summary>
        public void LoadGame()
        {
            // TODO: Save sisteminden GridContext verisi okunup SimulationWorld'e
            // yazılacak, ardından StartCoroutine(TransitionRoutine(gameplaySceneName))
            // çağrılacak. Şimdilik sadece bilgilendirme.
            Debug.LogWarning("[SessionFlowManager] LoadGame henüz implemente edilmedi.");
        }

        /// <summary>GameplayScene'deki "Menüye Dön" / "Quit to Menu" butonuna bağlanacak.</summary>
        public void QuitToMainMenu()
        {
            SimulationWorld.Instance.Clear();
            StartCoroutine(TransitionRoutine(mainMenuSceneName));
        }

        private IEnumerator TransitionRoutine(string targetSceneName)
        {
            yield return SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);

            // Single modda yükleme, önceki aktif sahneyi VE LoadingScene'i otomatik
            // unload eder (DontDestroyOnLoad işaretli persistent objeler hariç).
            yield return SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Single);
        }
    }
}
