using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SnowKore.Utils
{
    public static class SnowKoreExtensionsMethods
    {
        public static string ToJson(this Dictionary<string, object> dictionary)
        {
            return JsonConvert.SerializeObject(dictionary);
        }

        public static string ToURL(this Dictionary<string, object> dictionary)
        {
            if (dictionary == null)
                return string.Empty;

            if (dictionary.Count == 0)
                return string.Empty;

            string urlParameters = "?";

            foreach (KeyValuePair<string, object> item in dictionary)
                urlParameters += item.Key + "=" + item.Value + "&";

            urlParameters.Remove(urlParameters.Length - 1);
            return urlParameters;
        }

        public static string ReplaceKeys (this string text, params string[] replacements)
        {
            string replacedText = text;

            if (replacements != null && replacements.Length > 0)
            {
                for (int i = 0; i < replacements.Length; i++)
                    replacedText = replacedText.Replace("{" + i + "}", replacements[i]);
            }

            return replacedText;
        }

        private static int CountCornersVisibleFrom(this RectTransform rectTransform, Camera camera)
        {
            Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height);
            Vector3[] objectCorners = new Vector3[4];
            rectTransform.GetWorldCorners(objectCorners);

            int visibleCorners = 0;
            Vector3 tempScreenSpaceCorner;
            for (var i = 0; i < objectCorners.Length; i++)
            {
                tempScreenSpaceCorner = camera.WorldToScreenPoint(objectCorners[i]);

                if (screenBounds.Contains(tempScreenSpaceCorner))
                    visibleCorners++;
            }

            return visibleCorners;
        }

        public static bool IsFullyVisibleFrom(this RectTransform rectTransform, Camera camera)
        {
            return CountCornersVisibleFrom(rectTransform, camera) == 4; // True if all 4 corners are visible
        }

        public static bool IsVisibleFrom(this RectTransform rectTransform, Camera camera)
        {
            return CountCornersVisibleFrom(rectTransform, camera) > 0;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return true;

            var collection = enumerable as ICollection<T>;

            if (collection != null)
                return collection.Count < 1;

            return !enumerable.Any();
        }
    }
}
