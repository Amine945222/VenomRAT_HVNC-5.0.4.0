﻿using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace VenomRAT_HVNC.StreamLibrary.src
{
    // Token: 0x0200000B RID: 11
    public class JpgCompression
    {
        // Token: 0x06000047 RID: 71 RVA: 0x00002DF0 File Offset: 0x00002DF0
        public JpgCompression(int Quality)
        {
            this.parameter = new EncoderParameter(Encoder.Quality, (long)Quality);
            this.encoderInfo = this.GetEncoderInfo("image/jpeg");
            this.encoderParams = new EncoderParameters(2);
            this.encoderParams.Param[0] = this.parameter;
            this.encoderParams.Param[1] = new EncoderParameter(Encoder.Compression, 2L);
        }

        // Token: 0x06000048 RID: 72 RVA: 0x00002E60 File Offset: 0x00002E60
        public byte[] Compress(Bitmap bmp)
        {
            byte[] result;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bmp.Save(memoryStream, this.encoderInfo, this.encoderParams);
                result = memoryStream.ToArray();
            }
            return result;
        }

        // Token: 0x06000049 RID: 73 RVA: 0x00002EAC File Offset: 0x00002EAC
        public void Compress(Bitmap bmp, ref Stream TargetStream)
        {
            bmp.Save(TargetStream, this.encoderInfo, this.encoderParams);
        }

        // Token: 0x0600004A RID: 74 RVA: 0x00002EC4 File Offset: 0x00002EC4
        private ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
            int num = imageEncoders.Length - 1;
            for (int i = 0; i <= num; i++)
            {
                if (imageEncoders[i].MimeType == mimeType)
                {
                    return imageEncoders[i];
                }
            }
            return null;
        }

        // Token: 0x04000019 RID: 25
        private EncoderParameter parameter;

        // Token: 0x0400001A RID: 26
        private ImageCodecInfo encoderInfo;

        // Token: 0x0400001B RID: 27
        private EncoderParameters encoderParams;
    }
}