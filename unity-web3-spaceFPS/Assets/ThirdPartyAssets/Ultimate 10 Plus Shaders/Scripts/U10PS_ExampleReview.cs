using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class U10PS_ExampleReview : MonoBehaviour
{
    public Text exampleName;

    public Example[] examples;
    private int currentIndex = -1;

    private void Start()
    {
        Next();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Previous();
        if (Input.GetKeyDown(KeyCode.RightArrow))
            Next();
    }

    public void Next()
    {
        if (currentIndex + 1 >= examples.Length)
            return;

        if(currentIndex != -1)
            for (int i = 0; i < examples[currentIndex].objects.Length; ++i)
                examples[currentIndex].objects[i].gameObject.SetActive(false);
        ++currentIndex;
        for (int i = 0; i < examples[currentIndex].objects.Length; ++i)
            examples[currentIndex].objects[i].gameObject.SetActive(true);

        exampleName.text = examples[currentIndex].name;
    }

    public void Previous()
    {
        if (currentIndex - 1 < 0)
            return;

        for (int i = 0; i < examples[currentIndex].objects.Length; ++i)
            examples[currentIndex].objects[i].gameObject.SetActive(false);
        --currentIndex;
        for (int i = 0; i < examples[currentIndex].objects.Length; ++i)
            examples[currentIndex].objects[i].gameObject.SetActive(true);

        exampleName.text = examples[currentIndex].name;
    }
}

[System.Serializable]
public class Example
{
    public GameObject[] objects;
    public string name;
}