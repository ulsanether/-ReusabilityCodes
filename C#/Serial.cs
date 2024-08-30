using System;
using System.Diagnostics.Eventing.Reader;
using System.IO.Ports;
using System.Linq;
using System.Reflection.Emit;
using System.Windows;


class Serial {

   private SerialPort _serialPort;

   private Object thisLock = new Object();

   private const char _02 = (char)0x02;
   private const char _0D = (char)0x0D;
   private const char _0A = (char)0x0A;
   private string inStream = "";


   #region 1. �ø��� ���
   private void InitializeSerialPort() {  
      try {
         // setting.ini ���Ͽ��� ������ �н��ϴ�.
         var ini = new IniFileRead("setting.ini");

         // INI ���Ͽ��� PortName�� BaudRate ���� �����ɴϴ�.
         string portName = ini.GetSetting("PortName", "COM3");
         int baudRate = int.Parse(ini.GetSetting("BaudRate", "9600"));

         MessageBox.Show($"PortName: {portName}, BaudRate: {baudRate}", "Serial Port Configuration", MessageBoxButton.OK, MessageBoxImage.Information);

         // �ø��� ��Ʈ ����

         _serialPort = new SerialPort {
            PortName = portName, // �⺻ ��Ʈ �̸� (������ ���� ����)
            BaudRate = baudRate,   // �⺻ ���巹��Ʈ
            Parity = Parity.None,
            DataBits = 8,
            StopBits = StopBits.One,
            Handshake = Handshake.None,
            ReadTimeout = 500,
            WriteTimeout = 500
            };
         _serialPort.DataReceived += DataReceivedHandler;

         }

      catch(Exception ex) {
         // ���� ó�� (��: �α� ���)
         MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }
   private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e) {
      try {
         lock(thisLock) {
            this.Dispatcher.Invoke(() => {
               string indata = _serialPort.ReadExisting(); // ReadExisting() ���

               // ������ ó��
               OutputTextBlock.Text = indata;
               MessageBox.Show(indata);

               ProcessData(indata);
            });
            }
         }
      catch(Exception ex) {
         Console.WriteLine(ex.Message);
         }
      }
   private void ProcessData(string indata) {
      // ������ ��Ʈ�� ó��
      if(string.IsNullOrEmpty(indata)) return;

      if(indata.Contains(_02)) {
         inStream += indata; // ������ �߰�
         }

      string[] splited = inStream.Split(_02);

      foreach(string sp in splited) {
         if(string.IsNullOrEmpty(sp)) continue;

         // �����͸� ó���ϴ� ���� (��: ETX, CR, LF ó��)
         int etxIndex = sp.IndexOf(_0A);
         if(etxIndex >= 0) {
            etxIndex = sp.IndexOf(_0D);
            if(etxIndex >= 0) {
               string cmd = sp.Substring(0, etxIndex);
               SerialCommanRead(cmd);
               }
            }
         }
      }
   private void SerialPort_DataReceived_InMainThread(object s, EventArgs e) {

      string indata = _serialPort.ReadExisting();
      string _indata = indata;
      char[] arr = indata.ToCharArray();

      if(arr.Length <= 0) return;
      if(arr[0] == _02) {
         inStream = indata;
         } else {
         inStream += indata;
         }
      string[] splited = inStream.Split(_02);

      foreach(string sp in splited) {
         char[] arr2 = sp.ToCharArray();
         if(arr2.Length <= 0) continue;
         int etxIndex = Array.IndexOf(arr2, _0A);
         if(etxIndex <= 0) continue;
         etxIndex = Array.IndexOf(arr2, _0D);
         if(etxIndex <= 0) continue;
         var arr3 = arr2.SubArray(0, etxIndex);
         string cmd = new string(arr3);
         SerialCommanRead(cmd);
         }
      }
   public void SendSerialData(string data) { //�ø��� ������ ���� �κ� 
                                             //Ư�� �������Ͱ��� ���   string data = "02" + "SET,D100,5678" + "0D" + "0A"; ���� ��ɾ� Ȯ���Ұ�. 
      string strOutputData = _02 + data + _0D + _0A;
      if(_serialPort.IsOpen) {
         _serialPort.Write(strOutputData);
         } else {
         try {
            _serialPort.Open();
            _serialPort.Write(strOutputData);
            }
         catch(Exception ex) {
            MessageBox.Show(ex.ToString());
            }

         }
      }


   public void SerialCommanRead(string cmd) {  //�ø��� �޴� �κ� 
      OutputTextBlock.Text = cmd;
      string[] dataParts = cmd.Split(',');

      //������ ���İ� ����� �˾ƾ� ��. 


      if(dataParts.Length == 12) {

         //Function1(dataParts[0]) ~    Function2(dataParts[1]);

         } else {
         MessageBox.Show($"������ ������ �߸��Ǿ����ϴ�.");

         }

      }

   }
}