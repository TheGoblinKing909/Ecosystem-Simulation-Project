using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static System.Net.Mime.MediaTypeNames;

public class MainMenuController : MonoBehaviour
{
    public CanvasGroup OptionPanel, WorldParam;
    public TMPro.TMP_InputField userWidth, userHeight, userSeed, userOctaves, userScale, userPersistence, userLacunarity, userResDensity, userEntDensity, userWorldName, userLoadWorld;
    public static int inputWidth, inputHeight, inputSeed, inputOctaves;
    public static float inputScale, inputPersistence, inputLacunarity, inputResDensity, inputEntDensity;
    public static bool loadWorld;
    public static string worldName;
    public TMPro.TMP_InputField[] inputFields;
    public Slider[] sliders;
    public float[] low_values = { 50, 50, 1, 50, 1, 0.1f, 1, 0.001f, 0.001f };
    public float[] med_values = { 125, 125, 128, 100, 3, 0.3f, 3, 0.005f, 0.003f };
    public float[] high_values = { 200, 200, 255, 150, 5, 0.5f, 5, 0.01f, 0.005f };
    public float[] min_values = { 50, 50, 1, 25, 1, 0.1f, 1, 0.001f, 0.001f };
    public float[] max_values = { 200, 200, 10000, 150, 5, 0.5f, 5, 0.03f, 0.01f };
    public TMPro.TMP_Text worldNotFound;

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
            inputFields[i].onEndEdit.AddListener((value) => {

                // Parse input value as float
                if (float.TryParse(value, out float floatValue))
                {
                    if (floatValue >= min_values[index] && floatValue <= max_values[index])
                    {
                        // Update corresponding slider
                        sliders[index].value = floatValue;
                    }
                    else if (floatValue < min_values[index])
                    {
                        sliders[index].value = min_values[index];
                        inputFields[index].text = min_values[index].ToString();
                    }
                    else if (floatValue > max_values[index])
                    {
                        sliders[index].value = max_values[index];
                        inputFields[index].text = max_values[index].ToString();
                    }
                }
            });
        }
    }

    public void StartGame()
    {
        if (userWorldName.text == "")
        {
            userWorldName.text = "default_save";
        }

        for (int i = 0; i < inputFields.Length; i++)
        {
            int index = i;

            if (inputFields[i].text == "")
            {
                inputFields[i].text = med_values[index].ToString();
            }
        }

        loadWorld = false;
        worldName = userWorldName.text;
        inputWidth = int.Parse(userWidth.text);
        inputHeight = int.Parse(userHeight.text);
        inputSeed = int.Parse(userSeed.text);
        inputOctaves = int.Parse(userOctaves.text);
        inputScale = float.Parse(userScale.text);
        inputPersistence = float.Parse(userPersistence.text);
        inputLacunarity = float.Parse(userLacunarity.text);
        inputResDensity = float.Parse(userResDensity.text);
        inputEntDensity = float.Parse(userEntDensity.text);

        if (UnityEngine.Application.isEditor)
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
            mlagentsProcess.StandardInput.WriteLine("mlagents-learn trainer_config.yaml --run-id=" + worldName + @" --force --env=""Simulation\Ecosystem Simulation Project.exe"" --width=1920 --height=1080 --time-scale=1 --env-args " +
                inputWidth.ToString() + " " + inputHeight.ToString() + " " + inputSeed.ToString() + " " + inputOctaves.ToString() + " " + inputScale.ToString() + " " + inputPersistence.ToString() + " " +
                inputLacunarity.ToString() + " " + inputResDensity.ToString() + " " + inputEntDensity.ToString() + " " + loadWorld.ToString() + " " + worldName + " " + mlagentsProcess.Id.ToString());
        }
    }

    public void LoadGame()
    {
        if (userLoadWorld.text == "")
        {
            worldNotFound.text = "Please enter world name to load!";
        }
        else
        {
            worldName = userLoadWorld.text;
            string loadPath = UnityEngine.Application.persistentDataPath + "/Saves/" + worldName + ".json";
            if (!File.Exists(loadPath))
            {
                worldNotFound.text = "Save data not found!";
            }
            else
            {
                loadWorld = true;
                if (UnityEngine.Application.isEditor)
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
                    mlagentsProcess.StandardInput.WriteLine("mlagents-learn trainer_config.yaml --run-id=" + worldName + @" --resume --env=""Simulation\Ecosystem Simulation Project.exe"" --width=1920 --height=1080 --time-scale=1 --env-args " +
                        loadWorld.ToString() + " " + worldName + " " + mlagentsProcess.Id.ToString());
                }
            }
        }
    }

    public void SmallTemplate()
    {
        for (int i = 0; i < inputFields.Length; i++)
        {
            int index = i;
            inputFields[i].text = low_values[index].ToString();
            sliders[index].value = low_values[index];
        }
    }

    public void MedTemplate()
    {
        for (int i = 0; i < inputFields.Length; i++)
        {
            int index = i;
            inputFields[i].text = med_values[index].ToString();
            sliders[index].value = med_values[index];
        }
    }

    public void LargeTemplate()
    {
        for (int i = 0; i < inputFields.Length; i++)
        {
            int index = i;
            inputFields[i].text = high_values[index].ToString();
            sliders[index].value = high_values[index];
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
        userLoadWorld.text = "";
        worldNotFound.text = "";
        OptionPanel.alpha = 0;
        OptionPanel.blocksRaycasts = false;
    }

    public void ParamBack()
    {
        for (int i = 0; i < inputFields.Length; i++)
        {
            sliders[i].value = min_values[i];
            inputFields[i].text = "";
        }
        userWorldName.text = "";
        WorldParam.alpha = 0;
        WorldParam.blocksRaycasts = false;
    }

    public void ExitGame()
    {
        UnityEngine.Application.Quit();
    }
}
