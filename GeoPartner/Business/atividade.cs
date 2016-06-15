using Android.Gms.Maps.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
    }
}
