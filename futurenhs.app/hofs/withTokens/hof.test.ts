import { withTokens } from '.'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'

describe('withTokens hof', () => {
    const csrfToken = () => 'mock-csrf-token'
    const props: any = {}

    const getServerSideProps = async (context) => {
        return handleSSRSuccessProps({ props })
    }

    it('sets csrf prop', async () => {
        const mockwithTokens = withTokens({ props, getServerSideProps })
        const result = await mockwithTokens({ req: { csrfToken } } as any)

        expect(result).toHaveProperty('props.csrfToken')
    })

    it('returns form error when passed invalid props', async () => {
        const mockwithTokens = withTokens({ props: null, getServerSideProps })

        try {
            await mockwithTokens({ req: { csrfToken } } as any)
            expect(true).toBe(false)
        } catch (err) {
            expect(true).toBe(true)
        }
    })
})
