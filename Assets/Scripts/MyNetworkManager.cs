using MLAPI;
using MLAPI.Transports.UNET;

public class MyNetworkManager : NetworkManager
{
    /*UNetTransport uNetTransport;*/

    private void Start()
    {
        if (Singleton == null)
            SetSingleton();
    }
}
