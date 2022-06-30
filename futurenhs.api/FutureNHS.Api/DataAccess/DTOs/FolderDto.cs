﻿using FutureNHS.Api.DataAccess.Models;

namespace FutureNHS.Api.DataAccess.DTOs
{
    public sealed record FolderDto : BaseData
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public Guid CreatedBy { get; init; }
        public DateTime CreatedAtUTC { get; init; }
        public Guid? ModifiedBy { get; init; }
        public DateTime? ModifiedAtUTC { get; init; }
        public Guid? ParentFolder { get; init; }
        public int FileCount { get; init; }
        public Guid GroupId { get; init; }
        public bool IsDeleted { get; init; }
    }
}
