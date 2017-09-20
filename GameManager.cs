using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject tile;

    public float dotDelay = 2f;

    [SerializeField] private GameObject player;

    private LevelGenerator generator;
    [SerializeField] private Text mainText;
    [SerializeField] private Text loadingText;
    [SerializeField] private GameObject loadingImage;
    private int level = 1;
    private static bool doingSetup = false;

    IEnumerator DisplayLoadingText()
    {                      
        while(doingSetup)
        {
            loadingText.text = " .     ";
            yield return new WaitForSeconds(dotDelay);

            loadingText.text = "   .   ";
            yield return new WaitForSeconds(dotDelay);

            loadingText.text = "     . ";
            yield return new WaitForSeconds(dotDelay);
        }

        loadingImage.SetActive(false);
    }

    // void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    // {
    //     instance.level++;
    //     instance.InitGame();
    // }

    // /// <summary>
    // /// This function is called when the object becomes enabled and active.
    // /// </summary>
    // void OnEnable()
    // {
    //     SceneManager.sceneLoaded += OnLevelFinishedLoading;
    // }

    // /// <summary>
    // /// This function is called when the behaviour becomes disabled or inactive.
    // /// </summary>
    // void OnDisable()
    // {
    //     SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    // }

    // void HideLoadingImage()
    // {
    //     loadingImage.SetActive(false);
    //     doingSetup = false;
    // }

    public void GameOver()
    {
        mainText.text = "Glitched at room number " + level + ".\n\nLoading new program . . .";
        loadingImage.SetActive(true);

        this.enabled = false;
    }

    void DrawScene()
    {
        int i;
        Vector3 _position;

        foreach(Vector3 keyPoints in generator.solutionKeyPoints)
        {
            i = 0;
            foreach(Vector3 passPoints in generator.solution)
            {
                if(generator.solutionKeyPoints[i] == generator.solutionKeyPoints[generator.solution.Count-1])
                    return;

                _position = generator.solutionKeyPoints[i];
                while(_position != generator.solutionKeyPoints[i+1])
                Instantiate(tile, passPoints, Quaternion.identity);
            }
        }
    }

    void InitGame()
    {
        doingSetup = true;

        Debug.LogWarning("Setting up scene");

        mainText.text = "ROOM " + level;

        StartCoroutine(generator.Init(level));

        StartCoroutine(DisplayLoadingText());

        //DrawScene();

        doingSetup = false;
    }

    void Awake()
    {
        if(instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(true);

        generator = GetComponent<LevelGenerator>();

        InitGame();

        player.SetActive(true);
    }
}