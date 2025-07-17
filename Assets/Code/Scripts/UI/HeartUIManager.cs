using System.Collections.Generic;
using UnityEngine;

public class HeartUIManager : MonoBehaviour
{
    [SerializeField] public GameObject heart_1;
    [SerializeField] public GameObject heart_2;

    void Start()
    {
        heart_1.SetActive(true);
        heart_2.SetActive(false);
    }
    public void ActiveHeart_1()
    {
        heart_2.SetActive(true);
    }
    public void ActiveHeart_2()
    {
        heart_2.SetActive(true);
    }
    public void RemoveHeart_1()
    {
        heart_1.SetActive(false);
    }

    public void RemoveHeart_2()
    {
        heart_2.SetActive(false);
    }
}
