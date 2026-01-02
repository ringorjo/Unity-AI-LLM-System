using System.Collections.Generic;

public class StringChunkBuilder
{
    public static List<string> ChunkString(string input, char separator = '.')
    {
        List<string> chunks = new List<string>();
        if (string.IsNullOrWhiteSpace(input))
            return chunks;

        int lastCut = 0;
        for (int i = 0; i < input.Length; i++)
        {
            char currentChar = input[i];
            if (currentChar == separator)
            {
                int length = i - lastCut + 1;
                string chunk = input.Substring(lastCut, length).Trim();
                if (!string.IsNullOrEmpty(chunk))
                    chunks.Add(chunk);

                lastCut = i + 1;
            }
        }

        if (lastCut < input.Length)
        {
            string lastChunk = input.Substring(lastCut).Trim();
            if (!string.IsNullOrEmpty(lastChunk))
                chunks.Add(lastChunk);
        }

        return chunks;
    }
}
