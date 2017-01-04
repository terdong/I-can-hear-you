using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MainScene : MonoBehaviour {

    public Material mt;

    public Camera camera;

    public Image image;

    public GameObject plane2;

    public WebCamTexture cam;

    Texture2D texture;

    Color32[] data;

    // Use this for initialization
    void Start () {




        //Renderer _renderer = camera.GetComponent<Renderer>();
        //_renderer.material.mainTexture = cam;
        //mt.mainTexture = cam;
        //image.GetComponent<Renderer>().material.mainTexture = cam;

        texture = new Texture2D(320, 240, TextureFormat.RGBA32, false);

        //plane2.GetComponent<Renderer>().material.mainTexture = texture;

        //var colors = new Color32[3];
        //colors[0] = Color.red;
        //colors[1] = Color.green;
        //colors[2] = Color.blue;
        //var mipCount = Mathf.Min(3, texture.mipmapCount);
        //for (var mip = 0; mip < mipCount; ++mip)
        //{
        //    var cols = texture.GetPixels32(mip);
        //    for (var i = 0; i < cols.Length; ++i)
        //    {
        //        cols[i] = Color32.Lerp(cols[i], colors[mip], 0.33f);
        //    }
        //    texture.SetPixels32(cols, mip);
        //}

        //// actually apply all SetPixels32, don't recalculate mip levels
        //texture.Apply(false);

        //string[] strs = Microphone.devices;

        //foreach (string device1 in strs)
        //{
        //    Debug.Log("Name: " + device1);
        //}


        //Observable.EveryUpdate().SampleFrame(30).Subscribe(_ => Debug.Log("after 100 frame"));
    }

    void Awake()
    {
        //data = new Color32[(320 * 240)];

        float height = Camera.main.orthographicSize * 2.0f;
        float width = height * Screen.width / Screen.height;
        //transform.localScale = new Vector3(width * 0.1f, height * 0.1f, 1.0f);

        WebCamDevice[] device = WebCamTexture.devices;

        cam = new WebCamTexture(320, 240, 15);

        if (device.Length > 0)
        {
            cam.deviceName = device[0].name;
            //cam.Stop();
            this.GetComponent<Renderer>().material.mainTexture = cam;
            cam.Play();

        }
    }

    public void GetPicture()
    {
        Color[] colors = cam.GetPixels();
        texture.SetPixels(colors);
        texture.Apply();
    }

    // Update is called once per frame
    void Update () {
        //cam.GetPixels32(data);

        //Color[] colors = cam.GetPixels();



        //texture.SetPixels(cam.GetPixels());
        //texture.Apply();
    }
}
