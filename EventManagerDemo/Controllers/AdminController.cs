using EventManagerDemo.Models;
using EventManagerDemo.Utils;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EventManagerDemo.Controllers
{
    public class AdminController : Controller
    {
        GraphServiceClient graphClient;
        GraphService graphService;
        ManagedEventRepository managedEventRepository;
        SchemaExtensionsRepository schemaExtensionsRepository;

        public AdminController()
        {
            graphClient = SDKHelper.GetAuthenticatedClient();
            graphService = new GraphService(graphClient);
            managedEventRepository = new ManagedEventRepository(graphClient);
            schemaExtensionsRepository = new SchemaExtensionsRepository(graphClient);
        }

        public ActionResult Index(string SelectedSchemaExtension)
        {
            bool success = Enum.TryParse<SchemaExtensionTypes>(SelectedSchemaExtension, out SchemaExtensionTypes result);

            if (!success)
            {
                SelectedSchemaExtension = "";
            }

            var viewModel = new SchemaExtensionsManagementViewModel
            {
                SelectedSchemaExtension = SelectedSchemaExtension
            };

            return View(viewModel);
        }

        public ActionResult TestJSchema()
        {
            var result = schemaExtensionsRepository.ToSchemaExtensionPropertiesCollection(typeof(Models.JsonHelpers.MsEventManager_GroupExtensions));
            return View();
        }


        [Authorize, HandleAdalException]
        public async Task<ActionResult> Email()
        {
            GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();
            ViewBag.Email = await graphService.GetMyEmailAddress(graphClient);
            return View();
        }

        [Authorize, HandleAdalException]
        public async Task<ActionResult> IsSchemaExtensionPresent(SchemaExtensionTypes SchemaExtension)
        {
            bool b = true;
            bool successful = true; 
            
            b = await schemaExtensionsRepository.IsSchemaExtensionPresent(SchemaExtension);               

            if (successful)
            {
                ViewBag.Title = b ? $"SchemaExtension {SchemaExtension} is present" : $"SchemaExtension {SchemaExtension} is not present";
            }
            else
            {
                ViewBag.Title = "Unsuccessful";
                ViewBag.Message = $"{SchemaExtension} not supported";
            }
            return View("Message");
        }

        [Authorize, HandleAdalException]
        public async Task<ActionResult> DeleteSchemaExtension(SchemaExtensionTypes SchemaExtension)
        {
            try
            {
                await schemaExtensionsRepository.DeleteSchemaExtension(SchemaExtension);

                //await schemaExtensionsRepository.SetSchemaExtensionStatusInDevelopment<Models.JsonHelpers.ManagedEventSchemaExtension>();
                ViewBag.Title = $"Deleting of schema extension {SchemaExtension} successful";
                return View("Message");
            }
            catch (Exception ex)
            {
                if (ex is AdalException)
                {
                    throw ex;
                }
                return RedirectToAction("Index", "Error", new { message = ex.Message });
            }
        }

        [Authorize, HandleAdalException]
        public async Task<ActionResult> UpdateSchemaExtension()
        {
            await schemaExtensionsRepository.UpdateManagedEventSchemaExtension();
            ViewBag.Title = "Patching of schema extension successful";
            return View("Message");
        }

        [Authorize, HandleAdalException]
        public async Task<ActionResult> SetStatus(SchemaExtensionTypes SchemaExtension, Models.JsonHelpers.SchemaExtensionStatusTypes status)
        {
            try
            {
                await schemaExtensionsRepository.SetSchemaExtensionStatus(SchemaExtension, status);
                ViewBag.Title = "Successful";
                ViewBag.Message = $"{schemaExtensionsRepository.GetSchemaExtensionId(SchemaExtension)} status set to {status}";
                return View("Message");
            }
            catch (Exception ex)
            {
                if (ex is AdalException)
                {
                    throw ex;
                }

                return RedirectToAction("Index", "Error", new { message = ex.Message });
            }
        }

        [Authorize, HandleAdalException]
        public async Task<ActionResult> SetStatusToAvailable(SchemaExtensionTypes SchemaExtension)
        {
            // Patch the added schema extension. Throws an exception when it does not yet exist            
            return await SetStatus(SchemaExtension, Models.JsonHelpers.SchemaExtensionStatusTypes.Available);
        }

        [Authorize, HandleAdalException]
        public async Task<ActionResult> AddSchemaExtension(SchemaExtensionTypes SchemaExtension)
        {
            try
            {
                // add a new schema extension. Throws an exception when it already exists         
                await schemaExtensionsRepository.AddSchemaExtension(SchemaExtension);
                ViewBag.Title = "Successful";
                ViewBag.Message = $"{SchemaExtension} added";
                return View("Message");
            }
            catch (Exception ex)
            {
                if (ex is AdalException)
                {
                    throw ex;
                }

                return RedirectToAction("Index", "Error", new { message = ex.Message });
            }
        }

        [Authorize]
        public async Task<ActionResult> UploadExcelsheet(string id)
        {
            
            var di = await managedEventRepository.PutExcelsheet(id, FileHelper.GetAbsolutePath(this, SettingsHelper.local_path_to_excelsheet), SettingsHelper.drive_path_to_excelsheet);
            ViewBag.Title = "Uploading file successful";
            ViewBag.Message = $"File created: {di.CreatedDateTime}<br/>"
                            + $"File modified: {di.LastModifiedDateTime}<br/>"
                            + $"Weburl: {di.WebUrl}<br/>";
                                
            return View("Message");
        }

        [Authorize]
        public async Task<ActionResult> SchemaExtensions()
        {
            var viewModel = await schemaExtensionsRepository.SchemaExtensions();
            return View("SchemaExtensions", viewModel);
        }
    }
}