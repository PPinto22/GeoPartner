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

namespace GeoPartner
{
    class registo
    {
        public const int ROCHA = 0;
        public const int MINERAL = 1;
        public const int ND = -1;
        public List<Bitmap> fotos { get; set; }
        public int tipo { get; set; }

        public rocha rocha { get; set; }
        public mineral mineral { get; set; }

        public registo()
        {
            this.fotos = new List<Bitmap>();
            this.tipo = ND;
        }

        public registo(int tipo, List<Bitmap> fotos)
        {
            this.tipo = tipo;
            this.fotos = fotos;
        }

        public void addFoto(Bitmap foto)
        {
            this.fotos.Add(foto);
        }
    }
}