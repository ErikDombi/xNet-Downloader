//Copyright 2017, Erik Dombi, All rights reserved.

/* Just a friendly reminder guys, I didn't open source this so you could bash me for writing crappy code, or creating an application that already exsists.
 * It's here so you can suggest better practices to me, or even fix something that is broken.
 *
 * 
 * The purpose of this application is to create an application that can automatically download a song on youtube, soundcloud, ect and copy it to a connected apple device's music library.
 * The application IS NOT done yet, and still has a long way to go. I'm still fairly new to coding, and have a lot of room to improve so any help would be amazing!
 */


#region namespaces

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

#endregion

namespace xNet_Downloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        //Specifies working directory for which application works in.
        string parentDir = $@"C:\Users\{Environment.UserName}\Documents\xNetDownloader\RequiredFiles\";

        private void Form1_Load(object sender, EventArgs e)
        {
            //Initiating a new instance of splash and showing it
            SPLASH SPLASH = new SPLASH();
            SPLASH.Show();

            //Creates C:\Users\<user>\Documenets\xNetDownloader\RequiredFiles\ if it hasn't been created yet
            if (!Directory.Exists(parentDir)) Directory.CreateDirectory(parentDir);

            //If youtube-dl.exe has not yet been copied to the RequiredFiles folder, then copy it.
            if (!File.Exists($"{parentDir}youtube-dl.exe"))

                //Creates a new stream, using youtube-dl.exe as the embeded resource
                using (Stream input = Assembly.GetEntryAssembly().GetManifestResourceStream("xNet_Downloader.EmbeddedResources.youtube-dl.exe"))
                {
                    Console.WriteLine("youtube-dl.exe missing on device! copying...");
                    try
                    {
                        //Gets bytes of youtube-dl.exe
                        byte[] byteData = StreamToBytes(input);

                        //Creates a new exe named youtube-dl.exe
                        File.Create($@"{parentDir}youtube-dl.exe").Close();

                        //Writes bytes of newly created exe
                        System.IO.File.WriteAllBytes($@"{parentDir}youtube-dl.exe", byteData);

                        Console.WriteLine("Finished copying youtube-dl.exe");
                    }
                    //If copying fails, exit the application with a warning. Application exits because application depends on youtube-dl
                    catch { Console.WriteLine("Failed to copy youtube-dl.exe!"); MessageBox.Show("Failed when trying to initiate youtube-dl.exe.\nFatal error will now cause application to exit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); Environment.Exit(0); }
                }

            //If ffmpeg.exe has not yet been copied to the RequiredFiles folder, then copy it.
            if (!File.Exists($"{parentDir}ffmpeg.exe"))

                //Creates a new stream, using ffmpeg.exe as the embeded resource
                using (Stream input = Assembly.GetEntryAssembly().GetManifestResourceStream("xNet_Downloader.EmbeddedResources.ffmpeg.exe"))
                {
                    Console.WriteLine("ffmpeg.exe missing on device! copying...");
                    try
                    {
                        //Gets the bytes of ffmpeg.exe
                        byte[] byteData = StreamToBytes(input);

                        //Creates a new exe named ffmpeg.exe
                        File.Create($@"{parentDir}ffmpeg.exe").Close();

                        //writes bytes of newly created exe
                        System.IO.File.WriteAllBytes($@"{parentDir}ffmpeg.exe", byteData);

                        Console.WriteLine("Finished copying ffmpeg.exe!");
                    }
                    //If copying fails, warn the user. Application will not exit because althought youtube-dl is a bit reliant on ffmpeg, it does not fully depend on it
                    catch { Console.WriteLine("Failed to copy ffmpeg.exe!"); MessageBox.Show("Failed when trying to initiate ffmpeg.exe.\nError is typically not fatal... Application will attempt to continue.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }

            //A short delay so that the splash screen doesn't just quickly flash on screen.
            System.Threading.Thread.Sleep(4000);
            //Close the splash screen and dispose of it.
            SPLASH.Close(); SPLASH.Dispose();
        }


        //The following code (The summary and method) was not written by my, but by https://stackoverflow.com/users/3197457/baaleos. All credit goes to him! 
        /// <summary>
        /// StreamToBytes - Converts a Stream to a byte array. Eg: Get a Stream from a file,url, or open file handle.
        /// </summary>
        /// <param name="input">input is the stream we are to return as a byte array</param>
        /// <returns>byte[] The Array of bytes that represents the contents of the stream</returns>
        static byte[] StreamToBytes(Stream input)
        {

            int capacity = input.CanSeek ? (int)input.Length : 0; //Bitwise operator - If can seek, Capacity becomes Length, else becomes 0.
            using (MemoryStream output = new MemoryStream(capacity)) //Using the MemoryStream output, with the given capacity.
            {
                int readLength;
                byte[] buffer = new byte[capacity/*4096*/];  //An array of bytes
                do
                {
                    readLength = input.Read(buffer, 0, buffer.Length);   //Read the memory data, into the buffer
                    output.Write(buffer, 0, readLength); //Write the buffer to the output MemoryStream incrementally.
                }
                while (readLength != 0); //Do all this while the readLength is not 0
                return output.ToArray();  //When finished, return the finished MemoryStream object as an array.
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Deletes previously downloaded track (if there is any)
            if (File.Exists("download.mp3")){ File.Delete("download.mp3");Console.WriteLine("Found exsiting file.... Deleting..."); }

            //Specifies process that we will use to download the song
            ProcessStartInfo proc = new ProcessStartInfo($@"{parentDir}youtube-dl.exe");

            proc.CreateNoWindow = true; //true so the user does not see youtube-dl.exe pop up on their screen
            proc.RedirectStandardInput = true; //true so we can issue commands after starting the process
            proc.RedirectStandardOutput = true; //true so we can read the output of the application
            proc.UseShellExecute = false; //false to prevent using the operating system to start the command

            //Arguments we will use for youtube-dl.exe
            proc.Arguments = $"{textBox1.Text} --embed-thumbnail --add-metadata --audio-format mp3 --no-playlist --output \"download.mp3\"";

            //Starting youtube-dl.exe (proc)
            var newProc = Process.Start(proc);

            //Waits for youtube-dl to exit before continuing
            newProc.WaitForExit();
            
            //Tells the user the download is done!
            MessageBox.Show("Done!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}