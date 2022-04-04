import { rest } from 'msw';

interface SearchResultsHandlerProps {
    status?: number,
    shouldRespond?: boolean,
    shouldReturnData?: boolean
};

interface Group {
    id: string,
    name: string,
    strapLine: string,
    slug: string,
    isPublic: boolean,
    image?: any,
    themeId?: any,
    isMember: boolean
};

interface GroupPreview {
    id: string,
    nameText: string,
    strapLineText: string,
    discussionCount: number,
    memberCount: number,
    slug: string,
    image?: any,
    themeId?: any,
};

interface GroupFolder {
    id: string,
    name: string,
    description: string,
    slug?: string,
    path: Array<{ id: string, name: string }>,
    firstRegistered?: { atUtc: string, by: { id: string, name: string, slug: string } },
};

interface GroupFile extends GroupFolder {
    versions: Array<GroupFolder>
};

interface GroupFolderContent {
    id: string,
    type: string,
    name: string,
    description: string,
    firstRegistered?: { atUtc: string, by: { id: string, name: string, slug: string } },
    lastUpdated?: { atUtc: string, by: { id: string, name: string, slug: string } },
    additionalMetadata?: { fileExtension: string, mediaType: string }
};

const groupEndpoint = new RegExp(/([A-Za-z0-9\-\"\:\/]+.+)?\/v1\/users\/[A-Za-z0-9\-\"]+\/groups\/[A-Za-z0-9\-\"]+(\?[A-Za-z0-9\-\"]+.+)?$/, 'i');
const groupsEndpoint = new RegExp(/([A-Za-z0-9\-\"\:\/]+.+)?\/v1\/users\/[A-Za-z0-9\-\"]+\/groups(\s)?(\?[A-Za-z0-9\-\"]+.+)?$/, 'i');
const groupFolderEndpoint = new RegExp(/([A-Za-z0-9\-\"\:\/]+.+)?\/v1\/users\/[A-Za-z0-9\-\"]+\/groups\/[A-Za-z0-9\-\"]+\/folders\/[A-Za-z0-9\-\"]+(\?[A-Za-z0-9\-\"]+.+)?$/, 'i');
const groupFolderContentsEndpoint = new RegExp(/([A-Za-z0-9\-\"\:\/]+.+)?\/v1\/users\/[A-Za-z0-9\-\"]+\/groups\/[A-Za-z0-9\-\"]+\/folders(\/[A-Za-z0-9\-\"]+\/contents)?(\?[A-Za-z0-9\-\"]+.+)?$/, 'i');
const groupFileEndpoint = new RegExp(/([A-Za-z0-9\-\"\:\/]+.+)?\/v1\/users\/[A-Za-z0-9\-\"]+\/groups\/[A-Za-z0-9\-\"]+\/files\/[A-Za-z0-9\-\"]+(\?[A-Za-z0-9\-\"]+.+)?$/, 'i');
const groupFileDownloadEndpoint = new RegExp(/([A-Za-z0-9\-\"\:\/]+.+)?\/v1\/users\/[A-Za-z0-9\-\"]+\/groups\/[A-Za-z0-9\-\"]+\/files\/[A-Za-z0-9\-\"]+\/download(\?[A-Za-z0-9\-\"]+.+)?$/, 'i');

const mockSearchResults = [
    {
        type: 'group',
        id: 'msw-group-id',
        name: 'MSW Test - Visual Regression Empty Group',
        description: 'MSW Test - An Empty Group for Visual Regression Testing. DO NOT TOUCH!!!!!',
        lastUpdatedAtUtc: '2021-11-08T15:03:30Z',
        group: {
            id: 'msw-group-id',
            name: 'MSW Test - Visual Regression Empty Group',
            slug: 'msw-visual-regression-empty-group'
        }
    },
    {
        type: 'group',
        id: 'msw-group-id',
        name: 'MSW Test - Automation Visual Regression Group',
        description: 'MSW Test - Visual Regression group DO NOT TOUCH/EDIT/DELETE/JOIN/ANYTHING!',
        lastUpdatedAtUtc: '2021-11-08T14:53:23Z',
        group: {
            id: 'msw-group-id',
            name: 'MSW Test - Automation Visual Regression Group',
            slug: 'msw-automation-visual-regression-group'
        }
    },
    {
        type: 'group',
        id: 'msw-group-id',
        name: 'MSW Test - 30thSeptGroup test',
        description: 'MSW Test - strapline members can see the appropriate information when viewing the group home page. test As a platform administrator when I am creating tform administrator when I am creating or editing a group I want to add a group name, group label, group strap line, a group sub title, and a group introduction to the group so that platform members can segroup sub title, and a group introduction to the group so that platform members can see the appropriate information when viewing the group home page. members can see the appropriate information when viewing',
        lastUpdatedAtUtc: '2021-09-30T13:51:13Z',
        group: {
            id: 'msw-group-id',
            name: 'MSW Test - 30thSeptGroup test',
            slug: 'msw-30thseptgroup'
        }
    },
    {
        type: 'discussion-comment',
        id: 'msw-discussion-id',
        name: 'MSW Test - Georgie, Alice, Marianthi, it would be good to get your thoughts?',
        description: "MSW Test - <p>This bottom text box is confusing - it isn't immediately clear what comment you are replying to</p>",
        lastUpdatedAtUtc: '2021-09-09T07:10:23Z',
        group: {
            id: 'msw-group-id',
            name: 'MSW Test - Private Beta',
            slug: 'msw-private-beta'
        }
    },
    {
        type: 'folder',
        id: 'msw-folder-id',
        name: 'MSW Test - Parent folder',
        description: null,
        lastUpdatedAtUtc: '2021-09-08T13:43:41Z',
        group: {
            id: 'msw-group-id',
            name: 'MSW Test - Presentation Private Group',
            slug: 'msw-presentation-private-group'
        }
    },
    {
        type: 'discussion',
        id: 'msw-discussion-id',
        name: 'MSW Test - Georgie, Alice, Marianthi, it would be good to get your thoughts?',
        description: '<p> MSW Test - Hey, how are you all. It would be great to get your thoughts on this forum discussion?</p>',
        lastUpdatedAtUtc: '2021-09-02T16:19:20Z',
        group: {
            id: 'msw-group-id',
            name: 'MSW Test - Private Beta',
            slug: 'msw-private-beta'
        }
    }
];

/**
 * mock getSearchResults fetch
 */
const getSearchResultsHandler = ({ status = 200, shouldRespond = true, shouldReturnData = true }: SearchResultsHandlerProps) => rest.get('*/v1/search*',
    async (req, res, ctx) => res(shouldRespond && ctx.json({
        data: !shouldReturnData ? null : {
            results: mockSearchResults
        },
        offset: 0,
        limit: 10,
        totalRecords: 7
    }), ctx.status(status)));


/**
 * mock getAuth
 * */
const getAuthHandler = ({ status = 200 }: { status?: number }) => rest.get('*/auth/userinfo',
    async (req, res, ctx) => res(
        ctx.json({ Id: 'msw-admin-id', FullName: 'Mock User 2', UserAvatar: null }),
        ctx.status(status)
    ));

/**
 * mock getSiteActions
 */
const getSiteActions = ({ actions = [] }: { actions?: Array<string> }) => rest.get('*/v1/users/*/actions',
    async (req, res, ctx) => res(
        ctx.json(actions)
    ));


/**
 * mock getGroup service .../api/v1/users/../groups/groupId
 */
const getGroup = ({ status = 200, group }: { status?: number, group?: Group }) => rest.get(groupEndpoint,
    async (req, res, ctx) => res(
        ctx.json(
            group ??
            {
                id: "msw-aa-group",
                image: null,
                isMember: false,
                isPublic: false,
                name: "MSW Mock Admin Group",
                slug: "msw-aa",
                strapLine: "MSW usage only",
                themeId: null
            }
        ),
        ctx.status(status)
    )
);

/**
 * mock getGroups service .../api/v1/users/../groups
 */
const getGroups = ({ status = 200, groups }: { status?: number, groups?: Array<GroupPreview> }) => rest.get(groupsEndpoint,
    async (req, res, ctx) => res(
        ctx.json(

            {
                data: groups ?? [{
                    id: "msw-aa-group",
                    nameText: "MSW Mock Admin Group",
                    slug: "msw-aa",
                    discussionCount: 0,
                    image: null,
                    memberCount: 0,
                    strapLineText: "The MSW Operational Planning process for 2021/22 will ",
                    themeId: null

                }],

                firstPage: "/api/v1/users/msw-admin-id/groups?isMember=False&offset=0&limit=10",
                lastPage: "/api/v1/users/msw-admin-id/groups?isMember=False&offset=20&limit=10",
                limit: 10,
                nextPage: "/api/v1/users/msw-admin-id/groups?isMember=False&offset=10&limit=10",
                offset: 0,
                previousPage: null,
                totalPages: 3,
                totalRecords: 22
            }
        ),
        ctx.status(status)
    )
);

/**
 * mock getGroupFolder service /api/v1/users/userId/groups/groupId/folders/folderId
 */
const getGroupFolder = ({ status = 200, groupFolder }: { status?: number, groupFolder?: GroupFolder }) => rest.get(groupFolderEndpoint,
    async (req, res, ctx) => res(
        ctx.json(
            groupFolder ??
            {
                id: "fake-folder-id",
                name: "MSW Test Folder",
                description: null,
                path: [
                    {
                        id: "fake-folder-id",
                        name: "MSW Test Folder"
                    }
                ],
                firstRegistered: {
                    atUtc: "2021-10-18T12:45:06Z",
                    by: {
                        id: "jest-admin-id",
                        name: "Jest Admin",
                        slug: "jest-admin"
                    }
                }
            }
        ),
        ctx.status(status)
    )
);

/**
* mock getGroupFolderContents service /api/v1/users/userId/groups/groupId/folders (/folderId/contents)
*/
const getGroupFolderContents = ({ status = 200, groupFolderContents }: { status?: number, groupFolderContents?: Array<GroupFolderContent> }) => rest.get(groupFolderContentsEndpoint,
    async (req, res, ctx) => {

        if (req.url.pathname.split("?")[0].split('folders').length > 0) {

            return res(
                ctx.json(
                    {
                        "offset": 0,
                        "limit": 10,
                        "firstPage": "/api/v1/users/msw-admin-id/groups/aa/folders/f86d22cf-1b0e-4d24-8120-adc500d224fb/contents?offset=0&limit=10",
                        "lastPage": "/api/v1/users/msw-admin-id/groups/aa/folders/f86d22cf-1b0e-4d24-8120-adc500d224fb/contents?offset=0&limit=10",
                        "totalPages": 1,
                        "totalRecords": 3,
                        "nextPage": null,
                        "previousPage": null,
                        "data": groupFolderContents ?? [
                            {
                                "id": "msw-folder-id-1",
                                "type": "Folder",
                                "name": "MSW Folder 1",
                                "description": "Just a test folder in folder contents",
                                "firstRegistered": {
                                    "atUtc": "2022-01-14T11:25:07Z",
                                    "by": {
                                        "id": "msw-admin",
                                        "name": "MSW Admin",
                                        "slug": "msw-admin"
                                    }
                                },
                                "lastUpdated": null,
                                "additionalMetadata": null
                            },
                            {
                                "id": "msw-file-1",
                                "type": "File",
                                "name": "msw docTest",
                                "description": "msw test doc",
                                "firstRegistered": {
                                    "atUtc": "2021-12-15T11:04:49Z",
                                    "by": {
                                        "id": "msw-admin",
                                        "name": "MSW Admin",
                                        "slug": "msw-admin"
                                    }
                                },
                                "lastUpdated": {
                                    "atUtc": "2022-01-10T12:31:13Z",
                                    "by": {
                                        "id": "msw-admin",
                                        "name": "MSW Admin",
                                        "slug": "mswDocTest.doc"
                                    }
                                },
                                "additionalMetadata": {
                                    "mediaType": "application/msword",
                                    "fileExtension": ".doc"
                                }
                            },
                            {
                                "id": "msw-file-2",
                                "type": "File",
                                "name": "msw pdfTest",
                                "description": "msw test pdf",
                                "firstRegistered": {
                                    "atUtc": "2021-12-16T15:39:58Z",
                                    "by": {
                                        "id": "msw-admin",
                                        "name": "MSW Admin",
                                        "slug": "msw-admin"
                                    }
                                },
                                "lastUpdated": {
                                    "atUtc": "2021-12-16T15:39:58Z",
                                    "by": {
                                        "id": "msw-admin",
                                        "name": null,
                                        "slug": "mswPdfTest.pdf"
                                    }
                                },
                                "additionalMetadata": {
                                    "mediaType": "application/pdf",
                                    "fileExtension": ".pdf"
                                }
                            }]
                    },
                ),
                ctx.status(status)
            )
        }

        return res(
            ctx.json({
                "offset": 0,
                "limit": 10,
                "firstPage": "api/v1/users/mock-user/groups/aa/folders?offset=0&limit=10",
                "lastPage": "/api/v1/users/mock-user/groups/aa/folders?offset=0&limit=10",
                "totalPages": 1,
                "totalRecords": groupFolderContents.length ?? 2,
                "nextPage": null,
                "previousPage": null,
                data: groupFolderContents ?? [
                    {
                        id: "mock-folder-1",
                        type: "Folder",
                        name: "MSW Test Folder",
                        description: null,
                        firstRegistered: {
                            atUtc: "2021-10-18T12:45:06Z",
                            by: {
                                id: "msw-admin-id",
                                name: "MSW Admin",
                                slug: "msw-admin"
                            }
                        },
                        lastUpdated: null,
                        additionalMetadata: null
                    },
                    {
                        id: "mock-folder-2",
                        type: "Folder",
                        name: "MSW Empty Folder",
                        description: "MSW Folder for testing",
                        firstRegistered: {
                            atUtc: "2021-08-06T14:13:38Z",
                            by: {
                                id: "msw-admin-id",
                                name: "MSW Admin",
                                slug: "msw-admin"
                            }
                        },
                        lastUpdated: null,
                        additionalMetadata: null
                    }
                ]
            })
        )
    });

/**
 * mock delete Folder action
 */
const deleteFolder = ({ status = 204 }: { status?: number }) => rest.delete('*/folders/*/delete', async (req, res, ctx) => res(ctx.json({ message: "delete response from MSW" }), ctx.status(status)));


/**
 * mock getGroupFile service /api/v1/users/userId/groups/groupId/files/fileId
 */
const getGroupFile = ({ status = 200, groupFile }: { status?: number, groupFile?: GroupFile }) => rest.get(groupFileEndpoint,
    async (req, res, ctx) => res(
        ctx.json(
            groupFile ??
            {
                id: "msw-group-file",
                name: "msw docTest",
                description: "msw test doc",
                path: [
                    {
                        id: "msw-group-folder",
                        name: "MSW Test Folder"
                    }],
                firstRegistered: {
                    atUtc: "2021-12-15T11:04:49Z",
                    by: {
                        id: "msw-admin-id",
                        name: "MSW Admin",
                        slug: "msw-admin"
                    }
                },
                lastUpdated: {
                    atUtc: "2022-01-10T12:31:13Z", by: {
                        id: "msw-admin-id",
                        name: "MSW Admin",
                        slug: "msw-admin"
                    }
                },
                versions: [{
                    id: "msw-file-version-1",
                    name: "msw v1 docTest.doc",
                    firstRegistered: {
                        atUtc: "2021-12-15T11:04:49Z",
                        by: {
                            id: "msw-admin-id",
                            name: "MSW Admin",
                            slug: "msw-admin"
                        }
                    },
                    additionalMetadata: {
                        mediaType: "application/msword",
                        fileExtension: ".doc"
                    }
                }]
            }
        ),
        ctx.status(status)
    )
)

/**
* mock file Download action /api/v1/users/userId/groups/groupId/files/fileID/download
*/
const getGroupFileDownload = ({ status = 200 }: { status?: number }) => rest.get(groupFileDownloadEndpoint,
    async (req, res, ctx) => {

        return res(
            ctx.json({

            }),
            ctx.status(status)
        )
    });


/**
 * all handlers
 */
export const handlers = {
    getSearchResultsHandler,
    getAuthHandler,
    getSiteActions,
    getGroup,
    getGroups,
    getGroupFolder,
    getGroupFolderContents,
    deleteFolder,
    getGroupFile,
    getGroupFileDownload,
};

/**
 * default handlers
 */
export const defaultHandlers = [
    getSearchResultsHandler({}),
    getSiteActions({}),
    getAuthHandler({}),
    getSiteActions({}),
    getGroups({}),
    getGroup({}),
    getGroupFolder({}),
    getGroupFolderContents({}),
    deleteFolder({}),
    getGroupFile({}),
    getGroupFileDownload({})
];