using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterBoxing : MonoBehaviour
{
    private void Start()
    {
        float targetAspect = 19.0f / 9.0f;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float windowAspect = screenWidth / screenHeight;

        float scaleHeight = windowAspect / targetAspect;
        Camera camera = GetComponent<Camera>();

        if (scaleHeight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            camera.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = camera.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }
}
