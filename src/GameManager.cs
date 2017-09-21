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

        loadingImage.transform.parent.gameObject.SetActive(false);
    }

    void DrawScene()
    {
        foreach(Vector3 keyPoints in generator.solutionKeyPoints)
        {
            Instantiate(tile, keyPoints, Quaternion.identity);
        }
    }

    void InitGame()
    {
        doingSetup = true;

        mainText.text = "ROOM " + level;

        StartCoroutine(DisplayLoadingText());

        generator.Init(level);

        Debug.LogWarning("DRAWING SCENE");

        DrawScene();

        //doingSetup = false;
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