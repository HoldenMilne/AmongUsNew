using System;
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
public class SendActivity : MonoBehaviour
{
    private const string pluginName = "com.lynxpoint.uplugin.UPluginClass";
    private static AndroidJavaClass _pluginClass;
    private static AndroidJavaObject _pluginInstance;
    public static string activityName="Null Client";

    public static AndroidJavaClass PluginClass
    {
        get
        {
            if (_pluginClass == null)
            {
                _pluginClass = new AndroidJavaClass(pluginName);
            }
            
            Debug.Log(_pluginClass + " < PLUGIN CLASS");

            return _pluginClass;
        }
    }
    
    public static AndroidJavaObject PluginInstance
    {
        get
        {
            if (_pluginInstance == null)
            {
                _pluginInstance = PluginClass.GetStatic<AndroidJavaObject>("instance");
            }

            Debug.Log(_pluginInstance + " < PLUGIN INST");
            return _pluginInstance;
        }
    }
//Y62M2X9
    private static int c = 0;
    public static void StartActivity(string s)
    {
        
    }
    
}
