using System;
using UnityEngine;

public class ConditionalFieldAttribute : PropertyAttribute
{
    public string ConditionField;

    public ConditionalFieldAttribute(string conditionField)
    {
        ConditionField = conditionField;
    }
}