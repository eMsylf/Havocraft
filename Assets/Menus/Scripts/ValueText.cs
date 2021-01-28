using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ValueText : MonoBehaviour
{
    public NumberFormat numberFormat = NumberFormat.Percentage;
    public string CustomFormat = "";
    public string output;
    public TextMeshProUGUI OutputTextComponent;
    TextMeshProUGUI GetTMPComponent()
    {
        if (OutputTextComponent == null)
        {
            OutputTextComponent = GetComponent<TextMeshProUGUI>();
        }
        return OutputTextComponent;
    }

    public enum NumberFormat
    {
        Raw,
        Integer,
        Percentage,
        ZeroToOne,
        Custom
    }

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

    public void UpdateValue(float value)
    {
        output = value.ToString(GetNumberFormat());
        GetTMPComponent().text = output; //TODO https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings
                                                       //Debug.Log("Update value to " + formattedSliderValue, this);
    }

}
