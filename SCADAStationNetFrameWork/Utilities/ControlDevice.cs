using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S7.Net;
using EasyModbus;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SCADAStationNetFrameWork
{
    public class ControlDevice
    {
        ConnectDevice currentDevice { get; set; }
        public string ConnectionStatus { get; set; }
        Plc plcdevice;
        ModbusClient modbusClient;
        ConnectDevice.emConnectionType currentType;
        public ControlDevice()
        {
            //modbusClient = new ModbusClient("192.168.1.50", 502);
            //modbusClient.UnitIdentifier = 255;
            //currentType = ConnectDevice.emConnectionType.emTCP;
            //modbusClient.Connect();
        }
        public ControlDevice(ConnectDevice device)
        {
            currentDevice = device;
            currentType = (ConnectDevice.emConnectionType)device.ConnectionType;
            if (currentType == ConnectDevice.emConnectionType.emS7)
            {
                plcdevice = new Plc(ConvertToCPUType(device.S7PLCType), device.Destination, (short)device.S7PLCRack, (short)device.S7PLCSlot);
            }
            else if (currentType == ConnectDevice.emConnectionType.emTCP)
            {
                modbusClient = new ModbusClient(device.Destination, (int)device.ModbusPort);    //Ip-Address and Port of Modbus-TCP-Server
                modbusClient.UnitIdentifier = device.ModbusDeviceId;    //Ip-Address and Port of Modbus-TCP-Server
            }
        }
        public async Task Connect()
        {

            if (currentType == ConnectDevice.emConnectionType.emS7)
            {
                try
                {
                    await plcdevice.OpenAsync();
                    Trace.WriteLine($"Connect to the PLC {currentDevice.Name} {currentDevice.Destination} succesfully");
                    ConnectionStatus = "Successful";
                }
                catch
                {
                    ConnectionStatus = "Fail";
                    throw;
                }
            }
            else if (currentType == ConnectDevice.emConnectionType.emTCP)
            {
                try
                {
                    modbusClient.Connect();
                    ConnectionStatus = "Successful";
                }
                catch (Exception ex)
                {
                    ConnectionStatus = "Fail";
                    throw;
                }
            }
        }

        public void ReadTag(TagInfo tag)
        {
            if (currentType == ConnectDevice.emConnectionType.emS7)
            {
                var ob1 = plcdevice.Read(tag.MemoryAddress);
                long temp = Convert.ToInt64(ob1);
                if (temp != tag.Data)
                {
                    tag.Data = temp;
                    //SendTagValueToClient(tagInfo.Id, tagInfo.Data);
                }
            }
            else if (currentType == ConnectDevice.emConnectionType.emTCP)
            {

                if (int.TryParse(tag.MemoryAddress, out _))
                {
                    if (tag.MemoryAddress.Length != 5)
                    {
                        return;
                    }
                    int fulladdress = Convert.ToInt32(tag.MemoryAddress);
                    string ModbusType = tag.MemoryAddress.Substring(0, 1);
                    switch (ModbusType)
                    {
                        case "0":

                            if (tag.Type == TagInfo.TagType.eBool)
                            {
                                int address = fulladdress;
                                bool[] result = modbusClient.ReadCoils(address, 1);
                                tag.Data = result[0] ? 1L : 0L;
                            }
                            break;
                        case "1":
                            if (tag.Type == TagInfo.TagType.eBool)
                            {
                                int address = fulladdress - 10000;
                                bool[] result = modbusClient.ReadDiscreteInputs(address, 1);
                                tag.Data = result[0] ? 1L : 0L;
                            }
                            break;
                        case "3":
                            if (tag.Type == TagInfo.TagType.eBool)
                            {
                                int address = fulladdress - 30000;
                                int[] result = modbusClient.ReadInputRegisters(address, 1);
                                tag.Data = (long)result[0];
                            }
                            else if (tag.Type == TagInfo.TagType.eByte || tag.Type == TagInfo.TagType.eShort)
                            {
                                int address = fulladdress - 30000;
                                int[] result = modbusClient.ReadInputRegisters(address, 1);
                                tag.Data = (long)result[0];
                            }
                            else if (tag.Type == TagInfo.TagType.eInt || tag.Type == TagInfo.TagType.eReal)
                            {
                                int address = fulladdress - 30000;
                                int[] result = modbusClient.ReadInputRegisters(address, 2);
                                tag.Data = ((long)result[1] << 16) | ((long)result[0] & 0xFFFF);
                            }
                            else if (tag.Type == TagInfo.TagType.eDouble)
                            {
                                int address = fulladdress - 30000;
                                int[] result = modbusClient.ReadInputRegisters(address, 4);
                                tag.Data = ((long)result[3] << (16 * 3)) | ((long)result[2] << 32 & 0xFFFF00000000) | ((long)result[1] << 16 & 0xFFFF0000) | ((long)result[0] & 0xFFFF);
                            }
                            break;
                        case "4":
                            if (tag.Type == TagInfo.TagType.eBool)
                            {
                                int address = fulladdress - 40000;
                                int[] result = modbusClient.ReadHoldingRegisters(address, 1);
                                tag.Data = (long)result[0];
                            }
                            else if (tag.Type == TagInfo.TagType.eByte || tag.Type == TagInfo.TagType.eShort)
                            {
                                int address = fulladdress - 40000;
                                int[] result = modbusClient.ReadHoldingRegisters(address, 1);
                                tag.Data = (long)result[0];
                            }
                            else if (tag.Type == TagInfo.TagType.eInt || tag.Type == TagInfo.TagType.eReal)
                            {
                                int address = fulladdress - 40000;
                                int[] result = modbusClient.ReadHoldingRegisters(address, 2);
                                tag.Data = ((long)result[1] << 16) | ((long)result[0] & 0xFFFF);
                            }
                            else if (tag.Type == TagInfo.TagType.eDouble)
                            {
                                int address = fulladdress - 40000;
                                int[] result = modbusClient.ReadHoldingRegisters(address, 4);
                                tag.Data = ((long)result[3] << (16 * 3)) | ((long)result[2] << 32 & 0xFFFF00000000) | ((long)result[1] << 16 & 0xFFFF0000) | ((long)result[0] & 0xFFFF);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void WriteTag(TagInfo tag, object value)
        {
            if (currentType == ConnectDevice.emConnectionType.emS7)
            {
                if (tag.Type == TagInfo.TagType.eShort)
                {
                    var valuetowrite = Convert.ToUInt16(value);
                    plcdevice.Write(tag.MemoryAddress, valuetowrite);
                }
                else if (tag.Type == TagInfo.TagType.eBool)
                {
                    plcdevice.Write(tag.MemoryAddress, value);
                }


            }
            else if (currentType == ConnectDevice.emConnectionType.emTCP)
            {
                if (int.TryParse(tag.MemoryAddress, out _))
                {
                    if (tag.MemoryAddress.Length != 5)
                    {
                        return;
                    }
                    int fulladdress = Convert.ToInt32(tag.MemoryAddress);
                    string ModbusType = tag.MemoryAddress.Substring(0, 1);
                    switch (ModbusType)
                    {
                        case "0":

                            if (tag.Type == TagInfo.TagType.eBool)
                            {
                                int address = fulladdress;
                                bool valuetowrite = Convert.ToBoolean(value);
                                modbusClient.WriteSingleCoil(address, valuetowrite);
                            }
                            break;
                        case "4":
                            if (tag.Type == TagInfo.TagType.eBool)
                            {
                                if (value is int || value is short || value is byte || value is sbyte || value is ushort || value is long)
                                {
                                    int address = fulladdress - 40000;
                                    int valuetowrite = Convert.ToInt32(value);
                                    modbusClient.WriteSingleRegister(address, valuetowrite);
                                }

                            }
                            else if (tag.Type == TagInfo.TagType.eByte || tag.Type == TagInfo.TagType.eShort)
                            {
                                int address = fulladdress - 40000;
                                if (value is int || value is short || value is byte || value is sbyte || value is ushort || value is long)
                                {
                                    int valuetowrite = Convert.ToInt32(value);
                                    modbusClient.WriteSingleRegister(address, valuetowrite);
                                }
                            }
                            else if (tag.Type == TagInfo.TagType.eInt)
                            {
                                int address = fulladdress - 40000;
                                if (value is int || value is short || value is byte || value is sbyte || value is ushort || value is long)
                                {
                                    int temp = (int)value;
                                    int[] valuetowrite = new int[2];
                                    valuetowrite[0] = (temp & 0xFFFF);
                                    valuetowrite[1] = (temp >> 16);
                                    modbusClient.WriteMultipleRegisters(address, valuetowrite);
                                }

                            }
                            else if (tag.Type == TagInfo.TagType.eReal)
                            {
                                int address = fulladdress - 40000;

                                if ((value is float) || (value is int) || (value is double) || (value is long))
                                {
                                    float temp = Convert.ToSingle(value);
                                    byte[] floatBytes = BitConverter.GetBytes(temp);
                                    int[] valuetowrite = new int[2];
                                    valuetowrite[0] = (floatBytes[0] & 0xFF) | (floatBytes[1] << 8);
                                    valuetowrite[1] = (floatBytes[2] & 0xFF) | (floatBytes[3] << 8);
                                    modbusClient.WriteMultipleRegisters(address, valuetowrite);
                                }

                            }
                            else if (tag.Type == TagInfo.TagType.eDouble)
                            {
                                int address = fulladdress - 40000;

                                if ((value is float) || (value is int) || (value is double) || (value is long))
                                {
                                    double temp = Convert.ToDouble(value);
                                    byte[] floatBytes = BitConverter.GetBytes(temp);
                                    int[] valuetowrite = new int[4];
                                    valuetowrite[0] = (floatBytes[0] & 0xFF) | (floatBytes[1] << 8);
                                    valuetowrite[1] = (floatBytes[2] & 0xFF) | (floatBytes[3] << 8);
                                    valuetowrite[2] = (floatBytes[4] & 0xFF) | (floatBytes[5] << 8);
                                    valuetowrite[3] = (floatBytes[6] & 0xFF) | (floatBytes[7] << 8);
                                    modbusClient.WriteMultipleRegisters(address, valuetowrite);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        public void test()
        {
            //TagInfo tag = new TagInfo();
            //tag.MemoryAddress = "40111";
            //tag.Type = TagInfo.TagType.eReal;
            //tag.BitPosition = 0;
            //WriteTag(tag, -15.4);
            //ReadTag(tag);
            //Trace.WriteLine(tag.Value);
        }

        CpuType ConvertToCPUType(string type)
        {
            CpuType result = CpuType.S7200;
            switch (type)
            {
                case "S7-200":
                    result = CpuType.S7200;
                    break;
                case "S7-1200":
                    result = CpuType.S71200;
                    break;
                case "S7-1500":
                    result = CpuType.S71500;
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
