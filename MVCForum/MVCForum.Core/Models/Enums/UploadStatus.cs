using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcForum.Core.Models.Enums
{
    public enum UploadStatus
    {
        Uploading = 1,
    	Uploaded = 2,
        Failed = 3,
	    Verified = 4,
	    Quarantined = 5,
	    Recycled = 6,
	    Deleted = 7
    }
}
