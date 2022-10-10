using DSiSend.Core;
using Microsoft.Win32;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using NetFwTypeLib;
using System.Collections.Generic;
using System.Linq;

namespace DSiSend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SendWindow : Window
    {
        static HttpListener _httpListener = new HttpListener();
        string _localIpAddress = "127.0.0.1";
        bool _isConnectionThreadActive = false;
        string _currentFilePath = "";
        public SendWindow()
        {
            InitializeComponent();

            AddFirewallException();

            qrCodePic.Source = QrGen.GetImageForLink("https://github.com/Epicpkmn11/dsidl/releases/download/v0.1.0/dsidl.dsi");
            filenameText.Text = "Scan QR to download dsidl.dsi";
        }

        private void opnFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                if (_httpListener is not null)
                {
                    _httpListener.Close();
                }
                _currentFilePath = openFileDialog.FileName;



                _localIpAddress = Util.GetLanAddress();

                _httpListener = new HttpListener();
                _httpListener.Prefixes.Clear();
                _httpListener.Prefixes.Add($"http://{_localIpAddress}:5000/");
                _httpListener.Start();

                this.Title = $"DSiSend @ {_localIpAddress}";
                qrCodePic.Source = QrGen.GetImageForLink($"http://{_localIpAddress}:5000/{Util.SanitizeFilename(Path.GetFileName(_currentFilePath))}");
                filenameText.Text = Path.GetFileName(_currentFilePath);

                closeConnBtn.Visibility = Visibility.Visible;
                _isConnectionThreadActive = true;

                Thread _responseThread = new Thread(ResponseThread);
                _responseThread.SetApartmentState(ApartmentState.STA);
                _responseThread.Start();
            }
        }

        private void ResponseThread(object? obj)
        {
            while (_isConnectionThreadActive)
            {
                try
                {
                    HttpListenerContext context = _httpListener.GetContext();

                    if (File.Exists(_currentFilePath))
                    {
                        try
                        {
                            Stream input = new FileStream(_currentFilePath, FileMode.Open);

                            context.Response.ContentType = "application/octet-stream";
                            context.Response.ContentLength64 = input.Length;
                            context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                            context.Response.AddHeader("Last-Modified", File.GetLastWriteTime(_currentFilePath).ToString("r"));
                            context.Response.AddHeader("Content-Disposition", $"attachment; filename={Path.GetFileName(_currentFilePath)}");

                            byte[] buffer = new byte[1024 * 32];
                            int nbytes;

                            while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                                context.Response.OutputStream.Write(buffer, 0, nbytes);

                            input.Close();
                            context.Response.OutputStream.Flush();

                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                        }
                        catch (Exception ex) 
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        }
                    }
                    else context.Response.StatusCode = (int)HttpStatusCode.NotFound;

                    context.Response.Close(); 
                }
                catch (Exception)
                {                
                    //Essential to fix one bug
                }
            }
        }

        private void closeConnBtn_Click(object sender, RoutedEventArgs e)
        {
            _isConnectionThreadActive = false;

            qrCodePic.Source = QrGen.GetImageForLink("https://github.com/Epicpkmn11/dsidl/releases/download/v0.1.0/dsidl.dsi");
            filenameText.Text = "Scan QR to download dsidl.dsi";
            closeConnBtn.Visibility = Visibility.Collapsed;

            _httpListener.Stop();
            _httpListener.Prefixes.Clear();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _isConnectionThreadActive=false;
            _httpListener.Close();
        }
        private void AddFirewallException() 
        {


            Type tNetFwPolicy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            INetFwPolicy2 fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(tNetFwPolicy2);
            var currentProfiles = fwPolicy2.CurrentProfileTypes;

            try
            {
                var foo = fwPolicy2.Rules.Item("DSiSend");
            }
            catch (Exception)
            {
                INetFwRule2 inboundRule = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                inboundRule.Enabled = true;

                inboundRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;

                inboundRule.Protocol = 6; // tcp id
                inboundRule.LocalPorts = "5000";
                inboundRule.Name = "DSiSend";
                inboundRule.Description = "Rule to enable DSiSend port 5000 connections from outside";
                inboundRule.Profiles = currentProfiles;

                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                firewallPolicy.Rules.Add(inboundRule);
            }
        }

        private void githubProjLinkBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://github.com/awkitsune/dsisend");
        }

        private void githubDownAppBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Epicpkmn11/dsidl");
        }
    }
}
