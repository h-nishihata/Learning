using UnityEngine;
using System.Collections;

public class Easing : MonoBehaviour
{
    public AnimationCurve curve;
    public Vector3 startPos;
    public Vector3 endPos;
    private float duration = 10.0f;

    void Start()
    {
        curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        StartCoroutine(this.Move(startPos, endPos, curve, duration));
    }

    IEnumerator Move(Vector3 startPos, Vector3 endPos, AnimationCurve curve, float duration)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime <= duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, curve.Evaluate(elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            //Debug.Log(elapsedTime);
            yield return null;
        }
    }
}