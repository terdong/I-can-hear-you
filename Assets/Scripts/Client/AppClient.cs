using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class AppClient : MonoBehaviour {

    public Button Button_Connect_;

    public Button Button_Request_Stream_;

    public Transform Plane_;

    private WebSocket ws;

    private Texture2D plan_texture_;

    private Color32[] color_array_;

    private Queue<Action> queue_action_;

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

    private void EnQueueStartStream()
    {
        queue_action_.Enqueue(()=>
        {
            plan_texture_.SetPixels32(color_array_);
            plan_texture_.Apply();
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

        using (ws = new WebSocket("ws://teamgehem.com:9999/myhome"))
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
                    uint[] uint_array = SerializeHelper.Deserialize<uint[]>(e.RawData);

                    color_array_ = new Color32[uint_array.Length];
                    for(int i=0; i< uint_array.Length; ++i)
                    {
                        color_array_[i] = ColorConverter.UIntToColor(uint_array[i]);
                    }
                    EnQueueStartStream();
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
