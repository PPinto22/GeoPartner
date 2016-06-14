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
using System.Xml;

namespace GeoPartner.Business
{
    class geopartner
    {
        public List<atividade> atividades { get; set; }

        public geopartner()
        {
            this.atividades = new List<atividade>();
        }

        /*
         * Falta ler coordenadas
         */
        public void readXML(XmlDocument xmlDoc)
        {
            XmlNode sessao = xmlDoc.SelectSingleNode("sessao");
            if (sessao == null) throw new XmlException();

            XmlNode atividade = sessao.SelectSingleNode("atividade");
            if (atividade == null) throw new XmlException();

            for (; atividade != null; atividade = atividade.NextSibling)
            {
                if (atividade.NodeType == XmlNodeType.Element)
                {
                    XmlNode localizacao = atividade.SelectSingleNode("localizacao");
                    double longitude, latitude;
                    if (localizacao == null || localizacao.Attributes == null) throw new XmlException();
                    var attr = localizacao.Attributes["longitude"];
                    if (attr != null) longitude = double.Parse(attr.Value);
                    else throw new XmlException();
                    attr = localizacao.Attributes["latitude"];
                    if (attr != null) latitude = double.Parse(attr.Value);
                    else throw new XmlException();

                    XmlNode nodo_objetivos = atividade.SelectSingleNode("objetivos");
                    if (nodo_objetivos == null) throw new XmlException();
                    string objetivos = nodo_objetivos.Value;

                    XmlNode informacao = atividade.SelectSingleNode("informacao");
                    if (informacao == null) throw new XmlException();

                    XmlNode nodo_notas = informacao.SelectSingleNode("notas");
                    if (nodo_notas == null) throw new XmlException();
                    string notas = nodo_notas.Value;

                    XmlNode link = informacao.SelectSingleNode("link");
                    List<string> links = new List<string>();
                    for (; link != null; link = link.NextSibling)
                    {
                        if (link.NodeType == XmlNodeType.Element)
                        {
                            string endereco = link.Value;
                            links.Add(endereco);
                        }
                    }

                    atividade atv = new atividade(objetivos, notas, links);
                    this.atividades.Add(atv);
                }
            }
        }
    }
}