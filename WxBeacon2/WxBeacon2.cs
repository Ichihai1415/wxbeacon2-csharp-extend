using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Weathernews.Sensor
{
    /// <summary>
    /// WxBeacon2���i�[���܂�
    /// </summary>
    public class WxBeacon2 : IDisposable
    {
        /// <summary>
        /// Unix���� (time_t)�Ƃ̕ϊ��p�����
        /// </summary>
        private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static readonly Guid SERVICE_SENSOR = Guid.Parse("0c4c3000-7700-46f4-aa96-d5e974e32a54");
        private static readonly Guid CHARACTERISTIC_LATEST_DATA = Guid.Parse("0c4c3001-7700-46f4-aa96-d5e974e32a54");
        private static readonly Guid SERVICE_SENSOR_SETTING = Guid.Parse("0c4c3010-7700-46f4-aa96-d5e974e32a54");
        private static readonly Guid CHARACTERISTIC_MEASUREMENT_INTERVAL = Guid.Parse("0c4c3011-7700-46f4-aa96-d5e974e32a54");
        private static readonly Guid SERVICE_CONTROL = Guid.Parse("0c4c3030-7700-46f4-aa96-d5e974e32a54");
        private static readonly Guid CHARACTERISTIC_TIME_INFORMATION = Guid.Parse("0c4c3030-7700-46f4-aa96-d5e974e32a54");

        /// <summary>
        /// WxBeacon2��BLE�f�o�C�X�C���X�^���X���擾���܂�
        /// </summary>
        public BluetoothLEDevice Device
        {
            private set;
            get;
        }

        /// <summary>
        /// WxBeacon2�̃C���X�^���X�����������܂�
        /// </summary>
        /// <param name="device"></param>
        public WxBeacon2(BluetoothLEDevice device)
        {
            Device = device;
        }

        /// <summary>
        /// WxBeacon2�ő��肵���ŐV�̑���l���擾���܂�
        /// </summary>
        /// <returns></returns>
        public async Task<WxBeacon2Data> GetLatestDataAsync()
        {
            Debug.WriteLine("����l���擾���Ă��܂�...");
            byte[] data = await ReadCharacteristicAsync(Device, SERVICE_SENSOR, CHARACTERISTIC_LATEST_DATA);
            if (data != null)
            {
                Debug.WriteLine("����");
                return new WxBeacon2Data(
                        BitConverter.ToInt16(data, 1) * 0.01f,
                        BitConverter.ToInt16(data, 3) * 0.01f,
                        BitConverter.ToUInt16(data, 5),
                        BitConverter.ToInt16(data, 7) * 0.01f,
                        BitConverter.ToInt16(data, 9) * 0.1f,
                        BitConverter.ToInt16(data, 11) * 0.01f,
                        BitConverter.ToInt16(data, 13) * 0.01f,
                        BitConverter.ToInt16(data, 15) * 0.01f,
                        BitConverter.ToUInt16(data, 17) * 0.001f
                );
            }
            else
            {
                Debug.WriteLine("���s");
                return null;
            }
        }

        /// <summary>
        /// WxBeacon2�̌��ݎ������擾���܂�
        /// </summary>
        /// <returns></returns>
        public async Task<DateTime?> GetDateTimeAsync()
        {
            Debug.WriteLine("���ݎ������擾���܂�...");
            byte[] data = await ReadCharacteristicAsync(Device, SERVICE_CONTROL, CHARACTERISTIC_TIME_INFORMATION);
            Debug.WriteLine("����");
            return UNIX_EPOCH.AddSeconds(BitConverter.ToUInt32(data, 0)).ToLocalTime();
        }

        /// <summary>
        /// WxBeacon2�̌��ݎ�����ݒ肵�܂�
        /// </summary>
        /// <param name="time">���ݎ���</param>
        /// <returns></returns>
        public async Task SetDateTimeAsync(DateTime time)
        {
            Debug.WriteLine("���ݎ�����ݒ肵�܂�...");
            var unixtime = (uint)(time.ToUniversalTime() - UNIX_EPOCH).TotalSeconds;
            Debug.WriteLine("����");
            byte[] data = BitConverter.GetBytes(unixtime);
            await WriteCharacteristicAsync(Device, SERVICE_CONTROL, CHARACTERISTIC_TIME_INFORMATION, data);
        }

        /// <summary>
        /// WxBeacon2�̌v���Ԋu���擾���܂�
        /// </summary>
        /// <returns></returns>
        public async Task<TimeSpan> GetMeasureSpanAsync()
        {
            Debug.WriteLine("����Ԋu���擾���܂�...");
            byte[] data = await ReadCharacteristicAsync(Device, SERVICE_SENSOR_SETTING, CHARACTERISTIC_MEASUREMENT_INTERVAL);
            Debug.WriteLine("����");
            return TimeSpan.FromSeconds(BitConverter.ToUInt16(data, 0));
        }

        /// <summary>
        /// WxBeacon2�̌v���Ԋu��ݒ肵�܂�
        /// </summary>
        /// <param name="span">�v���Ԋu</param>
        /// <returns></returns>
        public async Task SetMeasureSpanAsync(TimeSpan span)
        {
            Debug.WriteLine("����Ԋu��ݒ肵�܂�...");
            byte[] data = BitConverter.GetBytes((ushort)span.TotalSeconds);
            Debug.WriteLine("����");
            await WriteCharacteristicAsync(Device, SERVICE_SENSOR_SETTING, CHARACTERISTIC_MEASUREMENT_INTERVAL, data);
        }

        public void Dispose()
        {
            Device.Dispose();
        }

        /// <summary>
        /// �w�肳�ꂽBluetoothLEDevice����L�����N�^���X�e�B�b�N�̒l��ǂݍ��݂܂�
        /// </summary>
        /// <param name="device">�f�o�C�X</param>
        /// <param name="serviceUuid">�T�[�r�XUUID</param>
        /// <param name="characteristicUuid">�L�����N�^���X�e�B�b�NUUID</param>
        /// <returns></returns>
        private static async Task<byte[]> ReadCharacteristicAsync(BluetoothLEDevice device, Guid serviceUuid, Guid characteristicUuid)
        {
            var serviceFinder = await device.GetGattServicesAsync();
            if (serviceFinder.Status != GattCommunicationStatus.Success)
                throw new Exception("�T�[�r�X�̃X�L�����Ɏ��s���܂���");
            var service = serviceFinder.Services.Single(s => s.Uuid == serviceUuid) ?? throw new Exception("�T�[�r�X��������܂���ł���");
            var characteristicFinder = await service.GetCharacteristicsAsync();
            if (characteristicFinder.Status != GattCommunicationStatus.Success)
                throw new Exception("�L�����N�^���X�e�B�b�N�̃X�L�����Ɏ��s���܂���");
            var characteristic = characteristicFinder.Characteristics.Single(c => c.Uuid == characteristicUuid) ?? throw new Exception("�L�����N�^���X�e�B�b�N��������܂���ł���");
            var readResult = await characteristic.ReadValueAsync();
            if (readResult.Status != GattCommunicationStatus.Success)
                throw new Exception("�L�����N�^���X�e�B�b�N�̓ǂݍ��݂Ɏ��s���܂���");
            return readResult.Value.ToArray();
        }

        /// <summary>
        /// �w�肳�ꂽBluetoothLEDevice�ɃL�����N�^���X�e�B�b�N�̒l���������݂܂�
        /// </summary>
        /// <param name="device">�f�o�C�X</param>
        /// <param name="serviceUuid">�T�[�r�XUUID</param>
        /// <param name="characteristicUuid">�L�����N�^���X�e�B�b�NUUID</param>
        /// <param name="data">�������ޒl</param>
        /// <returns></returns>
        private static async Task WriteCharacteristicAsync(BluetoothLEDevice device, Guid serviceUuid, Guid characteristicUuid, byte[] data)
        {
            var serviceFinder = await device.GetGattServicesAsync();
            if (serviceFinder.Status != GattCommunicationStatus.Success)
                throw new Exception("�T�[�r�X�̃X�L�����Ɏ��s���܂���");
            var service = serviceFinder.Services.Single(s => s.Uuid == serviceUuid) ?? throw new Exception("�T�[�r�X��������܂���ł���");
            var characteristicFinder = await service.GetCharacteristicsAsync();
            if (characteristicFinder.Status != GattCommunicationStatus.Success)
                throw new Exception("�L�����N�^���X�e�B�b�N�̃X�L�����Ɏ��s���܂���");
            var characteristic = characteristicFinder.Characteristics.Single(c => c.Uuid == characteristicUuid) ?? throw new Exception("�L�����N�^���X�e�B�b�N��������܂���ł���");
            await characteristic.WriteValueAsync(data.AsBuffer());
        }

        /// <summary>
        /// WxBeacon2�̌����p�A�h�o�^�C�Y�t�B���^���쐬���ĕԂ��܂�
        /// </summary>
        /// <returns>WxBeacon2�����p��BluetoothLEAdvertisementFilter�C���X�^���X</returns>
        public static BluetoothLEAdvertisementFilter CreateAdvertisementFilter()
        {
            var filter = new BluetoothLEAdvertisementFilter();
            filter.Advertisement.LocalName = "Env";
            return filter;
        }
    }
}
