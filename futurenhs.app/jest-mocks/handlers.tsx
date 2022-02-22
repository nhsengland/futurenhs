import { rest } from 'msw';

const mockSearchResults = [
    {
        type: 'group',
        id: 'ae0df608-8dcc-4e13-8dd6-adda00f82866',
        name: 'Visual Regression Empty Group',
        description: 'An Empty Group for Visual Regression Testing. DO NOT TOUCH!!!!!',
        lastUpdatedAtUtc: '2021-11-08T15:03:30Z',
        group: {
            id: 'ae0df608-8dcc-4e13-8dd6-adda00f82866',
            name: 'Visual Regression Empty Group',
            slug: 'visual-regression-empty-group'
        }
    },
    {
        type: 'group',
        id: '41234726-7e93-4767-afbe-adda00f560a2',
        name: 'Automation Visual Regression Group',
        description: 'Visual Regression group DO NOT TOUCH/EDIT/DELETE/JOIN/ANYTHING!',
        lastUpdatedAtUtc: '2021-11-08T14:53:23Z',
        group: {
            id: '41234726-7e93-4767-afbe-adda00f560a2',
            name: 'Automation Visual Regression Group',
            slug: 'automation-visual-regression-group'
        }
    },
    {
        type: 'group',
        id: '92bca64a-9821-45ed-b135-adb300e44dc4',
        name: '30thSeptGroup test',
        description: 'strapline members can see the appropriate information when viewing the group home page. test As a platform administrator when I am creating tform administrator when I am creating or editing a group I want to add a group name, group label, group strap line, a group sub title, and a group introduction to the group so that platform members can segroup sub title, and a group introduction to the group so that platform members can see the appropriate information when viewing the group home page. members can see the appropriate information when viewing',
        lastUpdatedAtUtc: '2021-09-30T13:51:13Z',
        group: {
            id: '92bca64a-9821-45ed-b135-adb300e44dc4',
            name: '30thSeptGroup test',
            slug: '30thseptgroup'
        }
    },
    {
        type: 'discussion-comment',
        id: '580a10bc-f204-402c-83e7-ad9e0076361e',
        name: 'Georgie, Alice, Marianthi, it would be good to get your thoughts?',
        description: "<p>This bottom text box is confusing - it isn't immediately clear what comment you are replying to</p>",
        lastUpdatedAtUtc: '2021-09-09T07:10:23Z',
        group: {
            id: '449da90e-abb2-4de1-8f26-ad4700d9d977',
            name: 'Private Beta',
            slug: 'private-beta'
        }
    },
    {
        type: 'folder',
        id: 'aee82e9e-fd39-4f67-99bd-ad9d00e23b97',
        name: 'Parent folder',
        description: null,
        lastUpdatedAtUtc: '2021-09-08T13:43:41Z',
        group: {
            id: 'dbde84f4-a924-4356-95bc-ad9d00d6fe02',
            name: 'Presentation Private Group',
            slug: 'presentation-private-group'
        }
    },
    {
        type: 'discussion',
        id: 'b4793268-1813-4a34-ac82-ad97010cfc33',
        name: 'Georgie, Alice, Marianthi, it would be good to get your thoughts?',
        description: '<p>Hey, how are you all. It would be great to get your thoughts on this forum discussion?</p>',
        lastUpdatedAtUtc: '2021-09-02T16:19:20Z',
        group: {
            id: '449da90e-abb2-4de1-8f26-ad4700d9d977',
            name: 'Private Beta',
            slug: 'private-beta'
        }
    }
];

// mock getSearchResults fetch
interface SearchResultsHandlerProps {
    status: number,
    shouldRespond?: boolean,
    shouldReturnData?: boolean
};

const getSearchResultsHandler = ({ status, shouldRespond = true, shouldReturnData = true }: SearchResultsHandlerProps) => rest.get('*/v1/search',
    async (req, res, ctx) => res(shouldRespond && ctx.json({
        data: !shouldReturnData ? null : {
            results: mockSearchResults
        },
        offset: 0,
        limit: 10,
        totalRecords: 7
    }), ctx.status(status)));


// mock getAuth
const getAuthHandler = ({ status }) => rest.get('*/auth/userinfo', async (req, res, ctx) => res(
    ctx.json({ Id: 'b19e1529-cea6-40f8-989a-ad36011e9e89', FullName: 'Mock User 2', UserAvatar: null }),
    ctx.status(status)
));

//mock getSiteActions
const getSiteActions = rest.get('*/v1/users/*/actions', async (req, res, ctx) => res(
    ctx.json([
        // 'https://schema.collaborate.future.nhs.uk/admin/v1/view',
        'msw Mock Action',
    ])
));
/**
 * all handlers
 */
export const handlers = { getSearchResultsHandler, getAuthHandler, getSiteActions };

/**
 * default handlers
 */
export const defaultHandlers = [getSiteActions];