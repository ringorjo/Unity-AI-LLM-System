using System;

namespace Services.AI
{
    public static class RunSplitResponseUtil
    {
        private static int _lastIndexLenght = 0;
        private static int _index = 0;
        public static void SplitResponse(string response, Action<OutputResponseData> action)
        {
            var split = response.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != _lastIndexLenght)
            {
                for (int i = _index; i < split.Length - 1; i++)
                {
                    _index++;
                    if (i % 2 == 0)
                    {
                        action?.Invoke(new OutputResponseData(split[i], split[i + 1]));
                    }
                }
                _lastIndexLenght = split.Length;
            }
        }

        public static void CleanData()
        {
            _lastIndexLenght = 0;
            _index = 0;
        }
    }
}
