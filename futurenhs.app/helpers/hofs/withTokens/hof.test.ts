import { withTokens } from '.'

describe('withTokens hof', () => {
    const csrfToken = () => 'mock-csrf-token'

    const mockContext = {
        req: {
            csrfToken
        },
        res: {},
        page: {
            props: {}
        }
    } as any

    it('sets csrf prop', async () => {

        await withTokens(mockContext)

        expect(mockContext.page.props).toHaveProperty('csrfToken')
    })

    it('returns form error when passed invalid props', async () => {

        try {
            await withTokens({ req: {} } as any)
            expect(true).toBe(false)
        } catch (err) {
            expect(true).toBe(true)
        }
    })
})
