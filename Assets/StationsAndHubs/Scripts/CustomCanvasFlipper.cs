using System.Collections;
using UnityEngine;

namespace StationsAndHubs.Scripts
{
    public class CustomCanvasFlipper : CanvasFlipper
    {
        public Canvas canvas1,canvas2;

        public void Flip(int i)
        {
            if (i==0)
            {
                canvas1.enabled = true;
                canvas2.enabled = false;
            }else{
                canvas1.enabled = true;
                canvas2.enabled = false;
                
            }
        }

        public void ShowCanvas1ForSeconds(int i)
        {
            StartCoroutine(Canvas1ForSeconds(i));
        }

        private IEnumerator  Canvas1ForSeconds(int i)
        {
            var canvas1 = GameObject.Find("SameNameErrorCanvas").GetComponent<Canvas>();
            canvas1.enabled = true;
            yield return new WaitForSeconds(i);
            canvas1.enabled = false;
        }
    }
}