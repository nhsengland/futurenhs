﻿using Dapper;
using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.DataAccess.Models.Search;
using FutureNHS.Api.DataAccess.Repositories.Database.DatabaseProviders.Interfaces;
using FutureNHS.Api.DataAccess.Repositories.Read.Interfaces;

namespace FutureNHS.Api.DataAccess.Repositories.Read
{
    public class SearchDataProvider : ISearchDataProvider
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<SearchDataProvider> _logger;

        public SearchDataProvider(IAzureSqlDbConnectionFactory connectionFactory, ILogger<SearchDataProvider> logger)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }
        public async Task<(uint totalCount, SearchResults)> Search(string term, uint offset, uint limit, CancellationToken cancellationToken)
        {
            if (limit is < PaginationSettings.MinLimit or > PaginationSettings.MaxLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }


            if (term.Length is < SearchSettings.TermMinimum or > SearchSettings.TermMaximum)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            const string query =
                @$" SELECT    
                                    [{nameof(SearchResult.Type)}] = results.[Type],
		                            [{nameof(SearchResult.Id)}] = results.[Id],
		                            [{nameof(SearchResult.Name)}] = results.[Name],
		                            [{nameof(SearchResult.Description)}] = results.[Description],
		                            [{nameof(SearchResult.LastUpdatedAtUtc)}] = FORMAT(results.[LastUpdatedAtUtc],'yyyy-MM-ddTHH:mm:ssZ'), 
                                    [{nameof(GroupNavProperty.Id)}] = results.[GroupId],
								    [{nameof(GroupNavProperty.Name)}] = groups.[Name],
                                    [{nameof(GroupNavProperty.Slug)}] = groups.[Slug]
                    FROM 
								(
	                SELECT		
                                    groups.[Id],
	                                groups.[Name],
	                                groups.[Description],
	                                LastUpdatedAtUtc = groups.[DateCreated],
	                                GroupId = groups.[Id],
	                                [Type] = 'group'

	                FROM		    dbo.[Group] groups
	                WHERE		    groups.[IsLocked] = 0
	                AND			
                                    (
                                    groups.[Name] 
                    LIKE		    @Term 
                    OR			    groups.[Description] 
                    LIKE		    @Term 
                    OR			    groups.[Subtitle] 
                    LIKE		    @Term 
                    OR			    groups.[PageTitle] 
                    LIKE		    @Term 
                    OR			    groups.[Introduction] 
                    LIKE		    @Term 
                    OR			    groups.[AboutUs] 
                    LIKE		    @Term
                                    )
	                UNION ALL
	                SELECT		
                                    f.[Id],
	                                f.[Title],
	                                f.[Description],
	                                ISNULL(f.[ModifiedAtUtc], f.[CreatedAtUtc]),

	                                fldr.[ParentGroup],
	                                'file'

	                FROM		    dbo.[File] f
	                JOIN		    dbo.[Folder] fldr 
                    ON			    fldr.[Id] = f.[ParentFolder]
	                WHERE		    f.[FileStatus] = 4 -- Verified
	                AND			
                                    (
                                    f.[Title] 
                    LIKE		    @Term 
                    OR			    f.[Description] 
                    LIKE            @Term
                                    )
	                UNION ALL
	                SELECT	    
                                    p.[Id],
                                    t.[Name],
                                    p.[PostContent],
                                    ISNULL(p.[DateEdited], p.[DateCreated]),
                                    GroupId = t.[Group_Id],
                                    'discussion-comment'

	                FROM	        dbo.[Post] p
	                JOIN	        dbo.[Topic] t 
                    ON              t.[Id] = p.[Topic_Id]
	                WHERE	
                                    (
                                    p.[PostContent] 
                    LIKE            @Term
                                    )
                    AND             p.[IsTopicStarter] = 0
	                UNION ALL
	                SELECT	    
                                    f.[Id],
                                    f.[Name],
                                    f.[Description],
                                    f.[CreatedAtUtc],
                                    f.[ParentGroup],
                                    'folder'

	                FROM	        dbo.[Folder] f
	                WHERE	        f.[IsDeleted] = 0
	                AND		
                                    (
                                    f.[Name] 
                                    LIKE @Term 
                                    OR f.[Description] 
                                    LIKE @Term
                                    )
	                UNION ALL
	                SELECT	        t.[Id],
                                    t.[Name],
                                    p.[PostContent],
                                    t.[CreateDate],
                                    t.[Group_Id],
                                    'discussion'

	                FROM	        dbo.[Topic] t
                    JOIN            dbo.[Post] p
                    ON              p.[Topic_Id] = t.[Id]
	                WHERE	
                                    (
                                    t.[Name] 
                    LIKE            @Term
                                    OR p.[PostContent] 
                                    LIKE @Term
                                    )
                    AND             p.[IsTopicStarter] = 1
					) results
                                    

                    JOIN            dbo.[Group] groups 
                    ON              groups.[Id] = results.[GroupId]
                    ORDER BY        results.[LastUpdatedAtUtc] DESC
                    OFFSET          @Offset ROWS 
					FETCH NEXT		@Limit ROWS ONLY;


                    -- COUNT


					SELECT
					(
					SELECT			COUNT(*)
	                FROM		    dbo.[Group] groups
	                WHERE		    groups.[IsLocked] = 0
	                AND			
                                    (
                                    groups.[Name] 
                    LIKE		    @Term 
                    OR			    groups.[Description] 
                    LIKE		    @Term 
                    OR			    groups.[Subtitle] 
                    LIKE		    @Term 
                    OR			    groups.[PageTitle] 
                    LIKE		    @Term 
                    OR			    groups.[Introduction] 
                    LIKE		    @Term 
                    OR			    groups.[AboutUs] 
                    LIKE		    @Term
                                    )
                    ) 
                    AS              Groups,
					
					(
					SELECT		    COUNT(*) 

                    AS              files 
	                FROM		    dbo.[File] f
	                JOIN		    dbo.[Folder] fldr 
                    ON			    fldr.[Id] = f.[ParentFolder]
	                WHERE		    f.[FileStatus] = 4 -- Verified
	                AND			
                                    (
                                    f.[Title] 
                    LIKE		    @Term 
                    OR			    f.[Description] 
                    LIKE            @Term
                                    )
					) 
                    AS              Files,
                    
					(
	                SELECT		    COUNT(*) 
	                FROM	        dbo.[Post] p
	                JOIN	        dbo.[Topic] t 
                    ON              t.[Id] = p.[Topic_Id]
	                WHERE	
                                    (
                                    p.[PostContent] 
                    LIKE            @Term
                                    )
	                ) 
                    AS              Comments,

					(
	                SELECT		    COUNT(*) 

                    AS              folders 
	                FROM	        dbo.[Folder] f
	                WHERE	        f.[IsDeleted] = 0
	                AND		
                                    (
                                    f.[Name] 
                                    LIKE @Term 
                                    OR f.[Description] 
                                    LIKE @Term
                                    )
	                ) 
                    AS              Folders,

					(
	                SELECT		    COUNT(*) 
					
                    AS              discussions 
	                FROM	        dbo.[Topic] t
	                WHERE	
                                    (
                                    t.[Name] 
                    LIKE            @Term
                                    ) 
									
					) 
                    AS              Discussions";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                Offset = Convert.ToInt32(offset),
                Limit = Convert.ToInt32(limit),
                Term = $"%{term}%"
            });
           
            var results = reader.Read<SearchResult, GroupNavProperty, SearchResult>(
                (result, group) =>
                {
                    if (group is not null)
                    {
                        return result with { Group  = group };
                    }

                    return result;

                }, splitOn: $"{nameof(GroupNavProperty.Id)}");

            var totalByType = await reader.ReadFirstAsync<SearchResultTotalsByType>();
            var searchResults = new SearchResults {Results = results, TotalsByType = totalByType};

            var totalCount = Convert.ToUInt32(totalByType.Groups + totalByType.Files + totalByType.Folders + totalByType.Discussions + totalByType.Comments);
          

            return (totalCount, searchResults);
        }
    }
}
