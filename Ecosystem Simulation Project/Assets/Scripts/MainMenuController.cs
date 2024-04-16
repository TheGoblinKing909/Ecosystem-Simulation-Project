using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public CanvasGroup OptionPanel, WorldParam;
    public TMPro.TMP_InputField userWidth, userHeight, userSeed, userOctaves, userScale, userPersistence, userLacunarity, userResDensity, userEntDensity;
    //public Slider sliderWidth, sliderHeight, sliderSeed, sliderOctaves, sliderScale, sliderPersistence, sliderLacunarity, sliderResDensity, sliderEntDensity;
    public static int inputWidth, inputHeight, inputSeed, inputOctaves;
    public static float inputScale, inputPersistence, inputLacunarity, inputResDensity, inputEntDensity;
    public TMPro.TMP_InputField[] inputFields;
    public Slider[] sliders;
    public float[] low_values = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public float[] med_values = { 1, 1, 1, 1, 1, 1, 1, 1, 1 };
    public float[] high_values = { 2, 2, 2, 2, 2, 2, 2, 2, 2 };

    void Start()
    {
        // Add listeners to sliders
        for (int i = 0; i < sliders.Length; i++)
        {
            int index = i; // To capture the current value of i in the lambda
            sliders[i].onValueChanged.AddListener((value) => {
                // Update corresponding input field
                inputFields[index].text = value.ToString();
            });
        }
        // Add listeners to input fields
        for (int i = 0; i < inputFields.Length; i++)
        {
            int index = i; // To capture the current value of i in the lambda
            inputFields[i].onValueChanged.AddListener((value) => {

                // Parse input value as float
                if (float.TryParse(value, out float floatValue))
                {
                    // Update corresponding slider
                    sliders[index].value = floatValue;
                }
            });
        }
    }

    public void StartGame()
    {
        inputWidth = int.Parse(userWidth.text);
        inputHeight = int.Parse(userHeight.text);
        inputSeed = int.Parse(userSeed.text);
        inputOctaves = int.Parse(userOctaves.text);
        inputScale = float.Parse(userScale.text);
        inputPersistence = float.Parse(userPersistence.text);
        inputLacunarity = float.Parse(userLacunarity.text);
        inputResDensity = float.Parse(userResDensity.text);
        inputEntDensity = float.Parse(userEntDensity.text);

        if (Application.isEditor)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            Process mlagentsProcess = new Process();
            mlagentsProcess.StartInfo.UseShellExecute = false;
            mlagentsProcess.StartInfo.RedirectStandardInput = true;
            mlagentsProcess.StartInfo.WorkingDirectory = "..";
            mlagentsProcess.StartInfo.FileName = "cmd.exe";
            mlagentsProcess.StartInfo.Arguments = @"/K ..\anaconda3\Scripts\activate.bat ..\anaconda3";
            mlagentsProcess.Start();

            if (!Directory.Exists(@"..\..\anaconda3\envs\build-env\"))
            {
                mlagentsProcess.StandardInput.WriteLine("conda env create -f environment.yml");
            }

            mlagentsProcess.StandardInput.WriteLine("conda activate build-env");
            mlagentsProcess.StandardInput.WriteLine(@"mlagents-learn trainer_config.yaml --run-id=build --force --env=""Simulation\Ecosystem Simulation Project.exe"" --env-args " +
                inputWidth.ToString() + " " + inputHeight.ToString() + " " + inputSeed.ToString() + " " + inputOctaves.ToString() + " " +
                inputScale.ToString() + " " + inputPersistence.ToString() + " " + inputLacunarity.ToString() + " " + inputResDensity.ToString() + " " +
                inputEntDensity.ToString());
        }
    }

    public void SmallTemplate()
    {
        for (int i = 0; i < inputFields.Length; i++)
        {
            inputFields[i].text = low_values[i].ToString();
        }
    }

    public void MedTemplate()
    {
        for (int i = 0; i < inputFields.Length; i++)
        {
            inputFields[i].text = med_values[i].ToString();
        }
    }

    public void LargeTemplate()
    {
        for (int i = 0; i < inputFields.Length; i++)
        {
            inputFields[i].text = high_values[i].ToString();
        }
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
        for (int i = 0; i < inputFields.Length; i++)
        {
            sliders[i].value = low_values[i];
            inputFields[i].text = "";
        }
        WorldParam.alpha = 0;
        WorldParam.blocksRaycasts = false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
