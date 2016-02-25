using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketDataModel;
 

namespace BusinessLogic
{
    public class DomainTreeViewModel
    {
        public DomainTreeViewModel(bool withCounts, string userName)
        {
            Items = new List<DomainItem>();
            WithCounts = withCounts;
            UserName = userName;
        }

        public string UserName { get; private set; }

        public bool WithCounts { get; private set; }

        public class DomainItemNameComparer : IComparer<DomainTreeViewModel.DomainItem>
        {
            public int Compare(DomainItem x, DomainItem y)
            {
                return (x.Name.CompareTo(y.Name));
            }
        }

        public class DomainItemTranslatorNumberReverseComparer : IComparer<DomainTreeViewModel.DomainItem>
        {

            public int Compare(DomainItem x, DomainItem y)
            {
                return -x.UsedBy.CompareTo(y.UsedBy);
            }
        }

        public class DomainItemJobNumberReverseComparer : IComparer<DomainTreeViewModel.DomainItem>
        {

            public int Compare(DomainItem x, DomainItem y)
            {
                return -x.UsedIn.CompareTo(y.UsedIn);
            }
        }

        public List<DomainItem> Items { get; private set; }
        public DomainItem Add(DomainItem item)
        {
            this.Items.Add(item);
            return item;
        }

        public void Sort(IComparer<DomainTreeViewModel.DomainItem> comparer)
        {
            this.Items.Sort(comparer);
            foreach (var item in Items)
                item.Children.Sort(new DomainItemNameComparer());
        }

        public class DomainItem
        {
            public DomainItem(Domain domain, bool withCounts = false)
            {
                this.ID = domain.ID;
                this.Name = domain.Name;
                this.ParentId = domain.ParentID;
                if (withCounts)
                {
                    this.UsedBy = domain.TranslatorsDomains.Select(x => x.TranslatorID).Distinct().Count();
                    this.UsedIn = domain.JobsWhereThisDomainIsPrimary.Union(domain.JobsWhereThisIsSecondaryDomain).Select(x => x.Name).Distinct().Count();
                }
                this.Children = new List<DomainItem>();
                this.CreatedBy = domain.CreatedBy.Name;
                this.CreatedDate = domain.CreatedDate.HasValue ? domain.CreatedDate.Value.ToString("dd.MM.yy") : "нет данных";
                this.ModifiedBy = domain.ModifiedBy == null ? "" :domain.ModifiedBy.Name;
                this.ModifiedDate = domain.ModifiedDate.HasValue ? domain.ModifiedDate.Value.ToString("dd.MM.yy") : "";
                this.IsDeleted = domain.IsDeleted;
            }
            public string CreatedBy { get; private set; }
            public string ModifiedBy { get; private set; }
            public string CreatedDate { get; private set; }
            public string ModifiedDate { get; private set; }
            public bool IsDeleted { get; private set; }
            public int UsedIn { get; private set; }
            public int UsedBy { get; private set; }
            public int ID { get; private set; }
            public string Name { get; private set; }
            public int? ParentId { get; private set; }
            public bool IsParent { get { return this.ParentId.HasValue; } }
            public List<DomainItem> Children { get; private set; }
            public void AddChild(DomainItem child)
            {
                this.Children.Add(child);
            }
        }
    }

    public class DomainManager
    {
        public class ResponseMessage
        {
            public ResponseMessage(bool success, string message, DomainTreeViewModel.DomainItem item = null)
            {
                Success = success;
                ErrorMessage = message;
                Item = item;
            }

            public bool Success { get; private set; }
            public string ErrorMessage { get; private set; }
            public DomainTreeViewModel.DomainItem Item { get; set; } 
        }
        
        private readonly TraktatEntities _ctx;
        private Dictionary<string, IComparer<DomainTreeViewModel.DomainItem>> _sorters 
            = new Dictionary<string, IComparer<DomainTreeViewModel.DomainItem>>();
        public DomainManager(TraktatEntities ctx)
        {
            _ctx = ctx;
            _sorters.Add(BYNAME, new DomainTreeViewModel.DomainItemNameComparer());
            _sorters.Add(USEDBY, new DomainTreeViewModel.DomainItemTranslatorNumberReverseComparer());
            _sorters.Add(USEDIN, new DomainTreeViewModel.DomainItemJobNumberReverseComparer());
        }

        private const string BYNAME = "name";
        private const string USEDBY = "usedby";
        private const string USEDIN = "usedin";

        public bool IsNameUnique(int id, string domainName)
        {
            domainName = domainName.Trim().ToLower();
            var thereAreDuplicates = _ctx.Domains.Any(x => x.ID != id && x.Name.ToLower().Equals(domainName));
            return !thereAreDuplicates;
        }

        public DomainTreeViewModel GetDomainTree(string sortType, string userName)
        {
            if(!_sorters.ContainsKey(sortType.ToLower())) 
                throw new ArgumentException("sortType");

            var comparer = _sorters[sortType];
            
            var withCounts = sortType != BYNAME;
            
            var domains = _ctx.Domains.ToList();
            var domainTreeViewModel = new DomainTreeViewModel(withCounts, userName);
            var topDomains = domains.Where(x => x.ID > 0 && x.ParentID == 0); // & not isdeleted;
            foreach (var topDomain in topDomains)
            {
                var domainItem = domainTreeViewModel.Add(new DomainTreeViewModel.DomainItem(topDomain, withCounts));
                foreach (var subdomain in domains.Where(x => x.ParentID.HasValue && x.ParentID.Value == topDomain.ID))
                {
                    domainItem.AddChild(new DomainTreeViewModel.DomainItem(subdomain, withCounts));
                }
            }
            domainTreeViewModel.Sort(comparer);
             
            return domainTreeViewModel;
        }





        public ResponseMessage Rename(int id, string name, int userID)
        {
            var item = _ctx.Domains.Find(id);
            if (item == null)
                return new ResponseMessage(false, "Такой тематики не существует");
            item.Name = name;
            item.ModifiedByID = userID;
            item.ModifiedDate = DateTime.Now;
            var translatorDomainsToUpdate = item.TranslatorsDomains.ToList();
            foreach (var td in translatorDomainsToUpdate)
            {
                td.pl_vid = name;
            }
            _ctx.SaveChanges();
            var domain = new DomainTreeViewModel.DomainItem(item);
            return new ResponseMessage(true, "", domain);
        }

        public ResponseMessage CreateSubItem(int parentID, string name, int userID)
        {
            var parentItem = _ctx.Domains.Find(parentID);
            if (parentItem == null)
                return new ResponseMessage(false, "Такой родительской тематики не существует");

            var item = new Domain
            {
                Name = name,
                CreatedByID = userID,
                CreatedBy = _ctx.Translators.Find(userID),
                CreatedDate = DateTime.Now,
                ID = _ctx.Domains.Max(x => x.ID) + 1,
                ParentID = parentID, 
                Description = "",
                IsDeleted = false
            };
            _ctx.Domains.Add(item);
            ResponseMessage message;
            if (TrySaveChanges(out message))
            {
                var domain = new DomainTreeViewModel.DomainItem(item);
                return new ResponseMessage(true, "", domain);
            }
            else return message;

        }

        public ResponseMessage CreateItem(string name, int userID)
        {
            var item = new Domain
            {
                Name = name,
                CreatedByID = userID,
                CreatedBy = _ctx.Translators.Find(userID),
                CreatedDate = DateTime.Now,
                ID = _ctx.Domains.Max(x => x.ID) + 1,
                Description = "",
                ParentID = 0,
                IsDeleted = false
            };
            _ctx.Domains.Add(item);
            ResponseMessage message;
            if (TrySaveChanges(out message))
            {
                var domain = new DomainTreeViewModel.DomainItem(item);
                return new ResponseMessage(true, "", domain);
            }
            else
                return message;
        }

        public ResponseMessage DeleteItem(int id, int userID)
        {
            var item = _ctx.Domains.Find(id);
            if(item == null)
                return new ResponseMessage(false, "Не найдена такая тематика");
            DeleteDomain(item, userID);
            
            var items = _ctx.Domains.Where(x => x.ParentID == id && x.IsDeleted == false).ToList();
            foreach(var i in items)
                DeleteDomain(i, userID);
            ResponseMessage message;
            if (TrySaveChanges(out message))
            {
                return new ResponseMessage(true, null);
            }
            else
                return message;
        }

        public ResponseMessage RestoreItem(int id, int userID)
        {
            var item = _ctx.Domains.Find(id);
            if (item == null)
                return new ResponseMessage(false, "не найдена такая тематика");
            var parentItem = _ctx.Domains.Find(item.ParentID);
            if (parentItem == null)
                throw new Exception(string.Format("нарушена иерахия тематик, тематика с ид={0} не имеет отца", item.ParentID));
            else
            {
                if (parentItem.IsDeleted)
                    return new ResponseMessage(false, "Невозможно восстановить тематику, т.к. ее родительская тематика удалена. Сначала восстановите родительскую тематику.");
                item.IsDeleted = false;
                item.ModifiedByID = userID;
                item.ModifiedDate = DateTime.Now;
            }
            ResponseMessage message;
            if (TrySaveChanges(out message))
            {
                return new ResponseMessage(true, null);
            }
            else
                return message;
        }

        private void DeleteDomain(Domain item, int userID)
        {
            item.IsDeleted = true;
            item.ModifiedByID = userID;
            item.ModifiedDate = DateTime.Now;
        }

        private bool TrySaveChanges(out ResponseMessage message)
        {
            var result = false;
            message = null;
            try
            {
                _ctx.SaveChanges();
                result = true;
            }
            catch (DbEntityValidationException ex)
            {
                string error = "";
                foreach (DbEntityValidationResult validationResult in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationResult.ValidationErrors)
                        error += Environment.NewLine + " - " + validationError.ErrorMessage + " (" + validationError.PropertyName + ")";
                }
                message = new ResponseMessage(false, error);
            }
            return result;
        }

        public ResponseMessage CheckTranslatorDomain(int translatorID, int languagePairID, int domainID)
        {
            var translator = _ctx.Translators.Where(x => x.TranslatorID == translatorID).SingleOrDefault();
            if (translator == null)
                return new ResponseMessage(false, "Нет такого исполнителя");
            var direction = translator.TranslationDirections.Where(x => x.ID == languagePairID).SingleOrDefault();
            if (direction == null)
                return new ResponseMessage(false, "У исполнителя нет такой языковой пары");
            var domain = direction.DomainsInLanguageDirection.Where(x => x.Domain.ID == domainID).SingleOrDefault();
            if (domain != null && !domain.is_del)
                return new ResponseMessage(false, "Уже есть такая тематика в этом направлении");
            else
                return new ResponseMessage(true, "");
        }

        public ResponseMessage AddOrUpdateTranslatorDomain(int translatorID, int languagePairID, int domainID, int userID, int competence, int qa)
        {
            var responseMessage = new ResponseMessage(false, "Неизвестная ошибка");
            var domain = _ctx.Domains.Where(x=>x.ID == domainID).SingleOrDefault();
            if(domain == null)
                return new ResponseMessage(false, "Такой тематики не существует");
            if(domain.IsDeleted) 
                return new ResponseMessage(false, "Эта тематика не является активной");
            var name = domain.Name;
            try
            {
                var domainToUpdate = _ctx.InternalDomains.Where(x => x.TranslatorID == translatorID && x.LanguageID == languagePairID
                    && x.DomainID == domainID).SingleOrDefault();
                if (domainToUpdate == null)
                {

                    _ctx.InternalDomains.Add(new InternalDomain
                    {
                        Competence = competence,
                        created_by = userID,
                        created_date = DateTime.Now,
                        DomainID = domainID,
                        IntDomainID = _ctx.InternalDomains.Max(x => x.IntDomainID) + 1,
                        LanguageID = languagePairID,
                        pl_vid = name,
                        is_del = false,
                        QA = qa.ToString(),
                        TranslatorID = translatorID
                    });
                    responseMessage = new ResponseMessage(true, "Тематика добавлена");
                }
                else
                {
                    domainToUpdate.Competence = competence;
                    domainToUpdate.QA = qa.ToString();
                    domainToUpdate.modified_by = userID;
                    domainToUpdate.modified_date = DateTime.Now;
                    domainToUpdate.is_del = false;
                    responseMessage = new ResponseMessage(true, "Тематика обновлена");
                }
                _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                return new ResponseMessage(false, ex.Message);
            }
            return responseMessage;
        }

        
    }

    

}

