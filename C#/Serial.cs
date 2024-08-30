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


   #region 1. 시리얼 통신
   private void InitializeSerialPort() {  
      try {
         // setting.ini 파일에서 설정을 읽습니다.
         var ini = new IniFileRead("setting.ini");

         // INI 파일에서 PortName과 BaudRate 값을 가져옵니다.
         string portName = ini.GetSetting("PortName", "COM3");
         int baudRate = int.Parse(ini.GetSetting("BaudRate", "9600"));

         MessageBox.Show($"PortName: {portName}, BaudRate: {baudRate}", "Serial Port Configuration", MessageBoxButton.OK, MessageBoxImage.Information);

         // 시리얼 포트 설정

         _serialPort = new SerialPort {
            PortName = portName, // 기본 포트 이름 (설정에 따라 변경)
            BaudRate = baudRate,   // 기본 보드레이트
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
         // 예외 처리 (예: 로그 기록)
         MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }
   private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e) {
      try {
         lock(thisLock) {
            this.Dispatcher.Invoke(() => {
               string indata = _serialPort.ReadExisting(); // ReadExisting() 사용

               // 데이터 처리
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
      // 데이터 스트림 처리
      if(string.IsNullOrEmpty(indata)) return;

      if(indata.Contains(_02)) {
         inStream += indata; // 데이터 추가
         }

      string[] splited = inStream.Split(_02);

      foreach(string sp in splited) {
         if(string.IsNullOrEmpty(sp)) continue;

         // 데이터를 처리하는 로직 (예: ETX, CR, LF 처리)
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
   public void SendSerialData(string data) { //시리얼 데이터 보는 부분 
                                             //특정 레지스터값일 경우   string data = "02" + "SET,D100,5678" + "0D" + "0A"; 같은 명령어 확인할것. 
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


   public void SerialCommanRead(string cmd) {  //시리얼 받는 부분 
      OutputTextBlock.Text = cmd;
      string[] dataParts = cmd.Split(',');

      //데이터 형식과 목록을 알아야 함. 


      if(dataParts.Length == 12) {

         //Function1(dataParts[0]) ~    Function2(dataParts[1]);

         } else {
         MessageBox.Show($"데이터 형식이 잘못되었습니다.");

         }

      }

   }
}