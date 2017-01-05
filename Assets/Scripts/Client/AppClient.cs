using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

//public static class KBPSCounter
//{
//    public static int BufferSize = 1;  // 샘플 수를 바꾸려면 여기를 바꾸는
//    public static IReadOnlyReactiveProperty<float> Current { get; private set; }

//    static KBPSCounter()
//    {
//        Current = Observable.EveryUpdate()
//            .Select(_ => Time.deltaTime)
//            .Buffer(BufferSize, 1)
//            .Select(x =>
//            {
//                float total = 0;
//                float average = 0;
//                for(int i=0; i<x.Count; ++i)
//                {
//                    total += x[i];
//                }
//                average = total / x.Count;
//                Debug.Log(average);
//                return 1.0f / (average * 0.01f);
//            })
//            .ToReadOnlyReactiveProperty();
//    }
//}

public class AppClient : MonoBehaviour {

    private static readonly string Data_Streaming_Format_ = "data streaming {0}KB/s";
    private static readonly string Elapsed_Time_Format_ = "elapsed time {0}";

    public Button Button_Connect_;

    public Button Button_Request_Stream_;

    public Text Text_KBps_;
    public Text Text_Elapsed_Time_;

    public Transform Plane_;

    private WebSocket ws;

    private Texture2D plan_texture_;

    private Color32[] color_array_;

    private Queue<Action> queue_action_;

    private float buffer_size = 0;

    public void SendMessage_()
    {
        ws.Send("testest");
    }

    private void InitializeButton()
    {
        Button_Connect_.onClick.AsObservable().Subscribe(_ =>
        {
            Text text = Button_Connect_.GetComponentInChildren<Text>();
            string title = string.Empty;
            if (text.text.Equals("Connect"))
            {
                title = "Disconnect";
                ws.Connect();
            }
            else
            {
                title = "Connect";
                ws.Close();
            }
            text.text = title;
        });

        Button_Request_Stream_.onClick.AsObservable().Subscribe(_ =>
        {
            Text text = Button_Request_Stream_.GetComponentInChildren<Text>();
            string title = string.Empty;
            if (text.text.Equals("Request Stream"))
            {
                title = "Stop Stream";
                ws.Send(ProtocolMessage.Streaming_On);
            }
            else
            {
                title = "Request Stream";
                ws.Send(ProtocolMessage.Streaming_Off);
            }
            text.text = title;
        });
    }

    void Awake()
    {
        queue_action_ = new Queue<Action>();

        InitializeButton();
    }

    void Start () {

        float height = Camera.main.orthographicSize * 2.0f;
        float width = height * Screen.width / Screen.height;
        Plane_.localScale = new Vector3(width * 0.1f, height * 0.1f, 1.0f);

        plan_texture_ = new Texture2D(320, 240, TextureFormat.RGBA32, false);

        Plane_.GetComponent<Renderer>().material.mainTexture = plan_texture_;

        Observable.Interval(TimeSpan.FromSeconds(1))
            .Where(_ => ws.IsAlive && buffer_size > 0)
            .Subscribe(t =>
            {
                buffer_size = buffer_size * 0.001f;
                //Debug.LogFormat("t = {0}, KBps = {1}", t, );
                Text_KBps_.text = string.Format(Data_Streaming_Format_, buffer_size);
                Text_Elapsed_Time_.text = string.Format(Elapsed_Time_Format_, t);
                buffer_size = 0;
            });

        using (ws = new WebSocket("ws://127.0.0.1:9999/myhome"))
        {
            ws.OnOpen += (sender, e) =>
            {
                Debug.Log("client: connect success!");
                Button_Request_Stream_.interactable = true;
            };

            ws.OnMessage += (sender, e) =>
            {
                if (e.IsBinary)
                {
                    byte [] raw_data = e.RawData;
                    buffer_size += raw_data.Length;
                    //Debug.LogFormat("before decompress array = {0}", raw_data.Length);
                    raw_data = CompressHelper.Decompress(raw_data);
                    //Debug.LogFormat("after decompress array = {0}", raw_data.Length);
                    uint[] uint_array = SerializeHelper.Deserialize<uint[]>(raw_data);

                    color_array_ = new Color32[uint_array.Length];
                    for(int i=0; i< uint_array.Length; ++i)
                    {
                        color_array_[i] = ColorConverter.UIntToColor(uint_array[i]);
                    }

                    queue_action_.Enqueue(() =>
                    {
                        plan_texture_.SetPixels32(color_array_);
                        plan_texture_.Apply();
                    });
                }
                else if(e.IsText)
                {
                    Debug.Log("myhome says: " + e.Data);
                }

            };

            ws.OnClose += (sender, e) =>
            {
                Button_Request_Stream_.interactable = false;
            };
        }

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
