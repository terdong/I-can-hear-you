using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mic : MonoBehaviour {

	// Use this for initialization
	void Start () {
        AudioSource aud = GetComponent<AudioSource>();
        //aud.clip = Microphone.Start(null, false, 10, 44100);
        //aud.Play();
    }

	// Update is called once per frame
	void Update () {
        if(Input.GetKey(KeyCode.P))
        {
            //string string_input =
        }
	}
}
