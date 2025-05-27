using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttonnn : MonoBehaviour
{
    public GameObject blackCanvas;
    public GameObject backCanvas;
    public void Stage1SceneButton()
    {
        SceneManager.LoadScene("Stage1Scene");
    }
    public void SampleSceneButton()
    {
        SceneManager.LoadScene("Stage2Scene");

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
    public void StartSceneButton()
    {
        SceneManager.LoadScene("StartScene");
    }
    public void BlackCar()
    {
        Camera camera = FindObjectOfType<Camera>();
        camera.transform.position = new Vector3(29f, 0.45f, -1.51f);
        blackCanvas.SetActive(false);
        backCanvas.SetActive(true);
    }
    public void TadaRedCar()
    {
        Camera camera = FindObjectOfType<Camera>();
        camera.transform.position = new Vector3(4f, 0.7f, -3f);
        blackCanvas.SetActive(true);
        backCanvas.SetActive(false);
    }
    public void TadaBlueCar()
    {
        Camera camera = FindObjectOfType<Camera>();
        camera.transform.position = new Vector3(49.5f, 0.45f, -1.51f);
        blackCanvas.SetActive(false);
        backCanvas.SetActive(true);
    }
    public void TadaWhiteCar()
    {
        Camera camera = FindObjectOfType<Camera>();
        camera.transform.position = new Vector3(70.7f, 0.45f, -1.51f);
        blackCanvas.SetActive(false);
        backCanvas.SetActive(true);
    }
}
