using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public AudioSource hitSFX;
    public AudioSource missSFX;
    public TMPro.TextMeshPro scoreText;
    static int comboScore;
    void Start()
    {
        Instance = this;
        comboScore = 0;
    }

    public static void Hit()
    {
        comboScore++;
        Instance.hitSFX.Play();
    }
    
    public static void Miss()
    {
        comboScore = 0;
        Instance.missSFX.Play();
    }
    void Update()
    {
        scoreText.text = comboScore.ToString();
    }
}