using System;
using System.ComponentModel.DataAnnotations;

namespace moderator_api.BindingModels
{
    public class ModerateBindingModel
    {
        [Required]
        public string Base64Image { get; set; }
        [Required]
        public string Ext { get; set; }
        [Required]
        [StringLength(maximumLength: 1024)]
        public string HtmlText { get; set; }
    }
}
