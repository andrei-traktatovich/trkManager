using System.Web.Mvc;

namespace TicketManager.Controllers
{
    public class HomeController : ADController
    {
        public ActionResult Index()
        {
            if (!Authenticated)
                return View("UserUnauthenticated"); // TODO 
            
            return View();
        }
        
    }
}
