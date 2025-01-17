﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.IO.Compression;

namespace Core.System
{
    public class Compressor
    {
        public static byte[] Compress(byte[] rawData)
        {
            byte[] result = null;

            using (MemoryStream compressedStream = new MemoryStream())
            {
                using (GZipStream gZipStream = new GZipStream(compressedStream, CompressionMode.Compress))
                {
                    gZipStream.Write(rawData, 0, rawData.Length);
                }
                result = compressedStream.ToArray();
            }

            return result;
        }

        public static byte[] Decompress(byte[] compressedData)
        {
            byte[] result = null;

            using (MemoryStream compressedStream = new MemoryStream(compressedData))
            {
                using (MemoryStream decompressedStream = new MemoryStream())
                {
                    using (GZipStream gZipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                    {
                        gZipStream.CopyTo(decompressedStream);
                    }
                    result = decompressedStream.ToArray();
                }
            }

            return result;
        }
    }
}