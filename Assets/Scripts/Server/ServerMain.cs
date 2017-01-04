using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ServerMain : MonoBehaviour {

    public Button Button_Server_On_Off_;

    public Transform Plane_;

    public AppServer App_Server_;

    private WebCamTexture web_cam_texture_;

    private void InitializeButton()
    {
        Button_Server_On_Off_.onClick.AsObservable().Subscribe(_ =>
        {
            Text text = Button_Server_On_Off_.GetComponentInChildren<Text>();
            string title = string.Empty;
            if (text.text.Equals("Start Server"))
            {
                title = "Stop Server";
                App_Server_.StartServer(web_cam_texture_);
            }
            else
            {
                title = "Start Server";
                App_Server_.StopServer();
            }
            text.text = title;
        });
    }

    void Start () {

        float height = Camera.main.orthographicSize * 2.0f;
        float width = height * Screen.width / Screen.height;
        Plane_.localScale = new Vector3(width * 0.1f, height * 0.1f, 1.0f);
        WebCamDevice[] device = WebCamTexture.devices;

        web_cam_texture_ = new WebCamTexture(320, 240, 15);

        if (device.Length > 0)
        {
            web_cam_texture_.deviceName = device[0].name;
            Plane_.GetComponent<Renderer>().material.mainTexture = web_cam_texture_;
            web_cam_texture_.Play();
        }

        InitializeButton();

    }
}
