using UnityEngine;


//[ExecuteInEditMode]
public class WallSetter : MonoBehaviour
{
    public bool setPlacerPosition = false;
    public bool setObjectPosition = false;

    public bool horizontal = false;
    public RectTransform placer;
    // Start is called before the first frame update
    void Start()
    {
        SetObjectPosition();
        SetObjectSize();
    }

    // Update is called once per frame
    void Update()
    {
        if (setPlacerPosition)
        {
            SetPlacerPosition();
            setPlacerPosition = false;
        }

        if (setObjectPosition)
        {
            SetObjectPosition();
            SetObjectSize();
            setObjectPosition = false;
            
        }
        

    }

    private void SetObjectSize()
    {
        if (horizontal)
        {
            var y = (Camera.main.ScreenToWorldPoint(placer.rect.center) -
                     Camera.main.ScreenToWorldPoint(placer.rect.center + Vector2.up * placer.rect.yMax)).magnitude /4f;
            var bc = transform.GetComponent<BoxCollider2D>();
            bc.size = new Vector2(bc.size.x, y);
        }

        else
        {
            var x = (Camera.main.ScreenToWorldPoint(placer.rect.center) -
                     Camera.main.ScreenToWorldPoint(placer.rect.center + Vector2.right * placer.rect.xMax)).magnitude /4f;
            var bc = transform.GetComponent<BoxCollider2D>();
            bc.size = new Vector2(x, bc.size.y);
        }
    }


    private void SetObjectPosition()
    {
        transform.position = placer.position;
        
        
    }

    private void SetPlacerPosition()
    {
        placer.position = transform.position;
    }

}
