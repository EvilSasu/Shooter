using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CalculateFps : MonoBehaviour
{
    public TextMeshProUGUI fpsCounterText;
    private int frameCount = 0;
    private float deltaTime = 0.0f;
    void Update()
    {
        if (fpsCounterText.gameObject.activeSelf)
        {
            frameCount++;
            deltaTime += Time.unscaledDeltaTime;

            if (deltaTime >= 1.0f)
            {
                float fps = frameCount / deltaTime;
                fpsCounterText.text = "FPS: " + Mathf.Ceil(fps).ToString();

                frameCount = 0;
                deltaTime -= 1.0f;
            }
        }
    }
}
