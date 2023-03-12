using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// It keeps the counter of:
///     * hits: the hits
///     * ammo: the shots that remain in the pistol 
///     * scoreValue: how many shots I have
/// </summary>
[RequireComponent(typeof(Text))]
public class Counter : MonoBehaviour
{

    /// <summary>
    /// Hits
    /// </summary>
    [SerializeField]
    private static int _hits = 0;
    public static int Hits { get => _hits; set => _hits = value; }

    /// <summary>
    /// Ammo
    /// </summary>
    [SerializeField]
    private static int _ammo = 0;
    public static int Ammo { get => _ammo; set => _ammo = value; }

    /// <summary>
    /// ScoreValue
    /// </summary>
    [SerializeField]
    private static int _scoreValue = 0;
    public static int ScoreValue { get => _scoreValue; set => _scoreValue = value; }

    /// <summary>
    /// Counter of hits
    /// </summary>
    [SerializeField]
    private GameObject _textHits;
    public GameObject TextHits { get => _textHits; }

    /// <summary>
    /// Counter of ammo
    /// </summary>
    [SerializeField]
    private GameObject _textAmmo;
    public GameObject TextAmmo { get => _textAmmo; }

    /// <summary>
    /// Counter of time
    /// </summary>
    [SerializeField]
    private GameObject _textTime;
    public GameObject TextTime { get => _textTime; }

    /// <summary>
    /// Counter of how many shots I have
    /// </summary>
    [SerializeField]
    private GameObject _textScoreValue;
    public GameObject TextScoreValue { get => _textScoreValue; }

    /// <summary>
    /// The gun
    /// </summary>
    [SerializeField]
    private GameObject _gun;
    public GameObject Gun { get => _gun; }

    /// <summary>
    /// Total time
    /// </summary>
    [SerializeField]
    [Min(0)]
    private float _time = 19f;
    public float Time { get => _time; set => _time = value; }


    // Start is called before the first frame update
    public void Start()
    {
        Debug.Assert(_textHits.GetComponent<Text>() != null, "hits not properly configured");
        Debug.Assert(_textAmmo.GetComponent<Text>() != null, "ammo not properly configured");
        Debug.Assert(_textTime.GetComponent<Text>() != null, "time not properly configured");
        Debug.Assert(_textScoreValue.GetComponent<Text>() != null, "scoreValue not properly configured");
        Debug.Assert(_gun.GetComponent<Gun>() != null, "gun not properly configured");

        var gunComp = _gun.GetComponent<Gun>();
        // Assign the value of the ammo
        _ammo = gunComp.MaxAmmo;

        //Invoke("UpdateTimer", 1f);
        StartCoroutine(StartCountdown());
        //Invoke("Countdown", 1f);
    }

    // Update is called once per frame
    public void Update()
    {
        // Update the value of the hits
        var scoreText = _textHits.GetComponent<Text>();
        scoreText.text = _hits.ToString();

        // Update the value of the ammo
        var scoreText2 = _textAmmo.GetComponent<Text>();
        scoreText2.text = _ammo.ToString();

        var scoreText3 = _textScoreValue.GetComponent<Text>();
        scoreText3.text = _scoreValue.ToString();

        if(currCountdownValue <= 0)
        {
            var timeText = _textTime.GetComponent<Text>();
            timeText.text = _time.ToString() + 's';
        }
    }

    private void UpdateTimer()
    {
        Debug.Log(Time);
        Time = Time - 1f;

        if(Time < 0)
        {
            SceneManager.LoadScene(3);
        }
        else
        {
            Invoke("UpdateTimer", 1f);
        }
    }

    private float currCountdownValue = 3;
    public IEnumerator StartCountdown(float countdownValue = 3)
    {
        currCountdownValue = countdownValue;
        while (currCountdownValue > 0)
        {
            // Debug.Log("Countdown: " + currCountdownValue);
            var timeText = _textTime.GetComponent<Text>();
            timeText.text = currCountdownValue.ToString() + "...";
            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
        }
        if(currCountdownValue == 0)
        {
            Invoke("UpdateTimer", 1f);
        }
    }

}
