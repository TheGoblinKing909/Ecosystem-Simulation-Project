using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public CanvasGroup OptionPanel, WorldParam;
    public TMPro.TMP_InputField userWidth, userHeight, userSeed, userOctaves, userScale, userPersistence, userLacunarity;
    public static int inputWidth, inputHeight, inputSeed, inputOctaves;
    public static float inputScale, inputPersistence, inputLacunarity;

    public void StartGame()
    {
        inputWidth = int.Parse(userWidth.text);
        inputHeight = int.Parse(userHeight.text);
        inputSeed = int.Parse(userSeed.text);
        inputOctaves = int.Parse(userOctaves.text);
        inputScale = float.Parse(userScale.text);
        inputPersistence = float.Parse(userPersistence.text);
        inputLacunarity = float.Parse(userLacunarity.text);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Create()
    {
        WorldParam.alpha = 1;
        WorldParam.blocksRaycasts = true;
    }
    public void Options()
    {
        OptionPanel.alpha = 1;
        OptionPanel.blocksRaycasts = true;
    }
    public void OptionBack()
    {
        OptionPanel.alpha = 0;
        OptionPanel.blocksRaycasts = false;
    }
    public void ParamBack()
    {
        WorldParam.alpha = 0;
        WorldParam.blocksRaycasts = false;
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
