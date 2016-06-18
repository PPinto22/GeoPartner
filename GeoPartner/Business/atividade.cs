using Android.Gms.Maps.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GeoPartner.Business
{
    class atividade
    {
        public LatLng coordenadas { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string objetivos { get; set; }
        public string notas { get; set; }
        public List<string> websites { get; set; }
        public bool terminada { get; set; }

        public registo registo { get; set; }

        public atividade()
        {
            this.latitude = 0;
            this.longitude = 0;
            this.objetivos = string.Empty;
            this.notas = string.Empty;
            this.websites = new List<string>();
            this.terminada = false;
        }

        public atividade(double lat, double lng, string objetivos, string notas, List<string> websites)
        {
            this.latitude = lat;
            this.longitude = lng;
            this.coordenadas = coordenadas;
            this.objetivos = objetivos;
            this.notas = notas;
            this.websites = websites;
            this.terminada = false;
        }

        public LatLng getCoordenadas()
        {
            return new LatLng(this.latitude, this.longitude);
        }

        public void writeXML(XmlWriter writer)
        {
            writer.WriteStartElement("atividade");

            //localizacao
            writer.WriteStartElement("localizacao");
            writer.WriteStartAttribute("longitude");
            writer.WriteString(this.longitude.ToString(CultureInfo.InvariantCulture.NumberFormat));
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("latitude");
            writer.WriteString(this.latitude.ToString(CultureInfo.InvariantCulture.NumberFormat));
            writer.WriteEndAttribute();
            writer.WriteEndElement();

            //Objetivos
            writer.WriteStartElement("objetivos");
            writer.WriteString(this.objetivos);
            writer.WriteEndElement();

            //Registo
            this.registo.writeXML(writer);

            writer.WriteEndElement(); // </atividade>
        }
    }
}
