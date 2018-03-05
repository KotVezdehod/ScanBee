using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;        //Microsoft.Bcl.Build (nuget)
using System.IO;


namespace ScanBee
{
    class DataCompression
    {
        public string CompressString(string inString)
        {

            byte[] in_Bytes = new byte[Encoding.Unicode.GetByteCount(inString)];
            in_Bytes = Encoding.UTF8.GetBytes(inString);

            MemoryStream out_ms = new MemoryStream();
            
            using (DeflateStream dfs = new DeflateStream(out_ms, CompressionLevel.Optimal, true))       //There is beta of Microsoft.Bcl.Build, that is why it's needed to call Dispose() method, or output stream will be empty
            {
                dfs.Write(in_Bytes, 0, in_Bytes.Length);
            }

            //DeflateStream dfs = new DeflateStream(out_ms,CompressionLevel.Optimal,true);
            //dfs.Dispose();
            out_ms.Position = 0;
            

            byte[] out_bytes = new byte[out_ms.Length];
            out_ms.Read(out_bytes, 0, out_bytes.Length);

            return Convert.ToBase64String(out_bytes);
            //string B64 = Convert.ToBase64String(out_bytes);
            
        }

        public string Extract(string inStr)
        {
            const int BUFFER_SIZE = 500;

            List <byte[]> listCommon = new List<byte[]>();
            byte[] tmpByte = new byte[BUFFER_SIZE];

            byte[] in_b = new byte[inStr.Length*2];
            in_b = Convert.FromBase64String(inStr);

            MemoryStream in_ms = new MemoryStream();
            in_ms.Write(in_b, 0, in_b.Length);
            in_ms.Position = 0;

            int intWholeSz = 0;
            using (DeflateStream ds = new DeflateStream(in_ms, CompressionMode.Decompress, true))
            {
                while (true)
                {
                    int Read = 0;
                    Read = ds.Read(tmpByte, 0, BUFFER_SIZE);
                    intWholeSz = intWholeSz + Read;
                    if (Read == BUFFER_SIZE)
                    {
                        listCommon.Add(tmpByte);
                        tmpByte = new byte[BUFFER_SIZE];
                    }
                    else
                    {
                        byte[] tmByteSmall = new byte[Read];
                        Buffer.BlockCopy(tmpByte, 0, tmByteSmall, 0, Read);
                        listCommon.Add(tmByteSmall);
                        break;
                    }
                }
            }

            byte[] byte_Whole = new byte[intWholeSz];

            int copyed = 0;
            foreach (byte[] arrByte in listCommon)
            {
                arrByte.CopyTo(byte_Whole, copyed);
                copyed = copyed + arrByte.Length;
            }
            
            string strout = Encoding.UTF8.GetString(byte_Whole, 3, byte_Whole.Length-3);        //+3 this is some shit

            strout = strout.Replace("\r\r\n", "\r\n");      //more shit
            
            return strout;

        }

    }
}
