namespace LAN.LiveLocation {
    [System.Serializable]
    public class LocationPayload {
        public string data, descriptor;

        public T GetDescriptor<T>() {
            return UnityEngine.JsonUtility.FromJson<T>(descriptor);
        }

        public Location GetData() {
            return UnityEngine.JsonUtility.FromJson<Location>(data);
        }
    }

}