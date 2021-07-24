using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class CanvasManager : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI recoveredCount;
    [SerializeField]
    private TextMeshProUGUI infectedCount;
    [SerializeField]
    private TextMeshProUGUI deceasedCount;
    [SerializeField]
    private TextMeshProUGUI daysCount;

    // Start is called before the first frame update
    void Start()
    {
        // wait one frame to get text color
        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return null;
        recoveredCount.color = GameManager.instance.People[0].recoveredColor;
        infectedCount.color = GameManager.instance.People[0].sickColor;
        deceasedCount.color = GameManager.instance.People[0].deadColor;
    }

    // Update is called once per frame
    void Update()
    {
        // Update count of deceased, infected and recovered
        Person[] people = GameManager.instance.People;

        int recovered = people.Where(p => p.IsRecovered).Count();
        int infected = people.Where(p => p.IsSick).Count();
        int deceased = people.Where(p => p.IsDead).Count();

        UpdateText(recoveredCount, recovered, people.Length);
        UpdateText(infectedCount, infected, people.Length);
        UpdateText(deceasedCount, deceased, people.Length);

        // Update days
        daysCount.text = $"Day {GameManager.instance.CurrentDay.ToString()}";
    }


    private void UpdateText(TextMeshProUGUI textMeshPro, int n, int total)
    {
        textMeshPro.text = $"{n} ({n / (float)total * 100}%)";
    }

}
