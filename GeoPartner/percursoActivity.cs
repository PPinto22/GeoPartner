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
using Android.Gms.Maps;
using GeoPartner.Business;
using Newtonsoft.Json;

namespace GeoPartner
{
    [Activity(Label = "percursoActivity")]
    public class percursoActivity : Activity, IOnMapReadyCallback
    {
        private GoogleMap mMap;
        private geopartner gp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            gp = JsonConvert.DeserializeObject<geopartner>(Intent.GetStringExtra("Percurso"));

            Toast.MakeText(this,""+gp.atividades.Count, ToastLength.Short).Show();


            SetContentView(Resource.Layout.layout_percurso);
            SetUpMap();
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            mMap = googleMap;
        }

        private void SetUpMap()
        {
            if(mMap == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            }
        }
    }
}