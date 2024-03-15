using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notes : MonoBehaviour
{
    double timeInstantiated;
    public float assignedTime;

    public float noteStartX;
    public float noteEndX;
    void Start()
    {
        timeInstantiated = SongManager.GetAudioSourceTime();
    }

    // Update is called once per frame
    void Update()
    {
        double timeSinceInstantiated = SongManager.GetAudioSourceTime() - timeInstantiated;
        float t = (float)(timeSinceInstantiated / (SongManager.Instance.noteTime * 2));

        if (t > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            float currentX = Mathf.Lerp(noteStartX, noteEndX, t);
            transform.position = new Vector3(currentX, transform.position.y, transform.position.z);
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
