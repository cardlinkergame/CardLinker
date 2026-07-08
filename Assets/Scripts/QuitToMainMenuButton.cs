using CardLinker.SceneFlow;
using UnityEngine;

namespace CardLinker.UI
{
    /// <summary>
    /// GameplayScene'deki "Menüye Dön" butonuna Button.OnClick üzerinden bağlanacak.
    /// SimulationWorld'ü temizler ve MainMenuScene'e döner (oturum sonu).
    /// </summary>
    public class QuitToMainMenuButton : MonoBehaviour
    {
        public void OnClick()
        {
            SessionFlowManager.Instance.QuitToMainMenu();
        }
    }
}
