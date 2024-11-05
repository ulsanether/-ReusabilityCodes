using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

  /// <summary>
   /* 작동 샘플 코드 예시, 실행 폴더내. setting.ini파일 있어야함.
    * 
    * 
    * <seting.ini> 예시
      ;크레인 설정 파일. 
      ;
      ;
      
      
      ;통신 설정
      [SerialPort]
      PortName=COM6
      BaudRate=19200
      
      [Crean]
      ;크레인 무게(t)
      CreanWeight = 5
      CreanType = 함미크레인
      
      
      main 설정 예시. 

      IIniFileReaderFactory factory = new IniFileReaderFactory();
      string iniFilePath = "setting.ini";
      var iniReader = factory.CreateIniFileReader(iniFilePath);

      _creanConfig.SerialCreateData.portName = iniReader.GetSetting("PortName", "COM5");
      _creanConfig.SerialCreateData.baudRate = int.Parse(iniReader.GetSetting("BaudRate", "19200"));
      _creanConfig.GroupBData.creaneSafeLoad = int.Parse(iniReader.GetSetting("CreanWeight", "15"));
      _creanConfig.GroupAData.creaneType = iniReader.GetSetting("CreanType", "함수크레인");

      serialData = new SerialData(_creanConfig.SerialCreateData.portName, _creanConfig.SerialCreateData.baudRate); 

   */
   /// </summary>


namespace UI {

   public interface IIniFileReader {
      string GetSetting(string key, string defaultValue = "");
    
      }

   public class IniFileRead :IIniFileReader {
      private readonly Dictionary<string, string> _settings = new Dictionary<string, string>();
      private readonly string _path;

      public IniFileRead(string path) {
         _path = path;
         if(!File.Exists(path)) {
            throw new FileNotFoundException("setting.ini 파일이 없습니다.", path);
            }
         foreach(var line in File.ReadAllLines(path)) {
            var trimmedLine = line.Trim();
            if(string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";") || trimmedLine.StartsWith("["))
               continue;

            var keyValue = trimmedLine.Split('=');
            if(keyValue.Length == 2) {
               _settings[keyValue[0].Trim()] = keyValue[1].Trim();
               }
            }
         }

      public string GetSetting(string key, string defaultValue = "") {
         return _settings.TryGetValue(key, out var value) ? value : defaultValue;
         }

   
      }


 

   public interface IIniFileReaderFactory {
      IIniFileReader CreateIniFileReader(string path);
      }

   public class IniFileReaderFactory :IIniFileReaderFactory {
      public IIniFileReader CreateIniFileReader(string path) {
         return new IniFileRead(path);  
         }
      }

   public class Client {
      private readonly IIniFileReader _iniFileReader;

      public Client(IIniFileReaderFactory factory, string path) {
         _iniFileReader = factory.CreateIniFileReader(path);
         }

      public void ShowIniFileSettings() {
         string portName = _iniFileReader.GetSetting("PortName", "COM2");
         int baudRate = int.Parse(_iniFileReader.GetSetting("BaudRate", "19200"));

         MessageBox.Show($"PortName: {portName}, BaudRate: {baudRate}", "Serial Port Configuration", MessageBoxButton.OK, MessageBoxImage.Information);
         }

      public void UpdateIniFile(Dictionary<string, string> newSettings) {
         _iniFileReader.IniFileWriteSampleCode(newSettings);
         }
      }
   }

