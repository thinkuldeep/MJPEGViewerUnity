using UnityEngine;
using System;
using UnityEngine.UI;

public class MjpegViewer : MonoBehaviour
{
   
    [SerializeField] private InputField inputField;
    
    const int ChunkSize = 4;
    const int InitWidth = 2;
    const int InitHeight = 2;
    private Texture2D _tex;
    private bool _updateFrame = false;
    private MjpegStreamProcessor _mjpegStream;
    private float _deltaTime = 0.0f;
    private float _mjpegDeltaTime = 0.0f;
    private Renderer _renderer;
    private string streamAddress;

    public void Start()
    {
        if (_mjpegStream != null)
        {
            _updateFrame = false;
            _deltaTime = 0f;
            _mjpegStream.StopStream();
        }
        _mjpegStream = new MjpegStreamProcessor(ChunkSize * 1024);
        _mjpegStream.FrameReady += OnMjpegStreamFrameReady;
        _mjpegStream.Error += OnMjpegStreamError;
        streamAddress = inputField.text;
        Uri mjpegAddress = new Uri(streamAddress);
        _mjpegStream.ParseStream(mjpegAddress);
        _tex = new Texture2D(InitWidth, InitHeight, TextureFormat.ARGB32, false);
        _renderer = GetComponent<Renderer>();
        _updateFrame = true;
    }
    private void OnMjpegStreamFrameReady(object sender, FrameReadyEventArgs e)
    {
        _updateFrame = true;
    }
    void OnMjpegStreamError(object sender, ErrorEventArgs e)
    {
        Debug.Log("Error received while reading the MJPEG.");
    }
    
    void Update()
    {
        _deltaTime += Time.deltaTime;

        if (_updateFrame)
        {
            _tex.LoadImage(_mjpegStream.CurrentFrame);
            // tex.Apply();
            // Assign texture to renderer's material.
            _renderer.material.mainTexture = _tex;
            _updateFrame = false;

            _mjpegDeltaTime += (_deltaTime - _mjpegDeltaTime) * 0.2f;

            _deltaTime = 0.0f;
        }
    }
    void OnDestroy()
    {
        _mjpegStream.StopStream();
    }
}