using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseSimulator
{
    private static string SimulatedResponsesFolder
    {
        get { return "SimulatedResponses/"; }
    }

    public static string GetSimulatedResponse(string serviceURL)
    {
        string response = "Simulated Response Error!";
        TextAsset responseTxt = Resources.Load<TextAsset>(SimulatedResponsesFolder + serviceURL);

        if (responseTxt != null)
            response = responseTxt.text;
        else
            Debug.LogWarning("Simulated response not found in Resources/" + SimulatedResponsesFolder + serviceURL);

        return response;
    }
}
