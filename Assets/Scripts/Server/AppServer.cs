using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using WebSocketSharp.Server;

public class ProtocolMessage
{
    public static readonly string Streaming_On = "streaming_on";
    public static readonly string Streaming_Off = "streaming_off";
}

public class MyHome : WebSocketBehavior
{
    private AppServer wc_;
    public MyHome(AppServer wc)
    {
        wc_ = wc;
    }

    protected override void OnOpen()
    {
        Debug.Log("server: the client is connected!");
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        if(e.IsText)
        {
            string data = e.Data;
            if (data.Equals(ProtocolMessage.Streaming_On))
            {
                wc_.EnQueueStartStream();
            }
            else if (data.Equals(ProtocolMessage.Streaming_Off))
            {
                wc_.EnQueueStopStream();
            }
        }
    }
}

public class AppServer : MonoBehaviour
{
    WebCamTexture web_cam_texture_;

    WebSocketServer wssv;

    WebSocketServiceManager wssv_manager_;

    CompositeDisposable disposables = new CompositeDisposable();

    Queue<Action> queue_action_;

    public void EnQueueStartStream()
    {
        queue_action_.Enqueue(StartStream);
    }

    public void EnQueueStopStream()
    {
        queue_action_.Enqueue(StopStream);
    }

    private void StartStream()
    {
        Observable.IntervalFrame(5).Subscribe(x =>
        {
            Color32[] color_array = web_cam_texture_.GetPixels32();

            uint[] uint_array = new uint[color_array.Length];
            for (int i = 0; i < color_array.Length; ++i)
            {
                uint_array[i] = ColorConverter.ColorToUInt(color_array[i]);
            }

            byte[] array = SerializeHelper.ObjectToByteArraySerialize(uint_array);
            //Debug.LogFormat("before compress array = {0}", array.Length);
            array = CompressHelper.Compress(array);
            //Debug.LogFormat("after compress array = {0}", array.Length);
            wssv_manager_.Broadcast(array);
        }).AddTo(disposables);
    }

    private void StopStream()
    {
        disposables.Clear();
    }

    public void StartServer(WebCamTexture web_cam_texture)
    {
        web_cam_texture_ = web_cam_texture;
        wssv.Start();
    }

    public void StopServer()
    {
        wssv.Stop();
    }

    void Awake()
    {
        queue_action_ = new Queue<Action>();
    }

    void Start () {
        wssv = new WebSocketServer(9999);
        wssv.AddWebSocketService<MyHome>("/myhome", () => new MyHome(this));

        wssv_manager_ = wssv.WebSocketServices;

        queue_action_.ObserveEveryValueChanged(queue => queue.Count > 0)
            .Where(x => x)
            .Subscribe(_ =>
            {
                for (; queue_action_.Count > 0;)
                {
                    queue_action_.Dequeue()();
                }
            });
    }
}
