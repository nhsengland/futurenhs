using System;
using System.ComponentModel.DataAnnotations;
using MvcForum.Core.Models.Enums;

namespace MvcForum.Core.Models.SystemPages
{
    public class SystemPageWriteResponse
    {
        public Guid Id { get; set; }
        public ResponseType Response { get; set; }
    }
}
