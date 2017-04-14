using UnityEngine;
using System.Collections;

/// <summary>
/// Used to create a smooth blinking effect for a light source component
/// </summary>
public class Flash : MonoBehaviour
{
    [Tooltip("The total of seconds the flash wil last")]
    public float totalSeconds;
    [Tooltip("The maximum intensity the flash will reach")]     
    public float maxIntensity; 
    [Tooltip("Light to flash")]
    public Light myLight;       

    public IEnumerator flashNow()
    {
        if (!myLight.enabled)
        {
            myLight.enabled = true;
        }

        // Get half of the seconds (One half to get brighter and one to get darker)
        float waitTime = totalSeconds / 2;
        
        while (myLight.intensity < maxIntensity)
        {
            myLight.intensity += Time.deltaTime / waitTime;        // Increase intensity
            yield return null;
        }
        while (myLight.intensity > 0)
        {
            myLight.intensity -= Time.deltaTime / waitTime;        //Decrease intensity
            yield return null;
        }
        yield return null;

        myLight.enabled = false;
    }
}
