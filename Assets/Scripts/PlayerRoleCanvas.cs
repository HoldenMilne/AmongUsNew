using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Scripts
{
    public class PlayerRoleCanvas : MonoBehaviour
    {
        public Canvas canvas;
        public CanvasGroup group;
        public CanvasGroup group2;
        public CanvasGroup otherImpostersGroup;
        public Image image;
        public Text text;
        public Text otherImpostersText;
        public Color crewColor = new Color(0x67/0xFF,0xAE/0xFF,0xEF/0xFF);
        public Color imposterColor = new Color(0xA2/0xFF,0x37/0xFF,0x2C/0xFF);

        private bool running = false;
        public Sprite imposter, crewmate;
        public void SetImposter(string inames)
        {
            image.sprite = imposter;
            text.color = imposterColor;
            text.text = "Imposter";
            otherImpostersText.text = inames;
            otherImpostersGroup.alpha = 1;
        }
        
        public void SetCrewmate()
        {
            image.sprite = crewmate;
            text.color = crewColor;
            text.text = "Crewmate";
            otherImpostersGroup.alpha = 0;
        }
        
        public void Set(bool isImposter,string inames)
        {
            if (isImposter) SetImposter(inames);
            else SetCrewmate();
        }

        public IEnumerator Show()
        {
            if (running) yield break;
            running = true;
            group2.alpha = 1;
            group.alpha = 0;
            canvas.enabled = true;
            while (group.alpha<1)
            {
                group.alpha += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(2);
            while (group2.alpha>0)
            {
                group2.alpha -= Time.deltaTime;
                yield return null;
            }
            canvas.enabled = false;            
            running = false;

        }
    }
}