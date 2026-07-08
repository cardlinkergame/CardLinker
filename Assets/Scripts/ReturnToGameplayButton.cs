using CardLinker.SceneFlow;
using UnityEngine;

namespace CardLinker.UI
{
    /// <summary>
    /// TechTreeScene VE DiscoveryScene'de aynı şekilde kullanılabilir -
    /// hangi overlay aktifse onu gizleyip GameplayScene'e döner.
    /// Button.OnClick üzerinden bağlanacak.
    /// </summary>
    public class ReturnToGameplayButton : MonoBehaviour
    {
        public void OnClick()
        {
            OverlaySceneManager.Instance.HideOverlayAndReturnToGameplay();
        }
    }
}
