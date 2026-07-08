using CardLinker.SceneFlow;
using UnityEngine;

namespace CardLinker.UI
{
    /// <summary>
    /// GameplayScene'deki "Discovery" butonuna Button.OnClick üzerinden bağlanacak.
    /// </summary>
    public class GoToDiscoveryButton : MonoBehaviour
    {
        private const string DiscoverySceneName = "DiscoveryScene";

        public void OnClick()
        {
            OverlaySceneManager.Instance.ShowOverlay(DiscoverySceneName);
        }
    }
}
