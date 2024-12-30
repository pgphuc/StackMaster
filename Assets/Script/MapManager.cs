using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject prefab_StartBrick;
    [SerializeField] private GameObject prefab_EndBrick;
    [SerializeField] private GameObject prefab_EatBrick;
    [SerializeField] private GameObject prefab_TakeBrick;
    [SerializeField] private GameObject prefab_MapBrick;
    [SerializeField] private GameObject prefab_BlockBrick;
    [SerializeField] private GameObject prefab_NormalBrick;
    [SerializeField] private TextAsset mapText;
    
    private static MapManager instance;

    private Vector3 startPosition;
    private Vector3 endPosition;
    
    public Vector3 StartPosition => startPosition;
    public Vector3 EndPosition => endPosition;
    // Start is called before the first frame update
    void Start()
    {
        string[] textSplit = mapText.text.Split("\r\n");
        for (int i = 0; i < textSplit.Length; i++)
        {
            string line = textSplit[i];
            for (int j = 0; j < line.Length; j++)
            {
                int brickNumber = int.Parse(line[j].ToString());
                switch (brickNumber)
                {
                    case 0:
                        Instantiate(prefab_MapBrick, new Vector3(j, 0, i), Quaternion.identity);
                        break;
                    case 1:
                        Instantiate(prefab_StartBrick, new Vector3(j, 0, i), Quaternion.identity);
                        startPosition = new Vector3(j, 1, i);
                        break;
                    case 2:
                        Instantiate(prefab_EatBrick, new Vector3(j, 0, i), Quaternion.identity);
                        break;
                    case 3:
                        Instantiate(prefab_TakeBrick, new Vector3(j, 0, i), Quaternion.identity);
                        break;
                    case 4:
                        Instantiate(prefab_EndBrick, new Vector3(j, 0, i), Quaternion.identity);
                        endPosition = new Vector3(j, 0, i);
                        break;
                    case 5:
                        Instantiate(prefab_BlockBrick, new Vector3(j, 0, i), Quaternion.identity);
                        break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
