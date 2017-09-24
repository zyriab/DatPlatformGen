using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        Vector3 coord = new Vector3();

        foreach(LevelGenerator.Room roomItem in generator.rooms)
        {
            foreach(Vector3 keyPoint in roomItem.platformList)
            {
                coord.x = (roomItem.position.x*LevelGenerator.Room.size.x) + keyPoint.x;
                coord.y = (roomItem.position.y*LevelGenerator.Room.size.y) + keyPoint.y;

                Instantiate(tile, coord, Quaternion.identity);
            }
        }
}

    void InitGame()
    {
        doingSetup = true;

        mainText.text = "ROOM " + level;

        StartCoroutine(DisplayLoadingText());

        generator.Init();

        Debug.LogWarning("DRAWING SCENE");

        DrawScene();

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
