using Services;
using UnityEngine;

namespace Prototyping
{
    public class ExitBattleModePrototype : MonoBehaviour
    {
        public void ExitBattleMode()
        {
            ServiceLocator.Instance.Get<ApplicationStateManager>().PopState();
        }
    }
}