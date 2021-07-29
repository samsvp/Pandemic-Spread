using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    // population variables
    [SerializeField]
    private int population = 500; // amount of people to instantiate
    [SerializeField]
    private int infectedPopulation = 10; // amount of people to start as infected
    private Person[] people; // array containing all of the population

    // prefabs
    [SerializeField]
    private Person personPrefab; // base person to instantiate everyone from
    [SerializeField]
    private Disease diseasePrefab; // base disease to instantiate every disease from

    private int[,] grid; // grid of possible positions to spawn people in

    // Simulation timing variables
    private int currentDay = 0;
    private float elapsedTime = 0;
    private float currentTime;
    [SerializeField]
    private float dayTime = 1; // time in seconds that a day should last in the simulation

    // modifiers
    public bool isVaccinating = false;
    public bool isSocialDistancing = false;
    public bool pause = false;


    public float DayTime { get => dayTime; }
    public int[,] Grid { get => grid; }
    public int CurrentDay { get => currentDay; }
    public Person[] People { get => people; }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        int gridSizeX = (int)Mathf.Ceil(Mathf.Sqrt(population));
        int gridSizeY = (int)Mathf.Ceil(Mathf.Sqrt(population));

        grid = new int[gridSizeX, gridSizeY];

        people = new Person[population];

        for (int i = 0; i < people.Length; i++)
        {
            people[i] = Instantiate(personPrefab);
            people[i].transform.position = 0.5f * new Vector3(i / gridSizeX, i % gridSizeY);
        }

        for (int i = 0; i < infectedPopulation; i++)
        {
            Disease disease = Instantiate(diseasePrefab);
            people[Random.Range(0, population)].SetAsSick(disease, true);
        }

        currentTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();

        if (pause)
        {
            currentTime = Time.time;
            return;
        }

        UpdateTimer();
    }


    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.T))
            pause = !pause;

        if (Input.GetKeyDown(KeyCode.S))
            isSocialDistancing = !isSocialDistancing;
    }


    private void UpdateTimer()
    {
        float currentTime = Time.time;
        int lastDay = currentDay;

        elapsedTime += (currentTime - this.currentTime);
        currentDay = (int)(elapsedTime / dayTime);

        if (currentDay != lastDay) 
        {
            int dead = people.Where(p => p.IsDead).Count();
            int infected = people.Where(p => p.IsSick).Count();

            if (infected > 0) Graph.instance.UpdateTotals(dead, infected);
            else pause = true;
        }

        this.currentTime = currentTime;
    }

}
