using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Discovery;
using StationsAndHubs.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    private Canvas loadingScreen;
    public void Start()
    {
        loadingScreen = GetComponent<Canvas>();
    }

    public float height;
    public Text[] chars;

    private bool isComplete;
    public IEnumerator Loading(String sceneName)
    {
        isComplete = false;
        var sins = new float[chars.Length];
        var param = new float[chars.Length];
        var rects = new RectTransform[chars.Length];
        for (var x = 0; x<param.Length;x++)
        {
            param[x] = - Mathf.PI / (float)chars.Length*x;
            rects[x] = chars[x].gameObject.GetComponent<RectTransform>();
        }
        
        float defY = rects[0].anchoredPosition.y;
        while (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            for (var x = 0; x<param.Length;x++)
            {
                sins[x] = height * Mathf.Clamp(Mathf.Sin((float)(Time.time+param[x])),0,float.PositiveInfinity);
                var a = rects[x].anchoredPosition;
                a.y = sins[x] + defY;
                rects[x].anchoredPosition = a;
            }
            yield return null;
            Debug.Log("loading");
        }

        isComplete = true;

    }

    private AsyncOperation loadingOp;

    public void StartLoad(SoundController sc, string scene,int mode = 0)
    {
        if(mode == 0)
            StartCoroutine(LoadScene(sc,scene));
        else if(mode == 1)
            StartCoroutine(LoadServer(sc,scene));
        else if(mode == 2)
            StartCoroutine(LoadClient(sc,scene));
    }

    public IEnumerator LoadScene(SoundController sc, string scene)
    {
        sc.MusicStop();
        yield return new WaitForSeconds(2f);
        
        sc.SoundStop();
        sc.PlayMusic("loading");
        
        loadingScreen.enabled = true;
        var loadController = FindObjectOfType<LoadingController>();
        StartCoroutine(loadController.Loading(scene));
        
        yield return new WaitForSeconds(3f);
        loadingOp = SceneManager.LoadSceneAsync(scene);
        //Start Corouting to Close Loading Canvas

        yield return new WaitUntil(SceneIsLoaded);
        Debug.Log("Load complete");
        loadingScreen.enabled = false;
    }
    
    public IEnumerator LoadServer(SoundController sc, string scene)
    {
        sc.MusicStop();
        yield return new WaitForSeconds(2f);
        
        sc.SoundStop();
        sc.PlayMusic("loading");
        
        loadingScreen.enabled = true;
        var loadController = FindObjectOfType<LoadingController>();
        StartCoroutine(loadController.Loading(scene));
        
        yield return new WaitForSeconds(3f);
        loadingOp = SceneManager.LoadSceneAsync(scene);
        //Start Corouting to Close Loading Canvas
        yield return new WaitUntil(SceneIsLoaded);
        
       // NetworkManager.singleton.StartServer();
        NetworkManager.singleton.StartServer();

        NetworkManager.singleton.GetComponent<NetworkDiscovery>().AdvertiseServer();
        Debug.Log("Load complete");
        loadingScreen.enabled = false;
    }
    
    public IEnumerator LoadClient(SoundController sc, string scene)
    {
        sc.MusicStop();
        yield return new WaitForSeconds(2f);
        
        sc.SoundStop();
        sc.PlayMusic("loading");
        
        loadingScreen.enabled = true;
        var loadController = FindObjectOfType<LoadingController>();
        StartCoroutine(loadController.Loading(scene));
        
        yield return new WaitForSeconds(3f);
        loadingOp = SceneManager.LoadSceneAsync(scene);
        //Start Corouting to Close Loading Canvas
        yield return new WaitUntil(SceneIsLoaded);
        FindObjectOfType<CustomNetworkManager>().StartClient();
        Debug.Log("Load complete");
        loadingScreen.enabled = false;
    }
    
    public bool SceneIsLoaded()
    {
        return loadingOp.isDone;
    }
}
