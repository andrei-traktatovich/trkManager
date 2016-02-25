using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketDataModel;
using BusinessLogic;

namespace TicketManager.Controllers
{
    public class DomainEditorController : ADController
    {
        //
        // GET: /DomainEditor/
        TraktatEntities _ctx = new TraktatEntities(); 
       
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /DomainEditor/Details/5

        public JsonResult GetAll(string sortType = "name")
        {
            var userName = CurrentUser.Name;
            var domainManager = new DomainManager(_ctx);
            var viewModel = domainManager.GetDomainTree(sortType, userName);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckUniqueName(int id, string name)
        {
            var domainManager = new DomainManager(_ctx);
            var isNameUnique = domainManager.IsNameUnique(id, name);
            var data = new { isNameUnique = isNameUnique };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //var url = '@Url.Content(Url.Action("ManageItem"))';

        //                var data = { id: this.ID(), parentID: this.ParentID(), name: this.NewName(), action: this.Action() };

        public JsonResult ManageItem(int id, int parentID, string name, string actionType)
        {
            var domainManager = new DomainManager(_ctx);

            var userID = CurrentUser.TranslatorID;

            DomainManager.ResponseMessage message; 
            
            var isNameUnique = domainManager.IsNameUnique(id, name);
            if (!isNameUnique)
            {
                message = new DomainManager.ResponseMessage(false, "Тематика с таким наименованием уже существует");
            }
            else
            {
                switch (actionType)
                {
                    case "rename": message = domainManager.Rename(id, name, userID); break;
                    case "subitem": message = domainManager.CreateSubItem(parentID, name, userID); break;
                    case "item": message = domainManager.CreateItem(name, userID); break;
                    default: message = new DomainManager.ResponseMessage(false, "Неизвестная команда"); break;
                }
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteItem(int id)
        {
            var userID = CurrentUser.TranslatorID;
            
            var domainManager = new DomainManager(_ctx);
            var result = domainManager.DeleteItem(id, userID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RestoreItem(int id)
        {
            var userID = CurrentUser.TranslatorID;
            
            var domainManager = new DomainManager(_ctx);
            var result = domainManager.RestoreItem(id, userID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /DomainEditor/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /DomainEditor/Create

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
        // GET: /DomainEditor/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /DomainEditor/Edit/5

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
        // GET: /DomainEditor/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /DomainEditor/Delete/5

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
