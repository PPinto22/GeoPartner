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
using Android.Graphics;
using System.IO;

namespace GeoPartner
{
    [Activity(Label = "Percurso")]
    public class percursoActivity : Activity/*, IOnMapReadyCallback*/
    {
        //private GoogleMap mMap;
        private geopartner gp;
        private Button buttonTerminar;
        private Button buttonRegisto;
        private TextView textView1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.gp = JsonConvert.DeserializeObject<geopartner>(Intent.GetStringExtra("Percurso"));

            SetContentView(Resource.Layout.layout_percurso_backup);

            this.textView1 = FindViewById<TextView>(Resource.Id.textView1);
            this.updateTextoAtividadeAtual();

            this.buttonTerminar = FindViewById<Button>(Resource.Id.buttonTerminar);
            this.buttonTerminar.Click += ButtonTerminar_Click;

            this.buttonRegisto = FindViewById<Button>(Resource.Id.buttonRegisto);
            this.buttonRegisto.Click += ButtonRegisto_Click;


            //SetUpMap();
        }

        private void ButtonRegisto_Click(object sender, EventArgs e)
        {
            Intent registoActivity = new Intent(this, typeof(registoActivity));
            registoActivity.AddFlags(ActivityFlags.ClearTop);
            registoActivity.PutExtra("Atividade", JsonConvert.SerializeObject(gp.getAtividadeAtual()));
            StartActivityForResult(registoActivity, 1000);
        }

        private void ButtonTerminar_Click(object sender, EventArgs e)
        {
            if (!this.gp.terminado())
            {
                new AlertDialog.Builder(this)
                .SetPositiveButton("Sim", (sender2, args) =>
                {
                    if (this.gp.isEmpty())
                    {
                        Toast.MakeText(this, "Percurso cancelado", ToastLength.Long).Show();
                        Finish();
                    }
                    else
                    {
                        this.terminar();
                    }
                })
                .SetNegativeButton("Não", (sender2, args) =>
                {

                })
                .SetMessage(String.Format("Existem {0} atividades por completar.\nTerminar o percurso?", this.gp.porCompletar()))
                .SetTitle("Aviso")
                .Show();
            }
            else
            {
                this.terminar();
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            switch (requestCode)
            {
                case 1000: //Fim registo
                    if (resultCode == Result.Ok)
                    {
                        registo reg = JsonConvert.DeserializeObject<registo>(data.GetStringExtra("Registo"));
                        this.gp.getAtividadeAtual().registo = reg;
                        this.gp.getAtividadeAtual().terminada = true;
                        this.gp.atividadeAtual++;
                        this.updateTextoAtividadeAtual();
                        if (gp.terminado())
                        {
                            Toast.MakeText(this, "Percurso Terminado", ToastLength.Short).Show();
                            this.buttonRegisto.Enabled = false;
                        }
                        else
                        {
                            Toast.MakeText(this, "Registo guardado", ToastLength.Short).Show();
                        }
                    }
                    break;
            }
        }

        private void terminar()
        {
            string path = Android.OS.Environment.ExternalStorageDirectory.Path + "/GeoPartner/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Toast.MakeText(this, "diretoria criada: " + path, ToastLength.Long).Show();
            }
            StreamWriter sw = new StreamWriter(new FileStream(path + "dados.gp", FileMode.Create));
            sw.Write(gp.writeXML());
            sw.Close();

            Toast.MakeText(this, "Ficheiro criado em " + path + "dados.gp", ToastLength.Long).Show();
            Finish();

        }

        private void updateTextoAtividadeAtual()
        {
            if(this.gp.atividadeAtual >= this.gp.atividades.Count)
            {
                this.textView1.Text = "Percurso terminado";
            }
            else
            {
                this.textView1.Text = String.Format("Atividade {0} de {1}", gp.atividadeAtual + 1, gp.atividades.Count);
            }
        }

        public override void OnBackPressed()
        {
        }


        //public void OnMapReady(GoogleMap googleMap)
        //{
        //    mMap = googleMap;
        //}

        //private void SetUpMap()
        //{
        //    if (mMap == null)
        //    {
        //        FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
        //    }
        //}
    }
}