using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class ObjectMover : MonoBehaviour
{
    [SerializeField] ComputeShader _computeShader;
    [SerializeField] Transform _MovingObj;

    private ComputeBuffer _buffer;
    private Vector3 center = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        _buffer = new ComputeBuffer(1, Marshal.SizeOf(typeof(Vector2)));
        _computeShader.SetBuffer(_computeShader.FindKernel("CSMain"), "ResultBuffer", _buffer);
    }

    // Update is called once per frame
    void Update()
    {
        _computeShader.SetFloats("position", center.x, center.y);
        _computeShader.SetFloat("time", Time.time);
        _computeShader.Dispatch(0, 8, 8, 1);

        var data = new float[2];
        _buffer.GetData(data);

        Vector3 pos = _MovingObj.transform.localPosition;
        pos.x = data[0];
        pos.y = data[1];
        _MovingObj.transform.localPosition = pos;
    }

    private void OnDestroy()
    {
        _buffer.Release();
    }
}
