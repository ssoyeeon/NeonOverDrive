using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttonnn : MonoBehaviour
{
    public GameObject blackCanvas;
    public GameObject backCanvas;
    public void StartButton()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void ExitButton()
    {
        Application.Quit();
    }
    public void MapButton()
    {
        SceneManager.LoadScene("MapScene");
    }
    public void GarageButton()
    {
        SceneManager.LoadScene("GarageScene");
    }
    public void BlackCar()
    {
        Camera camera = FindObjectOfType<Camera>();
        camera.transform.position = new Vector3(29f, 0.45f, -1.51f);
        blackCanvas.SetActive(false);
        backCanvas.SetActive(true);
    }
    public void Tada()
    {
        Camera camera = FindObjectOfType<Camera>();
        camera.transform.position = new Vector3(4f, 0.7f, -3f);
        blackCanvas.SetActive(true);
        backCanvas.SetActive(false);
    }
}
