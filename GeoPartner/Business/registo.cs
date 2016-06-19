using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using BackOffice.Business;
using System.IO;
using Android.Media;
using System.Xml;

namespace GeoPartner
{
    class registo
    {
        public const int ROCHA = 0;
        public const int MINERAL = 1;
        public const int ND = -1;
        public string foto { get; set; } // path para ficheiro .jpg
        public string voz { get; set; } // path para ficheiro .wav
        public int tipo { get; set; }

        public rocha rocha { get; set; }
        public mineral mineral { get; set; }

        public registo()
        {
            this.foto = null;
            this.tipo = ND;
        }

        public registo(rocha rocha, string foto, string voz)
        {
            this.tipo = ROCHA;
            this.rocha = rocha;
            this.foto = foto;
            this.voz = voz;
        }

        public registo(mineral mineral, string foto, string voz)
        {
            this.tipo = MINERAL;
            this.mineral = mineral;
            this.foto = foto;
            this.voz = voz;
        }

        public static byte[] imageToByteArray(Bitmap bitmap)
        {
            byte[] bitmapData;
            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                bitmapData = stream.ToArray();
            }
            return bitmapData;

        }

        public void writeXML(XmlWriter writer)
        {
            writer.WriteStartElement("registo");
            writer.WriteStartElement("fotografias");
            writer.WriteStartElement("fotografia");
            if (!String.IsNullOrEmpty(this.foto))
            {
                Bitmap bitmap_foto = BitmapFactory.DecodeFile(this.foto);
                byte[] bytes_foto = registo.imageToByteArray(bitmap_foto);
                writer.WriteString(Convert.ToBase64String(bytes_foto));
                File.Delete(this.foto);
            }
            writer.WriteEndElement(); //</fotografia>
            writer.WriteEndElement(); //</fotografias>

            writer.WriteStartElement("voz");
            if (!String.IsNullOrEmpty(this.voz))
            {
                byte[] bytes_voz = File.ReadAllBytes(this.voz);
                writer.WriteString(Convert.ToBase64String(bytes_voz));
                //File.Delete(this.voz);
            }
            writer.WriteEndElement(); //</voz>

            if(this.tipo == ROCHA)
            {
                this.rocha.writeXML(writer);
            }
            else if(this.tipo == MINERAL)
            {
                this.mineral.writeXML(writer);
            }

            writer.WriteEndElement(); //</registo>
        }
    }
}