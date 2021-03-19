using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorizeGhost : MonoBehaviour
{
    public static int[] colors = new int[]
    {
        0xc51111, 0x132ed1, 0x117f2d, 0xed54ba, 0xef7d0e,
        0xf6f658,0x3f474e,0xd6e0f0,0x6b31bc,0x71491e,
        0x38fedb,0x50ef39,0x1d9853,0x918977
    };

    public static Color? color = null;

    public Image ltd;
    // Start is called before the first frame update
    void Start()
    {
        SetColor(128);
        
        if (ltd == null)
            ltd = GetComponent<Image>();

        SetGhost();
    }

    private void SetGhost()
    {
        if (ltd != null && color!=null)
            ltd.color = (Color)color;
    }

    public static Color GetRandomColor(int a = 256)
    {
        var r1 = new System.Random().Next(0,colors.Length);
        var r2 = new System.Random().Next(0,colors.Length);
        
        int col = colors[r2 % (colors.Length)];
        if ((r1 % 10) == 0)
        {
            col = colors[r2 % (colors.Length - 2)];
            
        }

        return ConvertColor(col,a);
    }

    private static Color ConvertColor(int col, int a = 256)
    {
        int r=0, b=0,g=0;
        
        r = (col & (0xFF << 16))>>16;
        g = (col & (0xFF << 8))>>8;
        b = (col & (0xFF << 0))>>0;

        Debug.Log(r+ " : " + g + " : " + b );
        return new Color(r/256f,g/256f,b/256f,a/256f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void SetColor(int a = 256)
    {
        if (color == null)
        {
            color = GetRandomColor(a);
        }
    }

    public static Color GetColor(int i,int a = 256)
    {
        return ConvertColor(colors[i],a);
    }
}
