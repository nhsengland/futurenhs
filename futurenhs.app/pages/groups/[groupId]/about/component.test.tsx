import * as React from 'react'
import { render, screen } from '@jestMocks/index'
import { routes } from '@jestMocks/generic-props'
import GroupAboutUsPage, {
    Props,
} from '@pages/groups/[groupId]/about/index.page'

describe('Group about us template', () => {
    const props: Props = {
        id: 'mockId',
        routes: routes,
        tabId: 'index',
        actions: [],
        contentText: {
            secondaryHeading: 'Mock secondary heading',
        },
        user: undefined,
        image: null,
        entityText: {
            mainHeading: 'Mock heading',
        },
    }

    it('renders correctly', () => {
        render(<GroupAboutUsPage {...props} />)

        expect(screen.getAllByText('Mock secondary heading').length).toBe(1)
    })
})
