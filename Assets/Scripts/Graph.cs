using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour
{

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

    // Start is called before the first frame update
    void Start()
    {
        float[] values = new float[] { 5, 98, 56, 45, 30, 22, 17, 15, 13, 17, 25, 37, 50, 15 };
        ShowGraph(values);
    }


    /// <summary>
    /// Plots a bar graph with the given values. Lenght must be equal to 14.
    /// </summary>
    /// <param name="mvalues"></param>
    private void ShowGraph(float[] mvalues)
    {
        Trace.Assert(mvalues.Length == 14, "mvalues length must be equal to 14");

        float vMax = mvalues.Max();
        float[] values = mvalues.Select(v => v / vMax * graphHeight).ToArray();
        
        for (int i = 0; i < values.Length; i++)
        {
            float xPos = i * graphWidth / values.Length + xSpacing;
            float yPos = values[i];
            CreateBar(new Vector2(xPos, yPos), 0.8f * xSpacing);
        }
    }


    /// <summary>
    /// Creates the bars used by the ShowGraph function
    /// </summary>
    /// <param name="graphPosition"></param>
    /// <param name="barWidth"></param>
    /// <returns></returns>
    private GameObject CreateBar(Vector2 graphPosition, float barWidth)
    {
        GameObject gO = new GameObject("bar", typeof(Image));
        gO.transform.SetParent(graphContainer, false);
        gO.GetComponent<Image>().sprite = squareSprite;

        RectTransform rectTransform = gO.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(graphPosition.x, 0);
        rectTransform.sizeDelta = new Vector2(barWidth, graphPosition.y);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0);

        return gO;
    }
}
