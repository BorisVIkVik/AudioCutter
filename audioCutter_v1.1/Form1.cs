using Magnum.FileSystem;
using MediaToolkit;
using NAudio.Wave;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoLibrary;
using File = System.IO.File;
using NReco.Converting;

namespace audioCutter_v1._1
{
    public partial class Form1 : Form
    {
        string path = "D:\\";
        string path1 = "D:\\";
        audioCutter.WavFileUtils WavCutter = new audioCutter.WavFileUtils();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string textFromFile = null;
            using (FileStream fstream = File.OpenRead(path))
            {
                // преобразуем строку в байты
                byte[] array = new byte[fstream.Length];
                // считываем данные
                fstream.Read(array, 0, array.Length);
                // декодируем байты в строку
                textFromFile = System.Text.Encoding.Default.GetString(array);
                //Console.WriteLine($"Текст из файла: {textFromFile}");
            }
            if (textFromFile != null)
            {
                int len = 0;
                for(int i = 0; i < textFromFile.Length; i++)
                {
                    if (textFromFile[i] == ' ')
                    {
                        string fileName = textFromFile.Substring(0, len) + ".wav";
                        string filePath = Path.Combine("D:\\project\\youtube\\converted\\", fileName);
                        string outPath = @"D:\project\Audio_Cutted\" + fileName;

                        int len1 = 0;
                        int j = i + 1;
                        while(textFromFile[j] != ' ')
                        {
                            len1 *= 10;
                            len1 += (int)((int)textFromFile[j] - '0');
                            j++;
                        }
                        j++;
                        int len2 = 0;
                        while (j < textFromFile.Length && textFromFile[j] != '\r' && textFromFile[j] != ' ')
                        {
                            len2 *= 10;
                            len2 += (int)((int)textFromFile[j] - '0');
                            j++;
                        }
                        TimeSpan start = new TimeSpan(0,0, len1);
                        TimeSpan end = new TimeSpan(0,0, len2);
                        TrimWavFile(filePath, outPath, start, end);
                        len = 0;
                    }
                    len++;
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            path = openFileDialog1.FileName;
        }

        public void TrimWavFile(string inPath, string outPath, TimeSpan cutFromStart, TimeSpan cutFromEnd)
        {
            using (WaveFileReader reader = new WaveFileReader(inPath))
            {
                using (WaveFileWriter writer = new WaveFileWriter(outPath, reader.WaveFormat))
                {
                    int bytesPerMillisecond = reader.WaveFormat.AverageBytesPerSecond / 1000;

                    int startPos = (int)cutFromStart.TotalMilliseconds * bytesPerMillisecond;
                    startPos = startPos - startPos % reader.WaveFormat.BlockAlign;
                    int time = (int)(reader.TotalTime.TotalMilliseconds - (cutFromStart.TotalMilliseconds + cutFromEnd.TotalMilliseconds));
                    if(time <= 0)
                    {
                        time = (int)reader.TotalTime.TotalMilliseconds;
                    }
                    int endBytes = time * bytesPerMillisecond;
                    endBytes = endBytes - endBytes % reader.WaveFormat.BlockAlign;
                    int endPos = (int)reader.Length - endBytes;

                    TrimWavFile(reader, writer, startPos, endPos);
                }
            }
        }

        private void TrimWavFile(WaveFileReader reader, WaveFileWriter writer, int startPos, int endPos)
        {
            reader.Position = startPos;
            byte[] buffer = new byte[1024];
            while (reader.Position < endPos)
            {
                int bytesRequired = (int)(endPos - reader.Position);
                if (bytesRequired > 0)
                {
                    int bytesToRead = Math.Min(bytesRequired, buffer.Length);
                    int bytesRead = reader.Read(buffer, 0, bytesToRead);
                    if (bytesRead > 0)
                    {
                        writer.WriteData(buffer, 0, bytesRead);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string textFromFile1 = null;
            using (FileStream fstream = File.OpenRead(path))
            {
                // преобразуем строку в байты
                byte[] array = new byte[fstream.Length];
                // считываем данные
                fstream.Read(array, 0, array.Length);
                // декодируем байты в строку
                textFromFile1 = System.Text.Encoding.Default.GetString(array);
                //Console.WriteLine($"Текст из файла: {textFromFile}");
            }


            string textFromFile = null;
            using (FileStream fstream = File.OpenRead(path1))
            {
                // преобразуем строку в байты
                byte[] array = new byte[fstream.Length];
                // считываем данные
                fstream.Read(array, 0, array.Length);
                // декодируем байты в строку
                textFromFile = System.Text.Encoding.Default.GetString(array);
                //Console.WriteLine($"Текст из файла: {textFromFile}");
            }
            if (textFromFile != null && textFromFile1 != null)
            {
                int len = 0;
                int save = 0;
                int index = 0;
                for (int i = 0; i < textFromFile.Length; i++)
                {
                    if(textFromFile[i] == 'h' && len == 0)
                    {
                        len = 1;
                        save = i;
                    }
                    if (textFromFile[i] == '?')
                    {
                        string fileName = textFromFile.Substring(save, len - 1);
                        int len1 = 0;
                        int j = i - 1;
                        while (textFromFile[j] != '/')
                        {
                            len1++;
                            j--;
                        }
                        string WAVName = textFromFile.Substring(j + 1, len1);
                        bool check = false;
                        string outPath = @"D:\project\Audio_Cutted\" + WAVName + ".wav";
                        string filePath = Path.Combine("D:\\project\\youtube\\converted\\", WAVName + ".wav");
                        if (!System.IO.File.Exists(outPath))
                        {
                            SaveMP3(fileName, WAVName, ref check);
                        }
                        


                        len = 0;
                        while (textFromFile1[index] != ' ')
                        {
                            index++;
                        }
                        int length = 0;
                        index++;
                        while (textFromFile1[index] != ' ')
                        {
                            length *= 10;
                            length += (int)((int)textFromFile1[index] - '0');
                            index++;
                        }
                        index++;
                        int len2 = 0;
                        while (index < textFromFile1.Length && textFromFile1[index] != '\r')
                        {
                            len2 *= 10;
                            len2 += (int)((int)textFromFile1[index] - '0');
                            index++;
                        }
                        index++;
                        index++;
                        TimeSpan start = new TimeSpan(0, 0, length);
                        TimeSpan end = new TimeSpan(0, 0, len2);
                        if (!check && !System.IO.File.Exists(outPath))
                        {
                            TrimWavFile(filePath, outPath, start, end);
                            System.IO.File.Delete(filePath);
                        }
                    }
                    if (len != 0)
                    {
                        len++;
                    }
                }
            }
            //var source = @"D:\project\youtube";
            //var youtube = YouTube.Default;
            //var vid = youtube.GetVideo("<video url>");
            //File.WriteAllBytes(source + vid.FullName, vid.GetBytes());

            //var inputFile = new MediaFile {  source + vid.FullName,  };
            //var outputFile = new MediaFile { $"{source + vid.FullName}.mp3",  };

            //using (var engine = new Engine())
            //{
            //    engine.GetMetadata(inputFile);

            //    engine.Convert(inputFile, outputFile);
            //}
        }

        private void SaveMP3(string VideoURL, string WAVName, ref bool check)
        {
            string sourceIn = @"D:\project\youtube\";
            string sourceOut = @"D:\project\youtube\converted\";
            var youtube = YouTube.Default;
            try
            {
                var vid = youtube.GetVideo(VideoURL);
                File.WriteAllBytes(sourceIn + WAVName + ".mp4", vid.GetBytes());
                var ConvertVideo = new NReco.VideoConverter.FFMpegConverter();
                ConvertVideo.ConvertMedia(sourceIn + WAVName + ".mp4", sourceOut + WAVName + ".wav", "wav");
                System.IO.File.Delete(sourceIn + WAVName + ".mp4");
            }
            catch(Exception e)
            {
                check = true;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            path1 = openFileDialog1.FileName;
        }
    }
}
