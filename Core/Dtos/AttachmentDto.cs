using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class AttachmentDto
    {
        public string PublicId { get; set; }
        public string Url { get; set; }
        public int FileType { get; set; }/*1: image, 2 : video, 3: file */
    }
}
