using CardLinker.SceneFlow;
using UnityEngine;

namespace CardLinker.UI
{
    /// <summary>
    /// GameplayScene'deki "TechTree" butonuna Button.OnClick üzerinden bağlanacak.
    /// </summary>
    public class GoToTechTreeButton : MonoBehaviour
    {
        public void OnClick()
        {
            SceneFlowManager.Instance.GoToTechTree();
        }
    }
}
