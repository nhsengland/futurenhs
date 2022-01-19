import React from 'react';
import * as nextRouter from 'next/router';
import { render, screen } from '@testing-library/react';

import { GroupFoldersTemplate } from './index';
import { Props } from './interfaces';

describe('Group folders template', () => {

    (nextRouter as any).useRouter = jest.fn();
    (nextRouter as any).useRouter.mockImplementation(() => ({ 
        asPath: '/groups/group/folders',
        query: {
            groupId: 'group',
            folderId: 'folder'
        } 
    }));

    const props: Props = {
        id: 'mockId',
        folderId: 'mockId',
        folder: null,
        files: [],
        user: undefined,
        actions: [],
        content: {
            titleText: 'Mock title text',
            metaDescriptionText: 'Mock meta description text',
            mainHeadingHtml: 'Mock main heading html',
            introHtml: 'Mock intro html',
            navMenuTitleText: 'Mock nav menu title text',
            secondaryHeadingHtml: 'Mock secondary heading html'
        },
        image: null
    };

    it('renders correctly', () => {

        render(<GroupFoldersTemplate {...props} />);

        expect(screen.getAllByText('Mock main heading html').length).toEqual(1);

    });
    
});
