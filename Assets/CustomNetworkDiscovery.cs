using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Mirror;
using Mirror.Discovery;
using StationsAndHubs.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class CustomNetworkDiscovery : NetworkDiscoveryBase<DiscoveryRequest,DiscoveryResponse>
{
    public Text gameCode;
    public CustomNetworkManager manager;
    protected override DiscoveryResponse ProcessRequest(DiscoveryRequest request, IPEndPoint endpoint)
    {
        var disRes = new DiscoveryResponse();
        disRes.GameCode = gameCode.text;
        disRes.uri = new UriBuilder(manager.networkAddress).Uri;
        return disRes;
    }
    protected override DiscoveryRequest GetRequest()
    {
        return new DiscoveryRequest();
    }
    protected override void ProcessResponse(DiscoveryResponse response, IPEndPoint endpoint)
    {
        // TODO: a server replied,  do something with the response such as invoking a unityevent
        // ???????
    }
}
