using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateItems : MonoBehaviour
{
    [SerializeField] GameObject Item;
    [SerializeField] int itemAmount;

    private Vector3 randomPosition;
    private float rangeX;
    private float rangeZ;
    void Start()
    {
        SeedItems();
    }

    private void SeedItems()
    {
        rangeX = PlayerController.Instance.boundX;
        rangeZ = PlayerController.Instance.boundZ;
        randomPosition.y = 0.3f;
        itemAmount = Random.Range(5, 12);
        for (int i = 0; i <= itemAmount; i++)
        {
            randomPosition.x = Random.Range(-rangeX, rangeX);
            randomPosition.z = Random.Range(-rangeZ, rangeZ);

            Instantiate(Item, randomPosition, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
