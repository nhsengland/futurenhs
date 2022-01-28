import React from 'react';
import * as nextRouter from 'next/router';
import { render, screen } from '@testing-library/react';

import { GroupFileTemplate } from './index';
import { Props } from './interfaces';

describe('Group file template', () => {

    (nextRouter as any).useRouter = jest.fn();
    (nextRouter as any).useRouter.mockImplementation(() => ({ 
        asPath: '/groups/group/files',
        query: {
            groupId: 'group'
        } 
    }));

    const props: Props = {
        id: 'mockId',
        fileId: 'mockId',
        file: {},
        user: undefined,
        actions: [],
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

        render(<GroupFileTemplate {...props} />);

        expect(screen.getAllByText('Mock main heading html').length).toEqual(1);

    });
    
});
