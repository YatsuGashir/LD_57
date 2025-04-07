using System;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private GameObject firstScreen;
    [SerializeField] private GameObject secondScreen;
    [SerializeField] private GameObject thirdScreen;
    private static bool  first = true;

    private void Awake()
    {
        first = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (first)
        {
            loadFirst();
        }
        else
        {
            loadSecond();
        }
    }

    private void loadFirst()
    {
        secondScreen.SetActive(true);
        first=false;
        Destroy(gameObject);
    }

    private void loadSecond()
    {
        firstScreen.SetActive(false);
        thirdScreen.SetActive(true);
        Destroy(gameObject);
    }
}
