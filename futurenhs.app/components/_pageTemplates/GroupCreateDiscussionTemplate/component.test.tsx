import React from 'react';
import * as nextRouter from 'next/router';
import { render, screen } from '@testing-library/react';

import { createDiscussionForm } from '@formConfigs/create-discussion'
import { GroupCreateDiscussionTemplate } from './index';
import { Props } from './interfaces';

describe('Group create discussion template', () => {

    (nextRouter as any).useRouter = jest.fn();
    (nextRouter as any).useRouter.mockImplementation(() => ({ 
        asPath: '/groups/group/forum/create',
        query: {
            groupId: 'group'
        } 
    }));

    const props: Props = {
        id: 'mockId',
        tabId: 'forum',
        folderId: 'mockId',
        user: undefined,
        actions: [],
        forms: {
            'create-discussion': createDiscussionForm
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

        render(<GroupCreateDiscussionTemplate {...props} />);

        expect(screen.getAllByText('Create Discussion').length).toEqual(2);

    });
    
});
