using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class JokeMaker : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private Slider slider;
    
    [SerializeField]
    private Image sliderBackground;
    
    [SerializeField]
    private JokeFormat[] jokeList;

    [SerializeField] private AudioClip opening;

    [SerializeField] private AudioClip[] nouns;
    [SerializeField] private AudioClip[] verbs;
    [SerializeField] private AudioClip[] adjectives;
    [SerializeField] private AudioClip[] cheers;
    [SerializeField] private AudioClip[] boos;
    [SerializeField] private AudioClip[] claps;

    public int enemyStack;
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private GameObject[] peopleToSpawn;
    
    // Start is called before the first frame update
    void Start()
    {
        //_audioSource = GetComponent<AudioSource>();
        StartCoroutine(AudioLoop());
        StartCoroutine(SpawnLoop());
        UpdateSlider(FindObjectsOfType<Person>());
    }

    private void Update()
    {
        UpdateSlider(FindObjectsOfType<Person>());
    }

    private IEnumerator AudioLoop()
    {
        _audioSource.clip = opening;
        _audioSource.Play();
        yield return new WaitForSeconds(opening.length);
        
        while (true)
        {
            var jokeParts = jokeList[Random.Range(0, jokeList.Length)].jokeComponents;
            foreach (var jokeStuff in jokeParts)
            {
                var clipToPlay = jokeStuff.thingTypes switch
                {
                    ThingTypes.Text => jokeStuff.audioSource,
                    ThingTypes.Noun => nouns[Random.Range(0, nouns.Length)],
                    ThingTypes.Verb => verbs[Random.Range(0, verbs.Length)],
                    ThingTypes.Adjective => adjectives[Random.Range(0, adjectives.Length)],
                    _ => throw new ArgumentOutOfRangeException()
                };

                _audioSource.clip = clipToPlay;
                _audioSource.Play();
                yield return new WaitForSeconds(clipToPlay.length);
            }
            
            // Update opinions based on joke
            var jokeScore = NormalDistribution.NormalizedRandom(-1, 1, 3) * 0.25f;
            jokeScore -= 0.2f;
            var people = FindObjectsOfType<Person>();
            foreach (var person in people)
            {
                person.HearJoke(jokeScore);
            }

            float lowestOpinion = 2;
            var lowestPerson = people[0];
            // Find the person with the lowest opinion
            foreach (var person in people)
            {
                if (person.opinion < lowestOpinion)
                {
                    lowestPerson = person;
                    lowestOpinion = person.opinion;
                }
            }

            // Clap or talk
            float lineLength = 0;
            foreach (var person in people)
            {
                if (person != lowestPerson)
                {
                    if (person.opinion >= 0)
                    {
                        person.audioSource.volume = 0.5f;
                        person.audioSource.priority = 128;
                        person.Clap(claps[Random.Range(0, claps.Length)]);
                    }
                }
                else
                {
                    // Say nice or mean line depending on opinion
                    var voiceLine = lowestOpinion < 0 ? boos[Random.Range(0, boos.Length)] :
                        cheers[Random.Range(0, cheers.Length)];
                    lineLength = voiceLine.length / person.audioSource.pitch;
                    person.audioSource.volume = 1f;
                    person.audioSource.priority = 64;
                    person.Clap(voiceLine);
                }
            }
            
            // Update opinions based on proximity to the person who talked if it is negative
            if (lowestOpinion < 0)
            {
                foreach (var person in people)
                {
                    if (person != lowestPerson)
                    {
                        var dist = Vector3.Distance(person.transform.position, lowestPerson.transform.position);
                        person.HearAudience(lowestOpinion / dist);
                    }
                }
            }

            UpdateSlider(people);
            
            yield return new WaitForSeconds(lineLength);
            
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (enemyStack > 0)
            {
                Instantiate(peopleToSpawn[Random.Range(0, peopleToSpawn.Length)], spawnTransform.position,
                    quaternion.identity);
                enemyStack--;
            }
            yield return new WaitForSeconds(NormalDistribution.NormalizedRandom(0.5f, 1f, 1));
        }
    }
    
    // Count the number of people who are happy and update the slider
    // Called after opinions updated and after people are killed/added
    public void UpdateSlider(Person[] people)
    {
        float sum = 0;
        foreach (var person in people)
        {
            if (Vector3.Distance(transform.position, person.transform.position) < 24 && person.opinion >= 0)
            {
                sum++;
            }
        }

        var completion = sum / people.Length;
        slider.value = completion;
        sliderBackground.color = Color.Lerp(Color.magenta, Color.cyan, completion);
    }
}
