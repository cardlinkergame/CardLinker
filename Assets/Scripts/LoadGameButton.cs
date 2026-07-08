using CardLinker.SceneFlow;
using UnityEngine;

namespace CardLinker.UI
{
    /// <summary>
    /// MainMenuScene'deki "Load Game" butonuna Button.OnClick üzerinden bağlanacak.
    /// Şu an SessionFlowManager.LoadGame() henüz implemente edilmedi (TODO).
    /// </summary>
    public class LoadGameButton : MonoBehaviour
    {
        public void OnClick()
        {
            SessionFlowManager.Instance.LoadGame();
        }
    }
}
