import * as React from 'react'
import * as nextRouter from 'next/router'
import { render, screen, cleanup } from '@testing-library/react'

import { actions } from '@constants/actions'
import { routes } from '@jestMocks/generic-props'
import { GroupMemberListingTemplate } from './index'
import { Props } from './interfaces'

describe('Group member listing template', () => {
    ;(nextRouter as any).useRouter = jest.fn()
    ;(nextRouter as any).useRouter.mockImplementation(() => ({
        asPath: '/groups/group/members',
        query: {
            groupId: 'group',
        },
    }))

    const props: Props = {
        id: 'mockPageId',
        routes: routes,
        tabId: 'members',
        user: undefined,
        actions: [],
        contentText: {
            pendingMemberRequestsHeading: 'Mock pending members heading',
            membersHeading: 'Mock members heading',
            noPendingMembers: 'Mock no pending members text',
            noMembers: 'Mock no members text',
            acceptMember: 'Accept',
            rejectMember: 'Reject',
            editMember: 'Edit',
        },
        entityText: {
            title: 'Mock title text',
            metaDescription: 'Mock meta description text',
            mainHeading: 'Mock main heading html',
        },
        image: null,
        pendingMembers: [
            {
                fullName: 'Mock pending name 1',
                email: 'mockemail.address1',
                requestDate: 'mockDateString',
            },
            {
                fullName: 'Mock pending name 2',
                email: 'mockemail.address2',
                requestDate: 'mockDateString',
            },
        ],
        members: [
            {
                id: 'mockId',
                fullName: 'Mock member name',
                role: 'Mock member role',
                joinDate: '01/01/20222',
                lastLogInDate: '02/01/2022',
            },
        ],
    }

    // it('renders expected page heading', async () => {

    //     render(<GroupMemberListingTemplate {...props} />);

    //     expect(await screen.findByText('Mock members heading')).toBeVisible();

    // });

    it('conditionally renders pending members content', async () => {
        const pendingMembersNoViewAccess: Props = Object.assign(props, {
            actions: [],
        })

        render(<GroupMemberListingTemplate {...pendingMembersNoViewAccess} />)

        expect(screen.queryByText('Mock pending members heading')).toBeNull()
        expect(screen.queryByText('Mock pending name 1')).toBeNull()
        expect(screen.queryByText('Mock pending name 2')).toBeNull()

        cleanup()

        const pendingMembersViewAccess: Props = Object.assign(props, {
            actions: [actions.GROUPS_MEMBERS_PENDING_VIEW],
            pendingMembers: [
                {
                    fullName: 'Mock pending name 1',
                    email: 'mockemail.address1',
                    requestDate: 'mockDateString',
                },
                {
                    fullName: 'Mock pending name 2',
                    email: 'mockemail.address2',
                    requestDate: 'mockDateString',
                },
            ],
        })

        render(<GroupMemberListingTemplate {...pendingMembersViewAccess} />)

        // Temporarily removed functionality for private beta
        // expect(screen.getAllByText('Mock pending members heading').length).toBe(1);
        // expect(screen.getAllByText('Mock pending name 1').length).toBe(1);
        // expect(screen.getAllByText('Mock pending name 2').length).toBe(1);
    })

    // Temporarily removed functionality for private beta
    // it('conditionally renders fallback content when there are no pending members', async () => {

    //     const pendingMembers: Props = Object.assign(props, {
    //         actions: [actions.GROUPS_MEMBERS_PENDING_VIEW],
    //         pendingMembers: [
    //             {
    //                 fullName: 'Mock pending name 1',
    //                 email: 'mockemail.address1',
    //                 requestDate: 'mockDateString'
    //             },
    //             {
    //                 fullName: 'Mock pending name 2',
    //                 email: 'mockemail.address2',
    //                 requestDate: 'mockDateString'
    //             }
    //         ]
    //     });

    //     render(<GroupMemberListingTemplate {...pendingMembers} />);

    //     expect(screen.queryByText('Mock no pending members text')).toBeNull();

    //     cleanup();

    //     const noPendingMembers: Props = Object.assign(props, {
    //         actions: [actions.GROUPS_MEMBERS_PENDING_VIEW],
    //         pendingMembers: []
    //     });

    //     render(<GroupMemberListingTemplate {...noPendingMembers} />);

    //     expect(screen.getAllByText('Mock no pending members text').length).toBe(1);

    // });

    it('conditionally renders members list', () => {
        render(<GroupMemberListingTemplate {...props} />)

        expect(screen.getAllByText('Mock member name').length).toBe(1)

        cleanup()

        props.members = [
            {
                id: 'mockId',
                role: 'Mock member role',
                joinDate: '01/01/20222',
                lastLogInDate: '02/01/2022',
            },
        ]

        render(<GroupMemberListingTemplate {...props} />)

        expect(screen.getAllByText('Mock member role').length).toBeGreaterThan(
            1
        )
        expect(screen.queryByText('Mock member name')).toBeNull()

        cleanup()

        const propsCopy: Props = Object.assign({}, props, { members: [] })

        render(<GroupMemberListingTemplate {...propsCopy} />)

        expect(screen.getAllByText('Mock no members text').length).toBe(1)
        expect(screen.queryByText('Mock member name')).toBeNull()
    })

    it('conditionally renders member edit button', () => {
        props.actions.push(actions.GROUPS_MEMBERS_EDIT)

        render(<GroupMemberListingTemplate {...props} />)

        expect(screen.getAllByText('Edit').length).toBe(1)

        cleanup()

        const propsCopy = Object.assign({}, props, {
            actions: [],
        })

        render(<GroupMemberListingTemplate {...propsCopy} />)

        expect(screen.queryByText('Edit')).toBeNull()
    })
})
