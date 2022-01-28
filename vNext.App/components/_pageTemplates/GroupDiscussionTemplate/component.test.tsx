import * as React from 'react';
import * as nextRouter from 'next/router';
import { render, screen } from '@testing-library/react';

import { GroupDiscussionTemplate } from './index';
import { Props } from './interfaces';

describe('Group forum template', () => {

    (nextRouter as any).useRouter = jest.fn();
    (nextRouter as any).useRouter.mockImplementation(() => ({ 
        asPath: '/groups/group/forum',
        query: {
            groupId: 'group',
            discussionId: 'discussion'
        } 
    }));

    const props: Props = {
        id: 'mockId',
        discussionId: 'mockId',
        user: undefined,
        actions: [],
        discussion: {
            text: {
                title: 'Mock Title'
            }
        },
        contentText: null,
        entityText: {
            title: 'Mock title text',
            metaDescription: 'Mock meta description text',
            mainHeading: 'Mock main heading html',
            intro: 'Mock intro html',
            navMenuTitle: 'Mock nav menu title text',
            secondaryHeading: 'Mock secondary heading html'
        },
        image: null
    };

    it('renders correctly', () => {

        render(<GroupDiscussionTemplate {...props} />);

        expect(screen.getAllByText('Mock main heading html').length).toEqual(1);

    });
    
});
