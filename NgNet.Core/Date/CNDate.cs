﻿using System;

namespace NgNet.Date
{
    /// <summary>
    /// 中国日期
    /// </summary>
    public class CNDate
    {
        #region private fileds
        private DateTime _date;
        private DateTime _datetime;
        private int _cYear;
        private int _cMonth;
        private int _cDay;
        private bool _cIsLeapMonth; //当月是否闰月
        private bool _cIsLeapYear;  //当年是否有闰月
        private int _chineseTwentyFourDay = -1;
        private int _lastChineseTwentyFourDay;
        private int _nextChineseTwentyFourDay;
        private DateTime _lastChineseTwentyFourDate;
        private DateTime _nextChineseTwentyFourDate;
        #endregion

        #region  private base data
        #region 基本常量
        private const int MinYear = 1900;
        private const int MaxYear = 2050;
        private static DateTime MinDay = new DateTime(1900, 1, 30);
        private static DateTime MaxDay = new DateTime(2049, 12, 31);
        private const int StandardGanZhiYear = 1864; //干支基准年： 甲子年
        private static DateTime StandardGanZhiDay = new DateTime(1899, 12, 22);//干支基准日
        private const int StandardAnimalYear = 1900; //1900年为鼠年
        private static DateTime ChineseConstellationReferDay = new DateTime(2007, 9, 13);//28星宿参考值,本日为角
        #endregion

        #region 阴历数据
        /// <summary>
        /// 来源于网上的农历数据
        /// </summary>
        /// <remarks>
        /// 数据结构如下，共使用17位数据
        /// 第17位：表示闰月天数，0表示29天   1表示30天
        /// 第16位-第5位（共12位）表示12个月，其中第16位表示第一月，如果该月为30天则为1，29天为0
        /// 第4位-第1位（共4位）表示闰月是哪个月，如果当年没有闰月，则置0
        ///</remarks>
        private static int[] LunarDateArray = new int[]{
                0x04BD8,0x04AE0,0x0A570,0x054D5,0x0D260,0x0D950,0x16554,0x056A0,0x09AD0,0x055D2,
                0x04AE0,0x0A5B6,0x0A4D0,0x0D250,0x1D255,0x0B540,0x0D6A0,0x0ADA2,0x095B0,0x14977,
                0x04970,0x0A4B0,0x0B4B5,0x06A50,0x06D40,0x1AB54,0x02B60,0x09570,0x052F2,0x04970,
                0x06566,0x0D4A0,0x0EA50,0x06E95,0x05AD0,0x02B60,0x186E3,0x092E0,0x1C8D7,0x0C950,
                0x0D4A0,0x1D8A6,0x0B550,0x056A0,0x1A5B4,0x025D0,0x092D0,0x0D2B2,0x0A950,0x0B557,
                0x06CA0,0x0B550,0x15355,0x04DA0,0x0A5B0,0x14573,0x052B0,0x0A9A8,0x0E950,0x06AA0,
                0x0AEA6,0x0AB50,0x04B60,0x0AAE4,0x0A570,0x05260,0x0F263,0x0D950,0x05B57,0x056A0,
                0x096D0,0x04DD5,0x04AD0,0x0A4D0,0x0D4D4,0x0D250,0x0D558,0x0B540,0x0B6A0,0x195A6,
                0x095B0,0x049B0,0x0A974,0x0A4B0,0x0B27A,0x06A50,0x06D40,0x0AF46,0x0AB60,0x09570,
                0x04AF5,0x04970,0x064B0,0x074A3,0x0EA50,0x06B58,0x055C0,0x0AB60,0x096D5,0x092E0,
                0x0C960,0x0D954,0x0D4A0,0x0DA50,0x07552,0x056A0,0x0ABB7,0x025D0,0x092D0,0x0CAB5,
                0x0A950,0x0B4A0,0x0BAA4,0x0AD50,0x055D9,0x04BA0,0x0A5B0,0x15176,0x052B0,0x0A930,
                0x07954,0x06AA0,0x0AD50,0x05B52,0x04B60,0x0A6E6,0x0A4E0,0x0D260,0x0EA65,0x0D530,
                0x05AA0,0x076A3,0x096D0,0x04BD7,0x04AD0,0x0A4D0,0x1D0B6,0x0D250,0x0D520,0x0DD45,
                0x0B5A0,0x056D0,0x055B2,0x049B0,0x0A577,0x0A4B0,0x0AA50,0x1B255,0x06D20,0x0ADA0,
                0x14B63
                };

        #endregion

        #region 星座名称
        private static string[] constellationsName =
                {
                    "白羊座", "金牛座", "双子座",
                    "巨蟹座", "狮子座", "处女座",
                    "天秤座", "天蝎座", "射手座",
                    "摩羯座", "水瓶座", "双鱼座"
                };
        #endregion

        #region 二十八星宿
        private static string[] chineseConstellationsName =
            {
                 //  四       五       六       日       一       二       三  
                "角木蛟","亢金龙","女土蝠","房日兔","心月狐","尾火虎","箕水豹",
                "斗木獬","牛金牛","氐土貉","虚日鼠","危月燕","室火猪","壁水獝",
                "奎木狼","娄金狗","胃土彘","昴日鸡","毕月乌","觜火猴","参水猿",
                "井木犴","鬼金羊","柳土獐","星日马","张月鹿","翼火蛇","轸水蚓"
            };
        #endregion

        #region 二十四节气
        private static string[] solarTerm = new string[] { "小寒", "大寒", "立春", "雨水", "惊蛰", "春分", "清明", "谷雨", "立夏", "小满", "芒种", "夏至", "小暑", "大暑", "立秋", "处暑", "白露", "秋分", "寒露", "霜降", "立冬", "小雪", "大雪", "冬至" };
        private static int[] sTermInfo = new int[] { 0, 21208, 42467, 63836, 85337, 107014, 128867, 150921, 173149, 195551, 218072, 240693, 263343, 285989, 308563, 331033, 353350, 375494, 397447, 419210, 440795, 462224, 483532, 504758 };
        #endregion

        #region 农历相关数据
        private const string ganStr = "甲乙丙丁戊己庚辛壬癸";
        private const string zhiStr = "子丑寅卯辰巳午未申酉戌亥";
        private const string animalStr = "鼠牛虎兔龙蛇马羊猴鸡狗猪";
        private const string dayStr1 = "日一二三四五六七八九";
        private const string dayStr2 = "初十廿卅";
        private static string[] monthStr = { "出错", "正月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "腊月" };
        #endregion

        #region 按公历计算的节日
        private static SolarHoliday[] sHolidayInfo = new SolarHoliday[]{
            new SolarHoliday(1, 1, 1, "元旦"),
            new SolarHoliday(2, 2, 0, "世界湿地日"),
            new SolarHoliday(2, 10, 0, "国际气象节"),
            new SolarHoliday(2, 14, 0, "情人节"),
            new SolarHoliday(3, 1, 0, "国际海豹日"),
            new SolarHoliday(3, 5, 0, "学雷锋纪念日"),
            new SolarHoliday(3, 8, 0, "妇女节"),
            new SolarHoliday(3, 12, 0, "植树节 孙中山逝世纪念日"),
            new SolarHoliday(3, 14, 0, "国际警察日" ),
            new SolarHoliday(3, 15, 0, "消费者权益日"),
            new SolarHoliday(3, 17, 0, "中国国医节 国际航海日"),
            new SolarHoliday(3, 21, 0, "世界森林日 消除种族歧视国际日 世界儿歌日"),
            new SolarHoliday(3, 22, 0, "世界水日"),
            new SolarHoliday(3, 24, 0, "世界防治结核病日"),
            new SolarHoliday(4, 1, 0, "愚人节"),
            new SolarHoliday(4, 5, 0, "清明节"),
            new SolarHoliday(4, 7, 0, "世界卫生日"),
            new SolarHoliday(4, 22, 0, "世界地球日"),
            new SolarHoliday(5, 1, 1, "劳动节"),
            new SolarHoliday(5, 2, 1, "劳动节假日"),
            new SolarHoliday(5, 3, 1, "劳动节假日"),
            new SolarHoliday(5, 4, 0, "青年节"),
            new SolarHoliday(5, 8, 0, "世界红十字日"),
            new SolarHoliday(5, 12, 0, "国际护士节"),
            new SolarHoliday(5, 31, 0, "世界无烟日"),
            new SolarHoliday(6, 1, 0, "国际儿童节"),
            new SolarHoliday(6, 5, 0, "世界环境保护日"),
            new SolarHoliday(6, 26, 0, "国际禁毒日"),
            new SolarHoliday(7, 1, 0, "建党节 香港回归纪念 世界建筑日"),
            new SolarHoliday(7, 11, 0, "世界人口日"),
            new SolarHoliday(8, 1, 0, "建军节"),
            new SolarHoliday(8, 8, 0, "中国男子节 父亲节"),
            new SolarHoliday(8, 15, 0, "抗日战争胜利纪念"),
            new SolarHoliday(9, 9, 0, "毛主席逝世纪念"),
            new SolarHoliday(9, 10, 0, "教师节"),
            new SolarHoliday(9, 18, 0, "九·一八事变纪念日"),
            new SolarHoliday(9, 20, 0, "国际爱牙日"),
            new SolarHoliday(9, 27, 0, "世界旅游日"),
            new SolarHoliday(9, 28, 0, "孔子诞辰"),
            new SolarHoliday(10, 1, 1, "国庆节 国际音乐日"),
            new SolarHoliday(10, 2, 1, "国庆节假日"),
            new SolarHoliday(10, 3, 1, "国庆节假日"),
            new SolarHoliday(10, 6, 0, "老人节"),
            new SolarHoliday(10, 24, 0, "联合国日"),
            new SolarHoliday(11, 10, 0, "世界青年节"),
            new SolarHoliday(11, 12, 0, "孙中山诞辰纪念"),
            new SolarHoliday(12, 1, 0, "世界艾滋病日"),
            new SolarHoliday(12, 3, 0, "世界残疾人日"),
            new SolarHoliday(12, 20, 0, "澳门回归纪念"),
            new SolarHoliday(12, 24, 0, "平安夜"),
            new SolarHoliday(12, 25, 0, "圣诞节"),
            new SolarHoliday(12, 26, 0, "毛主席诞辰纪念")
           };
        #endregion

        #region 按农历计算的节日
        private static LunarHoliday[] lHolidayInfo = new LunarHoliday[]{
            new LunarHoliday(1, 1, 1, "春节"),
            new LunarHoliday(1, 15, 0, "元宵节"),
            new LunarHoliday(5, 5, 0, "端午节"),
            new LunarHoliday(7, 7, 0, "七夕情人节"),
            new LunarHoliday(7, 15, 0, "中元节 盂兰盆节"),
            new LunarHoliday(8, 15, 0, "中秋节"),
            new LunarHoliday(9, 9, 0, "重阳节"),
            new LunarHoliday(12, 8, 0, "腊八节"),
            new LunarHoliday(12, 23, 0, "北方小年(扫房)"),
            new LunarHoliday(12, 24, 0, "南方小年(掸尘)"),
            //new LunarHolidayStruct(12, 30, 0, "除夕")  //注意除夕需要其它方法进行计算
        };
        #endregion

        #region 按某月第几个星期几的节日
        private static WeekHoliday[] wHolidayInfo = new WeekHoliday[]{
            new WeekHoliday(5, 2, 1, "母亲节"),
            new WeekHoliday(5, 3, 1, "全国助残日"),
            new WeekHoliday(6, 3, 1, "父亲节"),
            new WeekHoliday(9, 3, 3, "国际和平日"),
            new WeekHoliday(9, 4, 1, "国际聋人节"),
            new WeekHoliday(10, 1, 2, "国际住房日"),
            new WeekHoliday(10, 1, 4, "国际减轻自然灾害日"),
            new WeekHoliday(11, 4, 5, "感恩节")
        };
        #endregion
        #endregion

        #region constructor
        #region 公历日期初始化
        /// <summary>
        /// 用一个标准的公历日期来初使化
        /// </summary>
        public CNDate(DateTime dt)
        {
            Reset(dt);
        }
        #endregion

        #region 农历日期初始化
        /// <summary>
        /// 用农历的日期来初使化
        /// </summary>
        /// <param name="cnYear">农历年</param>
        /// <param name="cnMonth">农历月</param>
        /// <param name="cnDay">农历日</param>
        /// <param name="LeapFlag">闰月标志</param>
        public CNDate(int cnYear, int cnMonth, int cnDay)
        {
            Reset(cnYear, cnMonth, cnDay);
        }
        #endregion
        #endregion

        #region private methods

        /// <summary>
        /// 检查公历日期是否符合要求，以及是否在本程序支持的范围内
        /// </summary>
        private void checkDateLimit(DateTime dt)
        {
            if ((dt < MinDay) || (dt > MaxDay))
            {
                throw new ArgumentOutOfRangeException("超出可转换的日期");
            }
        }

        /// <summary>
        /// 检查农历日期是否合理
        /// </summary>
        private void checkLunarDateLimit(int year, int month, int day)
        {
            if ((year < MinYear) || (year > MaxYear))
            {
                throw new ArgumentOutOfRangeException("农历日期超出范围");
            }
            if ((month < 1) || (month > 12))
            {
                throw new ArgumentException("非法农历日期");
            }
            if ((day < 1) || (day > 30)) //中国的月最多30天
            {
                throw new ArgumentException("非法农历日期");
            }
        }

        /// <summary>
        /// 测试某位是否为真
        /// </summary>
        private static bool bitTest32(int num, int bitpostion)
        {
            if (bitpostion > 31 || bitpostion < 0)
                throw new Exception("Error Param: bitpostion[0-31]:" + bitpostion.ToString());
            int bit = 1 << bitpostion;
            return (num & bit) != 0;
        }

        /// <summary>
        /// 将星期几转成数字表示
        /// </summary>
        private int dayOfWeekToInt(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return 1;
                case DayOfWeek.Monday:
                    return 2;
                case DayOfWeek.Tuesday:
                    return 3;
                case DayOfWeek.Wednesday:
                    return 4;
                case DayOfWeek.Thursday:
                    return 5;
                case DayOfWeek.Friday:
                    return 6;
                case DayOfWeek.Saturday:
                    return 7;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 比较指定的日期是不是指定的第几周的第几天
        /// </summary>
        private bool dateIsWeekdayOfMonth(DateTime date, int month, int week, int day)
        {
            bool ret = false;

            if (date.Month == month) //月份相同
            {
                if (dayOfWeekToInt(date.DayOfWeek) == day) //星期几相同
                {
                    DateTime firstDay = new DateTime(date.Year, date.Month, 1);//生成当月第一天
                    int i = dayOfWeekToInt(firstDay.DayOfWeek);
                    int firWeekDays = 7 - dayOfWeekToInt(firstDay.DayOfWeek) + 1; //计算第一周剩余天数

                    if (i > day)
                    {
                        if ((week - 1) * 7 + day + firWeekDays == date.Day)
                        {
                            ret = true;
                        }
                    }
                    else
                    {
                        if (day + firWeekDays + (week - 2) * 7 == date.Day)
                        {
                            ret = true;
                        }
                    }
                }
            }

            return ret;
        }

        private void initChineseTwentyFourDay()
        {
            DateTime baseDateAndTime = new DateTime(1900, 1, 6, 2, 5, 0); //#1/6/1900 2:05:00 AM#
            DateTime newDate;
            double num;
            int y = this._date.Year;
            for (int i = 1; i <= 24; i++)
            {
                num = 525948.76 * (y - 1900) + sTermInfo[i - 1];
                newDate = baseDateAndTime.AddMinutes(num);//按分钟计算
                if (newDate.DayOfYear == _date.DayOfYear)
                {
                    _chineseTwentyFourDay = i - 1;
                    break;
                }
            }


            newDate = new DateTime();
            for (int i = 24; i >= 1; i--)
            {
                num = 525948.76 * (y - 1900) + sTermInfo[i - 1];

                newDate = baseDateAndTime.AddMinutes(num);//按分钟计算

                if (newDate.DayOfYear < _date.DayOfYear)
                {
                    _lastChineseTwentyFourDay = i - 1;
                    _lastChineseTwentyFourDate = newDate;
                    break;
                }
            }

            newDate = new DateTime();
            for (int i = 1; i <= 24; i++)
            {
                num = 525948.76 * (y - 1900) + sTermInfo[i - 1];

                newDate = baseDateAndTime.AddMinutes(num);//按分钟计算

                if (newDate.DayOfYear > _date.DayOfYear)
                {
                    _nextChineseTwentyFourDay = i - 1;
                    _nextChineseTwentyFourDate = newDate;
                    break;
                }
            }
        }

        private int getGanZhiYearIndex()
        {
            int i = (this._cYear - StandardGanZhiYear) % 60;
            if (i < 0)
                i += 60;
            return i;
        }
        #endregion

        #region public static methods
        /// <summary>
        /// 传回公历y年m月的总天数
        /// </summary>
        public static int GetSolarDaysByMonth(int y, int m)
        {
            return DateTime.DaysInMonth(y, m);
        }

        /// <summary>
        /// 根据日期值获得周一的日期
        /// </summary>
        /// <param name="dt">输入日期</param>
        /// <returns>周一的日期</returns>
        public static DateTime GetMondayDateByDate(DateTime dt)
        {
            double d = 0;
            switch ((int)dt.DayOfWeek)
            {
                //case 1: d = 0; break;
                case 2: d = -1; break;
                case 3: d = -2; break;
                case 4: d = -3; break;
                case 5: d = -4; break;
                case 6: d = -5; break;
                case 0: d = -6; break;
            }
            return dt.AddDays(d);
        }

        /// <summary>
        /// //传回农历y年m月的总天数
        /// </summary>
        public static int GetLunarDaysByMonth(int year, int month)
        {
            if (bitTest32((LunarDateArray[year - MinYear] & 0x0000FFFF), (16 - month)))
                return 30;
            else
                return 29;
        }

        /// <summary>
        /// 传回农历 y年闰哪个月 1-12 , 没闰传回 0
        /// </summary>
        public static int GetLunarLeapMonth(int year)
        {
            return LunarDateArray[year - MinYear] & 0xF;
        }

        /// <summary>
        /// 传回农历y年闰月的天数
        /// </summary>
        public static int GetLunarLeapMonthDays(int year)
        {
            if (GetLunarLeapMonth(year) != 0)
            {
                if ((LunarDateArray[year - MinYear] & 0x10000) != 0)
                {
                    return 30;
                }
                else
                {
                    return 29;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 取农历年一年的天数
        /// </summary>
        public static int GetLunarYearDays(int year)
        {
            int i, f, sumDay, info;

            sumDay = 348; //29天*12个月
            i = 0x8000;
            info = LunarDateArray[year - MinYear] & 0x0FFFF;

            //计算12个月中有多少天为30天
            for (int m = 0; m < 12; m++)
            {
                f = info & i;
                if (f != 0)
                {
                    sumDay++;
                }
                i = i >> 1;
            }
            return sumDay + GetLunarLeapMonthDays(year);
        }

        /// <summary>
        /// 获得当前时间的时辰
        /// </summary> 
        public static string GetChineseHour(DateTime dt)
        {
            int _hour, _minute, offset, i;
            int indexGan;
            string tmpGan;

            //计算时辰的地支
            _hour = dt.Hour;    //获得当前时间小时
            _minute = dt.Minute;  //获得当前时间分钟

            if (_minute != 0) _hour += 1;
            offset = _hour / 2;
            if (offset >= 12) offset = 0;
            //zhiHour = zhiStr[offset].ToString();

            //计算天干
            TimeSpan ts = dt - StandardGanZhiDay;
            i = ts.Days % 60;

            //ganStr[i % 10] 为日的天干,(n*2-1) %10得出地支对应,n从1开始
            indexGan = ((i % 10 + 1) * 2 - 1) % 10 - 1;

            tmpGan = ganStr.Substring(indexGan) + ganStr.Substring(0, indexGan + 2);//凑齐12位
            //ganHour = ganStr[((i % 10 + 1) * 2 - 1) % 10 - 1].ToString();

            return tmpGan[offset].ToString() + zhiStr[offset].ToString();
        }
        #endregion

        #region  public properties
        #region 节日
        /// <summary>
        /// 计算中国农历节日
        /// </summary>
        public string LunarHoliday
        {
            get
            {
                string tempStr = "";
                if (_cIsLeapMonth == false) //闰月不计算节日
                {
                    foreach (LunarHoliday lh in lHolidayInfo)
                    {
                        if ((lh.Month == this._cMonth) && (lh.Day == this._cDay))
                        {

                            tempStr = lh.HolidayName;
                            break;

                        }
                    }

                    //对除夕进行特别处理
                    if (this._cMonth == 12)
                    {
                        int i = GetLunarDaysByMonth(this._cYear, 12); //计算当年农历12月的总天数
                        if (this._cDay == i) //如果为最后一天
                        {
                            tempStr = "除夕";
                        }
                    }
                }
                return tempStr;
            }
        }

        /// <summary>
        /// 按某月第几周第几日计算的节日
        /// </summary>
        public string WeekDayHoliday
        {
            get
            {
                string tempStr = "";
                foreach (WeekHoliday wh in wHolidayInfo)
                {
                    if (dateIsWeekdayOfMonth(_date, wh.Month, wh.WeekAtMonth, wh.WeekDay))
                    {
                        tempStr = wh.HolidayName;
                        break;
                    }
                }
                return tempStr;
            }
        }

        /// <summary>
        /// 按公历日计算的节日
        /// </summary>
        public string DateHoliday
        {
            get
            {
                string tempStr = "";

                foreach (SolarHoliday sh in sHolidayInfo)
                {
                    if ((sh.Month == _date.Month) && (sh.Day == _date.Day))
                    {
                        tempStr = sh.HolidayName;
                        break;
                    }
                }
                return tempStr;
            }
        }

        /// <summary>
        /// 获取下一个节日
        /// </summary>
        /// <param name="dt">输出节日的公历日期</param>
        /// <returns></returns>
        public string NextHolidays(out DateTime dt)
        {
            string tempStr1 = "";//, tempStr2 = "", tempStr3 = "";
            dt = new DateTime();
            foreach (SolarHoliday sh in sHolidayInfo)
            {
                if (
                    ((sh.Month == _date.Month) && (sh.Day > _date.Day))
                    || sh.Month > _date.Month
                    )
                {
                    tempStr1 = sh.HolidayName;
                    dt = new DateTime(_date.Year, sh.Month, sh.Day);
                    break;
                }
            }
            if (string.IsNullOrWhiteSpace(tempStr1))
            {
                tempStr1 = sHolidayInfo[0].HolidayName;
                dt = new DateTime(_date.Year + 1, sHolidayInfo[0].Month, sHolidayInfo[0].Day);
            }
            return tempStr1;
        }
        #endregion

        #region 公历日期
        /// <summary>
        /// 取对应的公历日期
        /// </summary>
        public DateTime Date
        {
            get { return _date; }
        }

        /// <summary>
        /// 取星期几
        /// </summary>
        public DayOfWeek WeekDay
        {
            get { return _date.DayOfWeek; }
        }

        /// <summary>
        /// 周几的字符
        /// </summary>
        public string WeekDayString
        {
            get
            {
                switch (_date.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        return "星期日";
                    case DayOfWeek.Monday:
                        return "星期一";
                    case DayOfWeek.Tuesday:
                        return "星期二";
                    case DayOfWeek.Wednesday:
                        return "星期三";
                    case DayOfWeek.Thursday:
                        return "星期四";
                    case DayOfWeek.Friday:
                        return "星期五";
                    case DayOfWeek.Saturday:
                        return "星期六";
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 公历日期中文表示法 如一九九七年七月一日
        /// </summary>
        public string DateString
        {
            get
            {
                return this._date.ToLongDateString();
            }
        }

        /// <summary>
        /// 当前是否公历闰年
        /// </summary>
        public bool IsLeapYear
        {
            get
            {
                return DateTime.IsLeapYear(this._date.Year);
            }
        }

        /// <summary>
        /// 28星宿计算
        /// </summary>
        public string ChineseConstellation
        {
            get
            {
                int offset = 0;
                int modStarDay = 0;

                TimeSpan ts = this._date - ChineseConstellationReferDay;
                offset = ts.Days;
                modStarDay = offset % 28;
                if (modStarDay < 0)
                    modStarDay += 28;
                return (modStarDay >= 0 ? chineseConstellationsName[modStarDay] : chineseConstellationsName[27 + modStarDay]);
            }
        }
        #endregion

        #region ChineseTwentyFourDay(Solar Date)   
        /// <summary>
        /// 定气法计算二十四节气,二十四节气是按地球公转来计算的，并非是阴历计算的
        /// </summary>
        /// <remarks>
        /// 节气的定法有两种。古代历法采用的称为"恒气"，即按时间把一年等分为24份，
        /// 每一节气平均得15天有余，所以又称"平气"。现代农历采用的称为"定气"，即
        /// 按地球在轨道上的位置为标准，一周360°，两节气之间相隔15°。由于冬至时地
        /// 球位于近日点附近，运动速度较快，因而太阳在黄道上移动15°的时间不到15天。
        /// 夏至前后的情况正好相反，太阳在黄道上移动较慢，一个节气达16天之多。采用
        /// 定气时可以保证春、秋两分必然在昼夜平分的那两天。
        /// </remarks>
        public string ChineseTwentyFourDayString
        {
            get
            {
                if (_chineseTwentyFourDay == -1)
                    return string.Empty;
                return solarTerm[_chineseTwentyFourDay];
            }
        }

        /// <summary>
        /// 当前日期前一个最近节气名称
        /// </summary>
        public string LastChineseTwentyFourDayString
        {
            get
            {
                return solarTerm[_lastChineseTwentyFourDay];
            }
        }

        /// <summary>
        /// 当前日期后一个最近节气名称
        /// </summary>
        public string NextChineseTwentyFourDayString
        {
            get
            {
                return solarTerm[_nextChineseTwentyFourDay];
            }

        }

        /// <summary>
        /// 上一个二十四节气名称
        /// </summary>
        /// <param name="dayIndex"> 输出二十四节气名称的索引</param>
        /// <param name="dt">输出二十四节气的公历时间</param>
        /// <returns>返回二十四节气的名称</returns>
        public string LastChineseTwentyFourDay(out int dayIndex, out DateTime dt)
        {
            dayIndex = _lastChineseTwentyFourDay;
            dt = _lastChineseTwentyFourDate;
            return LastChineseTwentyFourDayString;
        }

        /// <summary>
        /// 上一个二十四节气名称
        /// </summary>
        /// <param name="dayIndex"> 输出二十四节气名称的索引</param>
        /// <param name="dt">输出二十四节气的公历时间</param>
        /// <returns>返回二十四节气的名称</returns>
        public string NextChineseTwentyFourDay(out int dayIndex, out DateTime dt)
        {
            dayIndex = _nextChineseTwentyFourDay;
            dt = _nextChineseTwentyFourDate;
            return NextChineseTwentyFourDayString;
        }
        #endregion

        #region 农历日期
        /// <summary>
        /// 是否闰月
        /// </summary>
        public bool IsLunarLeapMonth
        {
            get { return this._cIsLeapMonth; }
        }

        /// <summary>
        /// 当年是否有闰月
        /// </summary>
        public bool IsLunarLeapYear
        {
            get
            {
                return this._cIsLeapYear;
            }
        }

        /// <summary>
        /// 农历日
        /// </summary>
        public int LunarDay
        {
            get { return this._cDay; }
        }

        /// <summary>
        /// 农历日中文表示
        /// </summary>
        public string LunarDayString
        {
            get
            {
                switch (this._cDay)
                {
                    case 0:
                        return "";
                    case 10:
                        return "初十";
                    case 20:
                        return "二十";
                    case 30:
                        return "三十";
                    default:
                        return dayStr2[(int)(_cDay / 10)].ToString() + dayStr1[_cDay % 10].ToString();

                }
            }
        }

        /// <summary>
        /// 农历的月份
        /// </summary>
        public int LunarMonth
        {
            get { return this._cMonth; }
        }

        /// <summary>
        /// 农历月份字符串
        /// </summary>
        public string LunarMonthString
        {
            get
            {
                return monthStr[this._cMonth];
            }
        }

        /// <summary>
        /// 取农历年份
        /// </summary>
        public int LunarYear
        {
            get { return this._cYear; }
        }

        /// <summary>
        /// 取农历年字符串如，一九九七年
        /// </summary>
        public string LunarYearString
        {
            get
            {
                return string.Format("{0}年", ConvertHelper.ToChineseFigures(_cYear));
            }
        }

        /// <summary>
        /// 取农历日期表示法：农历一九九七年正月初五
        /// </summary>
        public string LunarDateString
        {
            get
            {
                if (this._cIsLeapMonth == true)
                {
                    return LunarYearString + "闰" + LunarMonthString + LunarDayString;
                }
                else
                {
                    return LunarYearString + LunarMonthString + LunarDayString;
                }
            }
        }
        #endregion

        #region 星座(Solar Date)   
        /// <summary>
        /// 计算指定日期的星座序号 
        /// </summary>
        public string Constellation
        {
            get
            {
                int index = 0;
                int y, m, d;
                y = _date.Year;
                m = _date.Month;
                d = _date.Day;
                y = m * 100 + d;

                if (((y >= 321) && (y <= 419))) { index = 0; }
                else if ((y >= 420) && (y <= 520)) { index = 1; }
                else if ((y >= 521) && (y <= 620)) { index = 2; }
                else if ((y >= 621) && (y <= 722)) { index = 3; }
                else if ((y >= 723) && (y <= 822)) { index = 4; }
                else if ((y >= 823) && (y <= 922)) { index = 5; }
                else if ((y >= 923) && (y <= 1022)) { index = 6; }
                else if ((y >= 1023) && (y <= 1121)) { index = 7; }
                else if ((y >= 1122) && (y <= 1221)) { index = 8; }
                else if ((y >= 1222) || (y <= 119)) { index = 9; }
                else if ((y >= 120) && (y <= 218)) { index = 10; }
                else if ((y >= 219) && (y <= 320)) { index = 11; }
                else { index = 0; }

                return constellationsName[index];
            }
        }
        #endregion

        #region 属相(Lunar Date)
        /// <summary>
        /// 计算属相的索引，注意虽然属相是以农历年来区别的，但是目前在实际使用中是按公历来计算的
        /// 鼠年为1,其它类推
        /// </summary>
        public int Animal
        {
            get
            {
                int offset = (this._cYear - StandardAnimalYear) % 12;
                if (offset < 0)
                    offset += 12;
                return offset + 1;
            }
        }

        /// <summary>
        /// 取属相字符串
        /// </summary>
        public string AnimalString
        {
            get
            {
                return animalStr[Animal - 1].ToString();
            }
        }
        #endregion

        #region 天干地支
        /// <summary>
        /// 取农历年的干支表示法如 乙丑年
        /// </summary>
        public string GanZhiYearString
        {
            get
            {
                //一周期中的第几年
                int i = getGanZhiYearIndex();
                return ganStr[i % 10].ToString() + zhiStr[i % 12].ToString() + "年";
            }
        }

        /// <summary>
        /// 取干支的月表示字符串，注意农历的闰月不记干支
        /// </summary>
        public string GanZhiMonthString
        {
            get
            {
                //每个月的地支总是固定的,而且总是从寅月开始
                int zhiIndex;
                string zhi;
                if (this._cMonth > 10)
                {
                    zhiIndex = this._cMonth - 10;
                }
                else
                {
                    zhiIndex = this._cMonth + 2;
                }
                zhi = zhiStr[zhiIndex - 1].ToString();

                //根据当年的干支年的干来计算月干的第一个
                int ganIndex = 1;
                string gan;
                int i = getGanZhiYearIndex();
                switch (i % 10)
                {
                    #region ...
                    case 0: //甲
                        ganIndex = 3;
                        break;
                    case 1: //乙
                        ganIndex = 5;
                        break;
                    case 2: //丙
                        ganIndex = 7;
                        break;
                    case 3: //丁
                        ganIndex = 9;
                        break;
                    case 4: //戊
                        ganIndex = 1;
                        break;
                    case 5: //己
                        ganIndex = 3;
                        break;
                    case 6: //庚
                        ganIndex = 5;
                        break;
                    case 7: //辛
                        ganIndex = 7;
                        break;
                    case 8: //壬
                        ganIndex = 9;
                        break;
                    case 9: //癸
                        ganIndex = 1;
                        break;
                        #endregion
                }
                gan = ganStr[(ganIndex + this._cMonth - 2) % 10].ToString();

                return gan + zhi + "月";
            }
        }

        /// <summary>
        /// 取干支日表示法
        /// </summary>
        public string GanZhiDayString
        {
            get
            {
                int i, offset;
                TimeSpan ts = this._date - StandardGanZhiDay;
                offset = ts.Days;
                i = offset % 60;
                if (i < 0)
                    i += 60;
                return ganStr[i % 10].ToString() + zhiStr[i % 12].ToString() + "日";
            }
        }

        /// <summary>
        /// 取当前日期的干支表示法如 甲子年乙丑月丙庚日
        /// </summary>
        public string GanZhiDateString
        {
            get
            {
                return GanZhiYearString + GanZhiMonthString + GanZhiDayString;
            }
        }

        /// <summary>
        /// 时辰
        /// </summary>
        public string ChineseHour
        {
            get
            {
                return GetChineseHour(_datetime);
            }
        }
        #endregion
        #endregion

        #region init
        public void Reset(DateTime dt)
        {
            int i;
            int leap;
            int tmp;
            int offset;

            checkDateLimit(dt);

            _date = dt.Date;
            _datetime = dt;

            //农历日期计算部分
            leap = 0;
            tmp = 0;

            //计算两天的基本差距
            TimeSpan ts = _date - CNDate.MinDay;
            offset = ts.Days;

            for (i = MinYear; i <= MaxYear; i++)
            {
                //求当年农历年天数
                tmp = GetLunarYearDays(i);
                if (offset - tmp < 1)
                    break;
                else
                {
                    offset -= tmp;
                }
            }
            _cYear = i;

            //计算该年闰哪个月
            leap = GetLunarLeapMonth(_cYear);

            //设定当年是否有闰月
            if (leap > 0)
            {
                _cIsLeapYear = true;
            }
            else
            {
                _cIsLeapYear = false;
            }

            _cIsLeapMonth = false;
            for (i = 1; i <= 12; i++)
            {
                //闰月
                if ((leap > 0) && (i == leap + 1) && (_cIsLeapMonth == false))
                {
                    _cIsLeapMonth = true;
                    i--;
                    tmp = GetLunarLeapMonthDays(_cYear); //计算闰月天数
                }
                else
                {
                    _cIsLeapMonth = false;
                    tmp = GetLunarDaysByMonth(_cYear, i);  //计算非闰月天数
                }

                offset -= tmp;
                if (offset <= 0) break;
            }

            offset += tmp;
            _cMonth = i;
            _cDay = offset;
            initChineseTwentyFourDay();
        }

        /// <summary>
        /// 农历初始化
        /// </summary>
        /// <param name="cy"></param>
        /// <param name="cm"></param>
        /// <param name="cd"></param>
        public void Reset(int cy, int cm, int cd)
        {
            int i, leap, offset;

            checkLunarDateLimit(cy, cm, cd);

            _cYear = cy;
            _cMonth = cm;
            _cDay = cd;

            offset = 0;

            //计算参照年到当前年（不包过本年的天数）
            for (i = MinYear; i < cy; i++)
            {
                offset += GetLunarYearDays(i);
            }

            //计算该年应该闰哪个月
            leap = GetLunarLeapMonth(cy);

            _cIsLeapYear = leap != 0;

            //当年没有闰月||输入月份小于闰月
            if ((!_cIsLeapYear) || (cm < leap))
            {
                //计算当前农历年已过的天数
                for (i = 1; i < cm; i++)
                {
                    offset += GetLunarDaysByMonth(cy, i);
                }

                //检查日期是否大于最大天
                if (cd > GetLunarDaysByMonth(cy, cm))
                    throw new Exception("不合法的农历日期");
                //加上当月的天数
                offset += cd;
            }

            //是闰年&&计算月份大于或等于闰月
            else
            {
                //计算当前农历年已过的天数
                //
                for (i = 1; i < cm; i++)
                {
                    offset += GetLunarDaysByMonth(cy, i);
                }

                //计算月大于闰月
                if (cm > leap)
                {
                    offset += GetLunarLeapMonthDays(cy);         //加上闰月天数

                    if (cd > GetLunarDaysByMonth(cy, cm))
                        throw new Exception("不合法的农历日期");

                    offset += cd;
                }

                //计算月等于闰月
                else
                {
                    //如果输入的是闰月，则应首先加上与闰月对应的普通月的天数
                    if (_cIsLeapMonth)
                    {
                        offset += GetLunarDaysByMonth(cy, cm);
                    }

                    if (cd > GetLunarLeapMonthDays(cy))
                        throw new Exception("不合法的农历日期");

                    offset += cd;
                }
            }
            _date = MinDay.AddDays(offset);
            initChineseTwentyFourDay();
        }
        #endregion

        #region static public methods & properties
        public static CNDate Now
        {
            get
            {
                return new CNDate(DateTime.Now);
            }
        }
        #endregion
    }
}
