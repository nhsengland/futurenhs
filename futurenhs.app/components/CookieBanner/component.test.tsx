import Cookies from 'js-cookie'
import { render, screen } from '@jestMocks/index'
import { CookieBanner } from './index'

import { Props } from './interfaces'

describe('Cookie banner', () => {
    const props: Props = {
        text: {
            title: 'Mock title',
        },
    }

    it('renders correctly', () => {
        render(<CookieBanner {...props} />)

        expect(screen.getAllByText('Mock title').length).toBe(1)
    })

    it('does not render if cookies have been rejected or accepted', () => {
        const propsCopy: Props = Object.assign({}, props, {
            cookieName: 'mock-cookie',
        })

        Cookies.get = jest.fn().mockImplementation(() => 'mock-cookie')

        render(<CookieBanner {...propsCopy} />)

        expect(screen.queryByText('Mock title')).toBeNull()
    })
})
