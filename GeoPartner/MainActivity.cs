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
        private string file_path = Android.OS.Environment.ExternalStorageDirectory.Path + "/GeoPartner/percurso.gp";



        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);
            gp = new geopartner();

            string path = Android.OS.Environment.ExternalStorageDirectory.Path + "/GeoPartner/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Button iniciar = FindViewById<Button>(Resource.Id.button2);

            iniciar.Click += delegate
            {
                if (File.Exists(file_path))
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(file_path);
                        gp = new geopartner();
                        gp.readXML(doc);

                        var percursoActivity = new Intent(this, typeof(percursoActivity));
                        percursoActivity.PutExtra("Percurso", JsonConvert.SerializeObject(gp));
                        StartActivity(percursoActivity);

                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, "Ficheiro "+file_path+" inválido.", ToastLength.Long).Show();

                    }
                }
                else
                {
      
                    Toast.MakeText(this, "Ficheiro " + file_path +" não foi encontrado.", ToastLength.Short).Show();
                }
            };
        }
    }
}

