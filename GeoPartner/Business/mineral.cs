using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BackOffice.Business
{
    public class mineral
    {
        public string designacao { get; set; }
        public string risca { get; set; }
        public float peso { get; set; }
        public string cor { get; set; }

        public mineral() {
            this.designacao = string.Empty;
            this.peso = 0;
            this.risca = string.Empty;
            this.cor = string.Empty;
        }
        public mineral(string designacao, string risca, float peso, string cor)
        {
            this.designacao = designacao;
            this.peso = peso;
            this.risca = risca;
            this.cor = cor;
        }

        public void writeXML(XmlWriter writer)
        {
            writer.WriteStartElement("rocha");

            writer.WriteStartElement("designacao");
            writer.WriteString(this.designacao);
            writer.WriteEndElement();//</designacao>

            writer.WriteStartElement("peso");
            writer.WriteString(this.peso.ToString(CultureInfo.InvariantCulture.NumberFormat));
            writer.WriteEndElement();//</peso>

            writer.WriteStartElement("risca");
            writer.WriteString(this.risca);
            writer.WriteEndElement();//</risca>

            writer.WriteStartElement("cor");
            writer.WriteString(this.cor);
            writer.WriteEndElement();//</cor>

            writer.WriteEndElement(); //</rocha>
        }
    }
}
