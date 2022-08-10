import { routes } from '@jestMocks/generic-props'
import { cleanup, render, screen } from '@jestMocks/index'
import forms from '@formConfigs/index'
import mockRouter from 'next-router-mock'
import { actions as actionConstants } from '@constants/actions'

import { GroupMemberUpdateTemplate } from './index'

import { Props } from './interfaces'

describe('Group member update template', () => {
    beforeEach(() => {
        mockRouter.setCurrentUrl('/groups/:groupId/members/:memberId/update')
    })

    const props: Props = {
        member: {
            id: '1',
            firstName: 'Steven',
            lastName: 'Stevenson',
        },
        tabId: 'members',
        image: null,
        contentText: {},
        entityText: {},
        id: '123',
        routes: routes,
        forms: forms,
        actions: [],
    }

    it('renders correctly', () => {
        render(<GroupMemberUpdateTemplate {...props} />)

        expect(screen.getAllByText('Steven').length).toBe(1)
    })

    it('conditionally renders remove from group form', () => {
        render(<GroupMemberUpdateTemplate {...props} />)

        expect(screen.queryByText('Remove from group')).toBeNull()

        cleanup()

        const propsCopy: Props = Object.assign({}, props, {
            actions: [actionConstants.GROUPS_MEMBERS_DELETE],
        })

        render(<GroupMemberUpdateTemplate {...propsCopy} />)

        expect(screen.getAllByText('Remove from group').length).toBe(1)
    })
})
