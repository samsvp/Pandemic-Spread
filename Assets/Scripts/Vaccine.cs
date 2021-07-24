using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vaccine : MonoBehaviour
{

    // sources: 
    // [1] https://www.tuasaude.com/vacina-covid/
    // [2] https://www.yalemedicine.org/news/covid-19-vaccine-comparison
    // [3] https://www.nature.com/articles/d41586-021-01505-x
    public Dictionary<string, float> immunizationRate = new Dictionary<string, float>() {
        { "Pftzer", 0.950f },
        { "Sputnik", 0.916f },
        { "AstraZeneca", 0.760f },
        { "Sinovac", 0.780f },
        { "Johnson & Johnson", 0.860f }
    };

    // from [3] we can expect transmission to be lower on vaccinated people
    public static float infectionModifier = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
