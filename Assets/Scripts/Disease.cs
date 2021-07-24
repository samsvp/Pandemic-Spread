using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disease : MonoBehaviour
{

    [SerializeField]
    private float lethality = 0.03f; // chance of a person dying
    [SerializeField]
    private float infectionRate = 0.2f; // chance of passing down the virus
    [SerializeField]
    private float reinfectionRate = 0.05f; // chance of contracting the virus more than once
    
    [SerializeField]
    private int minTime = 9; // minimum amount of time with disease
    [SerializeField]
    private int maxTime = 14; // maximum amount of time with disease
    private int spawnDay; // day in which the virus spawned

    float lifespan; // time in seconds in which the desease can spread

    public Person host;

    public float ReinfectionRate { get => reinfectionRate; }
    public float Lethality { get => lethality; }
    public float InfectionRate { get => infectionRate; }

    void Start()
    {
        lifespan = Random.Range(minTime, maxTime);
        spawnDay = GameManager.instance.CurrentDay;
    }


    private void Update()
    {
        CountDown();
    }


    public void Spread(Person person)
    {
        Disease disease = Instantiate(this);
        if (!person.SetAsSick(disease)) Destroy(disease.gameObject);
    }


    public void CountDown()
    {
        if (GameManager.instance.CurrentDay - (spawnDay + lifespan) >= 0)
        {
            try
            {
                host.SetAsRecovered();
            }
            catch { }
            finally
            {
                Destroy(gameObject);
            }
        }
    }

}
