using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour
{
    public static Graph instance = null;


    // Graph GUI variables
    [SerializeField]
    private Sprite squareSprite;
    [SerializeField]
    private RectTransform graphContainer;
    [SerializeField]
    private float xSpacing;
    [SerializeField]
    private float graphHeight;
    [SerializeField]
    private float graphWidth;
    private RectTransform[] rectTransforms = new RectTransform[14];

    // Text fields
    [SerializeField]
    private TextMeshProUGUI title;
    [SerializeField]
    private TextMeshProUGUI count;
    [SerializeField]
    private TextMeshProUGUI toggle;

    // Value tracking
    private bool isCountingInfected = true;
    private int maxInfectedValue = 0;
    private int maxInfectedTotalValue = 0;
    private int maxDeathValue = 0;
    private int maxDeathTotalValue = 0;
    private int[] deathValues;
    private int[] infectedValues;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        gameObject.SetActive(false);
    }


    // Start is called before the first frame update
    void Start()
    {
        deathValues = new int[14];
        infectedValues = new int[14];

        rectTransforms = rectTransforms.Select((_, index) => CreateBar(index)).ToArray();
    }


    /// <summary>
    /// Plots a bar graph with the given values. Lenght must be equal to 14.
    /// </summary>
    /// <param name="mvalues"></param>
    private void ShowGraph()
    {
        int[] mvalues = isCountingInfected ? infectedValues : deathValues;

        Trace.Assert(mvalues.Length == 14, "mvalues length must be equal to 14");

        int vMax = mvalues.Max();
        float[] values = mvalues.Select(v => v * graphHeight / vMax).ToArray();

        for (int i = 0; i < values.Length; i++)
        {
            float yPos = values[i];
            UpdateBar(i, yPos, 0.8f * xSpacing);
        }

        int totalValue = mvalues.Sum();

        UpdateGrapghText();
    }


    /// <summary>
    /// Updates graphs total count
    /// </summary>
    /// <param name="maxValue"></param>
    /// <param name="maxTotalValue"></param>
    /// <param name="vMax"></param>
    /// <param name="totalValue"></param>
    private void UpdateGrapghText()
    {
        title.text = isCountingInfected ? "Infected" : "Deaths";
        count.text = isCountingInfected ?
            $"Max (two weeks): {maxInfectedTotalValue}\nMax(day): {maxInfectedValue}" :
            $"Max (two weeks): {maxDeathTotalValue}\nMax(day): {maxDeathValue}";
    }


    public void UpdateTotals(int mDeathValue, int mInfectedValue)
    {
        int[] ShiftArray(int[] arr, int value)
        {
            for (int i = 1; i < arr.Length; i++)
            {
                arr[i - 1] = arr[i];
            }
            arr[arr.Length - 1] = value;
            return arr;
        }
        deathValues = ShiftArray(deathValues, mDeathValue);
        infectedValues = ShiftArray(infectedValues, mInfectedValue);

        UpdateTotals();
    }


    /// <summary>
    /// Updates the death and infection toll
    /// </summary>
    /// <param name="deathValues"></param>
    /// <param name="infectedValues"></param>
    private void UpdateTotals()
    {
        int maxDeaths = deathValues.Max();
        int maxInfections = infectedValues.Max();

        int totalDeaths = deathValues.Sum();
        int totalInfections = infectedValues.Sum();

        if (maxDeaths > maxDeathValue) maxDeathValue = maxDeaths;
        if (maxInfections > maxInfectedValue) maxInfectedValue = maxInfections;

        if (totalDeaths > maxDeathTotalValue) maxDeathTotalValue = totalDeaths;
        if (totalInfections > maxInfectedTotalValue) maxInfectedTotalValue = totalInfections;

        ShowGraph();
    }


    /// <summary>
    /// Toggles count between infected toll and death toll
    /// </summary>
    public void ToggleCount()
    {
        isCountingInfected = !isCountingInfected;
        toggle.text = isCountingInfected ? "Show Deaths" : "Show infected";
        ShowGraph();
    }


    private void UpdateBar(int i, float yPos, float barWidth)
    {
        rectTransforms[i].sizeDelta = new Vector2(barWidth, yPos);
    }


    /// <summary>
    /// Creates the bars used by the ShowGraph function
    /// </summary>
    /// <param name="graphPosition"></param>
    /// <param name="barWidth"></param>
    /// <returns></returns>
    private RectTransform CreateBar(int i)
    {
        GameObject gO = new GameObject("bar", typeof(Image));
        gO.transform.SetParent(graphContainer, false);
        gO.GetComponent<Image>().sprite = squareSprite;

        RectTransform rectTransform = gO.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(i * graphWidth / 14 + xSpacing, 0);

        rectTransform.sizeDelta = new Vector2(0, 0);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0);

        return rectTransform;
    }

}
