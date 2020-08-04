using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public List<GameObject> Squares;

    public GizmoSettings GizmoSettings; 

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < Squares.Count; i++)
        {
            GameObject square = Squares[i];
            GameObject nextSquare = null;
            //Debug.Log("i = " + i + ", i + 1 = " + (i + 1) + ", square count: " + Squares.Count);
            if (i + 1 < Squares.Count)
            {
                //Debug.Log("Valid");
                nextSquare = Squares[i + 1];
            }
            else if (GizmoSettings.CloseLoop)
                nextSquare = Squares[0];

            if (square == null)
            {
                Debug.LogError("Square " + i + " is null", gameObject);
                return;
            }
            if (nextSquare == null)
            {
                //Debug.LogError("Square " + (i + 1) + " is null", gameObject);
                return;
            }
            Handles.color = GizmoSettings.ArcColor;
            Handles.DrawPolyLine(CalculateParabola(square.transform.position, nextSquare.transform.position, GizmoSettings.ArcHeight, GizmoSettings.ArcDetail));
        }
    }

    Vector3[] CalculateParabola(Vector3 from, Vector3 to, float amplitude, int points)
    {
        Vector3[] steps = new Vector3[points + 1];
        //Debug.Log("From: " + from + ", To: " + to);
        //steps[0] = from;
        for (int i = 0; i < points + 1; i++)
        {
            Vector3 yOffset = new Vector3(
                0f,
                
                    (-Mathf.Pow((i/(points/2f))*amplitude - amplitude, 2f)) + Mathf.Pow(amplitude, 2f)

                    //(Mathf.Pow((i - points / 2f) - amplitude, 2f))
                    //* -Mathf.Pow(amplitude, 2f)
                , 
                0f)
                ;
            //Debug.Log("Y offset: " + yOffset);
            steps[i] = Vector3.Lerp(to, from, (i) / (float)points) + yOffset;
        }
        //steps[steps.Length - 1] = to;
        return steps;
    }
}

[Serializable]
public class GizmoSettings
{
    public float ArcHeight = 1f;
    public Color ArcColor = Color.white;
    [Range(1, 20)]
    public int ArcDetail;
    public bool CloseLoop = true;
}
