using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
namespace CLAP
{
    public static class Tools
    {


        private const char CR = '\r';
        private const char LF = '\n';
        private const char NULL = (char)0;


        //http://www.nimaara.com/2018/03/20/counting-lines-of-a-text-file/
        public static long CountLines(StreamReader stream)
        {
            //Ensure.NotNull(stream, nameof(stream));

            var lineCount = 0L;

            var byteBuffer = new char[1024 * 1024];
            var detectedEOL = NULL;
            var currentChar = NULL;

            int bytesRead;
            while ((bytesRead = stream.Read(byteBuffer, 0, byteBuffer.Length)) > 0)
            {
                for (var i = 0; i < bytesRead; i++)
                {
                    currentChar = (char)byteBuffer[i];

                    if (detectedEOL != NULL)
                    {
                        if (currentChar == detectedEOL)
                        {
                            lineCount++;
                        }
                    }
                    else if (currentChar == LF || currentChar == CR)
                    {
                        detectedEOL = currentChar;
                        lineCount++;
                    }
                }
            }

            // We had a NON-EOL character at the end without a new line
            if (currentChar != LF && currentChar != CR && currentChar != NULL)
            {
                lineCount++;
            }
            return lineCount;
        }
    }
}
