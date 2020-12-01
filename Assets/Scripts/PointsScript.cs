using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;

public class PointsScript : MonoBehaviour
{
    // Start is called before the first frame update
    TextMeshProUGUI text;
    public int currentPoints = 0;
    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        currentPoints = 0;
    }

    public void updatePoints(int points)
    {
        currentPoints += points;
        string stringPoints = Convert.ToString(currentPoints);
        text.text = stringPoints;
    }
}
