using System.Collections;
using UnityEngine;

namespace CardLinker.SceneFlow
{
    /// <summary>
    /// Splash sahnesine konur. Belirli bir süre bekler (logo gösterimi vs.),
    /// sonra SessionFlowManager üzerinden MainMenu'ye geçer.
    /// </summary>
    public class SplashController : MonoBehaviour
    {
        [SerializeField] private float splashDuration = 1.5f;

        private void Start()
        {
            StartCoroutine(WaitThenGoToMainMenu());
        }

        private IEnumerator WaitThenGoToMainMenu()
        {
            yield return new WaitForSeconds(splashDuration);
            SessionFlowManager.Instance.GoToMainMenu();
        }
    }
}
