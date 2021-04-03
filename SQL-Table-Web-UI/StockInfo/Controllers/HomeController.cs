using StockInfo.Models;
using StockInfo.Repository;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using StockInfo.Services;
using Newtonsoft.Json.Linq;

//HomeContoller - This will pass variables to the relevant methods in the Database Reposiotry.


namespace StockInfo.Controllers
{
    public class HomeController : Controller
    {

        private readonly IDatabaseRepository repository;
        private readonly IViewRenderService _viewRenderService;
        public HomeController(IDatabaseRepository repository, IViewRenderService viewRenderService)
        {
            //Initialise interface. 


            this.repository = repository;
            this._viewRenderService = viewRenderService;
        }

      
     
        public ActionResult GetSearchResults(string name)
        {
            //Call the GetSearchResults in DatabaseRepository &
            //Return results in JSON format, in order to populat
            //Jquery Datatable.

            var array = repository.GetSearchResults(name);
            return Json(new { success = true, html = array });
        }

      

        public IActionResult Index()
        {
            //Return view

            return View();
        }

        public IActionResult DeleteStock(int id)
        {
            //Call the DeleteStock method from DatabaseRepository.
            //pass the id variable in order for SQL to delete row by id.

            repository.DeleteStock(id);
            return Redirect("/Home/Index");
        }

        public IActionResult AddStock(string name, string description)
        {
            //Call the AddStock method from DatabaseRepository.
            //name and description variables to be used to popualte new SQL row

            repository.AddStock(name, description);
            return Redirect("/Home/Index");

        }

        public IActionResult GetStockInfo(int itemId)
        {
            //This method is used to popualte the modal that appears when editing
            //one of the table rows.

            try
            {
                //Pass item ID so SQL can find the record that matches id.
                var stockInfo = repository.GetStockInfo(itemId);

                //render partial view with the info retrived by id.
                var html = _viewRenderService.RenderToStringAsync("Home/ModalAddEditStockPartialSearch", stockInfo);

                //return result in JSON format.
                return Json(new { success = true, html = html.Result });
            }
            //error exception in case of bad JSON retrival 
            catch (System.Exception ex)
            {
                return Json(new { success = true, errorMessage = ex.ToString() });
                throw;
            }
            
                      
        }


        public ActionResult UpdateStockInfo(int id, string name, string description)
        {
            //Method to update information of a record once user has modified information from partial view.


            try
            {   
                //call the UpdateStockInfo method from DatabaseRepository and pass the new information variables.
                repository.UpdateStockInfo(id, name, description);

                //return in JSON format.
                return Json(new { success = true });
            }
            //error exception in case of bad JSON retrival 

            catch (System.Exception ex)
            {                
                return Json(new { success = false, errorMessage = ex.ToString() });
            }
                     
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}