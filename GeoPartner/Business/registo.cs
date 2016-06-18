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
using Java.IO;
using System.Xml;

namespace GeoPartner
{
    class registo
    {
        public const int ROCHA = 0;
        public const int MINERAL = 1;
        public const int ND = -1;
        public byte[] foto { get; set; }
        public byte[] voz { get; set; }
        public int tipo { get; set; }

        public rocha rocha { get; set; }
        public mineral mineral { get; set; }

        public registo()
        {
            this.foto = null;
            this.tipo = ND;
        }

        public registo(rocha rocha, Bitmap foto, byte[] voz)
        {
            this.tipo = ROCHA;
            this.rocha = rocha;
            if (foto != null)
                this.foto = registo.imageToByteArray(foto);
            else this.foto = null;
            this.voz = voz;
        }

        public registo(mineral mineral, Bitmap foto, byte[] voz)
        {
            this.tipo = MINERAL;
            this.mineral = mineral;
            if (foto != null)
                this.foto = registo.imageToByteArray(foto);
            else this.foto = null;
            this.voz = voz;
        }

        public static byte[] imageToByteArray(Bitmap imageIn)
        {
            MemoryStream stream = new MemoryStream();
            imageIn.Compress(Bitmap.CompressFormat.Jpeg, 0, stream);
            byte[] bitmapData = stream.ToArray();
            return bitmapData;
        }

        public void writeXML(XmlWriter writer)
        {
            writer.WriteStartElement("registo");
            writer.WriteStartElement("fotografias");
            writer.WriteStartElement("fotografia");
            if (this.foto != null)
            {
                writer.WriteString(Convert.ToBase64String(this.foto));
            }
            writer.WriteEndElement(); //</fotografia>
            writer.WriteEndElement(); //</fotografias>

            writer.WriteStartElement("voz");
            if (this.voz != null)
            {
                writer.WriteString(Convert.ToBase64String(this.voz));
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