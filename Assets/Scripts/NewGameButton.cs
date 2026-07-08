using CardLinker.SceneFlow;
using UnityEngine;

namespace CardLinker.UI
{
    /// <summary>
    /// MainMenuScene'deki "New Game" butonuna Button.OnClick üzerinden bağlanacak.
    /// </summary>
    public class NewGameButton : MonoBehaviour
    {
        public void OnClick()
        {
            SessionFlowManager.Instance.StartNewGame();
        }
    }
}
