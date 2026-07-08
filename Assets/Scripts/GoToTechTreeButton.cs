using CardLinker.SceneFlow;
using UnityEngine;

namespace CardLinker.UI
{
    /// <summary>
    /// GameplayScene'deki "TechTree" butonuna Button.OnClick üzerinden bağlanacak.
    /// </summary>
    public class GoToTechTreeButton : MonoBehaviour
    {
        private const string TechTreeSceneName = "TechTreeScene";

        public void OnClick()
        {
            OverlaySceneManager.Instance.ShowOverlay(TechTreeSceneName);
        }
    }
}
