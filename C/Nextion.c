넥션 문자 보내기 

//문제 보내는 함수 
Nextion_Send_String("t1.txt=\"16\"");

void Netion_UART2_SendString(char *s)
{
    do
    {TransmitChar_USART2(*s);
        s++;
    }while(*s != 0);
}
void Nextion_Send_String(char* string)
{
    Netion_UART2_SendString(string);
    TransmitChar_USART2(0xFF);
    TransmitChar_USART2(0xFF);
    TransmitChar_USART2(0xFF);
    delay_ms(1000);
}

