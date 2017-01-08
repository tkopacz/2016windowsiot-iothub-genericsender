using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IoTClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DeviceClient m_clt;
        DispatcherTimer m_t;
        ObservableCollection<string> m_col = new ObservableCollection<string>();
        public MainPage()
        {
            this.InitializeComponent();
            SetupAsync();
        }

        public async void SetupAsync()
        {
            m_t = new DispatcherTimer();
            m_t.Tick += Timer_TicksAsync;
            lstEvents.ItemsSource = m_col;
         }
        string[] deviceNames = new string[] { "PC0", "PC1", "PC2", "PC3", "PC4", "PC5", "PC6", "PC7", "PC8", "PC9" };
        int msg_send = 0;
        const int BATCH_SIZE = 20;
        Random rnd = new Random();
        private async void Timer_TicksAsync(object sender, object e)
        {
            if (m_clt == null) return;
            if (chkMsg1.IsChecked == true)
            {
                MMsg1 msg1 = new MMsg1()
                {
                    DeviceName = deviceNames[rnd.Next(deviceNames.Length)],
                    MsgType = "M1",
                    MyProperty1 = rnd.Next()
                };
                await SendMsgObjAsync(msg1);
            }
            if (chkMsg2.IsChecked == true)
            {
                MMsg2 msg2 = new MMsg2()
                {
                    DeviceName = deviceNames[rnd.Next(deviceNames.Length)],
                    MyProperty2 = "AB" + Char.ConvertFromUtf32(Char.ConvertToUtf32("A", 0) + rnd.Next(24))
                };
                await SendMsgObjAsync(msg2);
            }

            if (chkError1.IsChecked==true)
            {
                MError err = new MError()
                {
                    DeviceName = deviceNames[rnd.Next(deviceNames.Length)],
                };
                await SendMsgObjAsync(err);
            }
            if (chkError2.IsChecked == true)
            {
                await SendMsgAsync("ABC"); //Trivia
            }
            DateTime now = DateTime.Now;
            TimeSpan ts = new TimeSpan(now.Year, now.Minute, now.Second, now.Millisecond);
            if (chkMAll.IsChecked == true)
            {
                MAll mall = new MAll()
                {
                    DeviceName = deviceNames[rnd.Next(deviceNames.Length)],
                    Light = 1000 * Math.Cos(ts.TotalMilliseconds / 175) * Math.Sin(ts.TotalMilliseconds / 360) + rnd.Next(30),
                    Potentiometer1 = 1000 * Math.Cos(ts.TotalMilliseconds / 275) * Math.Sin(ts.TotalMilliseconds / 560) + rnd.Next(30),
                    Potentiometer2 = 1000 * Math.Cos(ts.TotalMilliseconds / 60) * Math.Sin(ts.TotalMilliseconds / 120) + rnd.Next(30),
                    Pressure = (float)(1000 * Math.Cos(ts.TotalMilliseconds / 180) * Math.Sin(ts.TotalMilliseconds / 110) + rnd.Next(30)),
                    Temperature = (float)(1000 * Math.Cos(ts.TotalMilliseconds / 180) * Math.Sin(ts.TotalMilliseconds / 110) + rnd.Next(30)),
                    ADC3 = rnd.Next(1000),
                    ADC4 = rnd.Next(1000),
                    ADC5 = rnd.Next(1000),
                    ADC6 = rnd.Next(1000),
                    ADC7 = rnd.Next(1000),
                    Altitude = (float)(1000 * Math.Cos(ts.TotalMilliseconds / 480) * Math.Sin(ts.TotalMilliseconds / 2000) + rnd.Next(30)),
                    ColorName = "ABC",
                    ColorRaw = new ColorData() { Blue = (ushort)rnd.Next(255), Clear = (ushort)rnd.Next(255), Green = (ushort)rnd.Next(255), Red = (ushort)rnd.Next(255) },
                    ColorRgb = new RgbData() { Blue = (ushort)rnd.Next(255), Green = (ushort)rnd.Next(255), Red = (ushort)rnd.Next(255) }
                };
                await SendMsgObjAsync(mall); 
            }

            if (chkMAllBatch.IsChecked == true)
            {
                List<Message> lst = new List<Message>();
                for (int i = 0; i < BATCH_SIZE; i++)
                {
                    MAll mall = new MAll()
                    {
                        DeviceName = deviceNames[rnd.Next(deviceNames.Length)],
                        Light = 1000 * Math.Cos(ts.TotalMilliseconds / 175) * Math.Sin(ts.TotalMilliseconds / 360) + rnd.Next(30),
                        Potentiometer1 = 1000 * Math.Cos(ts.TotalMilliseconds / 275) * Math.Sin(ts.TotalMilliseconds / 560) + rnd.Next(30),
                        Potentiometer2 = 1000 * Math.Cos(ts.TotalMilliseconds / 60) * Math.Sin(ts.TotalMilliseconds / 120) + rnd.Next(30),
                        Pressure = (float)(1000 * Math.Cos(ts.TotalMilliseconds / 180) * Math.Sin(ts.TotalMilliseconds / 110) + rnd.Next(30)),
                        Temperature = (float)(1000 * Math.Cos(ts.TotalMilliseconds / 180) * Math.Sin(ts.TotalMilliseconds / 110) + rnd.Next(30)),
                        ADC3 = rnd.Next(1000),
                        ADC4 = rnd.Next(1000),
                        ADC5 = rnd.Next(1000),
                        ADC6 = rnd.Next(1000),
                        ADC7 = rnd.Next(1000),
                        Altitude = (float)(1000 * Math.Cos(ts.TotalMilliseconds / 480) * Math.Sin(ts.TotalMilliseconds / 2000) + rnd.Next(30)),
                        ColorName = "ABC",
                        ColorRaw = new ColorData() { Blue = (ushort)rnd.Next(255), Clear = (ushort)rnd.Next(255), Green = (ushort)rnd.Next(255), Red = (ushort)rnd.Next(255) },
                        ColorRgb = new RgbData() { Blue = (ushort)rnd.Next(255), Green = (ushort)rnd.Next(255), Red = (ushort)rnd.Next(255) }
                    };
                    lst.Add(new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mall))));
                }
                await m_clt.SendEventBatchAsync(lst); //Liczone jako BATCH_SIZE - nie jeden komunikat
                msg_send += BATCH_SIZE;
            }
            if (chkMSPI.IsChecked == true)
            {
                MSPI mspi = new MSPI()
                {
                    DeviceName = deviceNames[rnd.Next(deviceNames.Length)],
                    Light = 1000 * Math.Cos(ts.TotalMilliseconds / 175) * Math.Sin(ts.TotalMilliseconds / 360) + rnd.Next(30),
                    Potentiometer1 = 1000 * Math.Cos(ts.TotalMilliseconds / 275) * Math.Sin(ts.TotalMilliseconds / 560) + rnd.Next(30),
                    Potentiometer2 = 1000 * Math.Cos(ts.TotalMilliseconds / 60) * Math.Sin(ts.TotalMilliseconds / 120) + rnd.Next(30)
                };
                await SendMsgObjAsync(mspi);
            }
            if (chkMAllNum.IsChecked == true)
            {
                MAllNum mallnum = new MAllNum()
                {
                    DeviceName = deviceNames[rnd.Next(deviceNames.Length)],
                    Light = 1000 * Math.Cos(ts.TotalMilliseconds / 175) * Math.Sin(ts.TotalMilliseconds / 360) + rnd.Next(30),
                    Potentiometer1 = 1000 * Math.Cos(ts.TotalMilliseconds / 275) * Math.Sin(ts.TotalMilliseconds / 560) + rnd.Next(30),
                    Potentiometer2 = 1000 * Math.Cos(ts.TotalMilliseconds / 60) * Math.Sin(ts.TotalMilliseconds / 120) + rnd.Next(30),
                    Pressure = (float)(1000 * Math.Cos(ts.TotalMilliseconds / 180) * Math.Sin(ts.TotalMilliseconds / 110) + rnd.Next(30)),
                    Temperature = (float)(1000 * Math.Cos(ts.TotalMilliseconds / 180) * Math.Sin(ts.TotalMilliseconds / 110) + rnd.Next(30)),
                    ADC3 = rnd.Next(1000),
                    ADC4 = rnd.Next(1000),
                    ADC5 = rnd.Next(1000),
                    ADC6 = rnd.Next(1000),
                    ADC7 = rnd.Next(1000),
                    Altitude = (float)(1000 * Math.Cos(ts.TotalMilliseconds / 480) * Math.Sin(ts.TotalMilliseconds / 2000) + rnd.Next(30)),
                };
                await SendMsgObjAsync(mallnum);
            }
            if (chkMSentence.IsChecked == true)
            {
                MSentence m = new MSentence()
                {
                    DeviceName = deviceNames[rnd.Next(deviceNames.Length)],
                    Sentence = Data.Sentences[rnd.Next(Data.Sentences.Length)]
                };
                await SendMsgObjAsync(m);
            }
            if (chkMWord.IsChecked == true)
            {
                MWord m = new MWord()
                {
                    DeviceName = deviceNames[rnd.Next(deviceNames.Length)],
                    Word = Data.Words[rnd.Next(Data.Words.Length)]
                };
                await SendMsgObjAsync(m);
            }


            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                txtInfo.Text = msg_send.ToString();
            });
        }

        private async Task SendMsgObjAsync(object data)
        {
            var messageString = JsonConvert.SerializeObject(data);
            await SendMsgAsync(messageString);
        }

        private async Task SendMsgAsync(string messageString)
        {
            var message = new Message(Encoding.UTF8.GetBytes(messageString));
            //Add properties for routing
            if (rnd.NextDouble() > 0.8) { message.Properties.Add("direction", "eventhub"); }
            if (rnd.NextDouble() > 0.9) { message.Properties.Add("status", "error"); }
            msg_send++;
            await m_clt.SendEventAsync(message);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);
        }

        private void BtnConnectSubscribe_Click(object sender, RoutedEventArgs e)
        {
            btnConnectSubscribe.IsEnabled = false;
            tgSend.IsEnabled = true;
            m_clt =DeviceClient.CreateFromConnectionString(txtConnectionString.Text,TransportType.Amqp);
            m_clt.SetMethodHandler("STOP", OnStopAsync, null); //Wymaga MQTT - inny przykład!
            ReceiveDataFromAzureAsync(); //Loop, last command 

        }

        async Task<MethodResponse> OnStopAsync(MethodRequest methodRequest, object userContext)
        {
            m_t?.Stop();
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                tgSend.IsOn = false;
            });
            return new MethodResponse(0);
        }
        public async Task ReceiveDataFromAzureAsync()
        {

            Message receivedMessage;
            string messageData;

            while (true)
            {
                receivedMessage = await m_clt.ReceiveAsync();

                if (receivedMessage != null)
                {
                    messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    //Wykonanie polecenia
                    if (messageData.Length >= 1)
                    {
                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            m_col.Add(messageData);
                        });
                    }
                    //await m_deviceClient.RejectAsync(receivedMessage);
                    //await m_deviceClient.AbandonAsync(receivedMessage); - odrzuca, ale komunikat wraca
                    //Potwierdzenie wykonania
                    await m_clt.CompleteAsync(receivedMessage); //potwierdza odebranie
                }
            }
        }

        private void TgSend_Toggled(object sender, RoutedEventArgs e)
        {
            if (tgSend.IsOn)
            {
                m_t.Interval = TimeSpan.FromMilliseconds(int.Parse(txtDelay.Text)); //Not too often!
                m_t.Start();
            }
            else
            {
                m_t.Stop();
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            m_col.Clear();
        }

        private void TxtDelay_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(txtDelay.Text,out int d))
                m_t.Interval = TimeSpan.FromMilliseconds(d);
        }
    }
}
