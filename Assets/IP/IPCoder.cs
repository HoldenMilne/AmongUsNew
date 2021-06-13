using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;

namespace DefaultNamespace.IP
{
    public class IPCoder
    {
        static char[] characters;
        
        public static int[] codeToBytes(string code, DateTime now)
        {
            int i = 0;

            int[] D = GetDateMods(now);

            int p1=1,p2=1,p3=1,p4=1;
            int i1=0,i2=0,i3=0,i4;
            int ip = 0,port = 0;

            foreach(char c in code)
            {
                int ind = codeCharIndex(c);

                switch(i)
                {
                    case 0:
                    case 4:
                        p1 = ind-D[0];
                        while(p1<0)
                        {
                            p1+=36;
                        }
                        Debug.Log(i+":"+p1);
                        break;
                    case 1:
                    case 5:
                        // getIntChar(i1+d)+getChar(i2+d+d2)+getIntChar(i3+d+d3)+getChar(i4+d2+d3)+"";
                        p2 = ind-D[0]-D[1];
                        while(p2<0)
                        {
                            p2+=36;
                        }
                        Debug.Log(i+":"+p2);
                        break;

                    case 2:
                        p3 = ind - D[0]-D[2];
                        while(p3<0)
                        {
                            p3+=36;
                        }
                        Debug.Log("2"+":"+p3);
                        break;
                    case 3:
                        p4 = ind -D[1]-D[2];
                        while(p4<0)
                        {
                            p4+=36;
                        }
//i4+d2+d3
                        Debug.Log("3"+":"+p4);
                        ip=(p1<<12)|(p2<<8)|(p3<<4)|p4;

                        break;
                    case 6:
                        p3=ind-D[0]-D[1]-D[2];
                        while(p3<0)
                        {
                            p3+=36;
                        }
                        Debug.Log("6"+":"+p3);
                        Debug.Log("6"+"d0:"+D[0]);
                        Debug.Log("6"+"d1:"+D[1]);
                        Debug.Log("6"+"d2:"+D[2]);
                        
                        port=(p1<<11)|(p2<<5)|p3;
                        Debug.Log("6"+":"+port);
                        break;

                }
                i+=1;
            }
            
            Debug.Log("IP:"+ip+" PORT"+port);
            return new int[]{ip,port};
            
        }
        
        private static int[] GetDateMods(DateTime now) {
            int d = now.DayOfYear;
            int d2 = now.Day;
            int d3 = (int)now.DayOfWeek+1;

            Debug.Log(d+" < "+d2+  " <" + d3);
            int t = now.Hour;

            if(t<4)
            {
                d-=1;
                d2-=1;
                if(d3==0)
                    d3 = (int) DayOfWeek.Saturday+1;
                else d3 -= 1;
            }
            Debug.Log(d+" < "+d2+  " <" + d3);

            return new int[]{d,d2,d3};
        }
        
        static int codeCharIndex(char c)
        {
            if(characters == null)
            {
                makeCharacterArray();
            }
            int index = 0;
            foreach(char x in characters)
            {
                if(x==c) return index;
                index+=1;
            }

            return (int)(c-'0');
        }

        public static void makeCharacterArray()
        {
            characters = new char[36];
            int pos = 0;
            for (int i = 'A'; i <= 'Z'; i++)
            {
                characters[pos] = (char) (i);
                pos += 1;
            }

            for (int i = '0'; i <= '9'; i++)
            {
                characters[pos] = (char) (i);
                pos += 1;
            }
        }
        
        public static byte[] getIP(bool b)
        {
            foreach (NetworkInterface f in NetworkInterface.GetAllNetworkInterfaces())
                if (f.OperationalStatus == OperationalStatus.Up)
                    foreach (GatewayIPAddressInformation d in f.GetIPProperties().GatewayAddresses)
                    {
                        return d.Address.GetAddressBytes();

                    }
        //return bytes;
            return new byte[]{0,0,0,0};
        }
        public static byte[] getIP(string s)
        {
            string[] spl = s.Split('.');

            return new[]
            {
                Byte.Parse(spl[0]),
                Byte.Parse(spl[1]),
                Byte.Parse(spl[2]),
                Byte.Parse(spl[3]),
            };
        }


        public static byte[] getIP()
        {
            byte[] output = new byte[4];
            ADDRESSFAM Addfam = ADDRESSFAM.IPv4;
            LinkedList<byte[]> list = new LinkedList<byte[]>();
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;
            NetworkInterfaceType _type2 = NetworkInterfaceType.Ethernet;
 
            if ((item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2) && item.OperationalStatus == OperationalStatus.Up)
#endif
                {
                    
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        //IPv4
                        if (Addfam == ADDRESSFAM.IPv4)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                if (ip.Address.ToString() != "127.0.0.1")
                                    output = getIP(ip.Address.ToString());
                                list.AddLast(output);

                                if (ip.Address.ToString().StartsWith("192.168"))
                                    return output;
                            }
                        }

                        //IPv6
                        else if (Addfam == ADDRESSFAM.IPv6)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
                            {
                                if (ip.Address.ToString() != "127.0.0.1")
                                    output = getIP(ip.Address.ToString());
                                list.AddLast(output);
                                
                                if (ip.Address.ToString().StartsWith("192.168"))
                                    return output;
                            }
                        }
                    }
                }
            }

            return output;
        }
        
        public enum ADDRESSFAM
        {
            IPv4, IPv6
        }
    }
    
   
}
