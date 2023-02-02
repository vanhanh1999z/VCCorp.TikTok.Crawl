using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VCCorp.TikTokCrawler.Common
{
    internal class DateTimeFormatAgain
    {
        /// <summary>
        /// Thuộc tính chứa thông điệp lỗi
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Tìm giờ phút trong 1 đoạn text
        /// </summary>
        /// <param name="strDate"></param>
        /// <returns></returns>
        public string GetHour(string strDate)
        {
            string time = "";

            bool regTime = Regex.IsMatch(strDate, @"([\d]{1,2} phút trước)|([\d]{1,2} minute ago)|([\d]{1,2} minutes ago)", RegexOptions.IgnoreCase);
            if (regTime == true)
            {
                try
                {
                    int minute = Convert.ToInt32(Utilities.Regex_GetItemFirst(strDate, @"[\d]{1,2}"));
                    time = DateTime.Now.AddMinutes(-minute).ToString("hh:mm tt");
                }
                catch
                {
                    time = DateTime.Now.ToString("hh:mm tt");
                }

                return time;
            }

            regTime = Regex.IsMatch(strDate, @"([\d]{1,2} tiếng trước)|([\d]{1,2} giờ trước)|([\d]{1,2} hour ago)|([\d]{1,2} hours ago)|([\d]{1,2} hours trước)", RegexOptions.IgnoreCase);
            if (regTime == true)
            {
                try
                {
                    int hour = Convert.ToInt32(Utilities.Regex_GetItemFirst(strDate, @"[\d]{1,2}"));
                    time = DateTime.Now.AddHours(-hour).ToString("hh:mm tt");
                }
                catch
                {
                    time = DateTime.Now.ToString("hh:mm tt");
                }

                return time;
            }

            // Cách đây 6 giờ
            regTime = Regex.IsMatch(strDate, @"Cách đây [\d]+ giờ", RegexOptions.IgnoreCase);
            if (regTime == true)
            {
                try
                {
                    int hour = Convert.ToInt32(Utilities.Regex_GetItemFirst(strDate, @"[\d]{1,2}"));
                    time = DateTime.Now.AddHours(-hour).ToString("hh:mm tt");
                }
                catch
                {
                    time = DateTime.Now.ToString("hh:mm tt");
                }

                return time;
            }

            // nếu tồn tại dạng này: 3:52:15 CH
            regTime = Regex.IsMatch(strDate, @"[\d]+:[\d]+:[\d]+[\s]CH|[\d]+:[\d]+:[\d]+[\s]SA", RegexOptions.IgnoreCase);
            if (regTime == true)
            {
                if (strDate.ToLower().Contains(" sa"))
                {
                    time = Utilities.Regex_GetItemFirst(strDate, @"[\d]+:[\d]+:[\d]+") + " AM";
                }

                if (strDate.ToLower().Contains(" ch"))
                {
                    time = Utilities.Regex_GetItemFirst(strDate, @"[\d]+:[\d]+:[\d]+") + " PM";
                }
                return time;
            }

            // nếu tồn tại kiểu 8h11 
            regTime = Regex.IsMatch(strDate, @"([\d]+h[\d]+)", RegexOptions.IgnoreCase);
            if (regTime == true)
            {
                time = Utilities.Regex_GetItemFirst(strDate, @"([\d]+h[\d]+)"); // cố gắng dò kiểu 8h11 
                if (!string.IsNullOrEmpty(time))
                {
                    time = time.Replace("h", ":");
                }
            }

            // nếu tồn tại kiểu 3:52:52 am
            regTime = Regex.IsMatch(strDate, @"[\d]+:[\d]+:[\d]+ (AM|PM)", RegexOptions.IgnoreCase);
            if (regTime == true)
            {
                time = Utilities.Regex_GetItemFirst(strDate, @"[\d]+:[\d]+:[\d]+ (AM|PM)"); //3:52:52 am
                if (!string.IsNullOrEmpty(time))
                {
                    bool isAM = time.ToLower().Contains("am");

                    time = Utilities.Regex_GetItemFirst(strDate, @"[\d]{2,}:[\d]{2,}"); // lấy ra giờ, phút

                    string[] arrTime = time.Split(':');
                    if (arrTime.Length == 2)
                    {
                        if (isAM == true) // giờ buổi sáng giữ nguyên
                        {
                            return time;
                        }
                        else // giờ buổi chiều + 12
                        {
                            int hour = Convert.ToInt32(arrTime[0]);
                            if (hour < 13)
                            {
                                hour = hour + 12;
                            }

                            return hour.ToString() + ":" + arrTime[1];
                        }
                    }
                }

            }

            // nếu tồn tại kiểu 28/10/2015 3:52 am - chu y kieu nay dang truoc gio co khoang trang
            regTime = Regex.IsMatch(strDate, @"([\s]+[\d]+:[\d]+)[\s]+(AM|PM)", RegexOptions.IgnoreCase);
            if (regTime == true)
            {
                time = Utilities.Regex_GetItemFirst(strDate, @"([\s]+[\d]+:[\d]+)[\s]+(AM|PM)").Trim();
                // 13:42 PM
                if (!string.IsNullOrEmpty(time))
                {
                    bool isAM = time.ToLower().Contains("am");

                    time = Utilities.Regex_GetItemFirst(strDate, @"[\d]{2,}:[\d]{2,}");
                    string[] arrTime = time.Split(':');
                    if (arrTime.Length == 2)
                    {
                        if (isAM == true) // giờ buổi sáng giữ nguyên
                        {
                            return time;
                        }
                        else // giờ buổi chiều + 12
                        {
                            int hour = Convert.ToInt32(arrTime[0]);
                            if (hour < 13)
                            {
                                hour = hour + 12;
                            }

                            return hour.ToString() + ":" + arrTime[1];
                        }
                    }
                }
            }

            // nếu tồn tại kiểu 3:52am - chu y kieu nay dang truoc gio co khoang trang
            regTime = Regex.IsMatch(strDate.ToUpper(), @"([\s]+[\d]+:[\d]+)(AM|PM)", RegexOptions.IgnoreCase);
            if (regTime == true)
            {
                time = Utilities.Regex_GetItemFirst(strDate, @"([\s]+[\d]+:[\d]+)").Trim(); // 3:52am

                string apm = Utilities.Regex_GetItemFirst(strDate, @"(AM|PM)").Trim();

                time = time + " " + apm;
            }

            // nếu tồn tại kiểu 4:38 Chiều
            regTime = Regex.IsMatch(strDate, @"[\d]+:[\d]+[\s]+(Chiều|Sáng)", RegexOptions.IgnoreCase);
            if (regTime == true)
            {
                time = Utilities.Regex_GetItemFirst(strDate, @"[\d]+:[\d]+[\s]+(Chiều|Sáng)");
                if (time.ToLower().Contains(" chiều"))
                {
                    time = Utilities.Regex_GetItemFirst(time, @"[\d]+:[\d]+") + " PM";
                }

                if (time.ToLower().Contains(" sáng"))
                {
                    time = Utilities.Regex_GetItemFirst(time, @"[\d]+:[\d]+") + " AM";
                }
            }

            //8:46:18
            regTime = Regex.IsMatch(strDate, @"[\d]+:[\d]+:[\d]+", RegexOptions.IgnoreCase);
            if (regTime == true)
            {
                time = Utilities.Regex_GetItemFirst(strDate, @"[\d]+:[\d]+:[\d]+"); // cố gắng dò kiểu 10:03 
                string[] arrTime = time.Split(':');
                if (arrTime.Length == 3)
                {
                    string hour = arrTime[0];
                    if (hour.Length == 1)
                    {
                        hour = "0" + hour;
                    }

                    string mimute = arrTime[1];
                    if (mimute.Length == 1)
                    {
                        mimute = "0" + mimute;
                    }

                    return hour + ":" + mimute;
                }
            }

            // 09:12 AM
            regTime = Regex.IsMatch(strDate, @"[\d]{2,}:[\d]{2,} (am|pm)", RegexOptions.IgnoreCase);
            if (regTime == true)
            {
                time = Utilities.Regex_GetItemFirst(strDate, @"[\d]{2,}:[\d]{2,} (am|pm)"); // cố gắng dò kiểu 09:12 AM
                if (!string.IsNullOrEmpty(time))
                {
                    bool isAM = time.ToLower().Contains("am");

                    time = Utilities.Regex_GetItemFirst(strDate, @"[\d]{2,}:[\d]{2,}");
                    string[] arrTime = time.Split(':');
                    if (arrTime.Length == 2)
                    {
                        if (isAM == true) // giờ buổi sáng giữ nguyên
                        {
                            return time;
                        }
                        else // giờ buổi chiều + 12
                        {
                            int hour = Convert.ToInt32(arrTime[0]);
                            if (hour < 13)
                            {
                                hour = hour + 12;
                            }

                            return hour.ToString() + ":" + arrTime[1];
                        }
                    }
                }
            }

            // [\d]{2,}:[\d]{2,} -> 10:03
            regTime = Regex.IsMatch(strDate, @"[\d]{2,}:[\d]{2,}", RegexOptions.IgnoreCase);
            if (regTime == true)
            {
                time = Utilities.Regex_GetItemFirst(strDate, @"[\d]{2,}:[\d]{2,}"); // cố gắng dò kiểu 10:03 
                return time;
            }

            // nếu tồn tại kiểu 08.11 
            regTime = Regex.IsMatch(strDate, @"[\s]+([\d]{2}\.[\d]+)", RegexOptions.IgnoreCase);
            if (regTime == true)
            {
                time = Utilities.Regex_GetItemFirst(strDate, @"[\s]+([\d]{2}\.[\d]+)"); // cố gắng dò kiểu 8h11 
                if (!string.IsNullOrEmpty(time))
                {
                    time = time.Replace(".", ":");
                }

            }

            if (time == "") // các kiểu rồi mà vẫn chưa tìm ra time thì thử lấy dạng 12:00 xem sao
            {
                time = Utilities.Regex_GetItemFirst(strDate, @"[\d]+:[\d]+");

                // kiem tra them am hay pm
                string am = Utilities.Regex_GetItemFirst(strDate, @"(:[\d]+)([\s]+)(am)"); // chua dung am
                if (am != "")
                {
                    time = time + " AM";
                }

                string pm = Utilities.Regex_GetItemFirst(strDate, @"(:[\d]+)([\s]+)(pm)"); // chua dung am
                if (pm != "")
                {
                    time = time + " PM";
                }
            }

            regTime = Regex.IsMatch(strDate, @"[\d]{6,}");
            if (regTime == true)
            {
                time = UnixTimeStampToHour(strDate);
                if (!string.IsNullOrEmpty(time))
                {
                    return time;
                }
            }

            if (strDate.ToLower().Contains("giờ trước"))
            {
                // một giờ trước
                regTime = Regex.IsMatch(strDate, @"một giờ trước", RegexOptions.IgnoreCase);
                if (regTime == true)
                {
                    try
                    {
                        time = DateTime.Now.AddHours(-1).ToString("hh:mm tt");
                    }
                    catch
                    {
                        time = DateTime.Now.ToString("hh:mm tt");
                    }

                    return time;
                }

                // hai giờ trước
                regTime = Regex.IsMatch(strDate, @"hai giờ trước", RegexOptions.IgnoreCase);
                if (regTime == true)
                {
                    try
                    {
                        time = DateTime.Now.AddHours(-2).ToString("hh:mm tt");
                    }
                    catch
                    {
                        time = DateTime.Now.ToString("hh:mm tt");
                    }

                    return time;
                }

                // ba giờ trước
                regTime = Regex.IsMatch(strDate, @"ba giờ trước", RegexOptions.IgnoreCase);
                if (regTime == true)
                {
                    try
                    {
                        time = DateTime.Now.AddHours(-3).ToString("hh:mm tt");
                    }
                    catch
                    {
                        time = DateTime.Now.ToString("hh:mm tt");
                    }

                    return time;
                }
            }

            if (strDate.ToLower().Contains("phút trước"))
            {
                // một phút trước
                regTime = Regex.IsMatch(strDate, @"một phút trước", RegexOptions.IgnoreCase);
                if (regTime == true)
                {
                    try
                    {
                        time = DateTime.Now.AddMinutes(-1).ToString("hh:mm tt");
                    }
                    catch
                    {
                        time = DateTime.Now.ToString("hh:mm tt");
                    }

                    return time;
                }

                // hai phút trước
                regTime = Regex.IsMatch(strDate, @"hai phút trước", RegexOptions.IgnoreCase);
                if (regTime == true)
                {
                    try
                    {
                        time = DateTime.Now.AddMinutes(-2).ToString("hh:mm tt");
                    }
                    catch
                    {
                        time = DateTime.Now.ToString("hh:mm tt");
                    }

                    return time;
                }

                // ba phút trước
                regTime = Regex.IsMatch(strDate, @"ba phút trước", RegexOptions.IgnoreCase);
                if (regTime == true)
                {
                    try
                    {
                        time = DateTime.Now.AddMinutes(-3).ToString("hh:mm tt");
                    }
                    catch
                    {
                        time = DateTime.Now.ToString("hh:mm tt");
                    }

                    return time;
                }
            }

            if (strDate.ToLower().Contains("giây trước"))
            {
                time = DateTime.Now.ToString("hh:mm tt");
            }

            // chỉ có 1h
            if (Regex.IsMatch(strDate, @"[\d]+h") || Regex.IsMatch(strDate, @"[\d]+ h"))
            {
                int hour = Convert.ToInt32(Utilities.Regex_GetItemFirst(strDate, @"[\d]{1,2}"));
                time = DateTime.Now.AddHours(-hour).ToString("hh:mm tt");
            }

            // chỉ có 1 ngày
            if (Regex.IsMatch(strDate, @"[\d]+ ngày") || Regex.IsMatch(strDate, @"[\d]+ tháng") || Regex.IsMatch(strDate, @"[\d]+ năm"))
            {
                time = DateTime.Now.ToString("hh:mm tt");
            }

            return time;

        }

        /// <summary>
        /// Lấy ngày tháng năm
        /// </summary>
        /// <param name="sDate"></param>
        /// <param name="ruleDate"></param>
        /// <returns>yyyy-MM-dd</returns>
        public string GetDate(string sDate, string ruleDate)
        {
            try
            {
                #region có truyền vào Rule date
                if (ruleDate != string.Empty)
                {
                    return GetDateByRule(sDate, ruleDate);
                }
                #endregion

                string dateBySearch = GetDateBySearchText(sDate, ruleDate);

                if (!string.IsNullOrEmpty(dateBySearch)) // có tìm ra kết quả
                {
                    return dateBySearch;
                }

                // không tìm ra kết quả xử lý tiếp
                dateBySearch = GetDateByPattern(sDate, ruleDate);
                if (!string.IsNullOrEmpty(dateBySearch)) // có tìm ra kết quả
                {
                    return dateBySearch;
                }

                #region chỉ có: 1h, 1 ngày
                // 1h
                if (Regex.IsMatch(sDate, @"[\d]+h") || Regex.IsMatch(sDate, @"[\d]+ h"))
                {
                    return DateTime.Now.ToString("yyyy-MM-dd");
                }

                //1 ngày
                if (Regex.IsMatch(sDate, @"[\d]+ ngày") || Regex.IsMatch(sDate, @"[\d]+ tháng") || Regex.IsMatch(sDate, @"[\d]+ năm"))
                {
                    int d = Convert.ToInt32(Utilities.Regex_GetItemFirst(sDate, @"[\d]+"));
                    return DateTime.Now.AddDays(-d).ToString("yyyy-MM-dd");
                }

                //1 tháng
                if (Regex.IsMatch(sDate, @"[\d]+ tháng"))
                {
                    int m = Convert.ToInt32(Utilities.Regex_GetItemFirst(sDate, @"[\d]+"));
                    return DateTime.Now.AddMonths(-m).ToString("yyyy-MM-dd");
                }

                //1 năm
                if (Regex.IsMatch(sDate, @"[\d]+ năm"))
                {
                    int y = Convert.ToInt32(Utilities.Regex_GetItemFirst(sDate, @"[\d]+"));
                    return DateTime.Now.AddYears(-y).ToString("yyyy-MM-dd");
                }

                

                #endregion

                if (!string.IsNullOrEmpty(dateBySearch)) // vẫn không có thì đưa vào dạng này xem sao
                {
                    return GetDateByRule(sDate, "dd/MM/yyyy");
                }
            }
            catch { }
            return "";
        }

        /// <summary>
        /// lấy ngày tháng, năm theo 1 quy tắc mình đưa vào
        /// </summary>
        /// <param name="sDate">chuỗi chứa ngày tháng</param>
        /// <param name="ruleDate"></param>
        /// <returns></returns>
        public string GetDateByRule(string sDate, string ruleDate)
        {
            try
            {
                #region có truyền vào Rule date dạng dd/MM/yyyy hay dd/MM
                if (ruleDate != string.Empty)
                {
                    string[] arr;
                    string dateTmp = sDate;
                    switch (ruleDate)
                    {
                        case "dd/MM": // chi co ngay va thang
                            if (sDate.Length > 5)
                            {
                                dateTmp = Utilities.Regex_GetItemFirst(dateTmp, @"[\d]+\/[\d]+");
                            }

                            arr = dateTmp.Split('/');
                            if (arr.Length == 2)
                            {
                                return arr[0] + "-" + arr[1] + "-" + DateTime.Now.Year;
                            }
                            break;
                        case "dd.MM": // chi co ngay va thang
                            if (sDate.Length > 5)
                            {
                                dateTmp = Utilities.Regex_GetItemFirst(dateTmp, @"[\d]+\.[\d]+");
                            }

                            arr = dateTmp.Split('.');
                            if (arr.Length == 2)
                            {
                                return arr[0] + "-" + arr[1] + "-" + DateTime.Now.Year;
                            }
                            break;
                        case "dd-MM": // chi co ngay va thang
                            if (sDate.Length > 5)
                            {
                                dateTmp = Utilities.Regex_GetItemFirst(dateTmp, @"[\d]+-[\d]+");
                            }

                            arr = dateTmp.Split('-');
                            if (arr.Length == 2)
                            {
                                return arr[0] + "-" + arr[1] + "-" + DateTime.Now.Year;
                            }
                            break;
                        case "dd/MM/yyyy":
                            if (sDate.Length > 10)
                            {
                                dateTmp = Utilities.Regex_GetItemFirst(dateTmp, @"[\d]+\/[\d]+\/[\d]{4}");
                            }

                            arr = dateTmp.Split('/');
                            if (arr.Length >= 3)
                            {
                                return arr[2] + "-" + arr[1] + "-" + arr[0];
                            }
                            break;
                        case "MM/dd/yyyy":
                            if (sDate.Length > 10)
                            {
                                dateTmp = Utilities.Regex_GetItemFirst(dateTmp, @"[\d]+\/[\d]+\/[\d]{4}");
                            }

                            arr = dateTmp.Split('/');
                            if (arr.Length >= 3)
                            {
                                return arr[2] + "-" + arr[0] + "-" + arr[1];
                            }
                            break;
                        case "dd/MM/yy":
                            if (sDate.Length > 8)
                            {
                                dateTmp = Utilities.Regex_GetItemFirst(dateTmp, @"[\d]+\/[\d]+\/[\d]{2,}");
                            }

                            arr = dateTmp.Split('/');
                            if (arr.Length >= 3)
                            {
                                return "20" + arr[2] + "-" + arr[1] + "-" + arr[0];
                            }
                            break;
                        case "MM/dd/yy":
                            if (sDate.Length > 8)
                            {
                                dateTmp = Utilities.Regex_GetItemFirst(dateTmp, @"[\d]+\/[\d]+\/[\d]{2,}");
                            }

                            arr = dateTmp.Split('/');
                            if (arr.Length >= 3)
                            {
                                return "20" + arr[2] + "-" + arr[0] + "-" + arr[1];
                            }
                            break;
                        case "MMM dd, yyyy": // ex: May 14, 2014
                            if (sDate.Length > 12)
                            {
                                dateTmp = Utilities.Regex_GetItemFirst(dateTmp, @"[\w]{3}[\s]+[\d]+public int \s]+[\d]{4}");
                            }

                            arr = dateTmp.Split(' ');
                            if (arr.Length >= 3)
                            {
                                return arr[2] + "-" + GetMonthFromText_English(arr[0].ToLower()) + "-" + arr[1].Replace(",", "");
                            }
                            break;
                        case "dd-MM-yyyy":
                            if (sDate.Length > 10) // 23-02-2014, <span class='time'>07:34, hoặc 1-2-2014
                            {
                                dateTmp = Utilities.Regex_GetItemFirst(dateTmp, @"[\d]+-[\d]+-[\d]{4}");
                            }

                            arr = dateTmp.Split('-');
                            if (arr.Length >= 3)
                            {
                                string day = arr[0];
                                if (day.Length == 1)
                                {
                                    day = "0" + day;
                                }

                                string month = arr[1];
                                if (month.Length == 1)
                                {
                                    month = "0" + month;
                                }

                                return arr[2] + "-" + month + "-" + day;
                            }
                            break;
                        case "yyyy-MM-dd":
                            if (sDate.Length > 10) // 23-02-2014, <span class='time'>07:34
                            {
                                dateTmp = Utilities.Regex_GetItemFirst(dateTmp, @"[\d]{4}-[\d]+-[\d]+");
                            }

                            arr = dateTmp.Split('-');
                            if (arr.Length >= 3)
                            {
                                return arr[0] + "-" + arr[1] + "-" + arr[2];
                            }
                            break;
                        case "yyyy.MM.dd":
                            if (sDate.Length > 10)
                            {
                                dateTmp = Utilities.Regex_GetItemFirst(dateTmp, @"[\d]{4}\.[\d]+\.[\d]+");
                            }
                            return dateTmp.Replace(".", "-");
                        case "MM-dd-yyyy":
                            if (sDate.Length > 10) // 11-04-2011, <span class='time'>11:03 am
                            {
                                dateTmp = Utilities.Regex_GetItemFirst(dateTmp, @"[\d]+-[\d]+-[\d]{4}");
                            }

                            arr = dateTmp.Split('-');
                            if (arr.Length >= 3)
                            {
                                return arr[2] + "-" + arr[0] + "-" + arr[1];
                            }
                            break;
                        case "dd.MM.yyyy":
                            if (sDate.Length > 10)
                            {
                                dateTmp = Utilities.Regex_GetItemFirst(dateTmp, @"[\d]+\.[\d]+\.[\d]{4}");
                            }

                            arr = dateTmp.Split('.');
                            if (arr.Length >= 3)
                            {
                                return arr[2] + "-" + arr[1] + "-" + arr[0];
                            }
                            break;
                        case "dd.MM.yy":
                            if (sDate.Length > 10)
                            {
                                dateTmp = Utilities.Regex_GetItemFirst(dateTmp, @"[\d]+\.[\d]+\.[\d]{2}");
                            }

                            arr = dateTmp.Split('.');
                            if (arr.Length >= 3)
                            {
                                return "20" + arr[2] + "-" + arr[1] + "-" + arr[0];
                            }
                            break;
                        case "yyyy/MM/dd":
                            if (sDate.Length > 10)
                            {
                                dateTmp = Utilities.Regex_GetItemFirst(dateTmp, @"[\d]{4}\/[\d]+\/[\d]+");
                            }

                            arr = dateTmp.Split('/');
                            if (arr.Length >= 3)
                            {
                                return arr[0] + "-" + arr[1] + "-" + arr[2];
                            }
                            break;
                        case "ngày dd tháng MM":

                            if (sDate.Length > 10)
                            {
                                dateTmp = Utilities.Regex_GetItemFirst(dateTmp, @"ngày [\d]+ tháng [\d]+");

                                if (!string.IsNullOrEmpty(dateTmp))
                                {
                                    List<string> lisDig = Utilities.Regex_GetListItem(dateTmp, @"[\d]+");
                                    if (lisDig != null && lisDig.Count == 2)
                                    {
                                        return lisDig[0] + "-" + lisDig[1] + "-" + DateTime.Now.Year;
                                    }
                                }
                            }
                            break;

                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }

            return "";
        }

        public string GetDateBySearchText(string sDate, string ruleDate)
        {
            try
            {
                #region có chữ hôm nay, hôm qua, today, yesterday, cách đây giờ, phút, một ngày trước.... - return luôn

                if (sDate.ToLower().Contains("hôm nay") || sDate.ToLower().Contains("today"))
                {
                    return DateTime.Now.ToString("yyyy-MM-dd");
                }

                if (sDate.ToLower().Contains("hôm qua") || sDate.ToLower().Contains("yesterday"))
                {
                    return DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                }

                bool isMinute = Regex.IsMatch(sDate, @"([\d]{1,2} phút trước)|([\d]{1,2} minutes ago)|([\d]+m ago)|(cách đây [\d]{1,2} phút)", RegexOptions.IgnoreCase);
                if (isMinute == true)
                {
                    string minute = Utilities.Regex_GetItemFirst(sDate, @"([\d]{1,2} phút trước)|([\d]{1,2} minutes ago)|([\d]{1,2}m ago)|(cách đây [\d]{1,2} phút)");
                    int mm = Convert.ToInt32(Utilities.Regex_GetItemFirst(minute, @"[\d]+"));

                    return DateTime.Now.AddMinutes(-mm).ToString("yyyy-MM-dd HH:mm:ss");
                }

                // 1 tiếng trước
                bool isHour = Regex.IsMatch(sDate, @"([\d]{1,2} tiếng trước)|([\d]{1,2} giờ trước)|([\d]{1,2} hours trước)|([\d]+h ago)|([\d]{1,2} hours ago)|(cách đây [\d]{1,2} giờ)", RegexOptions.IgnoreCase);
                if (isHour == true)
                {
                    string hour = Utilities.Regex_GetItemFirst(sDate, @"([\d]{1,2} tiếng trước)|([\d]{1,2} giờ trước)|([\d]{1,2} hours trước)|([\d]{1,2} hours ago)|([\d]{1,2}h ago)|(cách đây [\d]{1,2} giờ)");
                    int hh = Convert.ToInt32(Utilities.Regex_GetItemFirst(hour, @"[\d]+"));

                    return DateTime.Now.AddHours(-hh).ToString("yyyy-MM-dd HH:mm:ss");
                }

                bool isAfterDate = Regex.IsMatch(sDate, @"([\d]+ ngày trước)|([\d]+d ago)", RegexOptions.IgnoreCase);
                if (isAfterDate == true)
                {
                    string d = Utilities.Regex_GetItemFirst(sDate, @"[\d]+");
                    if (!string.IsNullOrEmpty(d))
                    {
                        int iD = Convert.ToInt32(d);
                        return DateTime.Now.AddDays(-iD).ToString("yyyy-MM-dd HH:mm:ss");
                    }

                }

                // cách đây 8 ngày
                bool isAgoDay = Regex.IsMatch(sDate, @"cách đây [\d]+ ngày", RegexOptions.IgnoreCase);
                if (isAgoDay == true)
                {
                    string d = Utilities.Regex_GetItemFirst(sDate, @"[\d]+");
                    if (!string.IsNullOrEmpty(d))
                    {
                        int iD = Convert.ToInt32(d);
                        return DateTime.Now.AddDays(-iD).ToString("yyyy-MM-dd");
                    }

                }

                // cách đây 8 tháng
                bool isAgoMonth = Regex.IsMatch(sDate, @"cách đây [\d]+ tháng", RegexOptions.IgnoreCase);
                if (isAgoMonth == true)
                {
                    string d = Utilities.Regex_GetItemFirst(sDate, @"[\d]+");
                    if (!string.IsNullOrEmpty(d))
                    {
                        int iD = Convert.ToInt32(d);
                        return DateTime.Now.AddMonths(-iD).ToString("yyyy-MM-dd");
                    }

                }

                #region một ngày trước
                if (sDate.ToLower().Contains("ngày trước"))
                {
                    if (sDate.ToLower().Contains("một ngày trước"))
                    {
                        return DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    }

                    // hai ngày trước
                    if (sDate.ToLower().Contains("hai ngày trước"))
                    {
                        return DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
                    }

                    // ba ngày trước
                    if (sDate.ToLower().Contains("ba ngày trước"))
                    {
                        return DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd");
                    }

                    // ba ngày trước
                    if (sDate.ToLower().Contains("bốn ngày trước"))
                    {
                        return DateTime.Now.AddDays(-4).ToString("yyyy-MM-dd");
                    }

                    if (sDate.ToLower().Contains("năm ngày trước"))
                    {
                        return DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd");
                    }

                    if (sDate.ToLower().Contains("sáu ngày trước"))
                    {
                        return DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd");
                    }

                    if (sDate.ToLower().Contains("bảy ngày trước"))
                    {
                        return DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                    }

                    if (sDate.ToLower().Contains("bảy ngày trước"))
                    {
                        return DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                    }

                    bool isNgayTruoc = Regex.IsMatch(sDate, @"[\d]+ ngày trước", RegexOptions.IgnoreCase);
                    if (isNgayTruoc == true)
                    {
                        string d = Utilities.Regex_GetItemFirst(sDate, @"[\d]+");
                        if (!string.IsNullOrEmpty(d))
                        {
                            int iD = Convert.ToInt32(d);
                            return DateTime.Now.AddMonths(-iD).ToString("yyyy-MM-dd");
                        }
                    }
                }
                #endregion

                #region một giờ trước, phút trước
                if (sDate.ToLower().Contains("giờ trước") || sDate.ToLower().Contains("phút trước") || sDate.ToLower().Contains("giây trước"))
                {
                    if (sDate.ToLower().Contains("một giờ trước"))
                    {
                        return DateTime.Now.ToString("yyyy-MM-dd");
                    }
                }
                #endregion

                #region một tháng trước
                if (sDate.ToLower().Contains("tháng trước"))
                {
                    if (sDate.ToLower().Contains("một tháng trước"))
                    {
                        return DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
                    }

                    // hai tháng trước
                    if (sDate.ToLower().Contains("hai tháng trước"))
                    {
                        return DateTime.Now.AddMonths(-2).ToString("yyyy-MM-dd");
                    }

                    // ba tháng trước
                    if (sDate.ToLower().Contains("ba tháng trước"))
                    {
                        return DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd");
                    }

                    bool isThangTruoc = Regex.IsMatch(sDate, @"[\d]+ tháng trước", RegexOptions.IgnoreCase);
                    if (isThangTruoc == true)
                    {
                        string m = Utilities.Regex_GetItemFirst(sDate, @"[\d]+");
                        if (!string.IsNullOrEmpty(m))
                        {
                            int iM = Convert.ToInt32(m);
                            return DateTime.Now.AddMonths(-iM).ToString("yyyy-MM-dd");
                        }
                    }
                }
                #endregion

                #region một năm trước
                if (sDate.ToLower().Contains("năm trước"))
                {
                    if (sDate.ToLower().Contains("một năm trước"))
                    {
                        return DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd");
                    }

                    // hai năm trước
                    if (sDate.ToLower().Contains("hai năm trước"))
                    {
                        return DateTime.Now.AddYears(-2).ToString("yyyy-MM-dd");
                    }

                    // ba năm trước
                    if (sDate.ToLower().Contains("ba năm trước"))
                    {
                        return DateTime.Now.AddYears(-3).ToString("yyyy-MM-dd");
                    }

                    bool isNamTruoc = Regex.IsMatch(sDate, @"[\d]+ năm trước", RegexOptions.IgnoreCase);
                    if (isNamTruoc == true)
                    {
                        string y = Utilities.Regex_GetItemFirst(sDate, @"[\d]+");
                        if (!string.IsNullOrEmpty(y))
                        {
                            int iY = Convert.ToInt32(y);
                            return DateTime.Now.AddYears(-iY).ToString("yyyy-MM-dd");
                        }
                    }
                }
                #endregion

                #endregion

                #region dò kiểu 2021-11-14
                bool isMatch1 = Regex.IsMatch(sDate, @"[\d]{4}\-[\d]+\-[\d]+", RegexOptions.IgnoreCase);
                if (isMatch1 == true)
                {
                    string subDate = Utilities.Regex_GetItemFirst(sDate, @"[\d]{4}\-[\d]+\-[\d]+");

                    List<string> lisDig = Utilities.Regex_GetListItem(subDate, @"[\d]+");
                    if (lisDig.Count >= 3)
                    {
                        return lisDig[0] + "-" + lisDig[1] + "-" + lisDig[2];
                    }

                    return subDate;
                }
                #endregion

                #region dò kiểu 11-14
                bool isMatch = Regex.IsMatch(sDate, @"\b\d{1,2}\-\d{1,2}", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string subDate = Utilities.Regex_GetItemFirst(sDate, @"\b\d{1,2}\-\d{1,2}");

                    List<string> lisDig = Utilities.Regex_GetListItem(subDate, @"[\d]+");
                    if (lisDig.Count >= 2)
                    {
                        return DateTime.Now.Year + "-" + lisDig[0] + "-" + lisDig[1];
                    }

                    return subDate;
                }
                #endregion

               

            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }

            return "";
        }

        public string GetDateByPattern(string sDate, string ruleDate)
        {
            try
            {
                #region không truyền vào Rule date, cố gắng tìm thêm xem có kết quả không?
                string dt = "";

                #region dò kiểu April 4
                bool isMatch = Regex.IsMatch(sDate.ToLower(), @"(jan|january|feb|february|mar|march|apr|april|may|jun|june|jul|july|aug|august|sep|september|oct|october|nov|november|dec|december) [\d]+");
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"(jan|january|feb|february|mar|march|apr|april|may|jun|june|jul|july|aug|august|sep|september|oct|october|nov|november|dec|december) [\d]+");
                    if (tmpDate != "")
                    {
                        string[] arr = tmpDate.Split(' ');
                        if (arr.Length > 1)
                        {
                            string month = GetMonthFromText_English(arr[0]);
                            return DateTime.Now.ToString("yyyy") + "-" + month + "-" + arr[1].Replace(",", "");
                        }
                    }
                }
                #endregion

                #region Sunday 1 April 2018
                isMatch = Regex.IsMatch(sDate, @"[\d]+ (january|jan|february|feb|march|mar|april|apr|may|may|june|jun|july|jul|august|aug|september|sep|october|oct|november|nov|december|dec) [\s]{0,}[\d]{4,}", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"[\d]+ (january|jan|february|feb|march|mar|april|apr|may|may|june|jun|july|jul|august|aug|september|sep|october|oct|november|nov|december|dec) [\s]{0,}[\d]{4,}");
                    if (tmpDate != "")
                    {
                        string[] arr = tmpDate.Split(' '); // 1 April 2018
                        if (arr.Length > 2)
                        {
                            string day = arr[0];
                            string year = arr[2];
                            string month = GetMonthFromText_English(arr[1]);

                            return year + "-" + month + "-" + day;
                        }
                    }
                }
                #endregion

                #region dò kiểu 28/05.2016
                isMatch = Regex.IsMatch(sDate.ToLower(), @"[\d]+\/[\d]+\.[\d]{4}", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string subDate = Utilities.Regex_GetItemFirst(sDate, @"[\d]+\/[\d]+\.[\d]{4}");

                    List<string> lisDig = Utilities.Regex_GetListItem(subDate, @"[\d]+");
                    if (lisDig.Count == 3)
                    {
                        return lisDig[2] + "-" + lisDig[1] + "-" + lisDig[0];
                    }

                    return subDate;
                }
                #endregion

                #region gặp kiểu ngày 31 tháng 12 năm 2014
                isMatch = Regex.IsMatch(sDate.ToLower(), @"ngày [\d]+ tháng [\d]+ năm [\d]+", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string subDate = Utilities.Regex_GetItemFirst(sDate, @"ngày[\s\S]+");
                    List<string> lisDig = Utilities.Regex_GetListItem(subDate, @"[\d]+");

                    if (lisDig.Count >= 3)
                    {
                        return lisDig[2] + "-" + lisDig[1] + "-" + lisDig[0];
                    }
                }
                #endregion

                #region gặp kiểu tháng 12 năm 2014
                isMatch = Regex.IsMatch(sDate.ToLower(), @"tháng [\d]+ năm [\d]+", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string subMonth = Utilities.Regex_GetItemFirst(sDate, @"tháng[\s\S]+");
                    List<string> lisDig = Utilities.Regex_GetListItem(subMonth, @"[\d]+");

                    if (lisDig.Count >= 2)
                    {
                        return lisDig[1] + "-" + lisDig[0];
                    }
                }
                #endregion

                #region gặp kiểu:   27 Tháng 04, 2016 | 08:56 -> 
                isMatch = Regex.IsMatch(sDate.ToLower(), @"[\d]+ Tháng [\d]+, [\d]{4}", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    List<string> lisDig = Utilities.Regex_GetListItem(sDate, @"[\d]+");
                    if (lisDig.Count >= 3)
                    {
                        return lisDig[2] + "-" + lisDig[1] + "-" + lisDig[0];
                    }
                }
                #endregion

                #region Ngày 15 Tháng 5, 2015 | 07:49 PM
                isMatch = Regex.IsMatch(sDate.ToLower(), @"ngày [\d]+ tháng [\d]+, [\d]+", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    List<string> lisDig = Utilities.Regex_GetListItem(sDate, @"[\d]+");

                    if (lisDig.Count >= 3)
                    {
                        return lisDig[2] + "-" + lisDig[1] + "-" + lisDig[0];
                    }
                }
                #endregion

                #region gặp định dạng kiểu: 29 tháng 11 2016
                isMatch = Regex.IsMatch(sDate, @"[\d]+ tháng [\d]+ [\d]{4,}", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    List<string> lisDig = Utilities.Regex_GetListItem(sDate, @"[\d]+");

                    if (lisDig.Count >= 3)
                    {
                        return lisDig[2] + "-" + lisDig[1] + "-" + lisDig[0];
                    }
                }
                #endregion

                #region gặp định dạng kiểu: 15 Tháng mười hai 2012
                isMatch = Regex.IsMatch(sDate, @"[\d]+ (tháng|Tháng) (.*?) [\d]{4}", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string month = GetMonthFromText(sDate);

                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"[\d]+ (tháng|Tháng) (.*?) [\d]{4}");

                    List<string> lisDig = Utilities.Regex_GetListItem(tmpDate, @"[\d]+");
                    if (lisDig != null && lisDig.Count == 2)
                    {
                        return lisDig[1] + "-" + month + "-" + lisDig[0];
                    }
                }
                #endregion

                #region Cập nhật08:45, Thứ hai Ngày 23, Tháng 2, 2015
                isMatch = Regex.IsMatch(sDate, @"Ngày [\d]+, Tháng [\d]+, [\d]{4}", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    List<string> lisDig = Utilities.Regex_GetListItem(sDate, @"[\d]+");
                    return lisDig[lisDig.Count - 1] + "-" + lisDig[lisDig.Count - 2] + "-" + lisDig[lisDig.Count - 3];
                }
                #endregion

                #region Tháng 9 2, 2016
                isMatch = Regex.IsMatch(sDate, @"(tháng [\d]+ [\d]+, [\d]{4})", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"(tháng [\d]+ [\d]+, [\d]{4})");
                    if (tmpDate != "")
                    {
                        List<string> lisD = Utilities.Regex_GetListItem(tmpDate, @"[\d]+");

                        if (lisD != null && lisD.Count > 2)
                        {
                            return lisD[2] + "-" + lisD[0] + "-" + lisD[1];
                        }
                    }
                }
                #endregion

                #region dò xem kiểu này không:  Thứ ba, 29 Tháng 9 2015 17:24 hay Chủ nhật, 29 Tháng 9 2015 17:24
                isMatch = Regex.IsMatch(sDate.ToLower(), @"(thứ )(.*?), [\d]{1,2} (tháng) [\d]{1,2} [\d]{4}|(chủ nhật,) [\d]{1,2} (tháng) [\d]{1,2} [\d]{4}", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"(thứ )(.*?), [\d]{1,2} (tháng) [\d]{1,2} [\d]{4}|(chủ nhật,) [\d]{1,2} (tháng) [\d]{1,2} [\d]{4}");
                    if (tmpDate != "")
                    {
                        string[] arr = tmpDate.Split(' ');
                        return arr[5] + "-" + arr[4] + "-" + arr[2];
                    }
                }
                #endregion

                #region định dạng kiểu Thứ sáu, 31 Tháng 7 2015
                isMatch = Regex.IsMatch(sDate, @"(Thứ|Chủ)([\s]+)(.*?)(,)([\s]+)([\d]+)([\s]+)(Tháng)([\s]+)([\d]+)([\s]+)([\d]{4})", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"(Thứ|Chủ)([\s]+)(.*?)(,)([\s]+)([\d]+)([\s]+)(Tháng)([\s]+)([\d]+)([\s]+)([\d]{4})");
                    if (tmpDate != "")
                    {
                        string[] arr = sDate.Split(' ');
                        return arr[5] + "-" + arr[4] + "-" + arr[2];
                    }
                }
                #endregion

                #region gap kieu NGÀY 10 THÁNG 11, 2015 | 15:03 => http://suckhoedoisong.vn/kip-thoi-cuu-nam-thanh-nien-bi-thanh-sat-2m-dam-xuyen-that-lung-qua-hau-mon-n109044.html
                isMatch = Regex.IsMatch(sDate.ToLower(), @"(NGÀY )[\d]+( THÁNG )[\d]+(, )[\d]+", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"(NGÀY )[\d]+( THÁNG )[\d]+(, )[\d]+").Replace(",", "").Trim();
                    if (tmpDate != "")
                    {
                        List<string> ls = Utilities.Regex_GetListItem(tmpDate, @"[\d]+");

                        string[] arr = tmpDate.Split(' ');
                        if (ls.Count > 2)
                        {
                            return ls[2] + "-" + ls[1] + "-" + ls[0];
                        }
                    }
                }
                #endregion

                // 22 Th6 2017
                isMatch = Regex.IsMatch(sDate.ToLower(), @"[\d]+ Th[\d]+ [\d]{4}", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"[\d]+ Th[\d]+ [\d]{4}").Trim();
                    if (tmpDate != "")
                    {
                        List<string> ls = Utilities.Regex_GetListItem(tmpDate, @"[\d]+");

                        string[] arr = tmpDate.Split(' ');
                        if (ls.Count > 2)
                        {
                            return ls[2] + "-" + ls[1] + "-" + ls[0];
                        }
                    }
                }
                #endregion

                #region dò kiểu 8 Th5, 2017
                isMatch = Regex.IsMatch(sDate.ToLower(), @"[\d]+ Th[\d]+, [\d]{4,}", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"[\d]+ Th[\d]+, [\d]{4,}").Trim();
                    if (tmpDate != "")
                    {
                        List<string> ls = Utilities.Regex_GetListItem(tmpDate, @"[\d]+");

                        string[] arr = tmpDate.Split(' ');
                        if (ls.Count > 2)
                        {
                            return ls[2] + "-" + ls[1] + "-" + ls[0];
                        }
                    }
                }
                #endregion

                #region gặp kiểu 00:00 - 15/33
                isMatch = Regex.IsMatch(sDate, @"([\d]+:[\d]+ - [\d]{1,2}\/[\d]{1,2})$", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"([\d]+:[\d]+ - [\d]{1,2}\/[\d]{1,2})$");
                    if (tmpDate != "")
                    {
                        string[] arr = sDate.Split('-');
                        if (arr.Length > 0)
                        {
                            string[] arrSub = sDate.Split('/');
                            if (arrSub.Length > 0)
                            {
                                return DateTime.Now.Year + "-" + arrSub[1] + "-" + arrSub[0];
                            }
                        }

                    }
                }
                #endregion

                #region gặp kiểu 06:27 PM, 12 11 2015
                isMatch = Regex.IsMatch(sDate, @"([\d]+):([\d]+) (AM|PM), ([\d]+) ([\d]+) ([\d]{4})", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"([\d]+):([\d]+) (AM|PM), ([\d]+) ([\d]+) ([\d]{4})");
                    if (tmpDate != "")
                    {
                        string[] arr = sDate.Split(' ');
                        if (arr.Length > 0)
                        {
                            return arr[4] + "-" + arr[3] + "-" + arr[2];
                        }

                    }
                }
                #endregion

                #region gặp kiểu 06:27 PM 12 11 2015
                isMatch = Regex.IsMatch(sDate, @"([\d]+):([\d]+) (AM|PM) ([\d]+) ([\d]+) ([\d]{4})", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"([\d]+):([\d]+) (AM|PM) ([\d]+) ([\d]+) ([\d]{4})");
                    if (tmpDate != "")
                    {
                        string[] arr = sDate.Split(' ');
                        if (arr.Length > 0)
                        {
                            return arr[4] + "-" + arr[3] + "-" + arr[2];
                        }

                    }
                }
                #endregion

                #region gặp kiểu 15 Th7 2016 [\d]+[\s]+Th[\d]+[\s]+[\d]{4} 
                isMatch = Regex.IsMatch(sDate, @"[\d]+[\s]+Th[\d]+[\s]+[\d]{4}", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"[\d]+[\s]+Th[\d]+[\s]+[\d]{4} ");
                    if (tmpDate != "")
                    {
                        string[] arr = sDate.Split(' ');
                        if (arr.Length > 0)
                        {
                            return arr[2] + "-" + arr[1].Replace("Th", "") + "-" + arr[0];
                        }
                    }
                }
                #endregion

                #region gặp kiểu: 30 Tháng Mười Hai, 2015
                isMatch = Regex.IsMatch(sDate, @"[\d]+ tháng ([\w\W]+), [\d]{4,}", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"[\d]+ tháng ([\w\W]+), [\d]{4,}").ToLower();
                    if (tmpDate != "")
                    {
                        string year = Utilities.Regex_GetItemFirst(tmpDate, @"[\d]{4,}$");
                        string day = Utilities.Regex_GetItemFirst(tmpDate, @"^[\d]+");
                        string month = GetMonthFromText(Regex.Replace(tmpDate, @"[\d]+|,", "", RegexOptions.IgnoreCase).Trim());

                        return year + "-" + month + "-" + day;
                    }
                }
                #endregion

                #region T3, 07 / 2016
                isMatch = Regex.IsMatch(sDate, @"T[\d]+, [\d]+ \/ [\d]+", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"T[\d]+, [\d]+ \/ [\d]+");
                    if (tmpDate != "")
                    {
                        string[] arr = sDate.Split(' ');
                        if (arr.Length > 3)
                        {
                            string day = Utilities.Regex_GetItemFirst(arr[0], @"[\d]+");
                            return arr[3] + "-" + arr[1].Replace("Th", "") + "-" + day;
                        }
                    }
                }
                #endregion

                #region 21st, Tháng Bảy, 2016
                isMatch = Regex.IsMatch(sDate, @"[\d]+(th|st|nd|rd), Tháng [\w\W]+, [\d]{4}", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"[\d]+(th|st|nd|rd), Tháng [\w\W]+, [\d]{4}");
                    if (tmpDate != "")
                    {
                        string[] arr = tmpDate.Split(',');
                        if (arr.Length > 2)
                        {
                            string day = Utilities.Regex_GetItemFirst(arr[0], @"[\d]+");
                            string month = GetMonthFromText(arr[1].Trim());
                            return arr[2].Trim() + "-" + month + "-" + day;
                        }
                    }
                }
                #endregion

                #region Tháng Chín 10, 2016 4:29 chiều
                isMatch = Regex.IsMatch(sDate, @"(Tháng) [\w\W]+ [\d]+, [\d]{4}", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"(Tháng) [\w\W]+ [\d]+, [\d]{4}");
                    if (tmpDate != "")
                    {
                        //Tháng Chín 10, 2016
                        string day = Utilities.Regex_GetItemFirst(tmpDate, @"[\d]+");
                        string year = Utilities.Regex_GetItemFirst(tmpDate, @"[\d]{4}");

                        tmpDate = tmpDate.Replace(",", "");
                        tmpDate = Regex.Replace(tmpDate, @"[\d]+", "", RegexOptions.IgnoreCase).Trim();
                        string month = GetMonthFromText(tmpDate);

                        return year + "-" + month + "-" + day;
                    }
                }
                #endregion

                #region Jul 18, 2017, 11:06 PM ET
                isMatch = Regex.IsMatch(sDate.ToLower(), @"(jan|january|feb|february|mar|march|apr|april|may|jun|june|jul|july|aug|august|sep|september|oct|october|nov|november|dec|december) [\d]+, [\d]{4}");
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"(jan|january|feb|february|mar|march|apr|april|may|jun|june|jul|july|aug|august|sep|september|oct|october|nov|november|dec|december) [\d]+, [\d]{4}");
                    if (tmpDate != "")
                    {
                        string[] arr = tmpDate.Split(' ');
                        if (arr.Length > 2)
                        {
                            string month = GetMonthFromText_English(arr[0]);
                            return arr[2] + "-" + month + "-" + arr[1].Replace(",", "");
                        }
                    }
                }
                #endregion

                #region 1 Jan 2016 hay 1 january 2016
                isMatch = Regex.IsMatch(sDate.ToLower(), @"[\d]+[\s]+(jan|january|feb|february|mar|march|apr|april|may|jun|june|jul|july|aug|august|sep|september|oct|october|nov|november|dec|december)[\s]+[\d]{2,}");
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"[\d]+[\s]+(jan|january|feb|february|mar|march|apr|april|may|jun|june|jul|july|aug|august|sep|september|oct|october|nov|november|dec|december)[\s]+[\d]{2,}");
                    if (tmpDate != "")
                    {
                        string[] arr = tmpDate.Split(' ');
                        if (arr.Length > 2)
                        {
                            string month = GetMonthFromText_English(arr[1]);
                            return arr[2] + "-" + month + "-" + arr[0];
                        }
                    }
                }
                #endregion

                #region Sunday, July 16, 2017
                isMatch = Regex.IsMatch(sDate, @"(July|January|February|March|April|May|June|August|September|October|November|December)[\s]+[\d]{1,}public int \s]+[\d]{4}", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"(July|January|February|March|April|May|June|August|September|October|November|December)[\s]+[\d]{1,}public int \s]+[\d]{4}");
                    if (tmpDate != "") // July 16, 2017
                    {
                        string[] arr = tmpDate.Split(' ');
                        return arr[2] + "-" + GetMonthFromText_English(arr[0]) + "-" + arr[1].Replace(",", "");
                    }
                }
                #endregion

                #region gặp kiểu Nov 3 2015
                isMatch = Regex.IsMatch(sDate, @"(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)[\s]+[\d]+[\s]+[\d]{4}", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)[\s]+[\d]+[\s]+[\d]{4}");
                    if (tmpDate != "")
                    {
                        string[] arr = tmpDate.Split(' ');
                        return arr[2] + "-" + GetMonthFromText_English(arr[0]) + "-" + arr[1];
                    }
                }
                #endregion

                #region 3 August,2016 hay 3 August, 2016
                isMatch = Regex.IsMatch(sDate, @"[\d]+ (january|jan|february|feb|march|mar|april|apr|may|may|june|jun|july|jul|august|aug|september|sep|october|oct|november|nov|december|dec)public int \s]{0,}[\d]{4,}", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"[\d]+ (january|jan|february|feb|march|mar|april|apr|may|may|june|jun|july|jul|august|aug|september|sep|october|oct|november|nov|december|dec)public int \s]{0,}[\d]{4,}");
                    if (tmpDate != "")
                    {
                        string[] arr = tmpDate.Split(' ');
                        if (arr.Length > 2)
                        {
                            string day = arr[0];

                            string[] arrSub = arr[1].Split(',');
                            string year = arrSub[1];
                            string month = GetMonthFromText_English(arrSub[0].Replace(",", "").ToLower().Trim());

                            return year + "-" + month + "-" + day;
                        }
                    }
                }
                #endregion

                #region gặp kiểu May 17th, 2016 2:24 am
                isMatch = Regex.IsMatch(sDate, @"(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec) [\d]+(th|nd|st|rd), [\d]+ [\d]+:[\d]+ (am|pm)", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec) [\d]+(th|nd|st|rd), [\d]+ [\d]+:[\d]+ (am|pm)");
                    if (tmpDate != "")
                    {
                        string year = Utilities.Regex_GetItemFirst(tmpDate, @"[\d]{4,}");
                        string day = Utilities.Regex_GetItemFirst(tmpDate, @"[\d]+");
                        string month = Utilities.Regex_GetItemFirst(tmpDate, "(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)");
                        month = GetMonthFromText_English(month);

                        return year + "-" + month + "-" + day;
                    }
                }
                #endregion

                #region gặp kiểu: May 5th, 7:27 am
                isMatch = Regex.IsMatch(sDate, @"(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec) [\d]+(th|nd|st|rd), [\d]+:[\d]+ (am|pm)", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec) [\d]+(th|nd|st|rd), [\d]+:[\d]+ (am|pm)");
                    if (tmpDate != "")
                    {
                        string year = DateTime.Now.ToString("yyyy");
                        string day = Utilities.Regex_GetItemFirst(tmpDate, @"[\d]+");
                        string month = Utilities.Regex_GetItemFirst(tmpDate, "(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)");
                        month = GetMonthFromText_English(month);

                        return year + "-" + month + "-" + day;
                    }
                }
                #endregion

                #region gặp kiểu Monday, 9:29 am
                isMatch = Regex.IsMatch(sDate, @"(monday|tuesday|wednesday|thursday|friday|saturday|sunday), [\d]+:[\d]+ (am|pm)", RegexOptions.IgnoreCase);
                if (isMatch == true)
                {
                    string tmpDate = Utilities.Regex_GetItemFirst(sDate, @"(monday|tuesday|wednesday|thursday|friday|saturday|sunday), [\d]+:[\d]+ (am|pm)");
                    if (tmpDate != "")
                    {
                        string strDay = Utilities.Regex_GetItemFirst(tmpDate, "(monday|tuesday|wednesday|thursday|friday|saturday|sunday)");
                        string day = GetDayOfWeek(strDay.ToLower());

                        if (!string.IsNullOrEmpty(day))
                        {
                            return day;
                        }
                    }
                }
                #endregion

                #region kiểu toàn số thì kiểm tra chuyển thử sang kiểu
                isMatch = Regex.IsMatch(sDate, @"[\d]{10,}");
                if (isMatch == true)
                {
                    dt = UnixTimeStampToDateTime(sDate);
                    if (!string.IsNullOrEmpty(dt))
                    {
                        return dt;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }

            return "";
        }


        /// <summary>
        /// Chuyển đổi tháng từ tên dạng Text sang dạng số. Ví dụ: january thành 01
        /// </summary>
        /// <param name="text">january, february, march ....</param>
        /// <returns>01, 02, 03 ...</returns>
        public static string ConvertMonthTextToNumber(string text)
        {
            text = text.ToLower();

            // dùng hàm chứa đựng để đề phòng truyền vào có dư thừa ký tự
            if (text.Contains("january"))
            {
                return "01";
            }

            if (text.Contains("february"))
            {
                return "02";
            }

            if (text.Contains("march"))
            {
                return "03";
            }

            if (text.Contains("april"))
            {
                return "04";
            }

            if (text.Contains("may"))
            {
                return "05";
            }

            if (text.Contains("june"))
            {
                return "06";
            }

            if (text.Contains("july"))
            {
                return "07";
            }

            if (text.Contains("august"))
            {
                return "08";
            }

            if (text.Contains("september"))
            {
                return "09";
            }

            if (text.Contains("october"))
            {
                return "10";
            }

            if (text.Contains("november"))
            {
                return "11";
            }

            if (text.Contains("december"))
            {
                return "12";
            }

            return "";
        }

        public string Detect_ddMMyyyy(string date)
        {
            try
            {
                string[] arr = date.Split('/');

                if (arr.Length > 0 && arr[2].Length == 4)
                {
                    int itemFirst = Convert.ToInt32(arr[0]);

                    // nếu item đầu lớn hơn 12 => đích thị là dd/MM/yyyy
                    if (itemFirst > 12)
                    {
                        return arr[2] + "-" + arr[1] + "-" + arr[0];
                    }

                    int itemSecond = Convert.ToInt32(arr[1]);
                    if (itemSecond > 12) // đích thị là kiểu MM/dd/yyyy
                    {
                        return arr[2] + "-" + arr[0] + "-" + arr[1];
                    }

                    // ưu tiên trả về từ dạng dd/MM/yyyy
                    return arr[2] + "-" + arr[1] + "-" + arr[0];
                }
            }
            catch { }
            return "";
        }

        public string Detect_ddMMyy(string date)
        {
            try
            {
                string[] arr = date.Split('/');

                if (arr.Length > 0 && arr[2].Length == 4)
                {
                    int itemFirst = Convert.ToInt32(arr[0]);

                    // nếu item đầu lớn hơn 12 => đích thị là dd/MM/yy
                    if (itemFirst > 12)
                    {
                        return "20" + arr[2] + "-" + arr[1] + "-" + arr[0];
                    }

                    int itemSecond = Convert.ToInt32(arr[1]);
                    if (itemSecond > 12) // đích thị là kiểu MM/dd/yy
                    {
                        return "20" + arr[2] + "-" + arr[0] + "-" + arr[1];
                    }

                    // ưu tiên trả về từ dạng dd/MM/yy
                    return "20" + arr[2] + "-" + arr[1] + "-" + arr[0];
                }
            }
            catch { }
            return "";
        }

        /// <summary>
        /// Lấy ngày theo ngày trong tuần
        /// </summary>
        /// <param name="day">là Monday, Tueday...</param>
        /// <returns></returns>
        public string GetDayOfWeek(string day)
        {
            string year = DateTime.Now.ToString("yyyy");
            string month = DateTime.Now.ToString("MM");

            string dayofWeek = DateTime.Now.DayOfWeek.ToString(); // ngày hôm nay
            if (dayofWeek.ToLower() == day) // hôm nay
            {
                day = DateTime.Now.ToString("dd");
                return year + "-" + month + "-" + day;
            }

            dayofWeek = DateTime.Now.AddDays(-1).DayOfWeek.ToString(); // ngày hôm qua
            if (dayofWeek.ToLower() == day) // hôm nay
            {
                day = DateTime.Now.AddDays(-1).ToString("dd");
                return year + "-" + month + "-" + day;
            }

            dayofWeek = DateTime.Now.AddDays(-2).DayOfWeek.ToString(); // ngày hôm kia
            if (dayofWeek.ToLower() == day) // hôm nay
            {
                day = DateTime.Now.AddDays(-2).ToString("dd");
                return year + "-" + month + "-" + day;
            }

            dayofWeek = DateTime.Now.AddDays(-3).DayOfWeek.ToString(); // ngày hôm kia
            if (dayofWeek.ToLower() == day) // hôm nay
            {
                day = DateTime.Now.AddDays(-3).ToString("dd");
                return year + "-" + month + "-" + day;
            }

            dayofWeek = DateTime.Now.AddDays(-4).DayOfWeek.ToString(); // ngày hôm kia
            if (dayofWeek.ToLower() == day) // hôm nay
            {
                day = DateTime.Now.AddDays(-4).ToString("dd");
                return year + "-" + month + "-" + day;
            }

            dayofWeek = DateTime.Now.AddDays(-5).DayOfWeek.ToString(); // ngày hôm kia
            if (dayofWeek.ToLower() == day) // hôm nay
            {
                day = DateTime.Now.AddDays(-5).ToString("dd");
                return year + "-" + month + "-" + day;
            }

            dayofWeek = DateTime.Now.AddDays(-6).DayOfWeek.ToString(); // ngày hôm kia
            if (dayofWeek.ToLower() == day) // hôm nay
            {
                day = DateTime.Now.AddDays(-6).ToString("dd");
                return year + "-" + month + "-" + day;
            }
            return "";
        }

        public string FindDateFromText(string text)
        {
            bool isMatch = Regex.IsMatch(text, @"(cách đây[\s]+[\d]+[\s])|([\d]+)(.*?)(Ago)|([\d]+)(.*?)(ago)");
            if (isMatch == true)
            {
                text = text.ToLower();

                #region nếu tồn tại kiểu: cách đây 5 giây
                if (text.Contains("giây") || text.Contains("second"))
                {
                    int t;
                    try
                    {
                        string tt = Utilities.Regex_GetItemFirst(text, @"[\d]+");
                        t = Convert.ToInt32(tt);

                        return DateTime.Now.AddSeconds(-t).ToString("yyyy-MM-dd hh:mm:ss tt");
                    }
                    catch { }
                }
                #endregion

                #region nếu tồn tại kiểu: cách đây 5 phút
                if (text.Contains("phút") || text.Contains("minute"))
                {
                    int t;
                    try
                    {
                        string tt = Utilities.Regex_GetItemFirst(text, @"[\d]+");
                        t = Convert.ToInt32(tt);

                        return DateTime.Now.AddMinutes(-t).ToString("yyyy-MM-dd hh:mm:ss tt");
                    }
                    catch { }
                }
                #endregion

                #region nếu tồn tại kiểu: cách đây 5 tiếng
                if (text.Contains("tiếng") || text.Contains("hour"))
                {
                    int t;
                    try
                    {
                        string tt = Utilities.Regex_GetItemFirst(text, @"[\d]+");
                        t = Convert.ToInt32(tt);

                        return DateTime.Now.AddHours(-t).ToString("yyyy-MM-dd hh:mm:ss tt");
                    }
                    catch { }
                }
                #endregion

                #region nếu tồn tại kiểu: cách đây 1 ngày
                if (text.Contains("ngày") || text.Contains("day"))
                {
                    int t;
                    try
                    {
                        string tt = Utilities.Regex_GetItemFirst(text, @"[\d]+");
                        t = Convert.ToInt32(tt);

                        return DateTime.Now.AddDays(-t).ToString("yyyy-MM-dd hh:mm:ss tt");
                    }
                    catch { }
                }
                #endregion

                #region nếu tồn tại kiểu: cách đây 1 tuần
                if (text.Contains("tuần") || text.Contains("week"))
                {
                    int t;
                    try
                    {
                        string tt = Utilities.Regex_GetItemFirst(text, @"[\d]+");
                        t = Convert.ToInt32(tt) * 7;

                        return DateTime.Now.AddDays(-t).ToString("yyyy-MM-dd hh:mm:ss tt");
                    }
                    catch { }
                }
                #endregion
            }

            return "";
        }

        public string GetMonthFromText(string text)
        {
            text = text.ToLower();
            string month = "";
            switch (text)
            {
                case "tháng một":
                    month = "01";
                    break;
                case "tháng hai":
                    month = "02";
                    break;
                case "tháng ba":
                    month = "03";
                    break;
                case "tháng tư":
                    month = "04";
                    break;
                case "tháng năm":
                    month = "05";
                    break;
                case "tháng sáu":
                    month = "06";
                    break;
                case "tháng bảy":
                    month = "07";
                    break;
                case "tháng tám":
                    month = "08";
                    break;
                case "tháng chín":
                    month = "09";
                    break;
                case "tháng mười":
                    month = "10";
                    break;
                case "tháng mười một":
                    month = "11";
                    break;
                case "tháng mười hai":
                    month = "12";
                    break;
            }

            return month;
        }

        /// <summary>
        /// Chuyển đổi tháng bằng chữ sang tháng bằng số
        /// </summary>
        /// <param name="m">dạng: january, february hay march ...</param>
        /// <returns></returns>
        public string GetMonthFromText_English(string m)
        {
            m = m.ToLower();

            switch (m)
            {
                case "january":
                    return "01";
                case "jan":
                    return "01";
                case "february":
                    return "02";
                case "feb":
                    return "02";
                case "march":
                    return "03";
                case "mar":
                    return "03";
                case "april":
                    return "04";
                case "apr":
                    return "04";
                case "may":
                    return "05";
                case "june":
                    return "06";
                case "jun":
                    return "06";
                case "july":
                    return "07";
                case "jul":
                    return "07";
                case "august":
                    return "08";
                case "aug":
                    return "08";
                case "september":
                    return "09";
                case "sep":
                    return "09";
                case "october":
                    return "10";
                case "oct":
                    return "10";
                case "november":
                    return "11";
                case "nov":
                    return "11";
                case "december":
                    return "12";
                case "dec":
                    return "12";
            }
            return "";
        }

        public string UnixTimeStampToDateTime(string sUnixTimeStamp)
        {
            try
            {
                double unixTimeStamp = Convert.ToDouble(sUnixTimeStamp);

                // Unix timestamp is seconds past epoch
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();

                return dtDateTime.ToString("yyyy-MM-dd"); // đổi chỗ này ra kiểu: Your time zone
            }
            catch { }

            return "";
        }

        public string UnixTimeStampToHour(string sUnixTimeStamp)
        {
            try
            {
                double unixTimeStamp = Convert.ToDouble(sUnixTimeStamp);

                // Unix timestamp is seconds past epoch
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();

                return dtDateTime.ToString("HH:mm:ss"); // đổi chỗ này ra kiểu: Your time zone
            }
            catch { }

            return "";
        }

        /// <summary>
        /// Lấy ngày tháng của 1 đoạn text theo rule truyền vào, trả ra full ngày tháng năm và giờ
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ruleDateTime"></param>
        /// <returns></returns>
        public DateTime GetDateFromString(string text, string ruleDateTime)
        {
            DateTime dateReturn = DateTime.Now.AddYears(-30); // mặc định cũ hơn 30 năm
            try
            {
                string day = GetDate(text, ruleDateTime);

                string time = GetHour(text);
                if (time == "") // vẫn không tìm được time
                {
                    time = "00:00:00"; // mặc định
                }
                else // để ý có am hay pm thì cần đổi lại giờ cho đúng
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(ruleDateTime) && ruleDateTime.Contains("tt"))
                        {
                            if (text.ToLower().Contains(" pm")) // 01:43 PM => 13h
                            {
                                string[] arrHour = time.Split(':');
                                if (arrHour.Length == 2)
                                {
                                    int hour = Convert.ToInt32(arrHour[0]);

                                    if (hour < 13)
                                    {
                                        hour = hour + 12;
                                    }

                                    time = hour.ToString() + ":" + arrHour[1];
                                }
                            }
                        }
                    }
                    catch { }
                }

                if (!string.IsNullOrEmpty(day))
                {
                    day = day + " " + time;
                    dateReturn = Convert.ToDateTime(day.Trim());
                }
            }
            catch { }
            return dateReturn;
        }
    }
}
