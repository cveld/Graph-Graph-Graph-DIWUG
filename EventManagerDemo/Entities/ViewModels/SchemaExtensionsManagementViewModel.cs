using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventManagerDemo.Models
{
    public class SchemaExtensionsManagementViewModel
    {
        [Display(Name = "Schema Extension")]
        public string SelectedSchemaExtension { get; set; }

        public IEnumerable<SelectListItem> SchemaExtensionTypes {
            get {
                var values = Enum.GetNames(typeof(Models.SchemaExtensionTypes));

                foreach (var v in values)
                {
                    yield return new SelectListItem
                    {
                        Text = v,
                        Value = v
                    };
                }
            }
        }
    }
}