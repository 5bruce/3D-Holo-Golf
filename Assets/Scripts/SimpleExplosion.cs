using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleExplosion : MonoBehaviour {

	// Use this for initialization
	void Start () {
        while (true)
        {
            var exp = GetComponent<ParticleSystem>();
            exp.Play();
            Destroy(gameObject, exp.main.duration);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
