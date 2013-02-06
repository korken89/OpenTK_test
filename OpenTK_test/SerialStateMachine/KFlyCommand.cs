using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KFlySerialStateMachine
{
    class KFlyCommand
    {
        public static byte SYNC = (byte)0xa6;
	    public static byte ACK_BIT = (byte)0x40;
	    public static byte ACK_MASK = (byte)((int)~0x40 & 0xff);
	
	    public enum Command {
		    None = 0,
		    ACK = 1,
		    Ping,
		    DebugMessage,
		    GetRunningMode,
            PrepareWriteFirmware,       /* Bootloader specific, shall always require ACK */
		    WriteFirmwarePackage,		/* Bootloader specific, shall always require ACK */
		    WriteLastFirmwarePackage,	/* Bootloader specific, shall always require ACK */
		    ReadFirmwarePackage,		/* Bootloader specific, shall always require ACK */
		    ReadLastFirmwarePackage,    /* Bootloader specific, shall always require ACK */
		    NextPackage,				/* Bootloader specific, shall always require ACK */
		    ExitBootloader,             /* Bootloader specific, shall always require ACK */
		    GetBootloaderVersion,
		    GetFirmwareVersion,
		    SaveToFlash,
		    GetRegulatorData,
		    SetRegulatorData,
		    GetChannelMix,
		    SetChannelMix,
		    StartRCCalibration,
		    StopRCCalibration,
		    CalibrateRCCenters,
		    GetRCCalibration,
		    SetRCCalibration,
		    GetRCValues,
		    GetSensorData
	    };

        public static byte[] CommandLength = {  0,      /* None */
		                                        0,      /* ACK */
		                                        0,      /* Ping */
		                                        255,    /* DebugMessage */
		                                        255,    /* GetRunningMode */
		                                        2,      /* PrepareWriteFirmware */
		                                        0,      /* WriteFirmwarePackage */
		                                        0,      /* WriteLastFirmwarePackage */
		                                        66,     /* ReadFirmwarePackage */
		                                        255,    /* ReadLastFirmwarePackage */
		                                        0,      /* NextPackage */
		                                        0,      /* ExitBootloader */
		                                        0,      /* GetBootloaderVersion */
		                                        0,      /* GetFirmwareVersion */
		                                        0,      /* SaveToFlash */
		                                        255,    /* GetRegulatorData */
		                                        0,      /* SetRegulatorData */
		                                        255,    /* GetChannelMix */
		                                        0,      /* SetChannelMix */
		                                        0,      /* StartRCCalibration */
		                                        0,      /* StopRCCalibration */
		                                        0,      /* CalibrateRCCenters */
		                                        255,    /* GetRCCalibration */
		                                        0,      /* SetRCCalibration */
		                                        255,    /* GetRCValues */
		                                        255     /* GetDataDump */
		                                        }; 
    }
}
