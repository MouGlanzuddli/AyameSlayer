using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using UnityEngine.Networking;

public class SongManager : MonoBehaviour
{
    public static SongManager Instance;
    public AudioSource audioSource;
    public Lane[] lanes;
    public float songDelay;
    public int inputDelay;
    public double marginOfError; // in seconds

    public string fileLocation;
    public float noteTime;

    public float startXLeft; 
    public float endXLeft; 
    public float startXRight;
    public float endXRight;

    public float noteSpawnLeftX;
    public float noteSpawnRightX;
    public float noteSpawnY;

    private bool movingRight; // movement direction

    public static MidiFile midiFile;

    private void Start()
    {
        Instance = this;
        if (Application.streamingAssetsPath.StartsWith("http://") || Application.streamingAssetsPath.StartsWith("https://"))
        {
            ReadFromWebsite();
        }
        else
        {
            ReadFromFile();
        }
        transform.position = new Vector3(startXLeft, transform.position.y, transform.position.z);
        movingRight = true;
    }

    public class SongManagerException : System.Exception
    {
        public SongManagerException(string message) : base(message)
        {
            try
            {
                GameObject songManagerObject = GameObject.Find("SongManager");
                SongManager songManager = songManagerObject.GetComponent<SongManager>();
                songManager.Start();
            }
            catch (SongManager.SongManagerException ex)
            {
                Debug.LogError("Caught SongManagerException: " + ex.Message);
            }

        }
    }
    private void Update()
    {
        if (movingRight)
            transform.Translate(Vector3.right * noteTime * Time.deltaTime);
        else
            transform.Translate(Vector3.left * noteTime * Time.deltaTime);
        if ((movingRight && transform.position.x >= endXRight) || (!movingRight && transform.position.x <= endXLeft))
        {
            movingRight = !movingRight;
        }
    }

    private IEnumerator ReadFromWebsite()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + fileLocation))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                byte[] results = www.downloadHandler.data;
                using (var stream = new MemoryStream(results))
                {
                    midiFile = MidiFile.Read(stream);
                    GetDataFromMidi();
                }
            }
        }
    }

    private void ReadFromFile()
    {
        midiFile = MidiFile.Read(Application.streamingAssetsPath);
        GetDataFromMidi();

    }

    public void GetDataFromMidi()
    {
        var notes = midiFile.GetNotes();
        var array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
        notes.CopyTo(array, 0);

        foreach (var lane in lanes) lane.SetTimeStamps(array);
        Invoke(nameof(StartSong), songDelay);
    }

    public void StartSong()
    {
        audioSource.Play();
    }

    public static double GetAudioSourceTime()
    {
        return (double)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
    }


}
