using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class TestLobby : MonoBehaviour
{
   

    //Function to start unity services
    private async void Start()
    {
        //Start Unity Services
        await UnityServices.InitializeAsync();

        //Adding an event listener
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed In " + AuthenticationService.Instance.PlayerId);
        };

        //Letting users sign in anonymously
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
}
