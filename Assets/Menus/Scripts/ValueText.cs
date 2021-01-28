using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ValueText : MonoBehaviour
{
    public TextMeshProUGUI TMPComponent;
    TextMeshProUGUI GetTMPComponent()
    {
        if (TMPComponent == null)
        {
            TMPComponent = GetComponent<TextMeshProUGUI>();
        }
        return TMPComponent;
    }

    public float value;

    public enum NumberFormat
    {
        Raw,
        Integer,
        Percentage,
        ZeroToOne,
        Custom
    }
    public NumberFormat numberFormat = NumberFormat.Percentage;

    public string GetNumberFormat() => GetNumberFormat(numberFormat);

    public string GetNumberFormat(NumberFormat format)
    {
        switch (format)
        {
            case NumberFormat.Raw:
                return string.Empty;
            case NumberFormat.Integer:
                return "0";
            case NumberFormat.Percentage:
                return "##0%";
            case NumberFormat.ZeroToOne:
                return "0.00";
            case NumberFormat.Custom:
                return CustomFormat;
            default:
                return string.Empty;
        }
    }
    public string CustomFormat = "";

    public void UpdateValue(float value)
    {
        string formattedSliderValue = value.ToString(GetNumberFormat());
        GetTMPComponent().text = formattedSliderValue; //TODO https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings
                                                       //Debug.Log("Update value to " + formattedSliderValue, this);
        output = formattedSliderValue;
    }

    public string output;
}
