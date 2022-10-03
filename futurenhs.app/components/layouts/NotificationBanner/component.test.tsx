import { render, screen, cleanup } from '@jestMocks/index'

import { NotificationBanner } from './index'

import { Props } from './interfaces'

describe('Notification banner', () => {
    
    const props: Props = {
        id: 123,
        text: {
            main: 'Test notification'
        }
    }

    it('Renders correctly', () => {

        render(<NotificationBanner {...props}/>)

        expect(screen.getAllByText('Test notification').length).toBe(1)

    });

    it('Renders with an alert role by default and region role if passed a heading', () => {

        render(<NotificationBanner {...props} />)

        expect(screen.getAllByRole('alert').length).toBe(1)

        cleanup()

        const propsCopy: Props = Object.assign({}, props, {
            text: {
                heading: 'Important',
                main: 'Test notification'
            }
        })

        render(<NotificationBanner {...propsCopy} />)

        expect(screen.getAllByRole('region').length).toBe(1)

    })

});