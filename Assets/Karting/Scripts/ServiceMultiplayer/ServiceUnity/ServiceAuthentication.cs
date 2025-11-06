using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class ServiceAuthentication : MonoBehaviour
{
    async void Awake()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
        }
        else
        {
            Debug.Log("Already signed in as: " + AuthenticationService.Instance.PlayerId);
        }
    }
}
