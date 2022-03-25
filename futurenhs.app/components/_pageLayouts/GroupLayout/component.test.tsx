import * as React from 'react';
import * as nextRouter from 'next/router';

import { GroupLayout } from './index';
import { routes } from '@jestMocks/generic-props';

import { Props } from './interfaces';
import { render, screen } from '@testing-library/react';

describe('Group Layout', () => {
    
    (nextRouter as any).useRouter = jest.fn();
    (nextRouter as any).useRouter.mockImplementation(() => ({
        asPath: '/groups/groupId',
        query: {
            groupId: 'group',
            
        }
    }));

    const props: Props = {
        tabId: 'index',
        routes: routes,
        entityText: {
            mainHeading: 'Mock heading'
        }
    }

    it('renders correctly', () => {
        
        render(<GroupLayout {...props}/>);

        expect(screen.getAllByText('Mock heading').length).toBe(1);

    });
});
