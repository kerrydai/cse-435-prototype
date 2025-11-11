using UnityEngine;
using TMPro;
public class LMS: MonoBehaviour
{
    public TMP_Text Text;

    void Start()
    {
        Text.text =  "LMS Scenario Testing:\nClick buttons to test different LMS functionalities.";
    }

    public void Scenario1()
    {
        Text.text = "LMS Scenario 1:Normal Driving.";
    }
    public void Scenario2()
    {
        Text.text = "LMS Scenario 2: Lane Warning.";
    }
        




}
