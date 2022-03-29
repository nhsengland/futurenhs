using Dapper;
using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Models.Search;

namespace FutureNHS.Api.DataAccess.Database.Read
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
                @$"SELECT    
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
	                                groups.[Subtitle] AS Description,
	                                LastUpdatedAtUtc = groups.[CreatedAtUtc],
	                                GroupId = groups.[Id],
	                                [Type] = 'group'

	                FROM		    dbo.[Group] groups
	                WHERE		    groups.[IsLocked] = 0
	                AND			
                                    (
                                    groups.[Name] 
                    LIKE		    @Term 
                    OR			    groups.[Subtitle] 
                    LIKE		    @Term 
   
                                    )
	                UNION ALL
	                SELECT		
                                    f.[Id],
	                                f.[Title],
	                                f.[Description],
	                                ISNULL(f.[ModifiedAtUtc], f.[CreatedAtUtc]),

	                                fldr.[Group_Id],
	                                'file'

	                FROM		    dbo.[File] f
	                JOIN		    dbo.[Folder] fldr 
                    ON			    fldr.[Id] = f.[ParentFolder]
	                WHERE		    f.[FileStatus] = (SELECT [Id] FROM FileStatus WHERE [Name] = 'Verified') -- Verified
	                AND			
                                    (
                                    f.[Title] 
                    LIKE		    @Term 
                    OR			    f.[Description] 
                    LIKE            @Term
                                    )
	                UNION ALL
	                SELECT	    
                                    t.[Entity_Id],
                                    t.[Title],
                                    p.[Content],
                                    ISNULL(p.[ModifiedAtUtc], p.[CreatedAtUtc]),
                                    GroupId = t.[Group_Id],
                                    'discussion-comment'

	                FROM	        dbo.[Comment] p
	                JOIN	        dbo.[Discussion] t 
                    ON              p.[Parent_EntityId] = t.[Entity_Id]
	                WHERE	
                                    (
                                    p.[Content] 
                    LIKE            @Term
                                    )
	                UNION ALL
	                SELECT	    
                                    f.[Id],
                                    f.[Title],
                                    f.[Description],
                                    f.[CreatedAtUtc],
                                    f.[Group_Id],
                                    'folder'

	                FROM	        dbo.[Folder] f
	                WHERE	        f.[IsDeleted] = 0
	                AND		
                                    (
                                    f.[Title] 
                                    LIKE @Term 
                                    OR f.[Description] 
                                    LIKE @Term
                                    )
	                UNION ALL
	                SELECT	        t.[Entity_Id],
                                    t.[Title],
                                    t.[Content],
                                    t.[CreatedAtUtc],
                                    t.[Group_Id],
                                    'discussion'

	                FROM	        dbo.[Discussion] t
	                WHERE	
                                    (
                                    t.[Title] 
                    LIKE            @Term
                                    OR t.[Content] 
                                    LIKE @Term
                                    )
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
                    OR			    groups.[Subtitle] 
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
	                WHERE		    f.[FileStatus] = (SELECT [Id] FROM FileStatus WHERE [Name] = 'Verified') -- Verified
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
	                FROM	        dbo.[Discussion] t 
					JOIN			dbo.[Comment] p
                    ON              p.[Parent_EntityId] = t.[Entity_Id]
	                WHERE	
                                    (
                                    p.[Content] 
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
                                    f.[Title] 
                                    LIKE @Term 
                                    OR f.[Description] 
                                    LIKE @Term
                                    )
	                ) 
                    AS              Folders,

					(
	                SELECT		    COUNT(*) 
					
                    AS              discussions 
	                FROM	        dbo.[Discussion] t
	                WHERE	
                                    (
                                    t.[Title] 
                    LIKE            @Term
                    OR              t.[Content] 
                                    LIKE @Term
                                    ) 
									
					) 
                    AS              Discussions;";

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
                        return result with { Group = group };
                    }

                    return result;

                }, splitOn: $"{nameof(GroupNavProperty.Id)}");

            var totalByType = await reader.ReadFirstAsync<SearchResultTotalsByType>();
            var searchResults = new SearchResults { Results = results, TotalsByType = totalByType };

            var totalCount = Convert.ToUInt32(totalByType.Groups + totalByType.Files + totalByType.Folders + totalByType.Discussions + totalByType.Comments);


            return (totalCount, searchResults);
        }
    }
}
