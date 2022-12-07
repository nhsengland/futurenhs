import { getSearchResults } from '.'
import { mswServer } from './../../jest-mocks/msw-server'
import { handlers } from './../../jest-mocks/handlers'
import { mockUser } from '@helpers/hofs/withUser/hof.test'
import fetch from 'jest-fetch-mock'
import { ServiceError } from '..'

const props: any = {
    term: 'disc',
    minLength: 3,
    user: mockUser,
}

describe('get search results', () => {
    beforeEach(() => {
        fetch.resetMocks()
    })

    it('return 0 results for term < minLength', async () => {
        const result = await getSearchResults({
            user: mockUser,
            term: 'uk',
            minLength: props.minLength,
        })

        expect(result.data.length).toBe(0)
    })

    it('gets search data', async () => {
        const mockSearchResponse = {
            data: {
                results: [
                    {
                        type: 'group',
                        id: '111',
                        name: 'Test Name',
                        description: 'Test Desc',
                        group: {
                            slug: 'test-slug',
                            name: 'Test Name',
                            description: 'Test Desc',
                        },
                    },
                ],
            },
        }

        fetch.mockResponse(JSON.stringify(mockSearchResponse))

        const term = 'medical health'
        const result = await getSearchResults({
            user: mockUser,
            term,
            minLength: term.length,
        })

        expect(result.data[0].content.title).toEqual('Test Name')
    })

    it('throws an error when data not returned', async () => {
        // const mockSearchResponse = { }

        fetch.mockResponse(null, { status: 500 })

        const term = 'medical health'
        try {
            const result = await getSearchResults({
                user: mockUser,
                term,
                minLength: term.length,
            })
            expect(false).toBeTruthy()
        } catch (e) {
            expect(e.message).toBe(
                'An unexpected error occurred when attempting to get the search results'
            )
        }
    })
})
