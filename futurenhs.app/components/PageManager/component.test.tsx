import React from 'react';
import { render, screen } from '@testing-library/react';
import * as nextRouter from 'next/router';

import { GroupHomeTemplate } from './index';
import { routes } from '@jestMocks/generic-props';

import { Props } from './interfaces';

(nextRouter as any).useRouter = jest.fn();
(nextRouter as any).useRouter.mockImplementation(() => ({ 
    asPath: '/groups/group',
    query: {
        groupId: 'group'
    } 
}));

const props: Props = {
    id: 'mockId',
    routes: routes,
    tabId: 'index',
    actions: [],
    contentText: {},
    user: undefined,
    entityText: {
        mainHeading: "Mock heading"
    },
    image: {
        src: "https://www.google.com",
        height: 100,
        width: 100,
        altText: "Mock alt text"
    }
}

describe('Group home template', () => {
    
    it('renders correctly', () => {
        
        render(<GroupHomeTemplate {...props}/>);

        //TODO: template WIP

    });

});