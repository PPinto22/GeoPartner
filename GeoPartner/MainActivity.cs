using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Xml;
using GeoPartner.Business;
using Android.Gms.Maps.Model;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace GeoPartner
{
    [Activity(Label = "GeoPartner", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private geopartner gp;
        private string file_path = "/sdcard/Android/data/GeoPartner.GeoPartner/percurso.gp";

        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);
            gp = new geopartner();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Button procurar = FindViewById<Button>(Resource.Id.button1);
            procurar.Click += delegate
            {
                
                
            };

            Button iniciar = FindViewById<Button>(Resource.Id.button2);

            iniciar.Click += delegate
            {

                if (File.Exists(file_path))
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(file_path);
                        gp.readXML(doc);


                        var activity2 = new Intent(this, typeof(registoActivity));
                        //activity2.PutExtra("Percurso", JsonConvert.SerializeObject(gp));
                        activity2.PutExtra("Atividade", JsonConvert.SerializeObject(gp.atividades[0]));
                        StartActivity(activity2);
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, "Configuração de percurso inválida", ToastLength.Long).Show();

                    }
                }
                else
                {
                    Toast.MakeText(this, "Ficheiro percurso.gp não foi encontrado na diretoria Android/data/GeoPartner.GeoPartner", ToastLength.Short).Show();
                }
            };
        }

    }
}

/*                try
                {
                    gp = new geopartner();

                    List<string> websites = new List<string>();
                    websites.Add("http://www.google.pt");
                    websites.Add("http://www.wikipedia.com");
                    atividade a1 = new atividade(new LatLng(5.5, -5.5), "objetivos...\nadskjladskjl\naksldjasdjkldsak", "notas...", websites);
                    atividade a2 = new atividade(new LatLng(6.6, -6.6), "objetivos...", "notas...", websites);

                    gp.addAtividade(a1);
                    gp.addAtividade(a2); */

