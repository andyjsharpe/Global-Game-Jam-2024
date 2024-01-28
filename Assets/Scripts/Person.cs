using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NavMeshAgent))]
public class Person : MonoBehaviour
{
    public float opinion = 0.5f;
    public AudioSource audioSource;
    private NavMeshAgent _agent;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = NormalDistribution.NormalizedRandom(0.9f, 1.1f, 1);
        _agent = GetComponent<NavMeshAgent>();
        _agent.destination = FindObjectOfType<JokeMaker>().transform.position;
    }

    public void Clap(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.PlayDelayed(NormalDistribution.NormalizedRandom(0, 1, 3));
    }

    public void HearJoke(float baseline)
    {
        // Modify opinion by baseline plus a random value
        opinion = Mathf.Clamp(opinion + baseline + NormalDistribution.NormalizedRandom(-1, 1, 5)*0.25f, -1.0f, 1.0f);
    }

    public void HearAudience(float incomingOpinion)
    {
        opinion = Mathf.Clamp(opinion + incomingOpinion / 4, -1.0f, 1.0f);
    }

    public void GetShot()
    {
        // Make people nearby unhappy if wrong person dies
        var people = FindObjectsOfType<Person>();
        if (opinion >= 0)
        {
            foreach (var person in people)
            {
                if (person == this)
                {
                    continue;
                }

                var opinionChange = -1 / Vector3.Distance(transform.position, person.transform.position);
                person.HearAudience(opinionChange);
            }
        }
        // Turn off player behavior
        
        // Ragdoll
        
        // Spawn in new person
        
        // Recalc score?
        JokeMaker jokeMaker = FindObjectOfType<JokeMaker>();
        jokeMaker.enemyStack += 2;
        Destroy(gameObject);
    }
}
