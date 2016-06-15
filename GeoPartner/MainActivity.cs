using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Xml;
using GeoPartner.Business;

namespace GeoPartner
{
    [Activity(Label = "GeoPartner", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private geopartner gp;
        private string file_path;

        protected override void OnCreate(Bundle bundle)
        {
            file_path = "/GeoPartner/percurso.gp";
            gp = new geopartner();
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Button procurar = FindViewById<Button>(Resource.Id.button1);
            procurar.Click += delegate
            {
                
            };

            Button iniciar = FindViewById<Button>(Resource.Id.button2);

            iniciar.Click += delegate {
                EditText editText1 = FindViewById<EditText>(Resource.Id.editText1);
                //string file_path = editText1.Text;
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(file_path);
                    gp.readXML(doc);
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

