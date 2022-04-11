import { getSearchResults } from '.';
import { mswServer } from './../../jest-mocks/msw-server';
import { handlers } from './../../jest-mocks/handlers';

const props: any = {
    term: "disc",
    minLength: 3
};

beforeAll(() => mswServer.listen());
afterEach(() => mswServer.resetHandlers());
afterAll(() => mswServer.close());

describe('get search results', () => {

    it('returns data', async () => {

        const result = await getSearchResults(props);

        expect(result).toHaveProperty('data');
        expect(result.data.length).not.toBe(0);

    });

    it('return 0 results for term < minLength', async () => {

        const result = await getSearchResults({ term: "uk", minLength: props.minLength });

        expect(result.data.length).toBe(0);

    });

    it('throws an error when data not returned', async () => {

        mswServer.use(handlers.getSearchResultsHandler({ status: 500, shouldReturnData: false }));

        try {

            await getSearchResults(props);
            expect(false).toBe(true);

        }
        catch (err) {

            expect(err.message).toBe("An unexpected error occurred when attempting to get the search results");

        }

    });
})