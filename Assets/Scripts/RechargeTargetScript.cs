using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RechargeTargetScript : MonoBehaviour
{
    //YOLOSWAG
    // Start is called before the first frame update
    public Slider slider;
    public void setRecharge(float value)
    {
        //Small conditional to make sure that the slider never goes below it's ended value, or above it
        if(value >= 0 && value <= 3.0f) slider.value = value;
    }
}
