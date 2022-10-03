import { routes } from '@jestMocks/generic-props'
import { cleanup, render, screen } from '@jestMocks/index'
import forms from '@config/form-configs/index'
import mockRouter from 'next-router-mock'
import { actions as actionConstants } from '@constants/actions'
import GroupMemberUpdatePage, {
    Props,
} from '@pages/groups/[groupId]/members/[memberId]/update/index.page'

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
        render(<GroupMemberUpdatePage {...props} />)

        expect(screen.getAllByText('Steven').length).toBe(1)
    })

    it('conditionally renders remove from group form', () => {
        render(<GroupMemberUpdatePage {...props} />)

        expect(screen.queryByText('Remove from group')).toBeNull()

        cleanup()

        const propsCopy: Props = Object.assign({}, props, {
            actions: [actionConstants.GROUPS_MEMBERS_DELETE],
        })

        render(<GroupMemberUpdatePage {...propsCopy} />)

        expect(screen.getAllByText('Remove from group').length).toBe(1)
    })
})
