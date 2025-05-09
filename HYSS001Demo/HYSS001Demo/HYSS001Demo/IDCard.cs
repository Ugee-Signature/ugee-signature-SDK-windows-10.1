﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HYSS001Demo
{
    public class IDCard
    {
        public static Boolean IsConnected = false;
        public static Boolean IsAuthenticate = false;
        public static Boolean IsRead_Content = false;
        public static int Port = 0;
        public static int ComPort = 0;
        public const int cbDataSize = 128;
        public const int GphotoSize = 256 * 1024;

        [DllImport("termb.dll")]
        public static extern int InitComm(int port);//连接身份证阅读器 

        [DllImport("termb.dll")]
        public static extern int InitCommExt();//自动搜索身份证阅读器并连接身份证阅读器 

        [DllImport("termb.dll")]
        public static extern int CloseComm();//断开与身份证阅读器连接 

        [DllImport("termb.dll")]
        public static extern int Authenticate();//判断是否有放卡，且是否身份证 

        [DllImport("termb.dll")]
        public static extern int Read_Content(int index);//读卡操作,信息文件存储在dll所在下

        [DllImport("termb.dll")]
        public static extern int ReadContent(int index);//读卡操作,信息文件存储在dll所在下

        [DllImport("termb.dll")]
        public static extern int GetSAMID(StringBuilder SAMID);//获取SAM模块编号

        [DllImport("termb.dll")]
        public static extern int GetSAMIDEx(StringBuilder SAMID);//获取SAM模块编号（10位编号）

        [DllImport("termb.dll")]
        public static extern int GetBmpPhoto(string PhotoPath);//解析身份证照片

        [DllImport("termb.dll")]
        public static extern int GetBmpPhotoToMem(byte[] imageData, int cbImageData);//解析身份证照片

        [DllImport("termb.dll")]
        public static extern int GetBmpPhotoExt();//解析身份证照片

        [DllImport("termb.dll")]
        public static extern int Reset_SAM();//重置Sam模块

        [DllImport("termb.dll")]
        public static extern int GetSAMStatus();//获取SAM模块状态 

        [DllImport("termb.dll")]
        public static extern int GetCardInfo(int index, StringBuilder value);//解析身份证信息 

        [DllImport("termb.dll")]
        public static extern int ExportCardImageV();//生成竖版身份证正反两面图片(输出目录：dll所在目录的cardv.jpg和SetCardJPGPathNameV指定路径)

        [DllImport("termb.dll")]
        public static extern int ExportCardImageH();//生成横版身份证正反两面图片(输出目录：dll所在目录的cardh.jpg和SetCardJPGPathNameH指定路径) 

        [DllImport("termb.dll")]
        public static extern int SetTempDir(string DirPath);//设置生成文件临时目录

        [DllImport("termb.dll")]
        public static extern int GetTempDir(StringBuilder path, int cbPath);//获取文件生成临时目录

        [DllImport("termb.dll")]
        public static extern void GetPhotoJPGPathName(StringBuilder path, int cbPath);//获取jpg头像全路径名 


        [DllImport("termb.dll")]
        public static extern int SetPhotoJPGPathName(string path);//设置jpg头像全路径名

        [DllImport("termb.dll")]
        public static extern int SetCardJPGPathNameV(string path);//设置竖版身份证正反两面图片全路径

        [DllImport("termb.dll")]
        public static extern int GetCardJPGPathNameV(StringBuilder path, int cbPath);//获取竖版身份证正反两面图片全路径

        [DllImport("termb.dll")]
        public static extern int SetCardJPGPathNameH(string path);//设置横版身份证正反两面图片全路径

        [DllImport("termb.dll")]
        public static extern int GetCardJPGPathNameH(StringBuilder path, int cbPath);//获取横版身份证正反两面图片全路径

        [DllImport("termb.dll")]
        public static extern int getName(StringBuilder data, int cbData);//获取姓名

        [DllImport("termb.dll")]
        public static extern int getSex(StringBuilder data, int cbData);//获取性别

        [DllImport("termb.dll")]
        public static extern int getNation(StringBuilder data, int cbData);//获取民族

        [DllImport("termb.dll")]
        public static extern int getBirthdate(StringBuilder data, int cbData);//获取生日(YYYYMMDD)

        [DllImport("termb.dll")]
        public static extern int getAddress(StringBuilder data, int cbData);//获取地址

        [DllImport("termb.dll")]
        public static extern int getIDNum(StringBuilder data, int cbData);//获取身份证号

        [DllImport("termb.dll")]
        public static extern int getIssue(StringBuilder data, int cbData);//获取签发机关

        [DllImport("termb.dll")]
        public static extern int getEffectedDate(StringBuilder data, int cbData);//获取有效期起始日期(YYYYMMDD)

        [DllImport("termb.dll")]
        public static extern int getExpiredDate(StringBuilder data, int cbData);//获取有效期截止日期(YYYYMMDD) 

        [DllImport("termb.dll")]
        public static extern int getBMPPhotoBase64(StringBuilder data, int cbData);//获取BMP头像Base64编码 

        [DllImport("termb.dll")]
        public static extern int getJPGPhotoBase64(StringBuilder data, int cbData);//获取JPG头像Base64编码

        [DllImport("termb.dll")]
        public static extern int getJPGCardBase64V(StringBuilder data, int cbData);//获取竖版身份证正反两面JPG图像base64编码字符串

        [DllImport("termb.dll")]
        public static extern int getJPGCardBase64H(StringBuilder data, int cbData);//获取横版身份证正反两面JPG图像base64编码字符串

        [DllImport("termb.dll")]
        public static extern int HIDVoice(int nVoice);//语音提示。。仅适用于与带HID语音设备的身份证阅读器（如ID200）

        [DllImport("termb.dll")]
        public static extern int IC_SetDevNum(int iPort, StringBuilder data, int cbdata);//设置发卡器序列号

        [DllImport("termb.dll")]
        public static extern int IC_GetDevNum(int iPort, StringBuilder data, int cbdata);//获取发卡器序列号

        [DllImport("termb.dll")]
        public static extern int IC_GetDevVersion(int iPort, StringBuilder data, int cbdata);//设置发卡器序列号 

        [DllImport("termb.dll")]
        public static extern int IC_WriteData(int iPort, int keyMode, int sector, int idx, StringBuilder key, StringBuilder data, int cbdata, ref uint snr);//写数据

        [DllImport("termb.dll")]
        public static extern int IC_ReadData(int iPort, int keyMode, int sector, int idx, StringBuilder key, StringBuilder data, int cbdata, ref uint snr);//du数据

        [DllImport("termb.dll")]
        public static extern int IC_GetICSnr(int iPort, ref uint snr);//读IC卡物理卡号 

        [DllImport("termb.dll")]
        public static extern int IC_GetIDSnr(int iPort, StringBuilder data, int cbdata);//读身份证物理卡号 

        [DllImport("termb.dll")]
        public static extern int getEnName(StringBuilder data, int cbdata);//获取英文名

        [DllImport("termb.dll")]
        public static extern int getCnName(StringBuilder data, int cbdata);//获取中文名 

        [DllImport("termb.dll")]
        public static extern int getPassNum(StringBuilder data, int cbdata);//获取港澳台居通行证号码

        [DllImport("termb.dll")]
        public static extern int getVisaTimes();//获取签发次数

        [DllImport("termb.dll")]
        public static extern int IC_ChangeSectorKey(int iPort, int keyMode, int nSector, StringBuilder oldKey, StringBuilder newKey);
    }
}
