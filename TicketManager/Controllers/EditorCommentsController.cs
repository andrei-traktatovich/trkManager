using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketDataModel;
using BusinessLogic; 

namespace TicketManager.Controllers
{
    public class EditorCommentsController : ADController
    {
        //
        // GET: /EditorComments/

        public ActionResult Index()
        {
            // workaround, since currently id's of authors are not stored anywhere ... -
            // just the names 
            var allEditors = Context.JobGrades.Select(x => x.Author).Distinct().OrderBy(x => x).ToList();
            allEditors.Insert(0, "Все");

            var editorList = new SelectList(allEditors);

            var allTranslators = Context.Translators
                .Where(x => x.TranslatorRoles.Any(y=>y.RoleID == 1) 
                    && x.s_del == 0 
                    && x.Name.ToLower() != "temp" && !string.IsNullOrEmpty(x.Name))
                .Select(x => new { ID = x.TranslatorID, Name = x.Name })
                .OrderBy(x => x.Name)
                .ToList(); //Where(x=>x.TranslatorRoles.Select(y => y.ID).Contains(1));
            allTranslators.Insert(0, new { ID = -1, Name = "Все" }); 
            var translatorList = new SelectList(allTranslators, dataValueField: "ID", dataTextField: "Name");

            var allLanguageDirections = Context.Languages
                .OrderBy(x => x.grp.Value)
                .Select(x => x.Name).ToList();
            
            ViewData["languageName"] = new SelectList(allLanguageDirections);


            ViewData["DrDnEditors"] = editorList;
            ViewData["DrDnTranslators"] = translatorList;
            ViewData["StartDate"] = DateTime.Now.ToString("dd.MM.yyyy");
            ViewData["EndDate"] = DateTime.Now.AddMonths(-1).ToString("dd.MM.yyyy");
            return View();
        }

        //
        // GET: /EditorComments/Details/5
        /*
         * var searchParameters = {
                 
                filter: $('#txtFilter').val(),
                staff: $('#selectStaff').val(),
                unApproved: $('#chkUnapproved').is(':checked'),
                newEmployee: $('#chkNew').is(':checked'),
                active: $('#chkActive').is(':checked'),
                blackList: $('#chkBlackList').is(':checked'),
                reserve: $('#chkReserve').is(':checked'),
                sort: $('selectSort').val()
            };
         */
        public class GradeSearchParameters
        {
            public string editor { get; set; }
            public int translator { get; set; }
            public string startDate { get; set; }
            public string endDate { get; set; }
            public string languageName { get; set; }
            public string filter { get; set; }
            public int staff { get; set; }
            public bool newEmployee { get; set; }
            public bool unApproved { get; set; }
            public bool active { get; set; }
            public bool blackList { get; set; }
            public bool reserve { get; set; }
            public string sort { get; set; }
            public bool showOrphans { get; set; }
        }
        [HttpPost]
        public ActionResult Details(GradeSearchParameters searchParameters, bool getCount, int page)
        {
            
            GradeSearchParameters searchParams = (GradeSearchParameters)searchParameters;
            int pageLength = 15;
            var query = Context.JobGrades
                .Select(x => x);

            if (!string.IsNullOrWhiteSpace(searchParams.filter))
                query = query.Where(x => x.Translator.Name.Contains(searchParams.filter));

            if (searchParams.staff > 0)
            {
                // do something 
            }

            var excludeList = new List<string>();

            if (!searchParams.newEmployee)
                excludeList.Add("новый");

            if (!searchParams.active)
                excludeList.Add("активный");

            if (!searchParams.unApproved)
                excludeList.Add("непроверен");

            if (!searchParams.blackList)
                excludeList.Add("черный список");

            if (!searchParams.reserve)
                excludeList.Add("резерв");

            if (excludeList.Count() > 0)
                query = query.Where(x => !excludeList.Contains(x.Translator.active));

            // filtering by editor name
            if (searchParams.editor != "Все")
            {
                query = query.Where(x => x.Author == searchParams.editor);
            }
            
            // filtering by translator name
            if (searchParams.translator != -1)
            {
                query = query.Where(x => x.TranslatorID == searchParams.translator);
            }
            
            if (searchParams.languageName != "-")
            {
                query = query.Where(x => x.LegacyDirection == searchParams.languageName);
            }

            // filtering by date
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();

            if (DateTime.TryParse(searchParams.startDate, out startDate))
            {
                if (!DateTime.TryParse(searchParams.endDate, out endDate))
                {
                    endDate = DateTime.Now;
                }
                endDate = endDate.Date.AddDays(1);
                startDate = startDate.Date;

                query = query.Where(x => x.GradeDate >= startDate && x.GradeDate <= endDate);
            }

            var groupedQuery = query
                .OrderBy(x => x.Author)
                .ThenBy(x => x.Translator.Name)
                .ThenBy(x => x.GradeDate)
                .GroupBy(x => x.Translator);
            
            switch(searchParams.sort)
            {
                case "name" :  groupedQuery = groupedQuery.OrderBy(x => x.Key.Name); break;
                case "avgDesc" :  groupedQuery = groupedQuery.OrderBy(x => x.Key.Grades.Average(y => y.Grade ?? 0)); break;
                case "avgAsc" :  groupedQuery = groupedQuery.OrderByDescending(x => x.Key.Grades.Average(y => y.Grade ?? 0)); break;
                default: throw new ArgumentException();
            }

            //startDate = searchParameters.startDate.Date;
            //endDate = searchParameters.endDate.Date.AddDays(1);

            var searchResult = groupedQuery
                .Skip(page * pageLength)
                .Take(pageLength)
                .ToList()
                .Select(x => new
                {
                    trname = x.Key.Name,
                    id = x.Key.TranslatorID,
                    totalAverage = Math.Round(x.Key.Grades.Average(y => y.Grade ?? 0), 2),
                    grades = x.Select(y => new
                    {
                        ID = y.ID,
                        Grade = y.Grade.GetValueOrDefault(0),
                        y.TranslatorID,
                        Spelling = y.Spelling.GetValueOrDefault(),
                        Fact = y.Fact.GetValueOrDefault(),
                        Term = y.Term.GetValueOrDefault(),
                        Grammar = y.Grammar.GetValueOrDefault(),
                        Sense = y.Sense.GetValueOrDefault(),
                        Gaps = y.Gaps.GetValueOrDefault(),
                        Comment = y.Comment,
                        Requirements = y.Requirements.GetValueOrDefault(),
                        Style = y.Style.GetValueOrDefault(),
                        Delay = y.Delay.GetValueOrDefault(),
                        FormatError = y.FormatError ?? 0,
                        BonusForQuality = y.BonusForQuality ?? 0,
                        BonusToNative = y.BonusToNative ?? 0,
                        GradeDate = y.GradeDate.Value.ToString("dd.MM.yy"),
                        y.Author,
                        y.CreatedDate,
                        Direction = y.LegacyDirection,
                        JobNo = y.LegacyJobNo,
                        Domain1 = y.LegacyDomain1,
                        IsDomain1Registered = y.Translator.Domains
                         .Any(z => z.TranslationDirection.LanguageName.Trim().Equals(y.LegacyDirection) 
                             && z.Domain != null && z.Domain.Name.Trim().Equals(y.LegacyDomain1)),
                        Domain2 = y.LegacyDomain2,
                        IsDomain2Registered = y.Translator.Domains
                         .Any(z => z.TranslationDirection.LanguageName.Trim().Equals(y.LegacyDirection)
                             && z.Domain != null && z.Domain.Name.Trim().Equals(y.LegacyDomain2))
                    }).ToList()
                });
             

            // exlude where no orphan grades
            if (searchParameters.showOrphans)
            {
                searchResult = searchResult.Where(x => x.grades.Any(y => !y.IsDomain1Registered || !y.IsDomain2Registered)).ToList();
            }
            int count = 0;
            int totalPages = 0;

            if (getCount)
            {
                count = groupedQuery.Count();

                if (count <= pageLength)
                {
                    if (count > 0)
                    {
                        totalPages = 1;
                    }
                }
                else
                {
                    totalPages = count / pageLength + 1;
                }
            }
            
            var result = new { totalPages = totalPages,
                               page = searchResult
            };
            var jsonResult = new JsonResult() { Data = result, MaxJsonLength = 86753090, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            
            return jsonResult; 
        }

        public JsonResult AddOrUpdateDomain(int translatorID, string languageName, string domainName, int competence = 6, int qa = 2)
        {
            DomainManager.ResponseMessage response;
            var domainManager = new DomainManager(Context);

            languageName = languageName.Trim();
            domainName = domainName.Trim();

            var translator = Context.Translators.Where(x => x.TranslatorID == translatorID).SingleOrDefault();
            if (translator == null)
                return Json(new DomainManager.ResponseMessage(false, "Нет такого исполнителя"), JsonRequestBehavior.AllowGet);
            
            var language = translator.TranslationDirections.Where(x => x.LanguageName == languageName).SingleOrDefault();
            if (language == null)
                return Json(new DomainManager.ResponseMessage(false, "Нет такого языка. Сначала добавьте ему этот язык"), JsonRequestBehavior.AllowGet);
            var languagePairID = language.ID;

            var domain = Context.Domains.Where(x => x.Name == domainName).SingleOrDefault();
            if (domain == null)
                return Json(new DomainManager.ResponseMessage(false, "нет такой тематики"), JsonRequestBehavior.AllowGet);
            var domainID = domain.ID;

            response = domainManager.CheckTranslatorDomain(translatorID, languagePairID, domainID);
            if (response.Success)
            {
                try
                {
                    var userID = CurrentUser.TranslatorID;
                    response = domainManager.AddOrUpdateTranslatorDomain(translatorID, languagePairID, domainID, userID, competence, qa);
                }
                catch (Exception ex)
                {
                    response = new DomainManager.ResponseMessage(false, "Произошла ошибка: " + ex.Message);
                }
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult UpdateGrade(JobGrade grade)
        {
            
            var result = Context.JobGrades.Where(x=>x.ID == grade.ID).SingleOrDefault();
            var translator = Context.Translators.Where(x => x.TranslatorID == grade.TranslatorID).FirstOrDefault();
            grade.Translator = translator;
            if(result != null)
            {
                result.Grade = grade.Grade;
                result.Spelling = grade.Spelling;
                result.Fact = grade.Fact;
                result.Term = grade.Term;
                result.Comment = grade.Comment;
                result.Grammar = grade.Grammar;
                
                Context.SaveChanges();
            }

            return null;
        }
        //
        // GET: /EditorComments/Create
        public ActionResult GetGrade(int gradeID = -1)
        {
            if (gradeID == -1)
                throw new ArgumentException();
            
            var translators = Context.Translators.Where(x => x.Name != null).OrderBy(x => x.Name).Select(x => new { id = x.TranslatorID, name = x.Name }).ToList();
            var directions = Context.Languages.Distinct().OrderBy(x => x.Name).Select(x => new { name = x.Name }).ToList();
            var domains = Context.Domains.OrderBy(x => x.Name).Select(x => new { name = x.Name }).ToList();
            var grade = Context.JobGrades.Where(x => x.ID == gradeID).FirstOrDefault();
            
            if(grade == null)
                throw new InvalidOperationException();
            var result = new
            {
                grade = grade.Grade,
                translators = translators,
                selectedTranslator = grade.TranslatorID,
                directions = directions,
                selectedDirection = grade.LegacyDirection.Trim(),
                domains = domains,
                selectedDomain1 = grade.LegacyDomain1.Trim(),
                selectedDomain2 = grade.LegacyDomain2.Trim(),
                jobno = grade.LegacyJobNo,
                Grammar = grade.Grammar ?? 0,
                Spelling = grade.Spelling ?? 0,
                Fact = grade.Fact ?? 0,
                Term = grade.Term ?? 0,
                Sense = grade.Sense ?? 0,
                Gaps = grade.Gaps ?? 0,
                Requirements = grade.Requirements ?? 0,
                Style = grade.Style ?? 0,
                Delay = grade.Delay ?? 0,
                FormatError = grade.FormatError ?? 0,
                BonusForQuality = grade.BonusForQuality ?? 0,
                BonusToNative = grade.BonusToNative ?? 0,
                Comment = grade.Comment ?? ""
            };
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /EditorComments/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /EditorComments/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /EditorComments/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /EditorComments/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /EditorComments/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
