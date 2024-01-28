using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum SoundTypes
{
    Clap, Boo, Cheer, Unhappy
};

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Person : MonoBehaviour
{
    public float opinion = 0.9f;
    [HideInInspector]
    public AudioSource audioSource;
    private NavMeshAgent _agent;
    private Animator AnimIK;

    [SerializeField] private GameObject[] bodies;

    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightFoot;
    [SerializeField] private Transform leftFoot;
    private static readonly int WalkSpeed = Animator.StringToHash("WalkSpeed");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Walking = Animator.StringToHash("Walking");
    private static readonly int Clap1 = Animator.StringToHash("Clap");
    private static readonly int Boo = Animator.StringToHash("Boo");
    private static readonly int Cheer = Animator.StringToHash("Cheer");
    private static readonly int Unhappy = Animator.StringToHash("Unhappy");

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = NormalDistribution.NormalizedRandom(0.9f, 1.1f, 1);
        _agent = GetComponent<NavMeshAgent>();
        _agent.destination = FindObjectOfType<JokeMaker>().transform.position;

        AnimIK = GetComponent<Animator>();
        AnimIK.SetTrigger(Walking);

        Transform transform1;
        Instantiate(bodies[Random.Range(0, bodies.Length)], (transform1 = transform).position + Vector3.up, Quaternion.Euler(0, 0, 0), transform1);
        
        GetComponentInChildren<IK>().SetIKParts(rightHand, leftHand, rightFoot, leftFoot);
        
        foreach (var body in GetComponentsInChildren<Rigidbody>())
        {
            var callback = body.gameObject.AddComponent<BulletHitCallback>();
            callback.SetPerson(GetComponent<Person>());
        }
        ToggleRagdoll(false);
    }

    private void Update()
    {
        var magnitude = _agent.velocity.magnitude;
        if (!AnimIK)
        {
            AnimIK = GetComponent<Animator>();
        }
        
        AnimIK.SetFloat(WalkSpeed, magnitude);
        if (magnitude < 1 && AnimIK.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Walking")
        {
            ResetAnim();
        }
    }

    public void ResetAnim()
    {
        if (!AnimIK)
        {
            AnimIK = GetComponent<Animator>();
        }
        if (_agent.velocity.magnitude < 1)
        {
            AnimIK.SetTrigger(Idle);
        }
    }

    private void ToggleRagdoll(bool ragdollOn)
    {
        foreach (var body in GetComponentsInChildren<Rigidbody>())
        {
            body.isKinematic = !ragdollOn;
        }
        
        GetComponentInChildren<IK>().SetWeight(ragdollOn ? 0 : 1);
        foreach (Transform child in transform)
        {
            if(child.TryGetComponent<Animator>(out var anim))
            {
                anim.enabled = !ragdollOn;
            }
        }
        GetComponent<NavMeshAgent>().enabled = !ragdollOn;
        GetComponent<AudioSource>().enabled = !ragdollOn;
    }

    public void Clap(AudioClip audioClip, SoundTypes soundType)
    {
        AudioClip clip = null;
        switch (soundType)
        {
            case SoundTypes.Clap:
                AnimIK.SetTrigger(Clap1);
                clip = audioClip;
                break;
            case SoundTypes.Boo:
                AnimIK.SetTrigger(Boo);
                clip = audioClip;
                break;
            case SoundTypes.Cheer:
                AnimIK.SetTrigger(Cheer);
                clip = audioClip;
                break;
            case SoundTypes.Unhappy:
                AnimIK.SetTrigger(Unhappy);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(soundType), soundType, null);
        }

        if (clip != null)
        {
            audioSource.clip = audioClip;
            audioSource.PlayDelayed(NormalDistribution.NormalizedRandom(0, 1, 3));
        }
        
    }

    public void HearJoke(float baseline)
    {
        // Modify opinion by baseline plus a random value
        opinion = Mathf.Clamp(opinion + baseline + NormalDistribution.NormalizedRandom(-1, 1, 5)*0.25f, -1.0f, 1.0f);
    }

    public void HearAudience(float incomingOpinion)
    {
        opinion = Mathf.Clamp(opinion + incomingOpinion / 8, -1.0f, 1.0f);
    }

    public void GetShot(Vector3 vector3)
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

        var jokeMaker = FindObjectOfType<JokeMaker>();
        jokeMaker.enemyStack += 2;
        if (opinion >= 0)
        {
            jokeMaker.angryStack += 2;
        }
        ToggleRagdoll(true);
        gameObject.layer = LayerMask.NameToLayer("Default");
        foreach (var body in GetComponentsInChildren<Rigidbody>())
        {
            body.AddExplosionForce(1500, vector3 - Vector3.right, 2f, 0.25f, ForceMode.Acceleration);
        }
        Destroy(this);
    }
}
