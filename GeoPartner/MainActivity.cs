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
        // private string file_path = "\\Internal Storage\\Android\\data\\GeoPartner.GeoPartner\\percurso.gp";

        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Button procurar = FindViewById<Button>(Resource.Id.button1);
            procurar.Click += delegate
            {
                if (File.Exists(file_path))
                {
                    Toast.MakeText(this, "existe", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, "nao existe", ToastLength.Short).Show();
                }
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(file_path);
                    Toast.MakeText(this, "load ok", ToastLength.Short).Show();

                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();

                }
            };

            Button iniciar = FindViewById<Button>(Resource.Id.button2);

            iniciar.Click += delegate {

                try
                {
                    gp = new geopartner();

                    List<string> websites = new List<string>();
                    websites.Add("http://www.google.pt");
                    websites.Add("http://www.wikipedia.com");
                    atividade a1 = new atividade(new LatLng(5.5, -5.5), "objetivos...", "notas...", websites);
                    atividade a2 = new atividade(new LatLng(6.6, -6.6), "objetivos...", "notas...", websites);

                    gp.addAtividade(a1);
                    gp.addAtividade(a2);



                    var activity2 = new Intent(this, typeof(registoActivity));
                    //activity2.PutExtra("Percurso", JsonConvert.SerializeObject(gp));
                    activity2.PutExtra("Atividade", JsonConvert.SerializeObject(a1));
                    StartActivity(activity2);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
                    // Toast.MakeText(this, "Ficheiro inválido", ToastLength.Short).Show();
                }
            };
        }

    }
}

