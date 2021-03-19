using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

namespace Visuals
{
    public abstract class VisualsUpdater
    {
        public abstract void Update(Canvas canvas, Controller controller);
    }

    public class PhoneMainMenuUpdater : VisualsUpdater
    {
        public override void Update(Canvas canvas, Controller controller)
        {
            Text[] texts = canvas.GetComponentsInChildren<Text>();
            foreach (var t in texts)
            {
                t.text = GetText(t,controller);
            }
        }

        private string GetText(Text text, Controller c)
        {
            switch (text.gameObject.tag)
            {
                case "isconn":
                    return "Connected: "+c.IsReady();
                    break;
            }

            return "??? (Err)";
        }
    }
}