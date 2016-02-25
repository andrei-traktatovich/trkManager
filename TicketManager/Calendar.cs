using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using TicketDataModel;


namespace TicketManager
{
    public class CalendarInfo
    {
        public EmployeeStaffStatus Status { get; set; }
        public string Office { get; set; }
        public int OfficeID { get; set; }
        public bool StandBy { get; set; }
        public bool Confirmed { get; set; }
        public string Author { get; set; }
        public string Comment { get; set; }
        public string LastChanged { get; set; }
    }

    public struct EmployeeStaffStatus
    {
        public int ID;
        public string Name;
        public bool IsPaid;
    }

    public class GeneralCalendarInfo
    {
        public int Month {get;set;}
        public int Year { get; set; }
        public int MonthLength { get { return DateTime.DaysInMonth(Year,Month); }}
        public List<CalendarDataByTranslator> CalendarLines { get; set; }
        public GeneralCalendarInfo(int month, int year)
        {
            Month = month + 1;
            Year = year + 1;
            CalendarLines = new List<CalendarDataByTranslator>();
        }
    }

    public struct CalendarDataByTranslator
    {
        public string TranslatorName;
        public List<CalendarInfo> CalendarsDays { get; set; }
    }

    public class CalendarSelection
    {
        public int Status { get; set; }
        public int Office { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Id { get; set; }
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int EndX { get; set; }
        public int EndY { get; set; }
        public bool StandBy { get; set; }
        public bool Confirmed { get; set; }
        public string Comment { get; set; }
    }

    public class Calendar
    {
        private TraktatEntities _ctx;

        private Translator CurrentUser;

        public Calendar(TraktatEntities ctx, Translator user)
        {
            _ctx = ctx;
            CurrentUser = user;
        }

        public string GetCurrentCalendarStatus(Translator translator, DateTime currentDate)
        {
            var statuses = translator.CalendarPeriods;
            return GetCurrentCalendarRecord(statuses, currentDate);
        }

        public CalendarPeriod GetCurrentCalendarPeriod(int translatorID, DateTime currentDate)
        {
            currentDate = currentDate.Date;
            var results = _ctx.CalendarPeriods.Where(x => x.TranslatorID == translatorID && x.StartDate <= currentDate &&
                    (x.EndDate.HasValue && x.EndDate.Value >= currentDate)).OrderByDescending(x => x.CreatedDate);
            var result = results.FirstOrDefault();
            return result;
 
        }
        private string GetCurrentCalendarRecord(IEnumerable<CalendarPeriod> statuses, DateTime currentDate)
        {
            if (statuses.Count() == 0)
                return "нет данных";
            else
            {
                var recordedStatus = statuses.Where(x => x.StartDate <= currentDate.Date &&
                    (x.EndDate.HasValue && x.EndDate.Value.Date >= currentDate.Date)).LastOrDefault();
                if (recordedStatus == null)
                    return "нет данных";
                else
                    return recordedStatus.StaffStatus;
            }
        }

        public string GetCurrentCalendarStatus(int translatorID, DateTime currentDate)
        {
            var statuses = _ctx.CalendarPeriods.Where(x => x.TranslatorID == translatorID);
            return GetCurrentCalendarRecord(statuses, currentDate);
        }
        
        // Note: month in .Net format
        public CalendarInfo[] GetCalendar(Translator translator = null, int month = -1, int year = -1)
        {
            var currentDate = DateTime.Now.Date;
            #region GetCalendarValidation
            // validating
            // if month = -1 or wrong month number then month = this month
            if (month < 1 || month > 12)
                month = currentDate.Month;

            // if year = -1 year = this year
            if (year == -1)
                year = currentDate.Year;

            // if translator = null throw error 
            if (translator == null)
                throw new Exception("No Translator specified");
            #endregion
            var MaxDays = DateTime.DaysInMonth(year, month);
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddDays(-1 + MaxDays);
            return GetCalendar(translator, startDate, endDate);
        }

        public CalendarInfo[] GetCalendar(Translator translator, DateTime startDate, DateTime endDate)
        {
            var MaxDays = (int)(endDate - startDate).TotalDays +1;
            var result = new CalendarInfo[MaxDays];

            //Initialize CalendarInfo 
            var defaultStatus = 1;
            // will need to change model & establish relation through podr_now !!! Ned to check
            // with Igor the allow null in podr_now
            var defaultOffice = _ctx.Offices.Where(x => x.ID == translator.Podr_now).Single();
            
            var defaultOfficeName = defaultOffice.Name;
            defaultOfficeName = defaultOfficeName.Length > 17 ? defaultOfficeName.Substring(0, 16) + "..." : defaultOfficeName; 

            var defaultOfficeID = defaultOffice.ID;

            var thisDate = startDate.Date;
            for (var i = 0; i < MaxDays; i++)
            {
                // Set this calendar info based on calendar periods from ctx:
                
                var period = _ctx.CalendarPeriods
                    .Where(x => x.TranslatorID == translator.TranslatorID && x.StartDate <= thisDate && x.EndDate >= thisDate).OrderByDescending(x=>x.ID)
                    .FirstOrDefault();
                // do something if several found 
                // if there is one found,
                //      set as appropriate
                if (period != null)
                {
                    try
                    {
                        result[i] = new CalendarInfo
                        {
                            Status = period.StaffStatusEntity != null ? 
                                new EmployeeStaffStatus { ID = (int)period.StaffStatusEntity.ID, IsPaid = period.StaffStatusEntity.IsPaid, Name = period.StaffStatusEntity.Name } :
                                new EmployeeStaffStatus(),
                            Office = period.OfficeID == 0 ? "" : period.Office.Name.Length > 17 ? period.Office.Name.Substring(0,16) + "..." : period.Office.Name, 
                            OfficeID = period.OfficeID,
                            StandBy = period.StandBy,
                            Confirmed = period.Confirmed,
                            Author = period.Author,
                            Comment = period.Comment,
                            LastChanged = period.CreatedDate.ToString("dd.MM.yy")
                        };

                    }
                    catch (Exception ex)
                    {
                        var s = "";
                    }
                }
                 else
                {	//skip (will be default)
                    var isPreHirePeriod = thisDate < translator.CreationDate;
                    
                    
                    EmployeeStaffStatus status;
                    if (isPreHirePeriod)
                        status = new EmployeeStaffStatus { ID = (int)StaffStatuses.NotWorking, Name = "Не работает", IsPaid = false };
                    else
                    {
                        if(thisDate.DayOfWeek == DayOfWeek.Sunday || thisDate.DayOfWeek == DayOfWeek.Saturday)
                            status = new EmployeeStaffStatus { ID = (int)StaffStatuses.Holiday, Name = "Выходной", IsPaid = false };
                        else 
                            status = new EmployeeStaffStatus { ID = (int)StaffStatuses.Present, Name = "Явка", IsPaid = true };
                    }
                    result[i] = new CalendarInfo
                    {
                            Status = status,
                            OfficeID = isPreHirePeriod ? 0 : defaultOfficeID,
                            Office = isPreHirePeriod ? "" : (thisDate < DateTime.Now.Date) ? "нет данных" : defaultOfficeName,
                            LastChanged = "",
                            Comment = ""
                    };
                }
                thisDate = thisDate.AddDays(1);
            }
            return result;
        }

        public class ClientDay : IEquatable<ClientDay>
        {
            public int OfficeID { get; set; }
            public int StatusID { get; set; }
            public string Comment { get; set; }
            public string Author { get; set; }
            public DateTime LastChanged { get; set; }
            public bool Confirmed { get; set; }
            public bool StandBy { get; set; }

            public bool Equals(ClientDay other)
            {
                return (this.OfficeID == other.OfficeID 
                    && this.StatusID == other.StatusID 
                    && this.Comment == other.Comment 
                    && this.Author == other.Author 
                    && this.LastChanged == other.LastChanged 
                    && this.Confirmed == other.Confirmed 
                    && this.StandBy == other.StandBy);
            }
        }

        public bool TryUpdateCalendarFromClient(int translatorID, int javaScriptMonth, int year, ClientDay[] Days = null)
        {
            if (Days == null || Days.Length == 0)
                return false;

            int start = 0;
            int end = 0;
            var month = javaScriptMonth + 1;

            while (start < Days.Length && end < Days.Length)
            {
                
                while (Days[start].Equals(Days[end]) && (++end) < Days.Length - 1)
                {
                    // do nothing
                }
                var startDate = new DateTime(year, month, start + 1);
                var endDate = new DateTime(year, month, end);
                var day = Days[start];

                if(!TryUpdateCalendar(translatorID, day.StatusID, startDate, endDate, day.OfficeID, day.StandBy, day.Confirmed, day.Comment, day.Author, day.LastChanged))
                    return false;

                start = end;
            }

            return true;
        }

        public bool TryUpdateCalendar(int translatorID, int StatusID, DateTime startDate, DateTime endDate, int officeID, bool standBy, bool confirmed, string comment, string author, DateTime lastChanged)
        {
            try
            {
                UpdateCalendar(translatorID, (TicketDataModel.StaffStatuses)StatusID, startDate, endDate, officeID, standBy, confirmed, comment);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        private string TruncateString(string source, int len = 30)
        {
            if (source.Length > len)
                return source.Substring(0, len - 1);
            else return source;
        }
        public void UpdateCalendar(int translatorID, TicketDataModel.StaffStatuses Status, DateTime StartDate, DateTime EndDate,
            int OfficeID, bool StandBy, bool confirmed, string comment) 
        {
            // validate input data
            // ...
 
            if (StartDate > EndDate)
            {
                var temp = StartDate;
                StartDate = EndDate;
                EndDate = StartDate;
            }
            if (string.IsNullOrEmpty(comment))
                comment = "";
            
            

            byte statusCode = 1; // this is the status code stored in the database

            using (var ctx = new TraktatEntities())
            {

                var st = ctx.StaffStatuses.Where(x => x.ID == Status).SingleOrDefault();

                var statusName = st != null ? st.ShortCode : "Я ";

            var newPeriod = new CalendarPeriod
            {
                OfficeID = OfficeID,
                TranslatorID = translatorID,
                StartDate = StartDate,
                EndDate = EndDate,
                StaffStatusID = Status,
                StaffStatus = statusName,

                Author = TruncateString(CurrentUser.Name, 30),
                StatusCode = statusCode, 
                Comment = comment, 
                StandBy = StandBy, 
                CreatedDate = DateTime.Now, 
                CreatorID = CurrentUser.TranslatorID, 
                Confirmed = confirmed
            };

            // get and check all calendar periods that may interlock with the current calendar period.

             

                var otherPeriods = ctx.CalendarPeriods.Where(x => x.TranslatorID == translatorID).ToList();

                for (var i = 0; i < otherPeriods.Count; i++)
                {
                    var p = otherPeriods[i];
                    if (IsContained(p, newPeriod))
                        ctx.CalendarPeriods.Remove(p);
                    if (Contains(p, newPeriod))
                        SplitPeriod(ctx, ref p, newPeriod);
                }
                ctx.CalendarPeriods.Add(newPeriod);
                
                
                    ctx.SaveChanges();
                
                if((newPeriod.StartDate.Value.Date <= DateTime.Now.Date) && (newPeriod.EndDate.Value.Date >= DateTime.Now.Date))
                {
                    var translator = ctx.Translators.Find(translatorID);
                    translator.OfficeID = OfficeID;
                    ctx.SaveChanges();
                }
            }
        }

        private void SplitPeriod(TraktatEntities ctx, ref CalendarPeriod oldPeriod, CalendarPeriod newPeriod)
        {
            var oldStartDate = oldPeriod.StartDate.Value.Date;
            var oldEndDate = oldPeriod.EndDate.Value.Date;
            
            if (oldStartDate >= newPeriod.StartDate.Value.Date)
            {
                oldPeriod.StartDate = newPeriod.EndDate.Value.Date.AddDays(1);
                return;
            }
            if (oldEndDate <= newPeriod.EndDate.Value.Date)
            {
                oldPeriod.EndDate = newPeriod.StartDate.Value.Date.AddDays(-1);
            }
            var additionalPeriod = new CalendarPeriod
            {
                StartDate = newPeriod.EndDate.Value.Date.AddDays(1),
                EndDate = oldPeriod.EndDate,
                Author = oldPeriod.Author,
                Comment = oldPeriod.Comment, OfficeID = oldPeriod.OfficeID, StaffStatus = oldPeriod.StaffStatus, 
                StatusCode = oldPeriod.StatusCode, StaffStatusID = oldPeriod.StaffStatusID,
                TranslatorID = oldPeriod.TranslatorID, CreatedDate = oldPeriod.CreatedDate
            };

            oldPeriod.EndDate = newPeriod.StartDate.Value.Date.AddDays(-1); 
            
            ctx.CalendarPeriods.Add(additionalPeriod);
            ctx.SaveChanges();
        }
        private bool Contains(CalendarPeriod periodToTest, CalendarPeriod thisPeriod)
        {
            if (periodToTest.StartDate.Value.Date <= thisPeriod.StartDate.Value.Date && periodToTest.EndDate.Value.Date >= thisPeriod.EndDate.Value.Date)
                return true;
            else
                return false;
        }
        private bool IsContained(CalendarPeriod periodToTest, CalendarPeriod thisPeriod)
        {
            if (periodToTest.StartDate.Value.Date >= thisPeriod.StartDate.Value.Date && periodToTest.EndDate.Value.Date <= thisPeriod.EndDate.Value.Date)
                return true;
            else
                return false;
        }

        public DateTime GetDateFromWeekDayAndWeek(int Month, int Year, int StartWeekDay, int WeekNumber)
        {
            DateTime result;
            int firstWeekDay = (int)new DateTime(Year, Month, 1).DayOfWeek;
            if (firstWeekDay == 0)
                firstWeekDay = 7;
            var day = 1 + (StartWeekDay - firstWeekDay) + (WeekNumber - 1) * 7;

            result = new DateTime(Year, Month, day).Date;
            return result;
        }

        public void ConfirmStatus(int p, DateTime date)
        {
            var currentPeriod = GetCurrentCalendarPeriod(p, date);
            if (currentPeriod != null)
            {
                currentPeriod.Confirmed = true;
                currentPeriod.CreatedDate = DateTime.Now;
                
                currentPeriod.Author = CurrentUser.Name;
                currentPeriod.CreatorID = CurrentUser.TranslatorID;
                UpdateCalendar(p, currentPeriod.StaffStatusID, date.Date, date.Date, currentPeriod.OfficeID, currentPeriod.StandBy, true, currentPeriod.Comment);
            }
            else
            {
                var currentOffice = _ctx.Translators.Where(x => x.TranslatorID == p).Select(y => y.OfficeID).Single();

                currentPeriod = new CalendarPeriod()
                {
                    Confirmed = true,
                    CreatedDate = DateTime.Now,
                    Author = CurrentUser.Name,
                    Comment = "",
                    CreatorID = CurrentUser.TranslatorID,
                    OfficeID = currentOffice.Value,
                    StaffStatus = "Я ",
                    TranslatorID = p,
                    EndDate = date.Date,
                    StartDate = date.Date,
                    StatusCode = 1,
                    StandBy = false, 
                    StaffStatusID = StaffStatuses.Present
                };
                UpdateCalendar(p, currentPeriod.StaffStatusID, date.Date, date.Date, currentPeriod.OfficeID, currentPeriod.StandBy, true, currentPeriod.Comment);

            }
            
        }

        internal void UpdateTempOffice(int id, int officeID, DateTime date)
        {
            var currentPeriod = GetCurrentCalendarPeriod(id, date);
            if (currentPeriod != null)
            {
                currentPeriod.OfficeID = officeID;
                currentPeriod.CreatedDate = DateTime.Now;
                currentPeriod.Author = CurrentUser.Name;
                currentPeriod.CreatorID = CurrentUser.TranslatorID;
                UpdateCalendar(id, currentPeriod.StaffStatusID, date.Date, date.Date, currentPeriod.OfficeID, currentPeriod.StandBy, true, currentPeriod.Comment);
            }
            else
            {
                currentPeriod = new CalendarPeriod()
                {
                    CreatedDate = DateTime.Now,
                    Author = CurrentUser.Name,
                    Comment = "",
                    CreatorID = CurrentUser.TranslatorID,
                    OfficeID = officeID,
                    StaffStatus = "Я ",
                    TranslatorID = id,
                    EndDate = date.Date,
                    StartDate = date.Date,
                    StatusCode = 1,
                    StandBy = false,
                    StaffStatusID = StaffStatuses.Present
                };

                UpdateCalendar(id, currentPeriod.StaffStatusID, date.Date, date.Date, currentPeriod.OfficeID, currentPeriod.StandBy, true, currentPeriod.Comment);
            }
        }

        internal void UpdateTempStatus(int id, int newStatus, DateTime date)
        {
            var currentPeriod = GetCurrentCalendarPeriod(id, date);
            var translator = _ctx.Translators.Find(id);

            if (currentPeriod != null)
            {
                currentPeriod.CreatedDate = DateTime.Now;
                currentPeriod.Author = CurrentUser.Name;
                currentPeriod.CreatorID = CurrentUser.TranslatorID;
                UpdateCalendar(id, (StaffStatuses)newStatus, date.Date, date.Date, currentPeriod.OfficeID, currentPeriod.StandBy, true, currentPeriod.Comment);
            }
            else
            {
                currentPeriod = new CalendarPeriod()
                {
                    CreatedDate = DateTime.Now,
                    Author = CurrentUser.Name,
                    Comment = "",
                    CreatorID = CurrentUser.TranslatorID,
                    OfficeID = translator.OfficeID.Value,
                    StaffStatus = _ctx.StaffStatuses.Find((StaffStatuses)newStatus).ShortCode,
                    TranslatorID = id,
                    EndDate = date.Date,
                    StartDate = date.Date,
                    StatusCode = (byte)newStatus,
                    StandBy = false,
                    StaffStatusID = (StaffStatuses)newStatus
                };

                UpdateCalendar(id, (StaffStatuses)newStatus, date.Date, date.Date, currentPeriod.OfficeID, currentPeriod.StandBy, true, currentPeriod.Comment);
            }
        }
    }
}
