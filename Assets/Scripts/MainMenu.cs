using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public GameObject Server_;
    public GameObject Client_;
    public GameObject Main_Menu_;

    public Button Button_Server_;
    public Button Button_Client_;
    public Button Button_Quit_;



    // Use this for initialization
    void Start () {

        Button_Server_.onClick.AsObservable().Subscribe(_ =>
        {
            Main_Menu_.SetActive(false);
            Server_.SetActive(true);
        });

        Button_Client_.onClick.AsObservable().Subscribe(_ =>
        {
            Main_Menu_.SetActive(false);
            Client_.SetActive(true);
        });

        Button_Quit_.onClick.AsObservable().Subscribe(_ =>
        {
            Application.Quit();
        });

    }

	// Update is called once per frame
	void Update () {

	}
}
