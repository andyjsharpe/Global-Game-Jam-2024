using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DestroyAfterAudio : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(transform.gameObject, GetComponent<AudioSource>().clip.length + 1);
    }
}
