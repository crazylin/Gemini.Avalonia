using System;

namespace SCSA.Models;

/// <summary>
/// 参数范围和选项的枚举定义
/// </summary>
public static class ParameterRanges
{
    /// <summary>
    /// 采样率选项 (Hz) - 根据原项目GetSampleOptions()方法
    /// </summary>
    public enum SamplingRate : byte
    {
        /// <summary>2kHz</summary>
        KHz_2 = 0x00,
        /// <summary>5kHz</summary>
        KHz_5 = 0x01,
        /// <summary>10kHz</summary>
        KHz_10 = 0x02,
        /// <summary>20kHz</summary>
        KHz_20 = 0x03,
        /// <summary>50kHz</summary>
        KHz_50 = 0x04,
        /// <summary>100kHz</summary>
        KHz_100 = 0x05,
        /// <summary>200kHz</summary>
        KHz_200 = 0x06,
        /// <summary>400kHz</summary>
        KHz_400 = 0x07,
        /// <summary>800kHz</summary>
        KHz_800 = 0x08,
        /// <summary>1MHz</summary>
        MHz_1 = 0x09,
        /// <summary>2MHz</summary>
        MHz_2 = 0x0A,
        /// <summary>4MHz</summary>
        MHz_4 = 0x0B,
        /// <summary>5MHz</summary>
        MHz_5 = 0x0C,
        /// <summary>10MHz</summary>
        MHz_10 = 0x0D,
        /// <summary>20MHz</summary>
        MHz_20 = 0x0E
    }

    /// <summary>
    /// 上传数据类型
    /// </summary>
    public enum UploadDataType : byte
    {
        /// <summary>速度</summary>
        Velocity = 0,
        /// <summary>位移</summary>
        Displacement = 1,
        /// <summary>加速度</summary>
        Acceleration = 2,
        /// <summary>I、Q路信号</summary>
        IQSignal = 3
    }

    /// <summary>
    /// 激光功率指示器电平
    /// </summary>
    public enum LaserPowerLevel : byte
    {
        /// <summary>0</summary>
        Level_0 = 0,
        /// <summary>1</summary>
        Level_1 = 1,
        /// <summary>2</summary>
        Level_2 = 2,
        /// <summary>3</summary>
        Level_3 = 3,
        /// <summary>4</summary>
        Level_4 = 4,
        /// <summary>5</summary>
        Level_5 = 5,
        /// <summary>6</summary>
        Level_6 = 6,
        /// <summary>7</summary>
        Level_7 = 7,
        /// <summary>8</summary>
        Level_8 = 8,
        /// <summary>9</summary>
        Level_9 = 9,
        /// <summary>10</summary>
        Level_10 = 10
    }

    /// <summary>
    /// 低通滤波器频率 (Hz)
    /// </summary>
    public enum LowPassFilter : byte
    {
        /// <summary>5Hz</summary>
        Hz_5 = 0,
        /// <summary>10Hz</summary>
        Hz_10 = 1,
        /// <summary>25Hz</summary>
        Hz_25 = 2,
        /// <summary>50Hz</summary>
        Hz_50 = 3,
        /// <summary>100Hz</summary>
        Hz_100 = 4,
        /// <summary>250Hz</summary>
        Hz_250 = 5,
        /// <summary>500Hz</summary>
        Hz_500 = 6,
        /// <summary>1kHz</summary>
        KHz_1 = 7,
        /// <summary>2.5kHz</summary>
        KHz_2_5 = 8,
        /// <summary>5kHz</summary>
        KHz_5 = 9,
        /// <summary>10kHz</summary>
        KHz_10 = 10,
        /// <summary>25kHz</summary>
        KHz_25 = 11,
        /// <summary>50kHz</summary>
        KHz_50 = 12,
        /// <summary>100kHz</summary>
        KHz_100 = 13,
        /// <summary>250kHz</summary>
        KHz_250 = 14
    }

    /// <summary>
    /// 高通滤波器频率 (Hz)
    /// </summary>
    public enum HighPassFilter : byte
    {
        /// <summary>0.1Hz</summary>
        Hz_0_1 = 0,
        /// <summary>0.2Hz</summary>
        Hz_0_2 = 1,
        /// <summary>0.5Hz</summary>
        Hz_0_5 = 2,
        /// <summary>1Hz</summary>
        Hz_1 = 3,
        /// <summary>2Hz</summary>
        Hz_2 = 4,
        /// <summary>5Hz</summary>
        Hz_5 = 5,
        /// <summary>10Hz</summary>
        Hz_10 = 6,
        /// <summary>20Hz</summary>
        Hz_20 = 7,
        /// <summary>50Hz</summary>
        Hz_50 = 8,
        /// <summary>100Hz</summary>
        Hz_100 = 9
    }

    /// <summary>
    /// 速度量程 (mm/s)
    /// </summary>
    public enum VelocityRange : byte
    {
        /// <summary>0.5mm/s</summary>
        Range_0_5 = 0,
        /// <summary>1mm/s</summary>
        Range_1 = 1,
        /// <summary>2mm/s</summary>
        Range_2 = 2,
        /// <summary>5mm/s</summary>
        Range_5 = 3,
        /// <summary>10mm/s</summary>
        Range_10 = 4,
        /// <summary>20mm/s</summary>
        Range_20 = 5,
        /// <summary>50mm/s</summary>
        Range_50 = 6,
        /// <summary>100mm/s</summary>
        Range_100 = 7,
        /// <summary>200mm/s</summary>
        Range_200 = 8,
        /// <summary>500mm/s</summary>
        Range_500 = 9,
        /// <summary>1000mm/s</summary>
        Range_1000 = 10,
        /// <summary>2000mm/s</summary>
        Range_2000 = 11,
        /// <summary>5000mm/s</summary>
        Range_5000 = 12,
        /// <summary>10000mm/s</summary>
        Range_10000 = 13,
        /// <summary>20000mm/s</summary>
        Range_20000 = 14
    }

    /// <summary>
    /// 位移量程 (μm)
    /// </summary>
    public enum DisplacementRange : byte
    {
        /// <summary>0.5μm</summary>
        Range_0_5 = 0,
        /// <summary>1μm</summary>
        Range_1 = 1,
        /// <summary>2μm</summary>
        Range_2 = 2,
        /// <summary>5μm</summary>
        Range_5 = 3,
        /// <summary>10μm</summary>
        Range_10 = 4,
        /// <summary>20μm</summary>
        Range_20 = 5,
        /// <summary>50μm</summary>
        Range_50 = 6,
        /// <summary>100μm</summary>
        Range_100 = 7,
        /// <summary>200μm</summary>
        Range_200 = 8,
        /// <summary>500μm</summary>
        Range_500 = 9,
        /// <summary>1000μm</summary>
        Range_1000 = 10,
        /// <summary>2000μm</summary>
        Range_2000 = 11,
        /// <summary>5000μm</summary>
        Range_5000 = 12,
        /// <summary>10000μm</summary>
        Range_10000 = 13,
        /// <summary>20000μm</summary>
        Range_20000 = 14
    }

    /// <summary>
    /// 加速度量程 (mm/s²)
    /// </summary>
    public enum AccelerationRange : byte
    {
        /// <summary>0.5mm/s²</summary>
        Range_0_5 = 0,
        /// <summary>1mm/s²</summary>
        Range_1 = 1,
        /// <summary>2mm/s²</summary>
        Range_2 = 2,
        /// <summary>5mm/s²</summary>
        Range_5 = 3,
        /// <summary>10mm/s²</summary>
        Range_10 = 4,
        /// <summary>20mm/s²</summary>
        Range_20 = 5,
        /// <summary>50mm/s²</summary>
        Range_50 = 6,
        /// <summary>100mm/s²</summary>
        Range_100 = 7,
        /// <summary>200mm/s²</summary>
        Range_200 = 8,
        /// <summary>500mm/s²</summary>
        Range_500 = 9,
        /// <summary>1000mm/s²</summary>
        Range_1000 = 10,
        /// <summary>2000mm/s²</summary>
        Range_2000 = 11,
        /// <summary>5000mm/s²</summary>
        Range_5000 = 12,
        /// <summary>10000mm/s²</summary>
        Range_10000 = 13,
        /// <summary>20000mm/s²</summary>
        Range_20000 = 14
    }

    /// <summary>
    /// 数字量程
    /// </summary>
    public enum DigitalRange : byte
    {
        /// <summary>中</summary>
        Medium = 0,
        /// <summary>低</summary>
        Low = 1,
        /// <summary>高</summary>
        High = 2
    }

    /// <summary>
    /// 模拟输出类型
    /// </summary>
    public enum AnalogOutputType : byte
    {
        /// <summary>速度</summary>
        Velocity = 0,
        /// <summary>位移</summary>
        Displacement = 1,
        /// <summary>加速度</summary>
        Acceleration = 2
    }

    /// <summary>
    /// 触发采样类型
    /// </summary>
    public enum TriggerSampleType : byte
    {
        /// <summary>自由触发</summary>
        FreeTrigger = 0,
        /// <summary>软件触发</summary>
        SoftwareTrigger = 1,
        /// <summary>硬件触发</summary>
        HardwareTrigger = 2,
        /// <summary>调试触发</summary>
        DebugTrigger = 3
    }

    /// <summary>
    /// 触发采样通道
    /// </summary>
    public enum TriggerSampleChannel : byte
    {
        /// <summary>通道1</summary>
        Channel1 = 0,
        /// <summary>通道2</summary>
        Channel2 = 1
    }

    /// <summary>
    /// 触发采样模式
    /// </summary>
    public enum TriggerSampleMode : byte
    {
        /// <summary>上升沿（高电平）</summary>
        RisingEdge = 0x00,
        /// <summary>下降沿（低电平）</summary>
        FallingEdge = 0x01
    }

    /// <summary>
    /// 前端滤波器类型
    /// </summary>
    public enum FrontendFilterType : byte
    {
        /// <summary>hamming</summary>
        Hamming = 0,
        /// <summary>hann</summary>
        Hann = 1,
        /// <summary>kaiser</summary>
        Kaiser = 2
    }

    /// <summary>
    /// 正交性校正模式
    /// </summary>
    public enum OrthogonalityCorrectionMode : byte
    {
        /// <summary>自动</summary>
        Auto = 0,
        /// <summary>手动</summary>
        Manual = 1
    }

    /// <summary>
    /// 前端滤波器 (老版本兼容)
    /// </summary>
    public enum FrontendFilter : byte
    {
        /// <summary>0.5MHz</summary>
        MHz_0_5 = 0,
        /// <summary>1MHz</summary>
        MHz_1 = 1,
        /// <summary>2MHz</summary>
        MHz_2 = 2,
        /// <summary>5MHz</summary>
        MHz_5 = 3,
        /// <summary>10MHz</summary>
        MHz_10 = 4,
        /// <summary>20MHz</summary>
        MHz_20 = 5
    }

    /// <summary>
    /// 开关状态
    /// </summary>
    public enum SwitchState : byte
    {
        /// <summary>关</summary>
        Off = 0,
        /// <summary>开</summary>
        On = 1
    }

    /// <summary>
    /// 获取枚举类型的最大值
    /// </summary>
    public static byte GetMaxValue<T>() where T : Enum
    {
        var values = Enum.GetValues(typeof(T));
        return (byte)values.Cast<byte>().Max();
    }

    /// <summary>
    /// 获取枚举类型的选项数量
    /// </summary>
    public static int GetOptionCount<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T)).Length;
    }
}
