using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WalksAndBenches.Models
{
    public class WalkModel
    {
        [Required]
        public string WalkName { get; set; }
        [Required]
        public string SubmittedBy { get; set; }
        public string Description { get; set; }
        [Required]
        public IFormFile UploadedImage { get; set; }
    }
}
