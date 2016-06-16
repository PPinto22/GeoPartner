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
using GeoPartner.Business;
using Newtonsoft.Json;

namespace GeoPartner
{
    [Activity(Label = "Registo")]
    public class registoActivity : Activity
    {
        private atividade atv;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.layout_registo_rocha);

            atv = JsonConvert.DeserializeObject<atividade>(Intent.GetStringExtra("Atividade"));

            Button infoButton = FindViewById<Button>(Resource.Id.infoButton);
            infoButton.Click += delegate
            {
                var activity2 = new Intent(this, typeof(infoActivity));
                activity2.PutExtra("Atividade", JsonConvert.SerializeObject(atv));
                StartActivity(activity2);
            };
        }
    }
}