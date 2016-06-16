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
using Android.Graphics;
using Android.Provider;


namespace GeoPartner
{
    [Activity(Label = "Registo")]
    public class registoActivity : Activity
    {
        private atividade atv;
        private ImageView imageView1;
        private TextView textRegistoDe;
        private Button buttonTipoRegisto;
        private TextView textView1; //Designacao
        private TextView textView2; //Tipo/risca
        private TextView textView3; //Peso/peso
        private TextView textView4; //textura/cor
        private TextView textView5; //cor
        private EditText editText1;
        private EditText editText2;
        private EditText editText3;
        private EditText editText4;
        private EditText editText5;
        private int rochaOuMineral; //0-Rocha;1-Mineral

        private Bitmap foto;
        private string _imageUri;
   
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.layout_registo);

            this.textRegistoDe = FindViewById<TextView>(Resource.Id.textRegistoDe);
            if (textRegistoDe.Text.Equals("Registo de Mineral"))
                this.rochaOuMineral = 1;
            else this.rochaOuMineral = 0;
            this.buttonTipoRegisto = FindViewById<Button>(Resource.Id.buttonTipoRegisto);
            this.buttonTipoRegisto.Click += ButtonTipoRegisto_Click;
            this.textView1 = FindViewById<TextView>(Resource.Id.textView1);
            this.textView2 = FindViewById<TextView>(Resource.Id.textView2);
            this.textView3 = FindViewById<TextView>(Resource.Id.textView3);
            this.textView4 = FindViewById<TextView>(Resource.Id.textView4);
            this.textView5 = FindViewById<TextView>(Resource.Id.textView5);
            this.editText1 = FindViewById<EditText>(Resource.Id.editText1);
            this.editText2 = FindViewById<EditText>(Resource.Id.editText2);
            this.editText3 = FindViewById<EditText>(Resource.Id.editText3);
            this.editText4 = FindViewById<EditText>(Resource.Id.editText4);
            this.editText5 = FindViewById<EditText>(Resource.Id.editText5);


            this.imageView1 = FindViewById<ImageView>(Resource.Id.imageView1);
            this.imageView1.Click += TakeAPicture;

            atv = JsonConvert.DeserializeObject<atividade>(Intent.GetStringExtra("Atividade"));

            Button infoButton = FindViewById<Button>(Resource.Id.infoButton);
            infoButton.Click += delegate
            {
                var activity2 = new Intent(this, typeof(infoActivity));
                activity2.PutExtra("Atividade", JsonConvert.SerializeObject(atv));
                StartActivity(activity2);
            };
        }

        private void ButtonTipoRegisto_Click(object sender, EventArgs e)
        {
            this.editText1.Text = string.Empty;
            this.editText2.Text = string.Empty;
            this.editText3.Text = string.Empty;
            this.editText4.Text = string.Empty;
            this.editText5.Text = string.Empty;
            if(this.rochaOuMineral == 1) //mineral
            {
                this.textRegistoDe.Text = "Registo de Rocha";
                this.buttonTipoRegisto.Text = "Registar Mineral";
                this.textView1.Text = "Designacao";
                this.textView2.Text = "Tipo";
                this.textView3.Text = "Peso";
                this.textView4.Text = "Textura";
                this.textView5.Text = "Cor";
                this.textView5.Visibility = ViewStates.Visible;
                this.editText5.Visibility = ViewStates.Visible;
                this.rochaOuMineral = 0;
            }
            else if(this.rochaOuMineral == 0) //rocha
            {
                this.textRegistoDe.Text = "Registo de Mineral";
                this.buttonTipoRegisto.Text = "Registar Rocha";
                this.textView1.Text = "Designacao";
                this.textView2.Text = "Risca";
                this.textView3.Text = "Peso";
                this.textView4.Text = "Cor";
                this.textView5.Visibility = ViewStates.Invisible;
                this.editText5.Visibility = ViewStates.Invisible;
                this.rochaOuMineral = 1;
            }
        }

        private Boolean isMounted
        {
            get
            {
                return Android.OS.Environment.ExternalStorageState.Equals(Android.OS.Environment.MediaMounted);
            }
        }

        private void TakeAPicture(object sender, EventArgs e)
        {
            var uri = ContentResolver.Insert(isMounted
                                                    ? MediaStore.Images.Media.ExternalContentUri
                                                    : MediaStore.Images.Media.InternalContentUri, new ContentValues());
            _imageUri = uri.ToString();

            Intent intent = new Intent(MediaStore.ActionImageCapture);
            intent.PutExtra(MediaStore.ExtraOutput, uri);
            StartActivityForResult(intent, 1001);
        }


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if(resultCode == Result.Ok && requestCode == 1001)
            {
                Android.Net.Uri _currentImageUri = Android.Net.Uri.Parse(_imageUri);
                this.foto = BitmapFactory.DecodeStream(ContentResolver.OpenInputStream(_currentImageUri));

                this.imageView1.SetImageBitmap(this.foto);
            }
        }
    }
}