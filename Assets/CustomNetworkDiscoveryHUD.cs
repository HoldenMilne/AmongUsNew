using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using kcp2k;
using Mirror;
using Mirror.Discovery;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Object = UnityEngine.Object;

[RequireComponent(typeof(NetworkDiscovery))]
public class CustomNetworkDiscoveryHUD : NetworkDiscoveryHUD
{
    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
    public ScrollView scrollView;
    public Transform spawnpoint;
    public GameObject template;
    public RectTransform content;
    Vector2 scrollViewPos = Vector2.zero;

    //public NetworkDiscovery networkDiscovery;

    private static bool NeedsUpdate = false;
    public void Start()
    {
        discoveredServers.Clear();
        if (networkDiscovery == null) networkDiscovery = GetComponent<NetworkDiscovery>();
        networkDiscovery.StartDiscovery();
        StartCoroutine(PanelUpdater());
    }

    private IEnumerator PanelUpdater()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            UpdatePanel();
            NeedsUpdate = false;
        }
    }

    public void OnDiscoveredServer(ServerResponse info)
    {
        discoveredServers[info.serverId] = info;
        Debug.Log("SERVER");
        NeedsUpdate = true;
    }

    private void UpdatePanel()
    {
        Debug.Log("Update 1");
        if (SceneManager.GetActiveScene().name != "Intro Scene") return;
        foreach (Transform child in spawnpoint.transform) {
            Destroy(child.gameObject);
        }
        Debug.Log("Update 2");

        var height = 0f;
        var i = 0;
        foreach (var server in discoveredServers.Values)
        {
            var go = Instantiate(template, spawnpoint);
            var rt = go.GetComponent<RectTransform>();
            var y = rt.sizeDelta.y;
            
            height += y;
            // could I try casting server to DiscoveryResponse and
            // get server.gameCode???
            go.GetComponentInChildren<Text>().text = server.EndPoint.Address.ToString(); //server.EndPoint.Address.ToString() + ":"+server.EndPoint.Port.ToString();
            go.GetComponentInChildren<Button>().gameObject.GetComponent<DiscoveredServerController>().Server = server;
            if ((i & 1) == 1) // is odd
            {
                var img = go.GetComponent<Image>();
                var c = img.color;
                c.a -= .1f;
                img.color = c;
            }
            
            var anc = rt.anchoredPosition;
            anc.y -= y*i;
            rt.anchoredPosition = anc;


            i += 1;
        }

        var sd = content.sizeDelta;
        sd.y = height;
        content.sizeDelta = sd;
    }

    public void Connect(Uri uri,string ip="",string port="")
    {
        if(uri==null)
        foreach (var info in discoveredServers.Values)
        {
            Debug.Log(info.uri);
            Debug.Log("");

            
            if (info.EndPoint.Address.ToString() == ip)
            {
                NetworkManager.singleton.networkAddress = ip;
                //NetworkManager.singleton.GetComponent<KcpTransport>().Port = ushort.Parse(port);
                NetworkManager.singleton.StartClient(info.uri);
            }
        }
        else
        {
            NetworkManager.singleton.StartClient(uri);
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (networkDiscovery == null)
        {
            networkDiscovery = GetComponent<NetworkDiscovery>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, OnDiscoveredServer);
            UnityEditor.Undo.RecordObjects(new Object[] { this, networkDiscovery }, "Set NetworkDiscovery");
        }
    }
#endif
}

public class DiscoveryRequest : NetworkMessage
{
    public string language="en";

    // Add properties for whatever information you want sent by clients
    // in their broadcast messages that servers will consume.
}

public class DiscoveryResponse : NetworkMessage
{
    // you probably want uri so clients know how to connect to the server
    public Uri uri;

    public string GameCode;

    // Add properties for whatever information you want the server to return to
    // clients for them to display or consume for establishing a connection.
}