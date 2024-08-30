using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;



namespace UI {
    public class IniFileRead {
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




        /* ini파일 메인에서 읽기 샘플 코드  */
        public string IniFileReadSampleCode() {

            // IniFileRead 함수를 이용해 setting.ini 파일에서 설정을 읽습니다.
            var ini = new IniFileRead("setting.ini");

            // INI 파일에서 PortName과 BaudRate 값을 가져옵니다.
            string portName = ini.GetSetting("PortName", "COM3");
            int baudRate = int.Parse(ini.GetSetting("BaudRate", "9600"));

            //메시지 박스에 출력
            MessageBox.Show($"PortName: {portName}, BaudRate: {baudRate}", "Serial Port Configuration", MessageBoxButton.OK, MessageBoxImage.Information);

            //portName부분만 출력 
            return portName;

        }

        /* ini파일 메인에서 쓰기 샘플 코드  */
        public void IniFileWriteSampleCode() {
            // 변경할 설정값을 Dictionary에 추가합니다.
            var newSettings = new Dictionary<string, string> {
                { "PortName", "COM4" },
                { "BaudRate", "115200" }
            };

            // 파일에 설정을 씁니다.
            using(var writer = new StreamWriter(_path, false, Encoding.UTF8)) {
                foreach(var kvp in newSettings) {
                    writer.WriteLine($"{kvp.Key}={kvp.Value}");
                }
            }

            MessageBox.Show("INI 파일이 성공적으로 업데이트되었습니다.", "Serial Port Configuration", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
