import { getServerSideProps } from './index.page';

import { mswServer } from './../../jest-mocks/msw-server';
import { handlers } from './../../jest-mocks/handlers';

const mockLocales = {
    locales: undefined,
    locale: undefined,
    defaultLocale: undefined
};

const mockUser = {
    id: 'b19e1529-cea6-40f8-989a-ad36011e9e89',
    text: {
        userName: 'Mock User'
    },
    image: null
};

const mockQuery = {
    term: 'default Mock Term'
};

const mockContext = {
    req: {
        query: mockQuery,
        user: mockUser,
        cookies: {
            t: '23052395784310953345930'
        }
    },
    resolvedUrl: '/search?term=' + mockQuery.term,
    ...mockLocales
};

beforeAll(() => mswServer.listen());
afterEach(() => mswServer.resetHandlers());
afterAll(() => mswServer.close());

describe('Search results', () => {

    describe('user is authenticated', () => {

        let mockContextCopy;

        beforeEach(() => {
            mockContextCopy = JSON.parse(JSON.stringify(mockContext));

            mswServer.use(handlers.getAuthHandler({ status: 200 }));
        })

        it('should get required server side props', async () => {

            mswServer.use(handlers.getSearchResultsHandler({ status: 200 }));

            const serverSideProps = await getServerSideProps(mockContextCopy);

            expect(serverSideProps).toHaveProperty('props.user');
            expect(serverSideProps).toHaveProperty('props.contentText');
            expect(serverSideProps).toHaveProperty('props.term', mockContextCopy.req.query.term);
            expect(serverSideProps).toHaveProperty('props.resultsList');
            expect(serverSideProps).toHaveProperty('props.pagination');

            expect(serverSideProps).not.toHaveProperty('redirect');

        })

        it('when term is less than 3 characters, should return empty resultsList', async () => {

            mswServer.use(handlers.getSearchResultsHandler({ status: 200 }));

            mockContextCopy.req.query.term = 'uk';

            const serverSideProps = await getServerSideProps(mockContextCopy);

            expect(serverSideProps).toHaveProperty('props.resultsList');
            expect(serverSideProps).toEqual(expect.objectContaining({
                props: expect.objectContaining(
                    { resultsList: [] }
                )
            }));
        })

        it('fetch results fails, should return 400', async () => {

            mswServer.use(handlers.getSearchResultsHandler({ status: 400 }));

            const serverSideProps = await getServerSideProps(mockContextCopy);

            expect(serverSideProps).not.toHaveProperty('redirect');
            expect(serverSideProps["props"]["resultsList"].length).toEqual(0);
            expect(serverSideProps).toEqual(expect.objectContaining({ props: expect.objectContaining({ errors: [{ 400: "Bad Request" }] }) }));
        })

        it('when fetch results returns error due to response being missed, should return an error', async () => {
            mswServer.use(handlers.getSearchResultsHandler({ status: 200, shouldRespond: false }));

            const serverSideProps = await getServerSideProps(mockContextCopy);

            expect(serverSideProps).not.toHaveProperty('redirect');
            expect(serverSideProps["props"]["errors"].length).toBeGreaterThanOrEqual(1);

        })

        it('when fetch returns null data, should return an error', async () => {
            mswServer.use(handlers.getSearchResultsHandler({ status: 200, shouldReturnData: false }));

            const serverSideProps = await getServerSideProps(mockContextCopy);

            expect(serverSideProps).not.toHaveProperty('redirect');
            expect(serverSideProps["props"]["errors"].length).toBeGreaterThanOrEqual(1);

        })

        it('req query null, should return empty resultsList', async () => {

            mockContextCopy.req.query = null;

            const serverSideProps = await getServerSideProps(mockContextCopy);

            expect(serverSideProps).not.toHaveProperty('redirect');
            expect(serverSideProps).toEqual(expect.objectContaining({
                props: expect.objectContaining(
                    { resultsList: [] }
                )
            }));

        })

        it('passes empty context, should not redirect or return props', async () => {
            const serverSideProps = await getServerSideProps({ req: {} } as any);

            expect(serverSideProps).not.toHaveProperty("props");
            expect(serverSideProps).toHaveProperty("redirect");
        })

    })


    describe('user is not authenticated', () => {

        let mockContextCopy = JSON.parse(JSON.stringify(mockContext));

        beforeEach(() => {
            mswServer.use(handlers.getAuthHandler({ status: 403 }));
        })

        afterEach(() => {
            mockContextCopy = JSON.parse(JSON.stringify(mockContext));
        })

        it('should be redirected to login', async () => {

            const serverSideProps = await getServerSideProps(mockContextCopy);

            expect(serverSideProps).toHaveProperty('redirect');
            expect(serverSideProps).toHaveProperty('redirect.destination');
        })

        it('term less than 3 characters, should be redirected', async () => {

            mockContextCopy.req.query.term = 'tr';

            const serverSideProps = await getServerSideProps(mockContextCopy);

            expect(serverSideProps).toHaveProperty('redirect');

        })

        it('passes empty context, should redirect', async () => {
            const serverSideProps = await getServerSideProps({ req: {} } as any);

            expect(serverSideProps).toHaveProperty("redirect");
            expect(serverSideProps).not.toHaveProperty("props");
        })

    })


});