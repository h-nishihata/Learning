using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IEnumeratorTest : MonoBehaviour
{
    public Material mat;
    public List<int> list;

    // Start is called before the first frame update
    void Start()
    {
        //this.PrintEnumerator(FadeIn());
        //this.PrintEnumerable(list);

        var enumerator = this.GetEnumerator();
        Debug.Log("Loop start");
        /*
            while (enumerator.MoveNext())
            {
                Debug.Log(enumerator.Current);
            }
        */
        foreach (var value in this)
        {
            Debug.Log(value);
        }
    }

    private IEnumerator FadeIn()
    {
        for (float i = 0f; i < 1f; i += 0.1f)
        {
            Color c = mat.color;
            c.a = i;
            mat.color = c;
            yield return new WaitForSeconds(0.05f);
        }
    }

    void PrintEnumerator(IEnumerator enumerator)
    {
        while (enumerator.MoveNext())
        {
            Debug.Log(enumerator.Current);
        }
    }

    void PrintEnumerable(IEnumerable enumerable)
    {
        System.Collections.IEnumerator e = enumerable.GetEnumerator();
        try
        {
            while (e.MoveNext())
            {
                object v = e.Current;
                Debug.Log(v);
            }
        }
        finally
        {
            System.IDisposable d = e as System.IDisposable;
            if (d != null)
                d.Dispose();
        }
        /*
        foreach (var value in enumerable)
        {
            Debug.Log(value);
        }
        */
    }

    public IEnumerator GetEnumerator()
    {
        Debug.Log("GetEnumerator: null");
        yield return null;

        Debug.Log("GetEnumerator: WaitForEndOfFrame");
        yield return new WaitForEndOfFrame();

        Debug.Log("GetEnumerator: WaitForSeconds(1f)");
        yield return new WaitForSeconds(1f);
    }
}
