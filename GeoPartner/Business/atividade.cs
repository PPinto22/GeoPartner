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
        public string objetivos { get; set; }
        public string notas { get; set; }
        public List<string> websites { get; set; }
        public bool terminada { get; set; }

        public registo registo { get; set; }

        public atividade()
        {
            this.coordenadas = new LatLng(0.0,0.0);
            this.objetivos = string.Empty;
            this.notas = string.Empty;
            this.websites = new List<string>();
            this.terminada = false;
        }

        public atividade(LatLng coordenadas, string objetivos, string notas, List<string> websites)
        {
            this.coordenadas = coordenadas;
            this.objetivos = objetivos;
            this.notas = notas;
            this.websites = websites;
            this.terminada = false;
        }

        public void writeXML(XmlWriter writer)
        {
            writer.WriteStartElement("atividade");

            //localizacao
            writer.WriteStartElement("localizacao");
            writer.WriteStartAttribute("longitude");
            writer.WriteString(coordenadas.Longitude.ToString(CultureInfo.InvariantCulture.NumberFormat));
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("latitude");
            writer.WriteString(coordenadas.Latitude.ToString(CultureInfo.InvariantCulture.NumberFormat));
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
