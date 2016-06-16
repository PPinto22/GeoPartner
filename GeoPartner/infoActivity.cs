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
using Newtonsoft.Json;
using GeoPartner.Business;
using Android.Webkit;

namespace GeoPartner
{
    [Activity(Label = "infoActivity")]
    public class infoActivity : Activity
    {
        private atividade atv;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.layout_info);

            atv = JsonConvert.DeserializeObject<atividade>(Intent.GetStringExtra("Atividade"));
    
            EditText objetivos = FindViewById<EditText>(Resource.Id.objetivos);
            objetivos.Text = atv.objetivos;
            objetivos.KeyListener = null;

            EditText notas = FindViewById<EditText>(Resource.Id.notas);
            notas.Text = atv.notas;
            notas.KeyListener = null;


            ListView lvWebsites = FindViewById<ListView>(Resource.Id.listViewWebsites);
            lvWebsites.ItemClick += LvWebsites_ItemClick;
            List<string> items = new List<string>();
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, items);
            lvWebsites.Adapter = adapter;
            foreach(string link in atv.websites)
            {
                adapter.Add(link);
            }
            adapter.NotifyDataSetChanged();
        }

        private void LvWebsites_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            ListView lvWebsites = FindViewById<ListView>(Resource.Id.listViewWebsites);
            string item = (string)lvWebsites.GetItemAtPosition(e.Position);

            WebView webView1 = new WebView(this);
            webView1.Settings.JavaScriptEnabled = true;
            webView1.LoadUrl(item);
        }
    }
}