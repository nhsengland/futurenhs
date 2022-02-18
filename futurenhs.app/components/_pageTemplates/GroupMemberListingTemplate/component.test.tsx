import * as React from 'react';
import * as nextRouter from 'next/router';
import { render, screen } from '@testing-library/react';

import { actions } from '@constants/actions';
import { GroupMemberListingTemplate } from './index';
import { Props } from './interfaces';

describe('Group member listing template', () => {

    (nextRouter as any).useRouter = jest.fn();
    (nextRouter as any).useRouter.mockImplementation(() => ({ 
        asPath: '/groups/group/members',
        query: {
            groupId: 'group'
        } 
    }));

    const props: Props = {
        id: 'mockPageId',
        user: undefined,
        actions: [],
        contentText: { 
            pendingMemberRequestsHeading: 'Mock pending members heading', 
            membersHeading: 'Mock members heading',
            noPendingMembers: 'Mock no pending members text',
            noMembers: 'Mock no members text'
        },
        entityText: {
            title: 'Mock title text',
            metaDescription: 'Mock meta description text',
            mainHeading: 'Mock main heading html'
        },
        image: null,
        pendingMembers: [
            {
                fullName: 'Mock pending name 1',
                email: 'mockemail.address1', 
                requestDate: 'mockDateString'
            },
            {
                fullName: 'Mock pending name 2',
                email: 'mockemail.address2', 
                requestDate: 'mockDateString'
            }
        ],
        members: []
    };

    it('renders expected page heading', async () => {

        render(<GroupMemberListingTemplate {...props} />);

        expect(await screen.findByText('Mock main heading html')).toBeVisible();

    });

    // it('conditionally renders pending members content', async () => {

    //     const pendingMembersNoViewAccess: Props = Object.assign(props, {
    //         actions: []
    //     });

    //     render(<GroupMemberListingTemplate {...pendingMembersNoViewAccess} />);

    //     expect(await screen.findByText('Mock pending members heading')).not.toBeVisible();
    //     expect(await screen.findByText('Mock pending name 1')).not.toBeVisible();
    //     expect(await screen.findByText('Mock pending name 2')).not.toBeVisible();

        // const pendingMembersViewAccess: Props = Object.assign(props, {
        //     actions: [actions.GROUPS_MEMBERS_PENDING_VIEW],
        //     pendingMembers: [
        //         {
        //             fullName: 'Mock pending name 1',
        //             email: 'mockemail.address1', 
        //             requestDate: 'mockDateString'
        //         },
        //         {
        //             fullName: 'Mock pending name 2',
        //             email: 'mockemail.address2', 
        //             requestDate: 'mockDateString'
        //         }
        //     ]
        // });

        // render(<GroupMemberListingTemplate {...pendingMembersViewAccess} />);
        
        // expect(await screen.findByText('Mock pending members heading')).toBeVisible();
        // expect(await screen.findByText('Mock pending name 1')).toBeVisible();
        // expect(await screen.findByText('Mock pending name 2')).toBeVisible();

    // });

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
        
    //     expect(await screen.findByText('Mock no pending members text')).not.toBeVisible();
        
    //     const noPendingMembers: Props = Object.assign(props, {
    //         actions: [actions.GROUPS_MEMBERS_PENDING_VIEW],
    //         pendingMembers: []
    //     });

    //     render(<GroupMemberListingTemplate {...noPendingMembers} />);
        
    //     expect(await screen.findByText('Mock no pending members text')).toBeVisible();

    // });
    
});
