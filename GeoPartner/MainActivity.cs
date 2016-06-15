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

namespace GeoPartner
{
    [Activity(Label = "GeoPartner", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private geopartner gp;
        private string file_path;

        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Button procurar = FindViewById<Button>(Resource.Id.button1);
            procurar.Click += delegate
            {
                
            };

            Button iniciar = FindViewById<Button>(Resource.Id.button2);

            iniciar.Click += delegate {
                /*EditText editText1 = FindViewById<EditText>(Resource.Id.editText1);
                string file_path = editText1.Text;
                XmlDocument doc = new XmlDocument();
                doc.Load(file_path);
                gp.readXML(doc);*/
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

                    var activity2 = new Intent(this, typeof(percursoActivity));
                    activity2.PutExtra("Percurso", JsonConvert.SerializeObject(gp));
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

