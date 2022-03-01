import * as React from 'react';
import * as nextRouter from 'next/router';
import { render, screen } from '@testing-library/react';

import { GroupForumTemplate } from './index';
import { Props } from './interfaces';

describe('Group forum template', () => {

    (nextRouter as any).useRouter = jest.fn();
    (nextRouter as any).useRouter.mockImplementation(() => ({ 
        asPath: '/groups/group/forum',
        query: {
            groupId: 'group'
        } 
    }));

    const props: Props = {
        id: 'mockId',
        tabId: 'files',
        user: undefined,
        actions: [],
        discussionsList: [],
        contentText: {
            discussionsHeading: 'Discussions',
            noDiscussions: 'No discussions',
            createDiscussion: 'Create discussion'
        },
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

        render(<GroupForumTemplate {...props} />);

        expect(screen.getAllByText('Discussions').length).toEqual(1);

    });
    
});
