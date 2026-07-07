using CardLinker.SceneFlow;
using UnityEngine;

namespace CardLinker.UI
{
    /// <summary>
    /// TechTreeScene'deki "GameplayScene'e Dön" butonuna Button.OnClick üzerinden bağlanacak.
    /// </summary>
    public class GoToGameplayButton : MonoBehaviour
    {
        public void OnClick()
        {
            SceneFlowManager.Instance.GoToGameplay();
        }
    }
}
