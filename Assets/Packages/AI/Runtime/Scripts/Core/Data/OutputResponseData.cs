using System;
using UnityEngine;

namespace Services.AI
{
    [Serializable]
    public class OutputResponseData
    {
        [SerializeField]
        public string type;
        [SerializeField]
        public string data;

        public OutputResponseData(string type, string data)
        {
            this.type = CleanData(type, "event: ");
            this.data = CleanData(data, "data: ");
        }

        public OutputResponseData()
        {

        }

        public string CleanData(string data, string deletestring)
        {
            return data.Replace(deletestring, "");
        }

    }
}