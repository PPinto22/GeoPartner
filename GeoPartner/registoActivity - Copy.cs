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
using Android.Net;
using GeoPartner.Business;
using Newtonsoft.Json;
using Android.Graphics;
using Android.Provider;
using Java.IO;
using Android.Support.V4.Content;
using Android;
using BackOffice.Business;
using Android.Media;

namespace GeoPartner
{
    [Activity(Label = "Registo")]
    public class registoActivity2 : Activity
    {
        private atividade atv;
        private ImageView imageView1;
        private TextView textRegistoDe;
        private Button buttonTipoRegisto;
        private Button buttonGuardar;
        private Button buttonInfo;
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
        private TextView textVoz;
        private Button buttonVoz;
        private bool recording;
        private bool existeGravacao;

        // Foto
        private Bitmap foto;
        private File _dir;
        private File _file;

        // Audio
        MediaRecorder _recorder;
        MediaPlayer _player;
        private string path_voz = Android.OS.Environment.ExternalStorageDirectory.Path + "/GeoPartner/voz.wav";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            atv = JsonConvert.DeserializeObject<atividade>(Intent.GetStringExtra("Atividade"));

            SetContentView(Resource.Layout.layout_registo);

            this.textVoz = FindViewById<TextView>(Resource.Id.textVoz);
            this.textVoz.Text = string.Empty;
            this.buttonVoz = FindViewById<Button>(Resource.Id.buttonVoz);
            this.buttonVoz.Click += ButtonVoz_Click;
            this.recording = false;
            this.existeGravacao = false;
            this.buttonGuardar = FindViewById<Button>(Resource.Id.buttonGuardar);
            this.buttonGuardar.Click += ButtonGuardar_Click;
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


            this.buttonInfo = FindViewById<Button>(Resource.Id.infoButton);
            this.buttonInfo.Click += delegate
            {
                var activity2 = new Intent(this, typeof(infoActivity));
                activity2.PutExtra("Atividade", JsonConvert.SerializeObject(atv));
                StartActivity(activity2);
            };
        }

        private void ButtonVoz_Click(object sender, EventArgs e)
        {
            if (this.recording)
            {
                this.textVoz.Text = "Registo de voz gravado";
                this.buttonVoz.Background.SetTint(Color.ParseColor("#6D6968"));
                this.buttonVoz.Text = "Nova gravação";
                this.buttonInfo.Enabled = true;
                this.buttonGuardar.Enabled = true;
                this.buttonTipoRegisto.Enabled = true;

                this.recording = false;
                this.existeGravacao = true;

                this._recorder.Stop();
            }
            else
            {
                if (this.existeGravacao){
                    new AlertDialog.Builder(this)
                    .SetPositiveButton("Sim", (sender2, args) =>
                    {
                        this.comecarGravacao();
                    })
                    .SetNegativeButton("Não", (sender2, args) =>
                    {

                    })
                    .SetMessage("Já existe uma gravação de voz registada. Gravar uma nova?")
                    .SetTitle("Aviso")
                    .Show();
                }
                else
                {
                    this.comecarGravacao();
                }
            }
        }

        private void comecarGravacao()
        {
            this.recording = true;

            this.buttonVoz.Background.SetTint(Color.Red);
            this.buttonVoz.Text = "Terminar gravação";
            this.textVoz.Text = "A gravar voz...";
            this.buttonInfo.Enabled = false;
            this.buttonGuardar.Enabled = false;
            this.buttonTipoRegisto.Enabled = false;

            try
            {
                File fich_voz = new File(path_voz);
                if (fich_voz.Exists())
                {
                    fich_voz.Delete();
                }
                if(this._recorder == null)
                {
                    this._recorder = new MediaRecorder();
                }
                this._recorder.Reset();
                this._recorder.SetAudioSource(AudioSource.Mic);
                this._recorder.SetOutputFormat(OutputFormat.Default);
                this._recorder.SetAudioEncoder(AudioEncoder.Default);
                this._recorder.SetOutputFile(path_voz);
                this._recorder.Prepare();
                this._recorder.Start();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Impossível gravar voz", ToastLength.Short).Show();
                this.buttonVoz.Background.SetTint(Color.ParseColor("#727778"));
                this.buttonVoz.Text = "Nova gravação";
                this.textVoz.Text = string.Empty;
                this.buttonInfo.Enabled = true;
                this.buttonGuardar.Enabled = true;
                this.buttonTipoRegisto.Enabled = true;

                this.recording = false;
                this.existeGravacao = false;
            }
        }

        private bool isEmpty()
        {
            if(this.rochaOuMineral == 0) // rocha
            {
                return this.foto == null &&
                this.editText1.Text == string.Empty &&
                this.editText2.Text == string.Empty &&
                this.editText3.Text == string.Empty &&
                this.editText4.Text == string.Empty &&
                this.editText5.Text == string.Empty &&
                !this.existeGravacao;
            }
            else // mineral
            {
                return this.foto == null &&
                this.editText1.Text == string.Empty &&
                this.editText2.Text == string.Empty &&
                this.editText3.Text == string.Empty &&
                this.editText4.Text == string.Empty &&
                !this.existeGravacao;
            }
        }

        private bool isComplete()
        {
            if (this.rochaOuMineral == 0) // rocha
            {
                return this.foto != null &&
                this.editText1.Text != string.Empty &&
                this.editText2.Text != string.Empty &&
                this.editText3.Text != string.Empty &&
                this.editText4.Text != string.Empty &&
                this.editText5.Text != string.Empty &&
                this.existeGravacao;
            }
            else // mineral
            {
                return this.foto != null &&
                this.editText1.Text != string.Empty &&
                this.editText2.Text != string.Empty &&
                this.editText3.Text != string.Empty &&
                this.editText4.Text != string.Empty &&
                this.existeGravacao;
            }
        }

        private void ButtonGuardar_Click(object sender, EventArgs e)
        {
            if (this.isEmpty())
            {
                Toast.MakeText(this, "Registo Vazio!", ToastLength.Short).Show();
            }
            else
            {
                byte[] voz = null;
                try
                {
                    voz = System.IO.File.ReadAllBytes(path_voz);
                    System.IO.File.Delete(path_voz);
                }
                catch { }
                registo reg;
                if(this.rochaOuMineral == 0) // rocha
                {
                    string designacao = editText1.Text;
                    string tipo = editText2.Text;
                    float peso;
                    try
                    {
                        peso = float.Parse(editText3.Text);
                    }
                    catch
                    {
                        peso = 0.0f;
                    }
                    string textura = editText4.Text;
                    string cor = editText5.Text;
                    rocha r = new rocha(designacao,tipo,peso,textura,cor);

                    reg = new registo(r, this.foto, voz);
                }
                else // mineral
                {
                    string designacao = editText1.Text;
                    string risca = editText2.Text;
                    float peso;
                    try
                    {
                        peso = float.Parse(editText3.Text);
                    }
                    catch
                    {
                        peso = 0.0f;
                    }
                    string cor = editText4.Text;
                    mineral m = new mineral(designacao, risca, peso, cor);

                    reg = new registo(m, this.foto, voz);
                }

                Intent intent = new Intent(this, typeof(percursoActivity));
                intent.PutExtra("Registo", JsonConvert.SerializeObject(reg));
                SetResult(Result.Ok, intent);
                Finish();
            }
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
                this.textView1.Text = "Designação";
                this.textView2.Text = "Tipo";
                this.textView3.Text = "Peso (Kg)";
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
                this.textView1.Text = "Designação";
                this.textView2.Text = "Risca";
                this.textView3.Text = "Peso (Kg)";
                this.textView4.Text = "Cor";
                this.textView5.Visibility = ViewStates.Invisible;
                this.editText5.Visibility = ViewStates.Invisible;
                this.rochaOuMineral = 1;
            }
        }

        private void CreateDirectoryForPictures()
        {
            this._dir = new File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryPictures), "GeoPartnerFotos");
            if (!this._dir.Exists())
            {
                this._dir.Mkdirs();
            }
        }

        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            CreateDirectoryForPictures();
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            this._file = new File(this._dir, String.Format("foto_{0}.jpg", Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(this._file));

            StartActivityForResult(intent, 1000);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 1000)
            {
                if (resultCode == Result.Ok)
                {
                    Android.Net.Uri imageUri = Android.Net.Uri.FromFile(this._file);

                    int height = Resources.DisplayMetrics.HeightPixels;
                    int width = this.imageView1.Height;
                    Bitmap largeBitmap = MediaStore.Images.Media.GetBitmap(this.ContentResolver, imageUri);
                    Bitmap redim = Bitmap.CreateScaledBitmap(largeBitmap, 520, 550, true);
                    this.foto = redim;
                    if (this.foto != null)
                    {
                        this.imageView1.SetImageBitmap(redim);
                    }

                    // Dispose of the Java side bitmap.
                    GC.Collect();
                    File ficheiroFoto = new File(imageUri.Path);
                    ficheiroFoto.Delete();
                }
            }
        }

        public override void OnBackPressed()
        {
            if (this.recording)
            {
                this.buttonVoz.Background.SetTint(Color.ParseColor("#727778"));
            }
            base.OnBackPressed();
        }
    }
}