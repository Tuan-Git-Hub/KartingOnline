using Mirror;
using UnityEngine;

public class StartFinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            if (other.GetComponentInParent<NetworkIdentity>() is NetworkIdentity identity)
            {
                if (identity.isLocalPlayer)
                {
                    GameManager.Instance.StartFinishLap();
                    //Debug.Log("Trigger StartFinishLine");
                }
            } 
        }
    }
}
