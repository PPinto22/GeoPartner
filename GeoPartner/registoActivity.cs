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
using System.Threading;

namespace GeoPartner
{
    [Activity(Label = "Registo")]
    public class registoActivity : Activity
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
        private File _dir;
        private File _file;
        private File _fileAnterior;
        private string path_foto;

        // Audio
        private AudioRecord _recorder;
        private MediaPlayer _player;
        private byte[] audioBuffer;
        private string path_voz;

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
                this._recorder.Release();
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

            new Thread(delegate ()
            {
                RecordAudio();
            }).Start();
        }

        public void RecordAudio()
        {
            try
            {
                File fich_voz = new File(path_voz);
                if (fich_voz.Exists())
                {
                    fich_voz.Delete();
                }
            }
            catch
            {

            }
            try { 
                this.path_voz = Android.OS.Environment.ExternalStorageDirectory.Path + String.Format("/GeoPartner/voz{0}.wav", Guid.NewGuid());
                File fich_voz = new File(path_voz);
                System.IO.Stream outputStream = System.IO.File.Open(path_voz, System.IO.FileMode.Create);
                System.IO.BinaryWriter bWriter = new System.IO.BinaryWriter(outputStream);

                audioBuffer = new byte[8000];

                this._recorder = new AudioRecord(
                    // Hardware source of recording.
                    AudioSource.Mic,
                    // Frequency
                    44100,
                    // Mono or stereo
                    ChannelIn.Mono,
                    // Audio encoding
                    Android.Media.Encoding.Pcm16bit,
                    // Length of the audio clip.
                    audioBuffer.Length
                );

                long totalAudioLen = 0;
                long totalDataLen = totalAudioLen + 36;
                long longSampleRate = 44100;
                int channels = 1;
                long byteRate = 16 * longSampleRate * channels  / 8;

                totalAudioLen = audioBuffer.Length;
                totalDataLen = totalAudioLen + 36;

                WriteWaveFileHeader(
                    bWriter,
                    totalAudioLen,
                    totalDataLen,
                    longSampleRate,
                    channels,
                    byteRate);

                this._recorder.StartRecording();

                while (this.recording == true)
                {
                    try
                    {
                        /// Keep reading the buffer while there is audio input.
                        int lidos = this._recorder.Read(audioBuffer, 0, audioBuffer.Length);
                        totalAudioLen += lidos;
                        totalDataLen += lidos;

                        bWriter.Write(audioBuffer);
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.Out.WriteLine(ex.Message);
                        break;
                    }
                }

                outputStream.Close();
                bWriter.Close();

                outputStream = System.IO.File.Open(path_voz, System.IO.FileMode.Open);
                bWriter = new System.IO.BinaryWriter(outputStream);

                WriteWaveFileHeader(
                    bWriter,
                    totalAudioLen,
                    totalDataLen,
                    longSampleRate,
                    channels,
                    byteRate);

                outputStream.Close();
                bWriter.Close();
            }
            catch (Exception ex)
            {
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

        private void WriteWaveFileHeader(
        System.IO.BinaryWriter bWriter, long totalAudioLen,
        long totalDataLen, long longSampleRate, int channels,
        long byteRate)
        {

            byte[] header = new byte[44];

            header[0] = (byte)'R'; // RIFF/WAVE header
            header[1] = (byte)'I';
            header[2] = (byte)'F';
            header[3] = (byte)'F';
            header[4] = (byte)(totalDataLen & 0xff);
            header[5] = (byte)((totalDataLen >> 8) & 0xff);
            header[6] = (byte)((totalDataLen >> 16) & 0xff);
            header[7] = (byte)((totalDataLen >> 24) & 0xff);
            header[8] = (byte)'W';
            header[9] = (byte)'A';
            header[10] = (byte)'V';
            header[11] = (byte)'E';
            header[12] = (byte)'f'; // 'fmt ' chunk
            header[13] = (byte)'m';
            header[14] = (byte)'t';
            header[15] = (byte)' ';
            header[16] = 16; // 4 bytes: size of 'fmt ' chunk
            header[17] = 0;
            header[18] = 0;
            header[19] = 0;
            header[20] = 1; // format = 1
            header[21] = 0;
            header[22] = (byte)channels;
            header[23] = 0;
            header[24] = (byte)(longSampleRate & 0xff);
            header[25] = (byte)((longSampleRate >> 8) & 0xff);
            header[26] = (byte)((longSampleRate >> 16) & 0xff);
            header[27] = (byte)((longSampleRate >> 24) & 0xff);
            header[28] = (byte)(byteRate & 0xff);
            header[29] = (byte)((byteRate >> 8) & 0xff);
            header[30] = (byte)((byteRate >> 16) & 0xff);
            header[31] = (byte)((byteRate >> 24) & 0xff);
            header[32] = (byte)(2 * 16 / 8); // block align
            header[33] = 0;
            header[34] = 16; // bits per sample
            header[35] = 0;
            header[36] = (byte)'d';
            header[37] = (byte)'a';
            header[38] = (byte)'t';
            header[39] = (byte)'a';
            header[40] = (byte)(totalAudioLen & 0xff);
            header[41] = (byte)((totalAudioLen >> 8) & 0xff);
            header[42] = (byte)((totalAudioLen >> 16) & 0xff);
            header[43] = (byte)((totalAudioLen >> 24) & 0xff);

            bWriter.Write(header, 0, 44);
        }

        private bool isEmpty()
        {
            if(this.rochaOuMineral == 0) // rocha
            {
                return String.IsNullOrEmpty(this.path_foto) &&
                this.editText1.Text == string.Empty &&
                this.editText2.Text == string.Empty &&
                this.editText3.Text == string.Empty &&
                this.editText4.Text == string.Empty &&
                this.editText5.Text == string.Empty &&
                !this.existeGravacao;
            }
            else // mineral
            {
                return String.IsNullOrEmpty(this.path_foto) &&
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
                return String.IsNullOrEmpty(this.path_foto) &&
                this.editText1.Text != string.Empty &&
                this.editText2.Text != string.Empty &&
                this.editText3.Text != string.Empty &&
                this.editText4.Text != string.Empty &&
                this.editText5.Text != string.Empty &&
                this.existeGravacao;
            }
            else // mineral
            {
                return !String.IsNullOrEmpty(this.path_foto) &&
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

                    reg = new registo(r, this.path_foto, this.path_voz);
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

                    reg = new registo(m, this.path_foto, this.path_voz);
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
            this._dir = new File(Android.OS.Environment.ExternalStorageDirectory.Path + "/GeoPartner/");
            if (!this._dir.Exists())
            {
                this._dir.Mkdirs();
            }
        }

        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            CreateDirectoryForPictures();
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            this._fileAnterior = this._file;
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
                    if(this._fileAnterior != null && this._fileAnterior.Exists())
                    {
                        this._fileAnterior.Delete();
                    }
                    Android.Net.Uri imageUri = Android.Net.Uri.FromFile(this._file);

                    int height = Resources.DisplayMetrics.HeightPixels;
                    int width = this.imageView1.Height;
                    Bitmap largeBitmap = MediaStore.Images.Media.GetBitmap(this.ContentResolver, imageUri);
                    Bitmap redim = Bitmap.CreateScaledBitmap(largeBitmap, 520, 550, true);
                    if (redim != null)
                    {
                        this.imageView1.SetImageBitmap(redim);
                    }

                    // Dispose of the Java side bitmap.
                    GC.Collect();
                    File ficheiroFoto = new File(imageUri.Path);
                    ficheiroFoto.Delete();

                    this.path_foto = Android.OS.Environment.ExternalStorageDirectory.Path + String.Format("/GeoPartner/foto_{0}.jpg", Guid.NewGuid());
                    System.IO.Stream os = System.IO.File.Open(path_foto, System.IO.FileMode.Create);
                    System.IO.BinaryWriter bw = new System.IO.BinaryWriter(os);
                    bw.Write(registo.imageToByteArray(Bitmap.CreateScaledBitmap(largeBitmap,1200,1200,true)));
                    bw.Flush();
                    bw.Close();
                    os.Close();
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