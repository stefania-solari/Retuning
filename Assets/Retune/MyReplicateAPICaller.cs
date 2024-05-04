using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Collections.Generic;




public class ReplicateAPICaller : MonoBehaviour
{
    private string APIToken = "YOUR_TOKEN_HERE";
    private string replicateAPIUrl = "https://api.replicate.com/v1/predictions";
    private string predictionId;


    
	[SerializeField]
    public AudioSource audioSource;

    // List to store strings to be added to the JSON text field
    private List<string> textList = new List<string>();
     string input = @"
    {
        ""version"": ""b61392adecdd660326fc9cfc5398182437dbe5e97b5decfb36e1a36de68b5b95"",
        ""input"": {
            ""text"": []
        }
    }";

   


   public GameObject audioPanel; 
   public GameObject loaderPanel;



    public void stopAndSendData()
    {
        StartCoroutine(PostPredictionRequest());
    }

    IEnumerator PostPredictionRequest()
    {
        audioPanel.SetActive(false);
        loaderPanel.SetActive(true);

        // Construct the JSON string representing the request body
        string input = @"
        {
            ""version"": ""b61392adecdd660326fc9cfc5398182437dbe5e97b5decfb36e1a36de68b5b95"",
            ""input"": {
                ""text"": ""glass and forks clicking, clean recording""
            }
        }";

        // Create a UnityWebRequest object for the POST request
        UnityWebRequest postRequest = UnityWebRequest.PostWwwForm(replicateAPIUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(input);
        postRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        postRequest.SetRequestHeader("Content-Type", "application/json");
        postRequest.SetRequestHeader("Authorization", "Bearer "+ APIToken);

        // Send the POST request
        yield return postRequest.SendWebRequest();

        if (postRequest.result == UnityWebRequest.Result.Success)
        {
            // Check if the downloadHandler is not null before accessing its text property
            if (postRequest.downloadHandler != null)
            {
                // Extract the prediction ID from the response
                string responseJson = postRequest.downloadHandler.text;
                predictionId = JsonUtility.FromJson<PredictionResponse>(responseJson).id;


                // Start polling for prediction status
                StartCoroutine(PollPredictionStatus());
            }
            else
            {
                Debug.LogError("Error: Download handler is null");
            }
        }
        else
        {
            Debug.LogError("Error: " + postRequest.error);
        }
    }

    IEnumerator PollPredictionStatus()
    {
        while (true)
        {
            // Construct the URL for getting the prediction status
            string getUrl = $"{replicateAPIUrl}/{predictionId}";

            // Create a UnityWebRequest object for the GET request
            UnityWebRequest getRequest = UnityWebRequest.Get(getUrl);
            getRequest.SetRequestHeader("Authorization", "Bearer "+ APIToken);

            // Send the GET request
            yield return getRequest.SendWebRequest();

            if (getRequest.result == UnityWebRequest.Result.Success)
            {
                // Parse the response to check the prediction status
                string responseJson = getRequest.downloadHandler.text;
                PredictionStatusResponse statusResponse = JsonUtility.FromJson<PredictionStatusResponse>(responseJson);

                if (statusResponse.status == "succeeded")
                {

                    Debug.Log("Prediction succeeded!");
                    // Handle prediction result

                    // Output prediction data to the console
                    Debug.Log("Prediction Data: " + responseJson);
                    
                    // Inside the PollPredictionStatus() coroutine after receiving the response
                    int maxChunkSize = 1024; // Maximum characters per chunk
                    for (int i = 0; i < responseJson.Length; i += maxChunkSize)
                    {
                        int length = Mathf.Min(maxChunkSize, responseJson.Length - i);
                        string chunk = responseJson.Substring(i, length);
                        Debug.Log("Prediction Response Chunk " + (i / maxChunkSize) + ": " + chunk);
                    }

                    string filePath;
                    // Inside the PollPredictionStatus() coroutine after receiving the response
                    if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                    {
                        filePath = Path.Combine(Application.persistentDataPath, "prediction_response.json");
                    }
                    else
                    {
                        filePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "prediction_response.json");
                    }
                    File.WriteAllText(filePath, responseJson);
                    Debug.Log("Prediction response written to file: " + filePath);


                    // Parse prediction data
                    PredictionData predictionData = JsonUtility.FromJson<PredictionData>(responseJson);
                    // Access parsed data
                    Debug.Log("Prediction Result: " + predictionData.result);
                    Debug.Log("Prediction Output: " + predictionData.output);
                    StartCoroutine(DownloadAndPlayAudio(predictionData.output));
                     // Check if the prediction result is empty

                    if (string.IsNullOrEmpty(predictionData.result))
                    {
                        Debug.LogWarning("Prediction result is empty!");
                        // Handle empty prediction result
                    }
                    else
                    {
                        // Access parsed data
                        Debug.Log("Prediction Result: " + predictionData.result);
                        // You can access other properties of predictionData similarly
                    }
                    
                    break;
                }
                else if (statusResponse.status == "failed")
                {
                    Debug.LogError("Prediction failed!");
                    // Handle prediction failure
                    break;
                }
            }
            else
            {
                Debug.LogError("Error: " + getRequest.error);
            }

            // Wait before sending the next GET request
            yield return new WaitForSeconds(2f); // Adjust the polling interval as needed
        }
    }


IEnumerator DownloadAndPlayAudio(string audioURL)
    {
        loaderPanel.SetActive(false);
        audioPanel.SetActive(true);
        
        // Make the GET request
        using (UnityWebRequest www = UnityWebRequest.Get(audioURL))
        {
            // Request the audio asynchronously
            yield return www.SendWebRequest();

            // Check for errors
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Failed to download audio: " + www.error);
            }
            else
            {

                string filePath;
                   // Check if running on mobile
                if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    // Save the audio file to persistent data path
                    filePath = Path.Combine(Application.persistentDataPath, "audio.wav");
                }
                else
                {
                    // Save the audio file to cache directory
                    filePath = Path.Combine(Application.temporaryCachePath, "audio.wav");
                }
                
                File.WriteAllBytes(filePath, www.downloadHandler.data);
                Debug.Log("Audio saved to cache: " + filePath);

                // Load the audio file as AudioClip
                AudioClip audioClip = LoadAudioFromFile(filePath);

                // Play the audio
                if (audioClip != null)
                {
                    audioSource.clip = audioClip;
                    //audioSource.Play();
                }
                else
                {
                    Debug.Log("Failed to load audio clip.");
                }
            }
        }
    }

    AudioClip LoadAudioFromFile(string path)
    {
        // Load the audio file from path
        WWW www = new WWW("file://" + path);
        while (!www.isDone) { }

        if (string.IsNullOrEmpty(www.error))
        {
            return www.GetAudioClip(false, false);
        }
        else
        {
            Debug.LogError("Failed to load audio: " + www.error);
            return null;
        }
    }


 
    [System.Serializable]
    private class PredictionResponse
    {
        public string id;
    
    }

    [System.Serializable]
    private class PredictionStatusResponse
    {
        public string status;
    }
        [System.Serializable]
    private class PredictionData
    {
        // Define properties corresponding to the JSON structure
        public string result;
        public string input;
        public string output;
        // Add other properties as needed
    }
}

