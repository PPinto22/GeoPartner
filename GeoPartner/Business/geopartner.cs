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
using Android.Gms.Maps.Model;

namespace GeoPartner.Business
{
    class geopartner
    {
        public List<atividade> atividades { get; set; }
        public int atividadeAtual { get; set; }

        public geopartner()
        {
            this.atividades = new List<atividade>();
            this.atividadeAtual = 0;
        }

        public bool isEmpty()
        {
            return this.atividadeAtual == 0;
        }

        public void addAtividade(atividade a)
        {
            this.atividades.Add(a);
        }

        public int porCompletar()
        {
            return this.atividades.Count - this.atividadeAtual;
        }

        public bool terminado()
        {
            return this.atividadeAtual >= this.atividades.Count;
        }

        public atividade getAtividadeAtual()
        {
            return this.atividades[atividadeAtual];
        }

        public bool hasNext()
        {
            return this.atividadeAtual < (this.atividades.Count - 1);
        }

        public atividade proximaAtividade()
        {
            if (this.atividadeAtual >= this.atividades.Count)
                return null;
            else return this.atividades[atividadeAtual++];
        }

        public string writeXML()
        {
            StringWriterWithEncoding sw = new StringWriterWithEncoding(Encoding.UTF8);

            XmlWriterSettings ws = new XmlWriterSettings();
            ws.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(sw, ws))
            {
                writer.WriteStartElement("sessao");

                writer.WriteStartElement("data");

                DateTime data = DateTime.Now;

                writer.WriteAttributeString("ano", data.Year.ToString());
                writer.WriteAttributeString("mes", data.Month.ToString());
                writer.WriteAttributeString("dia", data.Day.ToString());
                writer.WriteAttributeString("hora", data.Hour.ToString());
                writer.WriteAttributeString("minuto", data.Minute.ToString());

                writer.WriteEndElement(); //</data>

                foreach (atividade a in this.atividades)
                {
                    if (a.terminada)
                    {
                        a.writeXML(writer);
                    }
                }
                writer.WriteEndElement();
            }

            return sw.ToString();
        }

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
                    if (attr != null) longitude = double.Parse(attr.InnerText);
                    else throw new XmlException();
                    attr = localizacao.Attributes["latitude"];
                    if (attr != null) latitude = double.Parse(attr.InnerText);
                    else throw new XmlException();

                    XmlNode nodo_objetivos = atividade.SelectSingleNode("objetivos");
                    if (nodo_objetivos == null) throw new XmlException();
                    string objetivos = nodo_objetivos.InnerText;
                    if (objetivos == null) objetivos = string.Empty;

                    XmlNode informacao = atividade.SelectSingleNode("informacao");
                    if (informacao == null) throw new XmlException();

                    XmlNode nodo_notas = informacao.SelectSingleNode("notas");
                    if (nodo_notas == null) throw new XmlException();
                    string notas = nodo_notas.InnerText;


                    XmlNode link = informacao.SelectSingleNode("link");
                    List<string> links = new List<string>();
                    for (; link != null; link = link.NextSibling)
                    {
                        if (link.NodeType == XmlNodeType.Element)
                        {
                            string endereco = link.InnerText;
                            links.Add(endereco);
                        }
                    }

                    atividade atv = new atividade(latitude, longitude, objetivos, notas, links);
                    this.atividades.Add(atv);
                }
            }
        }
    }
}