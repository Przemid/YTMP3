using NReco.VideoConverter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;

namespace YTMP3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public bool WalidujLink()
        {
            string link = textBox1.Text;
            if (link == null)
            {
                MessageBox.Show("Wpisz coś tam.");
                return false;
            }
            if (link.Length < 10)
            {
                MessageBox.Show("Coś za mało.");
                return false;
            }
            if (!link.Contains("youtube") && !link.Contains("yt"))
            {
                MessageBox.Show("To nie jest link do youtube.");
                return false;
            }
            return true;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (WalidujLink())
            {
                PobierzAsync();
            }
        }

        public async Task PobierzAsync()
        {
            try
            {
                var client = new YoutubeClient();
                var url = textBox1.Text;
                var id = YoutubeClient.ParsePlaylistId(url);

                var playlist = await client.GetPlaylistAsync(id);

                foreach (var vid in playlist.Videos)
                {

                    var streamInfoSet = await client.GetVideoMediaStreamInfosAsync(vid.Id);
                    var streamInfo = streamInfoSet.Audio.WithHighestBitrate();
                    var ext = streamInfo.Container.GetFileExtension();
                    var video = await client.GetVideoAsync(vid.Id);
                    string sourcePath = $"C:/YTMP3/{video.Title}.{ext}";
                    string outputPath = $"C:/YTMP3/{video.Title}.mp3";
                    await client.DownloadMediaStreamAsync(streamInfo, sourcePath);

                    var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                    ffMpeg.ConvertMedia(sourcePath, outputPath, Format.mp4);

                    File.Delete(sourcePath);
                }
                MessageBox.Show("Pobrałem.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Coś poszło nie tak." + Environment.NewLine + ex.Message);
            }
        }
    }
}
