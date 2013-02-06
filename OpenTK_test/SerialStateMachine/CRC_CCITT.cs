using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KFlySerialStateMachine
{
    class CRC_CCITT
    {
        public static int GenerateCRC(List<byte> message)
	    {
            int crc = 0xffff;
            int polynomial = 0x1021; 

            foreach (byte b in message) {
                for (int i = 0; i < 8; i++) {
                    bool bit = ((b >> (7-i) & 1) == 1);
                    bool c15 = ((crc >> 15 & 1) == 1);
                
                    crc <<= 1;
                
                    if (c15 ^ bit)
                	    crc ^= polynomial;
                 }
            }

            crc &= 0xffff;

		    return crc;
	    }
    }
}
