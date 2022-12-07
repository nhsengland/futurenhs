import { mockUser } from '@helpers/hofs/withUser/hof.test'
import { getGroup } from './index'
import fetch from 'jest-fetch-mock'

describe('getGroup service', () => {
    beforeEach(() => {
        fetch.resetMocks()
    })

    it('returns a data object on success', async () => {
        const mockGroupResponse = {
            name: 'Test Image',
            strapline: '',
            image: {
                source: '',
                width: '',
                height: '',
                altText: '',
            },
            imageId: '',
            themeId: '',
            isPublic: '',
        }

        fetch.mockResponseOnce(JSON.stringify(mockGroupResponse))
        const response = await getGroup({
            groupId: 'mock-slug',
            user: mockUser,
        })

        expect(response.data.text.title).toEqual('Test Image')
    })
})
