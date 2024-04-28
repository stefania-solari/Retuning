[System.Serializable]
public class PredictionResponse
{
    public string id;
    public string model;
    public string version;
    public InputData input;
    public string logs;
    public string error;
    public string status;
    public string created_at;
    public Urls urls;

    [System.Serializable]
    public class InputData
    {
        public string text;
    }

    [System.Serializable]
    public class Urls
    {
        public string cancel;
        public string get;
    }
}
