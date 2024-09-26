
DFPlayer mini mp3 플레이어 


// 볼륨 조절과 재생/정지만을 위한 핸들러

/* MP3_VolumeAndPlaybackHandler(SPECIFY_VOLUME, 15); 볼륨 15로 설정
 MP3_VolumeAndPlaybackHandler(0x04, 0); 볼륨 증가 
 MP3_VolumeAndPlaybackHandler(PLAYBACK, 0); 트랙 재생 시작
MP3_VolumeAndPlaybackHandler(0x16, 0); 재생 중지 

*/


typedef enum {
    NEXT = 0x01,
    PREVIOUS = 0x02,
    SPECIAL_TRACKING = 0x03,
    INCREASE_VOLUME = 0x04,
    DECREASE_VOLUME = 0x05,
    SPECIFY_VOLUME = 0x06,
    SPECIFY_EQ = 0x07,
    SPECIFY_PLAYBACK_MODE = 0x08,
    SPECIFY_PLAYBACK_SOURCE = 0x09,
    STANDBY = 0x0A,
    NORMAL_WORKING = 0x0B,
    RESET = 0x0C,
    PLAYBACK = 0x0D,
    PAUSE = 0x0E,
    SPECIFY_FOLDER = 0x0F,
    VOLUME_ADJUST = 0x10,
    REPEAT_PLAY = 0x11,
    SPECIFY_MP3_FOLDER = 0x12,
    COMMERCIALS = 0x13,
    SUPPORT_FOLDER = 0x14,
    STOP_PLAYBACK_BG = 0x15,
    STOP_PLAYBACK = 0x16
} MP3Command;


void MP3_VolumeAndPlaybackHandler(MP3Command command, WORD parameter) {
    switch (command) {
        case INCREASE_VOLUME:  // Increase volume
            MP3_SendCommand(0x04, 0);
            break;
        case DECREASE_VOLUME:  // Decrease volume
            MP3_SendCommand(0x05, 0);
            break;
        case SPECIFY_VOLUME:  // Specify volume 0-30
            if (parameter <= 30) {
                MP3_SendCommand(0x06, parameter);
            }
            break;
        case PLAYBACK:  // Playback
            MP3_SendCommand(0x0D, 0);
            break;
        case PAUSE:  // Pause
            MP3_SendCommand(0x0E, 0);
            break;
        case STOP_PLAYBACK:  // Stop playback
            MP3_SendCommand(0x16, 0);
            break;
        default:
            // Invalid command
            break;
    }
}

void MP3_SendCommand(BYTE Cmd, WORD wParameter){
BYTE Transfer;
	BYTE Value;
	WORD CheckSum;
	WORD Register;

	CheckSum = 0;
	Transfer = 0;
	DMA_TX_USART1[Transfer++] = 0x7E;	// $Start

	DMA_TX_USART1[Transfer++] = 0xFF;	CheckSum += 0xFF;	// Version
	DMA_TX_USART1[Transfer++] = 0x06;	CheckSum += 0x06;	// Length

	DMA_TX_USART1[Transfer++] = Cmd;	CheckSum += Cmd;	// Specify tracking(NUM) 0 ~ 2999 => 0x03, Volume => 0x06

	Value = 1;
	DMA_TX_USART1[Transfer++] = Value;	CheckSum += Value;	// Feedback 0 = false, 1 = true 
	

	Register = (wParameter >> 8) & 0x00FF;
	DMA_TX_USART1[Transfer++] = Register;	CheckSum += Register;	// Command

	Register = (wParameter >> 0) & 0x00FF;
	DMA_TX_USART1[Transfer++] = Register;	CheckSum += Register;	// Command
	

	Register = 0 - CheckSum;
	DMA_TX_USART1[Transfer++] = (Register >> 8) & 0x00FF;	// CheckSum H
	DMA_TX_USART1[Transfer++] = (Register >> 0) & 0x00FF;	// CheckSum L

	DMA_TX_USART1[Transfer++] = 0xEF;	// $End

	DMA3CNT = Transfer - 1;
	DMA3CONbits.CHEN = 1;
	DMA3REQbits.FORCE = 1;
	m_LEDTX_USART1 = 2;
}

