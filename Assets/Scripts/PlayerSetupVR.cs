using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetupVR : NetworkBehaviour
{
    // All components to disable for nonlocal players //
    [SerializeField]
    Behaviour[] ComponentsToDisable;

    void Start()
    {
        if (!isLocalPlayer)
        {
            // Disable Components for non-local players
            DisableComponents();
        }
    }

    void DisableComponents()
    {
        foreach (var iter in ComponentsToDisable)
        {
            iter.enabled = false;
        }
    }

}
