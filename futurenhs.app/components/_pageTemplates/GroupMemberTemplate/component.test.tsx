import * as React from 'react';
import * as nextRouter from 'next/router';
import { render, screen } from '@testing-library/react';

import { GroupMemberTemplate } from './index';
import forms from '@formConfigs/index';
import { Props } from './interfaces';

describe('Group member template', () => {

    (nextRouter as any).useRouter = jest.fn();
    (nextRouter as any).useRouter.mockImplementation(() => ({ 
        asPath: '/groups/group/members/member',
        query: {
            groupId: 'group',
            memberId: 'member'
        }
    }));

    const props: Props = {
        id: 'mockPageId',
        tabId: 'members',
        user: undefined,
        member: {
            firstName: 'Mock first name',
            lastName: 'Mock last name',
            pronouns: 'Mock pronouns',
            email: 'Mock email'
        },
        actions: [],
        forms: forms,
        contentText: {
            secondaryHeading: 'Mock secondary heading html'
        },
        entityText: null,
        image: null
    };

    it('renders correctly', () => {

        render(<GroupMemberTemplate {...props} />);

        expect(screen.getAllByText('Mock secondary heading html').length).toEqual(1);

    });

    it('renders contentText info if provided', () => {

        const propsCopy: Props = Object.assign({}, props, {
            contentText: {
                secondaryHeading: 'Mock secondary heading',
                firstNameLabel: 'Mock first name label',
                lastNameLabel: 'Mock last name label',
                pronounsLabel: 'Mock pronouns label',
                emailLabel: 'Mock email label'
            }
        });
        
        render(<GroupMemberTemplate {...propsCopy} />);

        expect(screen.getAllByText('Mock first name label').length).toBe(1);

    });
    
});
